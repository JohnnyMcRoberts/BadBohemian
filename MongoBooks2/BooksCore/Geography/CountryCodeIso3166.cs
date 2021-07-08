namespace BooksCore.Geography
{
    using BooksCore.Base;

    public class CountryCodeIso3166 : BaseMongoEntity
    {
        public string Name { get; set; }
        public string Alpha_2 { get; set; }
        public string Alpha_3 { get; set; }
        public int CountryCode { get; set; }
        public string Iso_3166_2 { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string IntermediateRegion { get; set; }
        public int RegionCode { get; set; }
        public int SubRegionCode { get; set; }
        public int IntermediateRegionCode { get; set; }

        /// <summary>
        /// Gets the name to use for equivalence checks.
        /// </summary>
        public override string EquivalenceName => Name + " " + Alpha_2 + " " + Alpha_3;

    }
}
