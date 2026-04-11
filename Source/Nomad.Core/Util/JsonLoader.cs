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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Nomad.Core.Compatibility.Guards;

namespace Nomad.Core.Util
{
    /// <summary>
    /// Shared helpers for reading JSON values with the project's permissive parsing settings.
    /// </summary>
    public static class JsonLoader
    {
        private delegate object ScalarReader(JsonElement element, string valueName);

        private static readonly JsonDocumentOptions _documentOptions = new()
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        private static readonly IReadOnlyDictionary<Type, ScalarReader> _scalarReaders = new Dictionary<Type, ScalarReader>
        {
            { typeof(bool), ReadBoolean },
            { typeof(sbyte), ReadSByte },
            { typeof(byte), ReadByte },
            { typeof(short), ReadInt16 },
            { typeof(ushort), ReadUInt16 },
            { typeof(int), ReadInt32 },
            { typeof(uint), ReadUInt32 },
            { typeof(long), ReadInt64 },
            { typeof(ulong), ReadUInt64 },
            { typeof(float), ReadSingle },
            { typeof(double), ReadDouble },
            { typeof(decimal), ReadDecimal },
            { typeof(char), ReadChar },
            { typeof(string), ReadString }
        };

        /// <summary>
        /// Parses a JSON document using Nomad's default loader options.
        /// </summary>
        /// <param name="stream">The UTF-8 JSON stream to parse.</param>
        /// <returns>A parsed JSON document.</returns>
        public static JsonDocument Parse(Stream stream)
        {
            ArgumentGuard.ThrowIfNull(stream);
            return JsonDocument.Parse(stream, _documentOptions);
        }

