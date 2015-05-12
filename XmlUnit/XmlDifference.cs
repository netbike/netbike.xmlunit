namespace NetBike.XmlUnit
{
    using System;

    public sealed class XmlDifference
    {
        private readonly XmlComparisonState state;
        private readonly XmlComparison difference;

        public XmlDifference(XmlComparisonState state, XmlComparison difference)
        {
            if (difference == null)
            {
                throw new ArgumentNullException("difference");
            }

            this.state = state;
            this.difference = difference;
        }

        public XmlComparisonState State
        {
            get { return this.state; }
        }

        public XmlComparison Difference
        {
            get { return this.difference; }
        }

        public override string ToString()
        {
            return this.ToString(XmlComparisonFormatter.Default);
        }

        private string ToString(IXmlComparisonFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            return formatter.ToString(this);
        }
    }
}