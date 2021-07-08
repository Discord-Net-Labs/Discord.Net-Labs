using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Discord.SlashCommands
{
    internal class SlashCommandMap<T> where T : class, IExecutableInfo
    {
        private static readonly char[] Delimiters = { ' ', '\n', '\r', ',' };

        private readonly ConcurrentDictionary<string, T> _map;
        private readonly object _lockObj = new object();

        public SlashCommandMap (SlashCommandService commandService)
        {
            _map = new ConcurrentDictionary<string, T>();
        }

        public bool AddCommand (T command)
        {
            string key = ParseCommandName(command);

            return _map.TryAdd(key, command);
        }

        public bool RemoveCommand (T command)
        {
            string key = ParseCommandName(command);

            return _map.TryRemove(key, out var _);
        }

        public IEnumerable<T> GetCommands (string input)
        {
            var result = new List<T>();

            lock (_lockObj)
            {
                foreach (var pair in _map)
                {
                    if (CompareCommands(pair.Key, input))
                        result.Add(pair.Value);
                }
            }
            return result;
        }

        private bool CompareCommands (string first, string second)
        {
            var dissectedFirst = first.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
            var dissectedSecond = second.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (dissectedFirst.Length != dissectedSecond.Length)
                return false;

            for (var i = 0; i < dissectedFirst.Length; i++)
                if (!string.Equals(dissectedFirst[i], dissectedSecond[i], StringComparison.OrdinalIgnoreCase))
                    return false;

            return true;
        }

        private static string ParseCommandName (T command)
        {
            string groupName = command.Group?.Name;
            string commandName = command.Name;
            string moduleName = command.Module?.Name;

            string delimiter = Delimiters[0].ToString();

            return string.Join(delimiter, moduleName, groupName, commandName);
        }
    }
}
