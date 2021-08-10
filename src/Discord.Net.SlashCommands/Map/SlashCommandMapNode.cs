using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord.SlashCommands
{
    internal class SlashCommandMapNode<T> where T : class, IExecutableInfo
    {
        private const string WildCardStr = "*";
        private const string RegexWildCardExp = ".*";

        private ConcurrentDictionary<string, SlashCommandMapNode<T>> _nodes;
        private ConcurrentDictionary<string, T> _commands;
        private ConcurrentDictionary<string, T> _wildCardCommands;

        public IReadOnlyDictionary<string, SlashCommandMapNode<T>> Nodes => _nodes;
        public IReadOnlyDictionary<string, T> Commands => _commands;
        public IReadOnlyDictionary<string, T> WildCardCommands => _wildCardCommands;
        public string Name { get; }

        public SlashCommandMapNode (string name)
        {
            Name = name;
            _nodes = new ConcurrentDictionary<string, SlashCommandMapNode<T>>();
            _commands = new ConcurrentDictionary<string, T>();
            _wildCardCommands = new ConcurrentDictionary<string, T>();
        }

        public void AddCommand (string[] keywords, int index, T commandInfo)
        {
            if (keywords.Length == index + 1)
            {
                if (commandInfo.IsWildCard)
                {
                    if (!_wildCardCommands.TryAdd(commandInfo.Name, commandInfo))
                        throw new InvalidOperationException($"A {typeof(T).FullName} already exists with the same name");
                }
                else
                {
                    if (!_commands.TryAdd(commandInfo.Name, commandInfo))
                        throw new InvalidOperationException($"A {typeof(T).FullName} already exists with the same name");
                }
            }
            else
            {
                var node = _nodes.GetOrAdd(keywords[index], (key) => new SlashCommandMapNode<T>(key));
                node.AddCommand(keywords, ++index, commandInfo);
            }
        }

        public void RemoveCommand (string[] keywords, int index)
        {
            if (keywords.Length == index + 1)
            {
                if (!_commands.TryRemove(keywords[index], out var _))
                    throw new Exception($"Could not remove any {typeof(T).FullName}s from this node for input: {keywords[index]}");
            }
            else
            {
                if (!_nodes.TryGetValue(keywords[index], out var node))
                    throw new InvalidOperationException($"No descendant node was found with the name {keywords[index]}");
                node.RemoveCommand(keywords, ++index);
            }
        }

        public SearchResult<T> GetCommand (string[] keywords, int index)
        {
            string name = string.Join(" ", keywords);

            if(keywords.Length == index + 1)
            {
                if (_commands.TryGetValue(keywords[index], out var cmd))
                    return SearchResult<T>.FromSuccess(name, cmd);
                else
                {
                    foreach(var cmdPair in _wildCardCommands)
                    {
                        var patternStr = cmdPair.Key.Replace(WildCardStr, RegexWildCardExp);

                        var match = Regex.Match(keywords[index], patternStr, RegexOptions.IgnoreCase);
                        if (match.Success && match.Value.Length == keywords[index].Length)
                        {
                            var statics = cmdPair.Key.Split(new string[] { WildCardStr }, StringSplitOptions.RemoveEmptyEntries);

                            var args = match.Value.Split(statics, StringSplitOptions.RemoveEmptyEntries);
                            return SearchResult<T>.FromSuccess(name, cmdPair.Value, args);
                        }
                    }
                }
            }
            else
            {
                if (_nodes.TryGetValue(keywords[index], out var node))
                    return node.GetCommand(keywords, ++index);
            }

            return SearchResult<T>.FromError(name, SlashCommandError.UnknownCommand, $"No {typeof(T).FullName} found for {name}");
        }

        public SearchResult<T> GetCommand (string text, int index, char[] seperators)
        {
            var keywords = text.Split(seperators);
            return GetCommand(keywords, index);
        }
    }
}
