using Discord.Interactions.Builders;
using System;
using System.Collections.Concurrent;

namespace Discord.Interactions
{
    internal static class ModalUtils
    {
        private static ConcurrentDictionary<Type, ModalInfo> _modalInfos = new();

        public static ModalInfo GetOrAdd(Type type)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            return _modalInfos.GetOrAdd(type, ModuleClassBuilder.BuildModalInfo(type));
        }

        public static ModalInfo GetOrAdd<T>() where T : class, IModal
            => GetOrAdd(typeof(T));

        public static bool TryGet(Type type, out ModalInfo modalInfo)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            return _modalInfos.TryGetValue(type, out modalInfo);
        }

        public static bool Remove(Type type)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            return _modalInfos.TryRemove(type, out _);
        }
    }
}
