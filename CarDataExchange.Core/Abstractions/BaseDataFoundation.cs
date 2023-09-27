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
    /// <summary>
    /// Базовая реализация десериализации и сериализации данных.
    /// Десериализация реализуется на основе структуры описания расшифрованных данных.
    /// </summary>
    public abstract class BaseDataFoundation<T> : IDecoder<DecodedInfo<T>>, IEncoder<T>
        where T : notnull
    {
        public abstract T ConvertToData(IReadOnlyCollection<DataObject> body);

        public DecodedInfo<T> Decode(byte[] data, int length)
        {
            DecodedInfo<T> decoded = new();
            decoded.ElementsCount = -1;
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
                        body.Add(new(Encoding.ASCII.GetString(strBytes), DataTokenType.String));
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
                    default:
                        throw new UnknownByteTypeException(type);
                }

                if (body.Count >= decoded.ElementsCount) break;
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
            using MemoryStream stream = new();

            foreach (var property in typeOfData.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var propertyValue = property.GetValue(data);
                if (propertyValue == null) continue;

                switch (propertyValue)
                {
                    case string strValue:
                        if (string.IsNullOrEmpty(strValue)) break;
                        else encodingObjects.Add(new(propertyValue, DataTokenType.String));

                        byte strLength = (byte)Math.Min(byte.MaxValue, strValue!.Length);
                        stream.WriteByte(0x09); // 0x09 - Тип строки
                        stream.WriteByte(strLength);
                        _AppendBytesToStream(Encoding.ASCII.GetBytes(strValue[..strLength]), stream);
                        break;
                    case short int16CodeValue:
                        stream.WriteByte(0x12); // 0x12 - Тип целого без знака
                        _AppendNumberBytesToStream(BitConverter.GetBytes(int16CodeValue), stream);
                        encodingObjects.Add(new(propertyValue, DataTokenType.Int16));
                        break;
                    case ushort uint16CodeValue:
                        stream.WriteByte(0x12); // 0x12 - Тип целого без знака
                        _AppendNumberBytesToStream(BitConverter.GetBytes(uint16CodeValue), stream);
                        encodingObjects.Add(new(propertyValue, DataTokenType.UInt16));
                        break;
                    case float singleCodeValue:
                        stream.WriteByte(0x13); // 0x12 - Тип целого без знака 
                        _AppendNumberBytesToStream(BitConverter.GetBytes(singleCodeValue), stream);
                        encodingObjects.Add(new(propertyValue, DataTokenType.UInt16));
                        break;
                    default:
                        throw new NotSupportedException($"Type {propertyValue?.GetType()} unsupported to encode");
                }
            }

            //Заголовки
            using MemoryStream headerStream = new();
            headerStream.WriteByte(0x02);
            headerStream.WriteByte((byte)encodingObjects.Where(x => x.Token != DataTokenType.Nothing).Count());
            stream.WriteTo(headerStream);

            return headerStream.ToArray();
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
