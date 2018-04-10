// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridTypeAttribute.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The grid enum extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.Utilities
{
    using System;
    using System.Reflection;

    public static class EnumExtensions
    {
        public static string GetTitle(this Enum value)
        {
            FieldInfo field;
            if (GetFieldInfo(value, out field))
                return null;

            GridTypeAttribute attr =
                Attribute.GetCustomAttribute(field, typeof(GridTypeAttribute)) as GridTypeAttribute;

            return attr?.Title;
        }

        public static Type GetGeneratorClass(this Enum value)
        {
            FieldInfo field;
            if (GetFieldInfo(value, out field))
                return null;

            GridTypeAttribute attr =
                Attribute.GetCustomAttribute(field, typeof(GridTypeAttribute)) as GridTypeAttribute;

            return attr?.GeneratorClass;
        }

        private static bool GetFieldInfo(Enum value, out FieldInfo field)
        {
            field = null;
            Type type = value.GetType();

            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return true;
            }

            field = type.GetField(name);
            if (field == null)
            {
                return true;
            }

            return false;
        }
    }
}