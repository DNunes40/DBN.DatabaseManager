using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace DBN.DatabaseManager.Abstractions.Helpers
{
    internal static class FastMapper
    {
        private static readonly ConcurrentDictionary<string, PropertySetter[]> _typeCache = new();

        public static object? MapReader(IDataReader reader, Type type)
        {
            // List object
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = type.GetGenericArguments()[0];
                var list = (IList?)Activator.CreateInstance(type) ?? throw new DatabaseManagerException($"Could not create instance of {type}");

                var setters = GetSetters(elementType, reader);

                while (reader.Read())
                {
                    var obj = Activator.CreateInstance(elementType);

                    foreach (var setter in setters)
                    {
                        if (setter.ColumnIndex >= reader.FieldCount)
                        {
                            throw new DatabaseManagerException($"Column index {setter.ColumnIndex} out of range for reader with {reader.FieldCount} columns.");
                        }

                        object rawValue = reader[setter.ColumnIndex];
                        object? converted = ConvertValue(rawValue, setter.PropertyType, setter.PropertyInfo);

                        if (converted != null && setter?.Setter != null && obj != null)
                        {
                            setter.Setter(obj, converted);
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }

            // Single object
            if (reader.Read())
            {
                var setters = GetSetters(type, reader);
                var obj = Activator.CreateInstance(type);

                foreach (var setter in setters)
                {
                    if (setter.ColumnIndex >= reader.FieldCount)
                    {
                        throw new DatabaseManagerException($"Column index {setter.ColumnIndex} out of range for reader with {reader.FieldCount} columns.");
                    }

                    object rawValue = reader[setter.ColumnIndex];
                    object? converted = ConvertValue(rawValue, setter.PropertyType, setter.PropertyInfo);

                    if (converted != null && setter?.Setter != null && obj != null)
                    {
                        setter.Setter(obj, converted);
                    }
                }

                return obj;
            }

            return null;
        }

        private static string GetSchemaKey(Type type, IDataReader reader)
        {
            var columns = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetName(i).ToLowerInvariant());
            return $"{type.FullName}:{string.Join(",", columns)}";
        }

        private static PropertySetter[] GetSetters(Type type, IDataReader reader)
        {
            var key = GetSchemaKey(type, reader);

            if (!_typeCache.TryGetValue(key, out var setters))
            {
                setters = BuildSetters(type, reader);
                _typeCache[key] = setters;
            }

            return setters;
        }

        private static PropertySetter[] BuildSetters(Type type, IDataReader reader)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite)
                            .ToArray();

            var setters = new List<PropertySetter>();

            // Determine if the class wants to ignore missing columns
            bool ignoreMissingColumns = type.GetCustomAttribute<IgnoreMissingColumnsAttribute>() != null;

            // Create a lookup for faster matching
            var columnNames = Enumerable.Range(0, reader.FieldCount)
                                        .Select(i => new { Index = i, Name = reader.GetName(i) })
                                        .ToList();

            foreach (var prop in props)
            {
                //Skip complex or collection types (except primitive-like ones)
                var propType = prop.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

                if (typeof(IEnumerable).IsAssignableFrom(underlyingType) && underlyingType != typeof(string))
                {
                    // Ignore lists, arrays, collections — but NOT strings
                    continue;
                }

                if (underlyingType.IsClass && underlyingType != typeof(string))
                {
                    // Ignore nested object types (complex classes)
                    continue;
                }

                // Try to get [MapsTo] or fall back to property name
                var mapAttr = prop.GetCustomAttribute<MapsToAttribute>();
                string targetColumn = mapAttr?.ColumnName ?? prop.Name;

                // Check if the column exists (case-insensitive)
                var col = columnNames.FirstOrDefault(c =>
                    string.Equals(c.Name, targetColumn, StringComparison.OrdinalIgnoreCase));

                if (col != null)
                {
                    var setterDelegate = CreateSetter(type, prop);

                    setters.Add(new PropertySetter
                    {
                        ColumnIndex = col.Index,
                        PropertyType = prop.PropertyType,
                        Setter = setterDelegate,
                        PropertyInfo = prop
                    });
                }
                else
                {
                    // If column not found, check if we should skip
                    bool ignoreForProp = prop.GetCustomAttribute<IgnoreIfMissingAttribute>() != null;

                    if (ignoreMissingColumns || ignoreForProp)
                    {
                        continue; // silently skip missing column
                    }

                    // Otherwise, this is considered an error
                    throw new DatabaseManagerException($"Column '{targetColumn}' not found for property '{prop.Name}' in type '{type.Name}'.");
                }
            }

            return [.. setters];
        }

        private static Action<object, object> CreateSetter(Type type, PropertyInfo prop)
        {
            var targetExp = Expression.Parameter(typeof(object), "target");
            var valueExp = Expression.Parameter(typeof(object), "value");

            var convertedTarget = Expression.Convert(targetExp, type);
            var convertedValue = Expression.Convert(
                Expression.Convert(valueExp, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType),
                prop.PropertyType
            );

            var body = Expression.Assign(Expression.Property(convertedTarget, prop), convertedValue);

            return Expression.Lambda<Action<object, object>>(body, targetExp, valueExp).Compile();
        }

        private static object? ConvertValue(object? value, Type targetType, PropertyInfo? prop = null)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            // Handle nullable types
            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            string? format = null;

            if (nonNullableType == typeof(DateTime))
            {
                format = prop?.GetCustomAttribute<DBFormatAttribute>()?.Format;
            }

            // Get the internal static ConvertValueAuto<T> method
            var method = typeof(DatabaseExtensions).GetMethod(nameof(DatabaseExtensions.ConvertValueAuto), BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                throw new InvalidOperationException("ConvertValueAuto method not found.");
            }

            // Make it generic for the runtime type
            var genericMethod = method.MakeGenericMethod(nonNullableType);

            // Invoke with the value & format when present
            var result = genericMethod.Invoke(null, [value, format]);

            return result;
        }

        private class PropertySetter
        {
            public int ColumnIndex;
            public Type PropertyType = null!;
            public Action<object, object>? Setter;
            public PropertyInfo PropertyInfo = null!;
        }
    }
}
