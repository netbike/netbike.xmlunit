using NUnit.Framework.Constraints;
using System.Linq;

namespace NetBike.XmlUnit.NUnitAdapter
{
    internal class XmlCompareConstraintResult : ConstraintResult
    {
        private XmlComparisonResult result;
        private IXmlComparisonFormatter formatter;

        public XmlCompareConstraintResult(IConstraint constraint, object actualValue, XmlComparisonState expectedState, XmlComparisonResult result, IXmlComparisonFormatter formatter)
            : base(constraint, actualValue)
        {
            var success = expectedState == XmlComparisonState.Similar ?
                result.State != XmlComparisonState.Different :
                result.State == expectedState;

            var differences = result.Differences.Where(x => x.State > expectedState);

            this.result = new XmlComparisonResult(result.State, differences);
            this.formatter = formatter;
            this.Status = success ? ConstraintStatus.Success : ConstraintStatus.Failure;
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.Write(this.result.ToString(this.formatter));
        }
    }
}