/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Console
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConsoleCommandParser : IConsoleCommandParser
    {
        private const int DEFAULT_MAX_COMMANDS_PER_LINE = 64;

        private readonly int _maxCommandsPerLine = 0;
        private readonly List<string> _tokenBuffer = new List<string>(24);
        private readonly StringBuilder _tokenBuilder = new StringBuilder(256);

        public ConsoleCommandParser(int maxCommandsPerLine = DEFAULT_MAX_COMMANDS_PER_LINE)
        {
            _maxCommandsPerLine = Math.Max(1, maxCommandsPerLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commands"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool TryParse(string text, List<ConsoleCommandInvocation> commands, out string error)
        {
            ArgumentGuard.ThrowIfNull(commands, nameof(commands));

            commands.Clear();
            _tokenBuffer.Clear();
            _tokenBuilder.Clear();

            error = string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            bool inQuotes = false;
            bool escaped = false;
            bool tokenStarted = false;

            int commandStart = 0;
            int commandIndex = 0;
            int commandEnd = text.Length;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (escaped)
                {
                    _tokenBuilder.Append(Unescape(c));
                    tokenStarted = true;
                    escaped = false;
                    continue;
                }

                if (!inQuotes && c == '/' && i + 1 < text.Length && text[i + 1] == '/')
                {
                    commandEnd = i;
                    break;
                }

                if (c == '\\')
                {
                    escaped = true;
                    tokenStarted = true;
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    tokenStarted = true;
                    continue;
                }

                if (!inQuotes && c == ';')
                {
                    FlushToken(ref tokenStarted);

                    if (!TryAppendCommand(text, commandStart, i, commandIndex, commands, out error))
                    {
                        return false;
                    }

                    if (commands.Count > _maxCommandsPerLine)
                    {
                        error = $"Too many commands in one line. Maximum is {_maxCommandsPerLine}.";
                        return false;
                    }

                    _tokenBuffer.Clear();
                    _tokenBuilder.Clear();

                    commandStart = i + 1;
                    commandIndex++;
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    FlushToken(ref tokenStarted);
                    continue;
                }

                _tokenBuilder.Append(c);
                tokenStarted = true;
            }

            if (escaped)
            {
                _tokenBuilder.Append('\\');
                tokenStarted = true;
            }

            if (inQuotes)
            {
                error = "Unterminated quoted string.";
                return false;
            }

            FlushToken(ref tokenStarted);

            if (!TryAppendCommand(text, commandStart, commandEnd, commandIndex, commands, out error))
            {
                return false;
            }

            if (commands.Count > _maxCommandsPerLine)
            {
                error = $"Too many commands in one line. Maximum is {_maxCommandsPerLine}.";
                return false;
            }

            return true;
        }

        private void FlushToken(ref bool tokenStarted)
        {
            if (!tokenStarted)
            {
                return;
            }

            _tokenBuffer.Add(_tokenBuilder.ToString());
            _tokenBuilder.Clear();
            tokenStarted = false;
        }

        private bool TryAppendCommand(
            string sourceText,
            int start,
            int end,
            int commandIndex,
            List<ConsoleCommandInvocation> commands,
            out string error
        )
        {
            error = string.Empty;

            if (_tokenBuffer.Count == 0)
            {
                return true;
            }

            string commandName = NormalizeCommandName(_tokenBuffer[0]);

            if (commandName.Length == 0)
            {
                return true;
            }

            if (commandName.IndexOfAny(new[] { ' ', '\t', '\r', '\n', ';' }) >= 0)
            {
                error = $"Invalid command name '{commandName}'.";
                return false;
            }

            string[] arguments = new string[_tokenBuffer.Count - 1];

            for (int i = 1; i < _tokenBuffer.Count; i++)
            {
                arguments[i - 1] = _tokenBuffer[i];
            }

            string rawText = sourceText
                .Substring(start, Math.Max(0, end - start))
                .Trim();

            commands.Add(new ConsoleCommandInvocation(
                commandName,
                rawText,
                arguments,
                commandIndex
            ));

            return true;
        }

        private static string NormalizeCommandName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            name = name.Trim();

            if (name.Length > 0 && name[0] == '/')
            {
                name = name.Substring(1);
            }

            return name.ToLowerInvariant();
        }

        private static char Unescape(char c)
        {
            return c switch
            {
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                '"' => '"',
                '\\' => '\\',
                _ => c
            };
        }
    }
}