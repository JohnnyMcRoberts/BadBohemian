using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLiveCharts.Utilities
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

            ChartTypeAttribute attr =
                Attribute.GetCustomAttribute(field, typeof(ChartTypeAttribute)) as ChartTypeAttribute;

            return attr?.Title;
        }

        public static Type GetGeneratorClass(this Enum value)
        {
            FieldInfo field;
            if (GetFieldInfo(value, out field))
                return null;

            ChartTypeAttribute attr =
                Attribute.GetCustomAttribute(field, typeof(ChartTypeAttribute)) as ChartTypeAttribute;

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
