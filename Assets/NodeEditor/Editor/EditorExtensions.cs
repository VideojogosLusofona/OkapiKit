using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace NodeEditor
{

    public static class SerializedPropertyExtensions
    {
        public static bool IsListOfT<T>(this SerializedProperty property)
        {
            // Check if this property is an array
            if (property.isArray)
            {
                // Get the full type of the field this property represents
                Type fieldType = GetSerializedPropertyType(property);

                // Check if the field type is a generic list
                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type elementType = fieldType.GetGenericArguments()[0];
                    return typeof(T).IsAssignableFrom(elementType);
                }
                // Check if the field type is an array
                else if (fieldType.IsArray)
                {
                    Type elementType = fieldType.GetElementType();
                    return typeof(T).IsAssignableFrom(elementType);
                }
            }
            return false;
        }

        // Helper method to extract a System.Type from a SerializedProperty
        public static Type GetSerializedPropertyType(SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var targetType = targetObject.GetType();
            var fieldInfo = GetFieldInfoFromPropertyPath(targetType, property.propertyPath);
            return fieldInfo?.FieldType;
        }

        // Recursive method to process complex property paths
        private static FieldInfo GetFieldInfoFromPropertyPath(Type type, string path)
        {
            FieldInfo field = null;
            var parts = path.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                field = type.GetField(parts[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (field == null)
                {
                    // Handle the array and list elements case
                    if (parts[i] == "Array" && i < parts.Length - 1 && parts[i + 1].StartsWith("data["))
                    {
                        // Move past the "Array" and "data[x]" parts of the path
                        i++; // Skip the "Array"
                        if (field.FieldType.IsArray)
                            type = field.FieldType.GetElementType();
                        else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                            type = field.FieldType.GetGenericArguments()[0];
                        else
                            throw new Exception("Error accessing array data");
                    }
                }
                else
                {
                    type = field.FieldType;
                }
            }
            return field;
        }

        public static bool IsDerivedFrom(this Type t, Type baseType)
        {
            return baseType.IsAssignableFrom(t);
        }

        // Utility function to get the value of a SerializedProperty
        public static object GetSerializedPropertyValue(this SerializedProperty prop)
        {
            if (prop == null) return null;

            switch (prop.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return GetTargetObjectOfProperty(prop);
                default:
                    return prop.managedReferenceValue;
            }
        }

        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext()) return null;
            }
            return enumerator.Current;
        }

        public static FieldInfo GetFieldInfo(this Type classType, string fieldName)
        {
            var fieldInfo = classType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            if (fieldInfo == null)
            {
                Type parentType = classType.BaseType;
                if (parentType != null)
                {
                    return parentType.GetFieldInfo(fieldName);
                }

                return null;
            }

            return fieldInfo;
        }
    }
}