        /// <summary>
        /// Attempts to find a property on an object using a case-insensitive match.
        /// </summary>
        /// <param name="element">The JSON object to search.</param>
        /// <param name="propertyName">The property name to find.</param>
        /// <param name="propertyValue">The matched property value.</param>
        /// <returns>True when the property exists; otherwise false.</returns>
        public static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement propertyValue)
        {
            ArgumentGuard.ThrowIfNull(propertyName);

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        propertyValue = property.Value;
                        return true;
                    }
                }
            }

            propertyValue = default;
            return false;
        }

        /// <summary>
        /// Attempts to read a property as the requested type.
        /// </summary>
        /// <typeparam name="T">The requested property type.</typeparam>
        /// <param name="element">The JSON object that owns the property.</param>
        /// <param name="propertyName">The property name to read.</param>
        /// <param name="value">The parsed value when found.</param>
        /// <returns>True when the property exists; otherwise false.</returns>
        public static bool TryGet<T>(JsonElement element, string propertyName, [MaybeNullWhen(false)] out T value)
        {
            if (!TryGetProperty(element, propertyName, out JsonElement propertyValue))
            {
                value = default;
                return false;
            }

            value = Read<T>(propertyValue, propertyName);
            return true;
        }

        /// <summary>
        /// Reads a required property as the requested type.
        /// </summary>
        /// <typeparam name="T">The requested property type.</typeparam>
        /// <param name="element">The JSON object that owns the property.</param>
        /// <param name="propertyName">The property name to read.</param>
        /// <returns>The parsed value.</returns>
        public static T GetRequired<T>(JsonElement element, string propertyName)
        {
            if (!TryGetProperty(element, propertyName, out JsonElement propertyValue))
            {
                throw new Exception($"JSON object is missing property '{propertyName}'.");
            }

            return Read<T>(propertyValue, propertyName);
        }

        /// <summary>
        /// Reads an optional property as the requested type.
        /// </summary>
        /// <typeparam name="T">The requested property type.</typeparam>
        /// <param name="element">The JSON object that owns the property.</param>
        /// <param name="propertyName">The property name to read.</param>
        /// <param name="defaultValue">The value to return when the property is absent.</param>
        /// <returns>The parsed value or the provided default.</returns>
        public static T GetOptional<T>(JsonElement element, string propertyName, T defaultValue)
        {
            if (!TryGetProperty(element, propertyName, out JsonElement propertyValue))
            {
                return defaultValue;
            }

            return Read<T>(propertyValue, propertyName);
        }

        /// <summary>
        /// Reads a JSON value as the requested type.
        /// </summary>
        /// <typeparam name="T">The requested value type.</typeparam>
        /// <param name="element">The JSON value to read.</param>
        /// <param name="valueName">A descriptive name used in error messages.</param>
        /// <returns>The parsed value.</returns>
        public static T Read<T>(JsonElement element, string valueName)
        {
            object value = ReadValue(typeof(T), element, valueName);
            return (T)value;
        }

        /// <summary>
        /// Reads a required array property.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="element">The JSON object that owns the property.</param>
        /// <param name="propertyName">The array property name.</param>
        /// <returns>The parsed array.</returns>
        public static T[] GetRequiredArray<T>(JsonElement element, string propertyName)
        {
            if (!TryGetProperty(element, propertyName, out JsonElement propertyValue))
            {
                throw new Exception($"JSON object is missing property '{propertyName}'.");
            }
            return ReadArray<T>(propertyValue, propertyName);
        }

        /// <summary>
        /// Reads an optional array property.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="element">The JSON object that owns the property.</param>
        /// <param name="propertyName">The array property name.</param>
        /// <param name="defaultValue">The value to return when the property is absent.</param>
        /// <returns>The parsed array or the provided default.</returns>
        public static T[] GetOptionalArray<T>(JsonElement element, string propertyName, T[]? defaultValue = null)
        {
            if (!TryGetProperty(element, propertyName, out JsonElement propertyValue))
            {
                return defaultValue ?? Array.Empty<T>();
            }
            return ReadArray<T>(propertyValue, propertyName);
        }

        /// <summary>
        /// Reads a JSON array value as the requested element type.
        /// </summary>
        /// <typeparam name="T">The array element type.</typeparam>
        /// <param name="element">The JSON array value.</param>
        /// <param name="valueName">A descriptive name used in error messages.</param>
        /// <returns>The parsed array.</returns>
        public static T[] ReadArray<T>(JsonElement element, string valueName)
        {
            return (T[])ReadArray(typeof(T), element, valueName);
        }

        private static object ReadValue(Type targetType, JsonElement element, string valueName)
        {
            if (targetType.IsArray)
            {
                if (targetType.GetArrayRank() != 1)
                {
                    throw new NotSupportedException($"JSON loader does not support multi-dimensional arrays for '{valueName}'.");
                }

                Type elementType = targetType.GetElementType() ??
                    throw new NotSupportedException($"JSON loader could not determine the array element type for '{valueName}'.");

                return ReadArray(elementType, element, valueName);
            }

            if (_scalarReaders.TryGetValue(targetType, out ScalarReader? reader))
            {
                return reader(element, valueName);
            }
            if (targetType.IsEnum)
            {
                return ReadEnum(targetType, element, valueName);
            }
            throw new NotSupportedException($"JSON loader does not support values of type '{targetType.FullName}'.");
        }

        private static object ReadBoolean(JsonElement element, string valueName)
        {
            if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                return element.GetBoolean();
            }
            throw CreateTypeException(valueName, "a boolean");
        }

        private static object ReadSByte(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetSByte(out sbyte value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a signed byte");
        }

        private static object ReadByte(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetByte(out byte value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "an unsigned byte");
        }

        private static object ReadInt16(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt16(out short value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a 16-bit integer");
        }

        private static object ReadUInt16(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetUInt16(out ushort value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "an unsigned 16-bit integer");
        }

        private static object ReadInt32(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out int value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a 32-bit integer");
        }

        private static object ReadUInt32(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetUInt32(out uint value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "an unsigned 32-bit integer");
        }

        private static object ReadInt64(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt64(out long value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a 64-bit integer");
        }

        private static object ReadUInt64(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetUInt64(out ulong value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "an unsigned 64-bit integer");
        }

        private static object ReadSingle(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetSingle(out float value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a single-precision floating point number");
        }

        private static object ReadDouble(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out double value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a double-precision floating point number");
        }

        private static object ReadDecimal(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out decimal value))
            {
                return value;
            }
            throw CreateTypeException(valueName, "a decimal number");
        }

        private static object ReadChar(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                string? value = element.GetString();
                if (!string.IsNullOrEmpty(value) && value.Length == 1)
                {
                    return value[0];
                }
            }
            throw CreateTypeException(valueName, "a single character string");
        }

        private static object ReadString(JsonElement element, string valueName)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                return element.GetString() ?? throw new Exception($"JSON value '{valueName}' contained a null string.");
            }
            throw CreateTypeException(valueName, "a string");
        }

        private static object ReadEnum(Type targetType, JsonElement element, string valueName)
        {
            if (element.ValueKind != JsonValueKind.String)
            {
                throw CreateTypeException(valueName, "a string");
            }

            string value = element.GetString() ?? throw new Exception($"JSON value '{valueName}' contained a null string.");
            if (Enum.TryParse(targetType, value, true, out object? result))
            {
                return result;
            }
            throw new Exception($"JSON value '{valueName}' has invalid enum value '{value}'.");
        }

        private static Array ReadArray(Type elementType, JsonElement element, string valueName)
        {
            if (element.ValueKind != JsonValueKind.Array)
            {
                throw CreateTypeException(valueName, "an array");
            }

            int length = element.GetArrayLength();
            Array values = Array.CreateInstance(elementType, length);

            int index = 0;
            foreach (JsonElement arrayElement in element.EnumerateArray())
            {
                values.SetValue(ReadValue(elementType, arrayElement, $"{valueName}[{index}]"), index);
                index++;
            }
            return values;
        }

        private static Exception CreateTypeException(string valueName, string expectedType)
        {
            return new Exception($"JSON value '{valueName}' must be {expectedType}.");
        }
    }
}
