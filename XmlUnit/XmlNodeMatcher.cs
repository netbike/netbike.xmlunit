namespace NetBike.XmlUnit
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public sealed class XmlNodeMatcher : IXmlNodeMatcher
    {
        private static IXmlNodeMatcher bySequenceMatcher = new XmlNodeMatcher(SequenceSelector);
        private static IXmlNodeMatcher byTypeMatcher = new XmlNodeMatcher(TypeSelector);
        private static IXmlNodeMatcher byNameMatcher = new XmlNodeMatcher(NameSelector);
        private static IXmlNodeMatcher byLocalNameMatcher = new XmlNodeMatcher(LocalNameSelector);

        private readonly Func<XNode, XNode, bool> selector;

        public XmlNodeMatcher(Func<XNode, XNode, bool> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("nodeSelector");
            }

            this.selector = selector;
        }

        public static IXmlNodeMatcher BySequence
        {
            get { return bySequenceMatcher; }
        }

        public static IXmlNodeMatcher ByType
        {
            get { return byTypeMatcher; }
        }

        public static IXmlNodeMatcher ByName
        {
            get { return byNameMatcher; }
        }

        public static IXmlNodeMatcher ByLocalName
        {
            get { return byLocalNameMatcher; }
        }

        public IEnumerable<XmlNodeMatch> MatchNodes(IEnumerable<XNode> expectedNodes, IEnumerable<XNode> actualNodes)
        {
            if (expectedNodes == null)
            {
                throw new ArgumentNullException("expectedNodes");
            }

            if (actualNodes == null)
            {
                throw new ArgumentNullException("actualNodes");
            }

            var matchItems = new List<XmlNodeMatch>();
            var freeLinkedNodes = new LinkedList<XNode>(actualNodes);

            foreach (var expectedNode in expectedNodes)
            {
                var currentNode = freeLinkedNodes.First;

                while (currentNode != null)
                {
                    if (this.selector(expectedNode, currentNode.Value))
                    {
                        freeLinkedNodes.Remove(currentNode);
                        break;
                    }

                    currentNode = currentNode.Next;
                }

                var match = new XmlNodeMatch(expectedNode, currentNode != null ? currentNode.Value : null);
                matchItems.Add(match);
            }

            return matchItems;
        }

        private static bool SequenceSelector(XNode expectedNode, XNode actualNode)
        {
            return true;
        }

        private static bool TypeSelector(XNode expectedNode, XNode actualNode)
        {
            return expectedNode.NodeType == actualNode.NodeType;
        }

        private static bool LocalNameSelector(XNode expectedNode, XNode actualNode)
        {
            if (expectedNode.NodeType != actualNode.NodeType)
            {
                return false;
            }

            var expectedName = expectedNode.GetName();
            var actualName = actualNode.GetName();

            if (expectedName == actualName)
            {
                return true;
            }

            if (expectedName != null && actualName != null)
            {
                return expectedName.LocalName == actualName.LocalName;
            }

            return false;
        }

        private static bool NameSelector(XNode expectedNode, XNode actualNode)
        {
            if (expectedNode.NodeType != actualNode.NodeType)
            {
                return false;
            }

            var expectedName = expectedNode.GetName();
            var actualName = actualNode.GetName();

            return expectedName == actualName;
        }
    }
}