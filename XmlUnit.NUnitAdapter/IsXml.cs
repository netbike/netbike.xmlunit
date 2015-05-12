namespace NetBike.XmlUnit.NUnitAdapter
{
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Schema;

    public static class IsXml
    {
        public static XmlCompareConstraint Similar(TextReader expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Similar, expectedValue);
        }

        public static XmlCompareConstraint Similar(string expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Similar, expectedValue);
        }

        public static XmlCompareConstraint Similar(Stream expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Similar, expectedValue);
        }

        public static XmlCompareConstraint Similar(XNode expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Similar, expectedValue);
        }

        public static XmlCompareConstraint Equals(TextReader expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Equal, expectedValue);
        }

        public static XmlCompareConstraint Equals(string expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Equal, expectedValue);
        }

        public static XmlCompareConstraint Equals(Stream expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Equal, expectedValue);
        }

        public static XmlCompareConstraint Equals(XNode expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Equal, expectedValue);
        }

        public static XmlCompareConstraint Different(TextReader expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Different, expectedValue);
        }

        public static XmlCompareConstraint Different(string expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Different, expectedValue);
        }

        public static XmlCompareConstraint Different(Stream expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Different, expectedValue);
        }

        public static XmlCompareConstraint Different(XNode expectedValue)
        {
            return new XmlCompareConstraint(XmlComparisonState.Different, expectedValue);
        }

        public static XmlValidationConstraint Valid()
        {
            return new XmlValidationConstraint();
        }
    }
}