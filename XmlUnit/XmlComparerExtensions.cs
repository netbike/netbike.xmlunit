namespace NetBike.XmlUnit
{
    using System;
    using System.IO;

    public static class XmlComparerExtensions
    {
        public static XmlComparisonResult Compare(this IXmlComparer comparer, Stream expectedStream, Stream actualStream)
        {
            if (expectedStream == null)
            {
                throw new ArgumentNullException("expectedStream");
            }

            if (actualStream == null)
            {
                throw new ArgumentNullException("actualStream");
            }

            using (var expectedReader = new StreamReader(expectedStream))
            using (var actualReader = new StreamReader(actualStream))
            {
                return comparer.Compare(expectedReader, actualReader);
            }
        }

        public static XmlComparisonResult Compare(this IXmlComparer comparer, string expectedXml, string actualXml)
        {
            if (expectedXml == null)
            {
                throw new ArgumentNullException("expectedXml");
            }

            if (actualXml == null)
            {
                throw new ArgumentNullException("actualXml");
            }

            using (var expectedReader = new StringReader(expectedXml))
            using (var actualReader = new StringReader(actualXml))
            {
                return comparer.Compare(expectedReader, actualReader);
            }
        }
    }
}