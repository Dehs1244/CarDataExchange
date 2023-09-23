using CarDataExchange.Core.Enums;
using CarDataExchange.Core.Exceptions;
using CarDataExchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Abstractions
{
    public abstract class BaseDataFoundation<T> : IDecoder<DecodedInfo<T>>, IEncoder<T>
        where T : notnull
    {
        public abstract T ConvertToData(IReadOnlyCollection<DataObject> body);

        public DecodedInfo<T> Decode(byte[] data, int length)
        {
            DecodedInfo<T> decoded = new();
            int index = 0;
            List<DataObject> body = new();

            while (index < length)
            {
                byte type = data[index++];

                switch (type)
                {
                    case 0x02:
                        decoded.ElementsCount = data[index++];
                        break;
                    case 0x09:
                        var strLen = data[index++];
                        Span<byte> strBytes = data.AsSpan(index, strLen);
                        body.Add(new(Encoding.ASCII.GetString(strBytes).TrimEnd('\0'), DataTokenType.String));
                        index += strLen;
                        break;
                    case 0x12:
                        Span<byte> uintBytes = data.AsSpan(index, 2);
                        if (BitConverter.IsLittleEndian)
                            uintBytes.Reverse();
                        body.Add(new(BitConverter.ToUInt16(uintBytes), DataTokenType.UInt16));
                        index += 2;
                        break;
                    case 0x13:
                        Span<byte> pointNumberBytes = data.AsSpan(index, 4);
                        if (BitConverter.IsLittleEndian)
                            pointNumberBytes.Reverse();
                        body.Add(new (BitConverter.ToSingle(pointNumberBytes), DataTokenType.FloatPoint));
                        index += 4;
                        break;
                    case 0x00: // Байт выравнивания
                        body.Add(DataObject.Empty);
                        break;
                    default:
                        throw new UnknownByteTypeException(type);
                }
            }

            decoded.Value = ConvertToData(body);

            return decoded;
        }

        private void _AppendBytesToStream(byte[] buffer, Stream stream)
        {
            foreach (var b in buffer)
            {
                stream.WriteByte(b);
            }
        }

        private void _AppendNumberBytesToStream(byte[] buffer, Stream stream)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            _AppendBytesToStream(buffer, stream);
        }

        public virtual byte[] Encode(T data)
        {
            ICollection<DataObject> encodingObjects = new List<DataObject>();
            Type typeOfData = data.GetType();

            foreach(var property in typeOfData.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var propertyValue = property.GetValue(data);
                switch (propertyValue)
                {
                    case string strValue:
                        if (string.IsNullOrEmpty(strValue)) encodingObjects.Add(DataObject.Empty);
                        else encodingObjects.Add(new(propertyValue, DataTokenType.String));
                        break;
                    case ushort: 
                        encodingObjects.Add(new(propertyValue, DataTokenType.UInt16));
                        break;
                    case short:
                        encodingObjects.Add(new(propertyValue, DataTokenType.Int16));
                        break;
                    case float:
                        encodingObjects.Add(new(propertyValue, DataTokenType.FloatPoint));
                        break;
                    case null:
                        encodingObjects.Add(DataObject.Empty);
                        break;
                    default:
                        throw new NotSupportedException($"Type {propertyValue.GetType()} unsupported to encode");
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                //Заголовки
                stream.WriteByte(0x02);
                stream.WriteByte((byte)encodingObjects.Where(x=> x.Token != DataTokenType.Nothing).Count());

                foreach(var dataObject in encodingObjects)
                {

                    switch (dataObject.Token)
                    {
                        case DataTokenType.String:
                            stream.WriteByte(0x09); // 0x09 - Тип строки
                            stream.WriteByte(0x06); // 0x06 - Длина строки
                            var strValue = (string)dataObject.Value;
                            _AppendBytesToStream(Encoding.ASCII.GetBytes(strValue[..Math.Min(6, strValue.Length)].PadRight(6, '\0')), stream);
                            break;
                        case DataTokenType.UInt16:
                            stream.WriteByte(0x12); // 0x12 - Тип целого без знака
                            _AppendNumberBytesToStream(BitConverter.GetBytes((ushort)dataObject.Value), stream);
                            break;
                        case DataTokenType.Int16:
                            stream.WriteByte(0x12);
                            _AppendNumberBytesToStream(BitConverter.GetBytes((short)dataObject.Value), stream);
                            break;
                        case DataTokenType.FloatPoint:
                            stream.WriteByte(0x13); // 0x13 - Тип с плавающей точкой
                            _AppendNumberBytesToStream(BitConverter.GetBytes((float)dataObject.Value), stream);
                            break;
                        case DataTokenType.Nothing:
                            stream.WriteByte(0x00); // Пустой бит
                            break;
                        default:
                            throw new NotSupportedException($"Token {dataObject.Token} unsupported");
                    }
                }

                return stream.ToArray();
            }
        }

        public bool TryDecode(Stream stream, out DecodedInfo<T> decoded)
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            decoded = default;

            try
            {
                if ((bytesRead = stream.Read(buffer, 0, buffer.Length)) == 0) return false;
            }
            catch(Exception)
            {
                return false;
            }

            decoded = Decode(buffer, bytesRead);
            return true;
        }
    }
}
