namespace NetBike.XmlUnit.NUnitAdapter
{
    using System;
    using System.Xml;

    public static class XmlGlobalsSettings
    {
        private static IXmlComparisonFormatter formatter;
        private static IXmlAnalyzer analyzer;
        private static IXmlNodeMatcher nodeMatcher;
        private static XmlCompareHandler compareHandler = XmlCompareHandling.Default;
        private static XmlResolver xmlResolver;

        public static XmlCompareHandler CompareHandler
        {
            get => compareHandler;

            set => compareHandler = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static IXmlAnalyzer Analyzer
        {
            get => analyzer ?? (analyzer = XmlAnalyzer.Default);

            set => analyzer = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static IXmlNodeMatcher NodeMatcher
        {
            get => nodeMatcher ?? (nodeMatcher = XmlNodeMatcher.ByLocalName);

            set => nodeMatcher = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static IXmlComparisonFormatter Formatter
        {
            get => formatter ?? (formatter = new NUnitComparisonFormatter());

            set => formatter = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static XmlResolver XmlResolver
        {
            get => xmlResolver ?? (xmlResolver = new XmlUrlResolver());

            set => xmlResolver = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}