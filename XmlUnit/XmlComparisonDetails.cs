namespace NetBike.XmlUnit
{
    using System.Xml;
    using System.Xml.Linq;

    public sealed class XmlComparisonDetails
    {
        private readonly XNode node;
        private readonly object value;
        private string xpath;

        public XmlComparisonDetails(XNode node)
            : this(node, null)
        {
        }

        public XmlComparisonDetails(XNode node, object value)
        {
            this.node = node;
            this.value = value;
        }

        public string XPath
        {
            get
            {
                if (this.xpath == null && this.node != null)
                {
                    this.xpath = this.node.GetXPath();
                }

                return this.xpath;
            }
        }

        public XNode Node
        {
            get { return this.node; }
        }

        public object Value
        {
            get { return this.value; }
        }

        public int LineNumber
        {
            get { return this.node != null ? ((IXmlLineInfo)this.node).LineNumber : -1; }
        }

        public int LinePosition
        {
            get { return this.node != null ? ((IXmlLineInfo)this.node).LinePosition : -1; }
        }
    }
}