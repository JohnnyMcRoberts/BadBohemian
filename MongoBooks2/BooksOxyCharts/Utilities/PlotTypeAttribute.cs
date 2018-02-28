namespace BooksOxyCharts.Utilities
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class PlotTypeAttribute : Attribute
    {
        public string Title { get; set; }

        public bool CanHover { get; set; }

        public Type GeneratorClass { get; set; }

        public PlotTypeAttribute()
        {
            Title = string.Empty;
            CanHover = false;
        }
    }
}
