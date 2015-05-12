namespace NetBike.XmlUnit
{
    using System.Xml.Linq;

    public class XmlAnalyzer : IXmlAnalyzer
    {
        private static XmlAnalyzer defaultAnalyzer = new XmlAnalyzer();

        public static IXmlAnalyzer Default
        {
            get { return defaultAnalyzer; }
        }

        public static XmlCustomAnalyzer Custom()
        {
            return new XmlCustomAnalyzer();
        }

        public static IXmlAnalyzer Constant(XmlComparisonState comparisonState)
        {
            return new XmlConstantAnalyzer(comparisonState);
        }

        public virtual XmlComparisonState Analyze(XmlComparison comparison)
        {
            var state = XmlComparisonState.Different;

            switch (comparison.ComparisonType)
            {
                case XmlComparisonType.Doctype:
                case XmlComparisonType.NodeListSequence:
                case XmlComparisonType.NamespacePrefix:
                case XmlComparisonType.Encoding:
                    state = XmlComparisonState.Similar;
                    break;

                case XmlComparisonType.NodeType:
                    var expectedNode = comparison.ExpectedDetails.Node as XText;
                    var actualNode = comparison.ActualDetails.Node as XText;

                    if (expectedNode != null && actualNode != null)
                    {
                        var expectedText = XmlComparer.GetNormalizedText(expectedNode.Value);
                        var actualText = XmlComparer.GetNormalizedText(actualNode.Value);

                        if (expectedText == actualText)
                        {
                            state = XmlComparisonState.Similar;
                        }
                    }

                    break;
            }

            return state;
        }

        private sealed class XmlConstantAnalyzer : IXmlAnalyzer
        {
            private readonly XmlComparisonState state;

            public XmlConstantAnalyzer(XmlComparisonState state)
            {
                this.state = state;
            }

            public XmlComparisonState Analyze(XmlComparison comparison)
            {
                return this.state;
            }
        }
    }
}