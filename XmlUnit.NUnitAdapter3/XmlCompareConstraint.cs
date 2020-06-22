namespace NetBike.XmlUnit.NUnitAdapter
{
    using global::NUnit.Framework.Constraints;
    using NetBike.XmlUnit;
    using System;
    using System.IO;
    using System.Xml.Linq;

    public class XmlCompareConstraint : Constraint
    {
        private readonly XNode expectedNode;
        private readonly XmlComparer comparer;
        private readonly XmlComparisonState expectedState;
        private IXmlComparisonFormatter formatter;

        public XmlCompareConstraint(XmlComparisonState expectedState, object expected)
        {
            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            this.expectedNode = GetXNode(expected);
            this.expectedState = expectedState;

            this.comparer = new XmlComparer();

            this.UseAnalizer(XmlGlobalsSettings.Analyzer)
                .UseHandler(XmlGlobalsSettings.CompareHandler)
                .UseMatcher(XmlGlobalsSettings.NodeMatcher)
                .UseFormatter(XmlGlobalsSettings.Formatter);
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }

            var actualNode = GetXNode(actual);

            var result = this.comparer.Compare(this.expectedNode, actualNode);

            return new XmlCompareConstraintResult(this, actual, expectedState, result, formatter);
        }

        public XmlCompareConstraint UseAnalizer(IXmlAnalyzer analyzer)
        {
            this.comparer.Analyzer = new XmlCustomAnalyzer(analyzer);
            return this;
        }

        public XmlCompareConstraint UseMatcher(Func<XNode, XNode, bool> nodeSelector)
        {
            this.comparer.NodeMatcher = new XmlNodeMatcher(nodeSelector);
            return this;
        }

        public XmlCompareConstraint UseMatcher(IXmlNodeMatcher nodeMatcher)
        {
            this.comparer.NodeMatcher = nodeMatcher;
            return this;
        }

        public XmlCompareConstraint UseHandler(XmlCompareHandler compareHandler)
        {
            this.comparer.Handler = compareHandler;
            return this;
        }

        public XmlCompareConstraint UseFormatter(IXmlComparisonFormatter formatter)
        {
            this.formatter = formatter;
            return this;
        }

        public XmlCompareConstraint WithState(XmlComparisonState comparisonState, XmlComparisonType comparisonType)
        {
            var customAnalyzer = (XmlCustomAnalyzer)this.comparer.Analyzer;
            customAnalyzer.SetState(comparisonState, comparisonType);
            return this;
        }

        public XmlCompareConstraint WithIgnore(XmlComparisonType comparisonType)
        {
            return this.WithState(XmlComparisonState.Equal, comparisonType);
        }

        public XmlCompareConstraint WithIgnoreComment(bool ignoreComment = true)
        {
            this.comparer.IgnoreComments = ignoreComment;
            return this;
        }

        public XmlCompareConstraint WithIgnoreProcessingInstructions(bool ignoreProcessingInstructions = true)
        {
            this.comparer.IgnoreProcessingInstructions = ignoreProcessingInstructions;
            return this;
        }

        public XmlCompareConstraint WithIgnoreDocumentType(bool ignoreDocumentType = true)
        {
            this.comparer.IgnoreDocumentTypes = ignoreDocumentType;
            return this;
        }

        public XmlCompareConstraint WithIgnoreDeclaration(bool ignoreDeclaration = true)
        {
            this.comparer.IgnoreDeclarations = ignoreDeclaration;
            return this;
        }

        public XmlCompareConstraint WithNormalizeText(bool normalizeText = true)
        {
            this.comparer.NormalizeText = normalizeText;
            return this;
        }

        private static XNode GetXNode(object value)
        {
            if (value == null)
            {
                return null;
            }

            var valueNode = value as XNode;

            if (valueNode == null)
            {
                if (value is TextReader)
                {
                    valueNode = XDocument.Load((TextReader)value, LoadOptions.SetLineInfo);
                }
                else if (value is string)
                {
                    valueNode = XDocument.Load(new StringReader((string)value), LoadOptions.SetLineInfo);
                }
                else if (value is Stream)
                {
                    var streamReader = new StreamReader((Stream)value);
                    valueNode = XDocument.Load(streamReader, LoadOptions.SetLineInfo);
                }
                else
                {
                    throw new XmlCompareException(string.Format("Value type \"{0}\" for XML comparison is invalid .", value.GetType()));
                }
            }

            return valueNode;
        }

        private static bool ResolveState(XmlComparisonState expectedState, XmlComparisonState actualState)
        {
            if (expectedState == XmlComparisonState.Similar)
            {
                return actualState != XmlComparisonState.Different;
            }

            return expectedState == actualState;
        }
    }
}
