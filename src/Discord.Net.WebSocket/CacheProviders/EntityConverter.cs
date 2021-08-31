using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal static class EntityConverter
    {
        public static TModel Decode<TModel>(IEnumerable<byte> data) where TModel : class
        {
            var type = typeof(TModel);

            var hash = BitConverter.ToInt32(data.Take(4).ToArray(), 0);

            if (type.GetHashCode() != hash)
                throw new Exception("The passed in type is not in format of the buffer");

            var props = type.GetProperties();
            var model = Activator.CreateInstance<TModel>();

            int indx = 4;
            foreach (var prop in props)
            {
                int length = 0;
                if (Deserializers.ContainsKey(prop.PropertyType))
                {
                    var d = data.Skip(indx);

                    if (prop.PropertyType == typeof(string))
                    {
                        length = Array.IndexOf(d.ToArray(), d.FirstOrDefault(x => x == 0)) + 1;
                    }
                    else if (prop.PropertyType == typeof(bool))
                    {
                        length = 1;
                    }
                    else if (prop.PropertyType.IsValueType)
                    {
                        length = Marshal.SizeOf(prop.PropertyType);
                    }

                    var val = Deserializers[prop.PropertyType].Invoke(d.Take(length).ToArray());

                    prop.SetValue(model, val);

                    indx += length;
                    continue;
                }

                if (prop.PropertyType.IsClass && !prop.PropertyType.FullName.StartsWith("System."))
                {
                    var m = typeof(EntityConverter)
                                .GetMethod(nameof(EntityConverter.Decode), BindingFlags.NonPublic | BindingFlags.Static)
                                .MakeGenericMethod(prop.PropertyType)
                                .Invoke(null, new object[] { data.Skip(indx) });

                    prop.SetValue(model, m);
                    continue;
                }

                if (prop.PropertyType.IsEnum)
                {
                    var encodingType = Enum.GetUnderlyingType(prop.PropertyType);
                    length = Marshal.SizeOf(encodingType);
                    var enumData = Deserializers[encodingType].Invoke(data.Skip(indx).Take(length).ToArray());
                    prop.SetValue(model, enumData);
                    indx += length;
                    continue;
                }
            }

            return model;
        }

        public static byte[] Encode<TModel>(TModel model) where TModel : class
        {
            var type = model.GetType();
            var props = type.GetProperties();

            var hash = type.GetHashCode();

            List<byte> l = new List<byte>();
            l.AddRange(BitConverter.GetBytes(hash));

            foreach (var prop in props)
            {
                if (Serializers.ContainsKey(prop.PropertyType))
                {
                    var buff = Serializers[prop.PropertyType].Invoke(prop.GetValue(model));
                    l.AddRange(buff);
                    continue;
                }

                if (prop.PropertyType.IsClass && !prop.PropertyType.FullName.StartsWith("System."))
                {
                    var m = typeof(EntityConverter)
                                 .GetMethod(nameof(EntityConverter.Encode), BindingFlags.NonPublic | BindingFlags.Static);
                    var buff = m.MakeGenericMethod(prop.PropertyType)
                                .Invoke(null, new object[] { prop.GetValue(model) }) as byte[];
                    l.AddRange(buff);
                    continue;
                }

                if (prop.PropertyType.IsEnum)
                {
                    var encodingType = Enum.GetUnderlyingType(prop.PropertyType);
                    var buff = Serializers[encodingType].Invoke(prop.GetValue(model));
                    l.AddRange(buff);
                    continue;
                }
            }

            return l.ToArray();
        }

        private static Dictionary<Type, Func<object, IEnumerable<byte>>> Serializers = new Dictionary<Type, Func<object, IEnumerable<byte>>>()
        {
            {typeof(string), (v) =>
            {
                var s = (v as string).Replace("\u0000", "\u0001");
                var buff = new byte[s.Length + 1];
                Encoding.UTF8.GetBytes(s).CopyTo(buff, 0);
                buff[s.Length] = 0x00;
                return buff;
            } },
            {typeof(uint),   (v) => BitConverter.GetBytes((uint)v) },
            {typeof(int),    (v) => BitConverter.GetBytes((int)v) },
            {typeof(double), (v) => BitConverter.GetBytes((double)v) },
            {typeof(ushort), (v) => BitConverter.GetBytes((ushort)v) },
            {typeof(float),  (v) => BitConverter.GetBytes((float)v) },
            {typeof(long),   (v) => BitConverter.GetBytes((long)v) },
            {typeof(char),   (v) => BitConverter.GetBytes((char)v) },
            {typeof(short),  (v) => BitConverter.GetBytes((short)v) },
            {typeof(ulong),  (v) => BitConverter.GetBytes((ulong)v) },
            {typeof(bool),   (v) => BitConverter.GetBytes((bool)v) },
        };

        private static Dictionary<Type, Func<byte[], object>> Deserializers = new Dictionary<Type, Func<byte[], object>>()
        {
            {typeof(string), (v) => Encoding.UTF8.GetString(v.Take(v.Length - 1).ToArray()) },
            {typeof(uint),   (v) => BitConverter.ToUInt32(v, 0) },
            {typeof(int),    (v) => BitConverter.ToInt32(v, 0) },
            {typeof(double), (v) => BitConverter.ToDouble(v, 0) },
            {typeof(ushort), (v) => BitConverter.ToUInt16(v, 0) },
            {typeof(float),  (v) => BitConverter.ToSingle(v, 0) },
            {typeof(long),   (v) => BitConverter.ToInt64(v, 0) },
            {typeof(char),   (v) => BitConverter.ToChar(v, 0) },
            {typeof(short),  (v) => BitConverter.ToInt16(v, 0) },
            {typeof(ulong),  (v) => BitConverter.ToUInt64(v, 0) },
            {typeof(bool),   (v) => BitConverter.ToBoolean(v, 0) },
        };
    }
}
