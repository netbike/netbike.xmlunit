namespace NetBike.XmlUnit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;

    public class XmlComparer : IXmlComparer
    {
        private IXmlAnalyzer analyzer;

        private IXmlNodeMatcher nodeMatcher;

        public XmlComparer()
        {
            this.Analyzer = XmlAnalyzer.Default;
            this.NodeMatcher = XmlNodeMatcher.ByLocalName;
            this.Handler = XmlCompareHandling.Default;
            this.IgnoreComments = true;
            this.NormalizeText = true;
            this.IgnoreProcessingInstructions = true;
        }

        public bool NormalizeText { get; set; }

        public bool IgnoreDeclarations { get; set; }

        public bool IgnoreProcessingInstructions { get; set; }

        public bool IgnoreDocumentTypes { get; set; }

        public bool IgnoreComments { get; set; }

        public IXmlAnalyzer Analyzer
        {
            get
            {
                return this.analyzer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.analyzer = value;
            }
        }

        public IXmlNodeMatcher NodeMatcher
        {
            get
            {
                return this.nodeMatcher;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.nodeMatcher = value;
            }
        }

        public XmlCompareHandler Handler { get; set; }

        public XmlComparisonResult Compare(TextReader expectedReader, TextReader actualReader)
        {
            if (expectedReader == null)
            {
                throw new ArgumentNullException("expectedReader");
            }

            if (actualReader == null)
            {
                throw new ArgumentNullException("actualReader");
            }

            var options = LoadOptions.SetLineInfo;
            var expected = XDocument.Load(expectedReader, options);
            var actual = XDocument.Load(actualReader, options);
            var context = new XmlCompareContext();

            this.CompareDocuments(context, expected, actual);

            return new XmlComparisonResult(context.FinalState, context.Differences);
        }

        public XmlComparisonResult Compare(XNode expected, XNode actual)
        {
            if (expected == null)
            {
                throw new ArgumentNullException("expected");
            }

            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }

            var context = new XmlCompareContext();

            this.CompareNodes(context, expected, actual);

            return new XmlComparisonResult(context.FinalState, context.Differences);
        }

        internal static string GetNormalizedText(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Trim();
        }

        private bool IsAcceptableType(XNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Comment:
                    return !this.IgnoreComments;

                case XmlNodeType.ProcessingInstruction:
                    return !this.IgnoreProcessingInstructions;

                case XmlNodeType.DocumentType:
                    return false;
            }

            return true;
        }

        private bool CompareDocuments(XmlCompareContext context, XDocument expected, XDocument actual)
        {
            if (!this.IgnoreDeclarations)
            {
                if (!this.CompareDeclarations(context, expected, actual))
                {
                    return false;
                }
            }

            if (!this.IgnoreDocumentTypes)
            {
                if (!this.CompareDocumentTypes(context, expected, actual))
                {
                    return false;
                }
            }

            if (!this.CompareContainers(context, expected, actual))
            {
                return false;
            }

            return true;
        }

        private bool CompareContainers(XmlCompareContext context, XContainer expectedContainer, XContainer actualContainer)
        {
            var expectedNodes = expectedContainer.Nodes().Where(this.IsAcceptableType).ToList();
            var actualNodes = actualContainer.Nodes().Where(this.IsAcceptableType).ToList();

            if (expectedNodes.Count != actualNodes.Count)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.NodeList,
                    new XmlComparisonDetails(expectedContainer, expectedNodes.Count),
                    new XmlComparisonDetails(actualContainer, actualNodes.Count));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            var matches = this.NodeMatcher.MatchNodes(expectedNodes, actualNodes);

            foreach (var match in matches)
            {
                var expectedNode = match.ExpectedNode;
                var actualNode = match.ActualNode;

                if (actualNode == null)
                {
                    var comparison = new XmlComparison(
                        XmlComparisonType.NodeListLookup,
                        new XmlComparisonDetails(expectedContainer, expectedNode),
                        new XmlComparisonDetails(actualContainer, null));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }
                else
                {
                    if (expectedNode.NodeType == XmlNodeType.Element &&
                          actualNode.NodeType == XmlNodeType.Element)
                    {
                        var expectedIndex = ((XElement)expectedNode).GetElementIndex();
                        var actualIndex = ((XElement)actualNode).GetElementIndex();

                        if (expectedIndex != actualIndex)
                        {
                            var comparison = new XmlComparison(
                                XmlComparisonType.NodeListSequence,
                                new XmlComparisonDetails(expectedNode, expectedIndex),
                                new XmlComparisonDetails(actualNode, actualIndex));

                            if (!this.HandleDifference(context, comparison))
                            {
                                return false;
                            }
                        }
                    }

                    if (!this.CompareNodes(context, expectedNode, actualNode))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CompareNodes(XmlCompareContext context, XNode expectedNode, XNode actualNode)
        {
            if (actualNode.NodeType != expectedNode.NodeType)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.NodeType,
                    new XmlComparisonDetails(expectedNode, expectedNode.NodeType),
                    new XmlComparisonDetails(actualNode, expectedNode.NodeType));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }

                return true;
            }

            switch (actualNode.NodeType)
            {
                case XmlNodeType.Document:
                    return this.CompareDocuments(context, (XDocument)expectedNode, (XDocument)actualNode);

                case XmlNodeType.ProcessingInstruction:
                    return this.CompareProcessingInstructions(context, (XProcessingInstruction)expectedNode, (XProcessingInstruction)actualNode);

                case XmlNodeType.CDATA:
                case XmlNodeType.Text:
                    return this.CompareText(context, (XText)expectedNode, (XText)actualNode);

                case XmlNodeType.Comment:
                    return this.CompareComments(context, (XComment)expectedNode, (XComment)actualNode);

                case XmlNodeType.Element:
                    return this.CompareElements(context, (XElement)expectedNode, (XElement)actualNode);
            }

            throw new XmlCompareException(string.Format("Unexpected node type \"{0}\".", actualNode.NodeType));
        }

        private bool CompareComments(XmlCompareContext context, XComment expectedComment, XComment actualComment)
        {
            if (!this.IgnoreComments)
            {
                if (!this.TextEquals(expectedComment.Value, actualComment.Value))
                {
                    var comparison = new XmlComparison(
                        XmlComparisonType.Comment,
                        new XmlComparisonDetails(expectedComment, expectedComment.Value),
                        new XmlComparisonDetails(actualComment, actualComment.Value));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CompareText(XmlCompareContext context, XText expectedText, XText actualText)
        {
            if (!this.TextEquals(expectedText.Value, actualText.Value))
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.TextValue,
                    new XmlComparisonDetails(expectedText, expectedText),
                    new XmlComparisonDetails(actualText, actualText));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareProcessingInstructions(XmlCompareContext context, XProcessingInstruction expectedInstruction, XProcessingInstruction actualInstruction)
        {
            if (!this.IgnoreProcessingInstructions)
            {
                if (expectedInstruction.Target != actualInstruction.Target)
                {
                    var comparison = new XmlComparison(
                       XmlComparisonType.ProcessingInstructionTarget,
                       new XmlComparisonDetails(expectedInstruction, expectedInstruction.Target),
                       new XmlComparisonDetails(actualInstruction, actualInstruction.Target));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }

                if (expectedInstruction.Data != actualInstruction.Data)
                {
                    var comparison = new XmlComparison(
                       XmlComparisonType.ProcessingInstructionData,
                       new XmlComparisonDetails(expectedInstruction, expectedInstruction.Data),
                       new XmlComparisonDetails(actualInstruction, actualInstruction.Data));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CompareDocumentTypes(XmlCompareContext context, XDocument expectedDocument, XDocument actualDocument)
        {
            var expectedDocumentType = expectedDocument.DocumentType;
            var actualDocumentType = actualDocument.DocumentType;

            if (expectedDocumentType == null || actualDocumentType == null)
            {
                if (expectedDocumentType != actualDocumentType)
                {
                    var comparison = new XmlComparison(
                        XmlComparisonType.Doctype,
                        new XmlComparisonDetails(expectedDocument, expectedDocumentType),
                        new XmlComparisonDetails(actualDocument, actualDocumentType));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (string.Compare(expectedDocumentType.PublicId, actualDocumentType.PublicId, StringComparison.OrdinalIgnoreCase) != 0)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.DoctypePublicId,
                    new XmlComparisonDetails(expectedDocument, expectedDocumentType.PublicId),
                    new XmlComparisonDetails(actualDocument, actualDocumentType.PublicId));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            if (string.Compare(expectedDocumentType.SystemId, actualDocumentType.SystemId, StringComparison.OrdinalIgnoreCase) != 0)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.DoctypePublicId,
                    new XmlComparisonDetails(expectedDocument, expectedDocumentType.SystemId),
                    new XmlComparisonDetails(actualDocument, actualDocumentType.SystemId));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareDeclarations(XmlCompareContext context, XDocument expectedDocument, XDocument actualDocument)
        {
            var expectedDeclaration = expectedDocument.Declaration;
            var actualDeclaration = actualDocument.Declaration;

            if (expectedDeclaration == null || actualDeclaration == null)
            {
                if (!object.ReferenceEquals(expectedDeclaration, actualDeclaration))
                {
                    var comparison = new XmlComparison(
                        XmlComparisonType.Declaration,
                        new XmlComparisonDetails(expectedDocument, expectedDeclaration != null),
                        new XmlComparisonDetails(actualDocument, actualDeclaration != null));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (expectedDeclaration.Encoding != actualDeclaration.Encoding)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.Encoding,
                    new XmlComparisonDetails(expectedDocument, expectedDeclaration.Encoding),
                    new XmlComparisonDetails(actualDocument, actualDeclaration.Encoding));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            if (expectedDeclaration.Standalone != actualDeclaration.Standalone)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.Standalone,
                    new XmlComparisonDetails(expectedDocument, expectedDeclaration.Standalone),
                    new XmlComparisonDetails(actualDocument, actualDeclaration.Standalone));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            if (expectedDeclaration.Version != actualDeclaration.Version)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.Version,
                    new XmlComparisonDetails(expectedDocument, expectedDeclaration.Version),
                    new XmlComparisonDetails(actualDocument, actualDeclaration.Version));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareElements(XmlCompareContext context, XElement expectedElement, XElement actualElement)
        {
            if (expectedElement.Name != actualElement.Name)
            {
                var comparisonType = XmlComparisonType.ElementName;

                if (expectedElement.Name.LocalName == actualElement.Name.LocalName)
                {
                    comparisonType = XmlComparisonType.NamespaceUri;
                }

                var comparison = new XmlComparison(
                    comparisonType,
                    new XmlComparisonDetails(expectedElement, expectedElement.Name),
                    new XmlComparisonDetails(actualElement, actualElement.Name));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }

                if (context.State == XmlComparisonState.Different)
                {
                    return true;
                }
            }

            if (!this.CompareAttributes(context, expectedElement, actualElement))
            {
                return false;
            }

            if (expectedElement.HasElements)
            {
                return this.CompareContainers(context, expectedElement, actualElement);
            }

            if (!this.TextEquals(expectedElement.Value, actualElement.Value))
            {
                var comparison = new XmlComparison(
                       XmlComparisonType.TextValue,
                       new XmlComparisonDetails(expectedElement, expectedElement.Value),
                       new XmlComparisonDetails(actualElement, actualElement.Value));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareAttributes(XmlCompareContext context, XElement expectedElement, XElement actualElement)
        {
            var expectedAttributes = new Attributes(expectedElement);
            var actualAttributes = new Attributes(actualElement);

            if (expectedAttributes.Items.Count != actualAttributes.Items.Count)
            {
                var comparison = new XmlComparison(
                    XmlComparisonType.AttributeList,
                    new XmlComparisonDetails(expectedElement, expectedAttributes.Items.Count),
                    new XmlComparisonDetails(actualElement, actualAttributes.Items.Count));

                if (!this.HandleDifference(context, comparison))
                {
                    return false;
                }
            }

            if (!this.CompareSchemaLocationAttribute(
                    context,
                    expectedAttributes.SchemaLocation,
                    actualAttributes.SchemaLocation,
                    expectedElement,
                    actualElement))
            {
                return false;
            }

            if (!this.CompareSchemaLocationAttribute(
                    context,
                    expectedAttributes.NoNamespaceSchemaLocation,
                    actualAttributes.NoNamespaceSchemaLocation,
                    expectedElement,
                    actualElement))
            {
                return false;
            }

            foreach (var expectedAttribute in expectedAttributes.Declarations)
            {
                var actualAttribute = actualAttributes.Declarations.FirstOrDefault(x => x.Name == expectedAttribute.Name);

                if (actualAttribute == null)
                {
                    actualAttribute = actualAttributes.Declarations.FirstOrDefault(x => x.Value == expectedAttribute.Value);

                    if (actualAttribute != null)
                    {
                        var comparison = new XmlComparison(
                            XmlComparisonType.NamespacePrefix,
                            new XmlComparisonDetails(expectedElement, expectedAttribute),
                            new XmlComparisonDetails(actualElement, actualAttribute));

                        if (!this.HandleDifference(context, comparison))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        var comparison = new XmlComparison(
                            XmlComparisonType.NamespaceUri,
                            new XmlComparisonDetails(expectedElement, expectedAttribute),
                            new XmlComparisonDetails(actualElement, actualAttribute));

                        if (!this.HandleDifference(context, comparison))
                        {
                            return false;
                        }
                    }
                }
            }

            foreach (var expectedAttribute in expectedAttributes.Items)
            {
                var actualAttribute = actualAttributes.Items.FirstOrDefault(x => x.Name == expectedAttribute.Name);

                if (actualAttribute == null)
                {
                    actualAttribute = actualAttributes.Items.FirstOrDefault(x => x.Name.LocalName == expectedAttribute.Name.LocalName);

                    if (actualAttribute != null)
                    {
                        var comparison = new XmlComparison(
                            XmlComparisonType.NamespaceUri,
                            new XmlComparisonDetails(expectedElement, expectedAttribute),
                            new XmlComparisonDetails(actualElement, actualAttribute));

                        if (!this.HandleDifference(context, comparison))
                        {
                            return false;
                        }
                    }
                }

                if (actualAttribute == null)
                {
                    var comparison = new XmlComparison(
                            XmlComparisonType.AttributeLookup,
                            new XmlComparisonDetails(expectedElement, expectedAttribute),
                            new XmlComparisonDetails(actualElement, actualAttribute));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }
                else if (!this.TextEquals(expectedAttribute.Value, actualAttribute.Value))
                {
                    var comparison = new XmlComparison(
                        XmlComparisonType.AttributeValue,
                        new XmlComparisonDetails(expectedElement, expectedAttribute.Value),
                        new XmlComparisonDetails(actualElement, actualAttribute.Value));

                    if (!this.HandleDifference(context, comparison))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CompareSchemaLocationAttribute(
            XmlCompareContext context,
            XAttribute expectedAttribute,
            XAttribute actualAttribute,
            XElement expectedElement,
            XElement actualElement)
        {
            XmlComparison comparison = null;

            if (expectedAttribute != null && actualAttribute != null)
            {
                if (expectedAttribute.Value != actualAttribute.Value)
                {
                    comparison = new XmlComparison(
                        XmlComparisonType.SchemaLocation,
                        new XmlComparisonDetails(expectedElement, expectedAttribute),
                        new XmlComparisonDetails(actualElement, expectedAttribute));
                }
            }
            else if (!object.ReferenceEquals(expectedAttribute, actualAttribute))
            {
                comparison = new XmlComparison(
                    XmlComparisonType.SchemaLocation,
                    new XmlComparisonDetails(expectedElement, expectedAttribute != null),
                    new XmlComparisonDetails(actualElement, expectedAttribute != null));
            }

            return comparison == null || this.HandleDifference(context, comparison);
        }

        private bool TextEquals(string expectedText, string actualText)
        {
            if (this.NormalizeText)
            {
                expectedText = GetNormalizedText(expectedText);
                actualText = GetNormalizedText(actualText);
            }

            return expectedText.Equals(actualText);
        }

        private bool HandleDifference(XmlCompareContext context, XmlComparison comparison)
        {
            var state = this.Analyzer.Analyze(comparison);

            context.SetState(state);

            if (state != XmlComparisonState.Equal)
            {
                var item = new XmlDifference(state, comparison);
                context.Differences.Add(item);
                return this.Handler(context);
            }

            return true;
        }

        private class Attributes
        {
            private static readonly XName SchemaLocationName = XName.Get("schemaLocation", XmlSchema.InstanceNamespace);
            private static readonly XName NoNamespaceSchemaLocationName = XName.Get("noNamespaceSchemaLocation", XmlSchema.InstanceNamespace);

            public Attributes(XElement element)
            {
                this.Items = new List<XAttribute>();
                this.Declarations = new List<XAttribute>();

                foreach (var attribute in element.Attributes())
                {
                    if (attribute.Name == SchemaLocationName)
                    {
                        this.SchemaLocation = attribute;
                    }
                    else if (attribute.Name == NoNamespaceSchemaLocationName)
                    {
                        this.NoNamespaceSchemaLocation = attribute;
                    }
                    else if (attribute.IsNamespaceDeclaration)
                    {
                        this.Declarations.Add(attribute);
                    }
                    else
                    {
                        this.Items.Add(attribute);
                    }
                }
            }

            public List<XAttribute> Items { get; set; }

            public List<XAttribute> Declarations { get; set; }

            public XAttribute SchemaLocation { get; set; }

            public XAttribute NoNamespaceSchemaLocation { get; set; }
        }
    }
}