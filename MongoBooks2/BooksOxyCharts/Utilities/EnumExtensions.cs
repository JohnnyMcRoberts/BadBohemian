namespace BooksOxyCharts.Utilities
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

            PlotTypeAttribute attr = Attribute.GetCustomAttribute(field, typeof(PlotTypeAttribute)) as PlotTypeAttribute;

            return attr?.Title;
        }

        public static bool? GetCanHover(this Enum value)
        {
            FieldInfo field;
            if (GetFieldInfo(value, out field))
                return null;

            PlotTypeAttribute attr = Attribute.GetCustomAttribute(field, typeof(PlotTypeAttribute)) as PlotTypeAttribute;

            return attr?.CanHover;
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
