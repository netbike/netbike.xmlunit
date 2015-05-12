namespace NetBike.XmlUnit
{
    using System;

    public sealed class XmlComparison
    {
        private readonly XmlComparisonType comparisonType;
        private readonly XmlComparisonDetails actualDetails;
        private readonly XmlComparisonDetails expectedDetails;

        public XmlComparison(
            XmlComparisonType comparisonType,
            XmlComparisonDetails expectedDetails,
            XmlComparisonDetails actualDetails)
        {
            if (expectedDetails == null)
            {
                throw new ArgumentNullException("expectedDetails");
            }

            if (actualDetails == null)
            {
                throw new ArgumentNullException("actualDetails");
            }

            this.comparisonType = comparisonType;
            this.actualDetails = actualDetails;
            this.expectedDetails = expectedDetails;
        }

        public XmlComparisonType ComparisonType
        {
            get { return this.comparisonType; }
        }

        public XmlComparisonDetails ExpectedDetails
        {
            get { return this.expectedDetails; }
        }

        public XmlComparisonDetails ActualDetails
        {
            get { return this.actualDetails; }
        }

        public override string ToString()
        {
            return this.ToString(XmlComparisonFormatter.Default);
        }

        public string ToString(IXmlComparisonFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            return formatter.ToString(this);
        }
    }
}