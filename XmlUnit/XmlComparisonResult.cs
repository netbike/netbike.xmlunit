namespace NetBike.XmlUnit
{
    using System;
    using System.Collections.Generic;

    public sealed class XmlComparisonResult
    {
        private readonly XmlComparisonState state;
        private readonly List<XmlDifference> differences;

        public XmlComparisonResult(XmlComparisonState result)
            : this(result, null)
        {
        }

        public XmlComparisonResult(XmlComparisonState state, IEnumerable<XmlDifference> differences)
        {
            this.state = state;
            this.differences = new List<XmlDifference>(differences);
        }

        public bool IsEqual
        {
            get { return this.state == XmlComparisonState.Equal; }
        }

        public bool IsSimilar
        {
            get { return this.state != XmlComparisonState.Different; }
        }

        public bool IsDifferent
        {
            get { return this.state == XmlComparisonState.Different; }
        }

        public XmlComparisonState State
        {
            get { return this.state; }
        }

        public IEnumerable<XmlDifference> Differences
        {
            get { return this.differences; }
        }

        public override string ToString()
        {
            return this.ToString(XmlComparisonFormatter.Default);
        }

        public string ToString(IXmlComparisonFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            return formatter.ToString(this);
        }
    }
}