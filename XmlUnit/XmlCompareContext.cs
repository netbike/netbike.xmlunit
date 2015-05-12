namespace NetBike.XmlUnit
{
    using System.Collections.Generic;

    public class XmlCompareContext
    {
        private readonly List<XmlDifference> differences;
        private XmlComparisonState finalState;
        private XmlComparisonState state;

        public XmlCompareContext()
        {
            this.differences = new List<XmlDifference>();
        }

        public XmlComparisonState State
        {
            get { return this.state; }
        }

        public XmlComparisonState FinalState
        {
            get { return this.finalState; }
        }

        public List<XmlDifference> Differences
        {
            get { return this.differences; }
        }

        public bool SetState(XmlComparisonState newState)
        {
            this.state = newState;

            if ((int)this.finalState < (int)newState)
            {
                this.finalState = newState;
                return true;
            }

            return false;
        }
    }
}