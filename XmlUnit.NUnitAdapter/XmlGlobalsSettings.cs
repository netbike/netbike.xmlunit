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
            get
            {
                return compareHandler;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                compareHandler = value;
            }
        }

        public static IXmlAnalyzer Analyzer
        {
            get
            {
                if (analyzer == null)
                {
                    analyzer = XmlAnalyzer.Default;
                }

                return analyzer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                analyzer = value;
            }
        }

        public static IXmlNodeMatcher NodeMatcher
        {
            get
            {
                if (nodeMatcher == null)
                {
                    nodeMatcher = XmlNodeMatcher.ByLocalName;
                }

                return nodeMatcher;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                nodeMatcher = value;
            }
        }

        public static IXmlComparisonFormatter Formatter
        {
            get
            {
                if (formatter == null)
                {
                    formatter = new NUnitComparisonFormatter();
                }

                return formatter;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                formatter = value;
            }
        }

        public static XmlResolver XmlResolver
        {
            get
            {
                if (xmlResolver == null)
                {
                    xmlResolver = new XmlUrlResolver();
                }

                return xmlResolver;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                xmlResolver = value;
            }
        }
    }
}