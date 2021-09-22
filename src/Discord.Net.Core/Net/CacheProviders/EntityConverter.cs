using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    internal static class EntityConverter
    {
        public static object Decode<TModel>(IEnumerable<byte> data) where TModel : class
        {
            int l = 0;
            return Decode<TModel>(data, ref l);
        }
        public static object Decode<TModel>(IEnumerable<byte> data, ref int length) where TModel : class
        {
            var type = GetModelTypeOrDefault<TModel>();

            var hash = BitConverter.ToInt32(data.Take(4).ToArray(), 0);

            if (type.GetHashCode() != hash)
                throw new Exception("The passed in type is not in format of the buffer");

            var props = type.GetProperties();
            var model = Activator.CreateInstance(type);

            int indx = 4 + length;
            foreach (var prop in props)
            {
                var result = Decode(prop.PropertyType, data.ToArray(), ref indx);

                if (result.value == null && result.length == 0)
                    throw new Exception($"Failed to parse property {prop.Name} with type {prop.PropertyType}");

                prop.SetValue(model, result.value);
            }

            return model;
        }

        private static (object value, int length) Decode(Type type, byte[] data, ref int indx)
        {
            int length = 0;
            if (Deserializers.ContainsKey(type))
            {
                var d = data.Skip(indx);

                if (type == typeof(string))
                {
                    length = Array.IndexOf(d.ToArray(), d.FirstOrDefault(x => x == 0)) + 1;
                }
                else if (type == typeof(bool))
                {
                    length = 1;
                }
                else if (type.IsValueType)
                {
                    length = Marshal.SizeOf(type);
                }

                var val = Deserializers[type].Invoke(d.Take(length).ToArray());

                indx += length;

                return (val, length);
            }
            else if (IsOptional(type))
            {
                var hasValue = Deserializers[typeof(bool)].Invoke(data.Take(1).ToArray());
                length = GetTypeLength(type.GenericTypeArguments[0], data) ?? 0;
                length++; // add for first bool
                var opt = DecodeOptional(type.GenericTypeArguments[0], data);
                return (opt, length);
            }
            else if (type.IsClass && !type.FullName.StartsWith("System."))
            {
                var args = new object[] { data.Skip(indx), indx };
                var m = typeof(EntityConverter)
                            .GetMethod(nameof(EntityConverter.Decode), BindingFlags.NonPublic | BindingFlags.Static)
                            .MakeGenericMethod(type)
                            .Invoke(null, args);

                var l = (int)args[1];
                indx += l;
                return (m, l);
            }
            else if (type.IsEnum)
            {
                var encodingType = Enum.GetUnderlyingType(type);
                length = Marshal.SizeOf(encodingType);
                var enumData = Deserializers[encodingType].Invoke(data.Skip(indx).Take(length).ToArray());
                indx += length;
                return (enumData, length);
            }

            return (null, 0);
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
                l.AddRange(EncodeProperty(prop.PropertyType, prop.GetValue(model)));
            }

            return l.ToArray();
        }

        private static byte[] EncodeProperty(Type type, object value)
        {
            List<byte> l = new List<byte>();

            if (Serializers.ContainsKey(type))
            {
                var buff = Serializers[type].Invoke(value);
                l.AddRange(buff);
            }
            else if (IsOptional(type))
            {
                var hasValue = (bool)type.GetProperty("IsSpecified").GetValue(value);
                var optValue = type.GetMethod("GetValueOrDefault").Invoke(value, new object[0]);
                l.AddRange(EncodeOptional(type.GenericTypeArguments[0], hasValue, optValue));
            }
            else if (type.IsClass && !type.FullName.StartsWith("System."))
            {
                var m = typeof(EntityConverter)
                             .GetMethod(nameof(EntityConverter.Encode), BindingFlags.NonPublic | BindingFlags.Static);
                var buff = m.MakeGenericMethod(type)
                            .Invoke(null, new object[] { value }) as byte[];
                l.AddRange(buff);
            }
            else if (type.IsEnum)
            {
                var encodingType = Enum.GetUnderlyingType(type);
                var buff = Serializers[encodingType].Invoke(value);
                l.AddRange(buff);
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

        private static int? GetTypeLength(Type type, byte[] d)
        {
            if (type == typeof(string))
            {
                return Array.IndexOf(d.ToArray(), d.FirstOrDefault(x => x == 0)) + 1;
            }
            else if (type == typeof(bool))
            {
                return 1;
            }
            else if (type.IsValueType)
            {
                return Marshal.SizeOf(type);
            }

            return null;
        }

        private static byte[] EncodeOptional(Type inner, bool hasValue, object value = null)
        {
            List<byte> b = new List<byte>();
            b.Add(Serializers[typeof(bool)].Invoke(hasValue).First());
            b.AddRange(Serializers[inner].Invoke(value));
            return b.ToArray();
        }

        private static byte[] EncodeOptional<TInner>(Optional<TInner> opt)
        {
            if (opt.IsSpecified)
            {
                var value = Serializers[typeof(TInner)].Invoke(opt.Value);

                List<byte> b = new List<byte>();
                b.Add(Serializers[typeof(bool)].Invoke(true).First());
                b.AddRange(value);
                return b.ToArray();
            }
            else
                return Serializers[typeof(bool)].Invoke(false).ToArray();
        }

        private static object DecodeOptional(Type inner, byte[] arr)
        {
            var method = typeof(EntityConverter).GetMethod(nameof(EntityConverter.DecodeOptional));

            var generic = method.MakeGenericMethod(inner);

            return generic.Invoke(null, new object[] { arr });
        }

        private static Optional<TValue> DecodeOptional<TValue>(byte[] arr)
        {
            var hasValue = (bool)Deserializers[typeof(bool)].Invoke(arr);

            if (!hasValue)
            {
                return Optional<TValue>.Unspecified;
            }
            else
            {
                var type = typeof(TValue);

                var value = (TValue)Deserializers[type].Invoke(arr.Skip(1).ToArray());

                return new Optional<TValue>(value);
            }
        }

        private static bool IsOptional(Type type)
        {
            return type.Name == typeof(Optional<>).Name;
        }

        private static Type GetModelTypeOrDefault<TType>()
        {
            var type = typeof(TType).GetTypeInfo();
            var infType = typeof(ICacheableEntity<,>);

            var intf = type.ImplementedInterfaces.FirstOrDefault(x => x.Name == infType.Name);

            if (intf != null)
            {
                return intf.GenericTypeArguments.First();
            }
            else
                return type;
        }
    }
}
