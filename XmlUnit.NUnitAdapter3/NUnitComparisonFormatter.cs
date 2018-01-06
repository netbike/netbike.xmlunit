namespace NetBike.XmlUnit.NUnitAdapter
{
    using System.Text;

    public class NUnitComparisonFormatter : XmlComparisonFormatter
    {
        protected override void Append(StringBuilder builder, XmlComparison difference)
        {
            builder.Append("  ").Append(difference.ComparisonType).Append(":");

            if (difference.ExpectedDetails != null)
            {
                builder.AppendLine();
                builder.Append("    Expected: ");
                this.Append(builder, difference.ExpectedDetails);
            }

            if (difference.ActualDetails != null)
            {
                builder.AppendLine();
                builder.Append("    Actual: ");
                this.Append(builder, difference.ActualDetails);
            }
        }
    }
}