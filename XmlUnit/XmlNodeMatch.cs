namespace NetBike.XmlUnit
{
    using System;
    using System.Xml.Linq;

    public class XmlNodeMatch
    {
        private readonly XNode expectedNode;
        private readonly XNode actualNode;

        public XmlNodeMatch(XNode expectedNode, XNode actualNode)
        {
            if (expectedNode == null)
            {
                throw new ArgumentNullException("expectedNode");
            }

            this.expectedNode = expectedNode;
            this.actualNode = actualNode;
        }

        public XNode ExpectedNode
        {
            get { return this.expectedNode; }
        }

        public XNode ActualNode
        {
            get { return this.actualNode; }
        }
    }
}