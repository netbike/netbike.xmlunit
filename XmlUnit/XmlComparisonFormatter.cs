namespace NetBike.XmlUnit
{
    using System.Text;
    using System.Xml.Linq;

    public class XmlComparisonFormatter : IXmlComparisonFormatter
    {
        private static IXmlComparisonFormatter defaultFormatter;

        internal static IXmlComparisonFormatter Default
        {
            get
            {
                if (defaultFormatter == null)
                {
                    defaultFormatter = new XmlComparisonFormatter();
                }

                return defaultFormatter;
            }
        }

        public virtual string ToString(XmlComparison comparison)
        {
            var builder = new StringBuilder();
            this.Append(builder, comparison);
            return builder.ToString();
        }

        public virtual string ToString(XmlDifference difference)
        {
            var builder = new StringBuilder();
            this.Append(builder, difference);
            return builder.ToString();
        }

        public virtual string ToString(XmlComparisonResult comparisonResult)
        {
            var builder = new StringBuilder();

            builder.Append("XML documents are ").Append(comparisonResult.State).Append(".");

            foreach (var item in comparisonResult.Differences)
            {
                builder.AppendLine();
                this.Append(builder, item.Difference);
            }

            return builder.ToString();
        }

        protected virtual void Append(StringBuilder builder, XmlDifference difference)
        {
            this.Append(builder, difference.Difference);
        }

        protected virtual void Append(StringBuilder builder, XmlComparison comparison)
        {
            builder.Append(comparison.ComparisonType).Append(": ");
            builder.Append("expected ");
            this.Append(builder, comparison.ExpectedDetails);
            builder.Append(", but was ");
            this.Append(builder, comparison.ActualDetails);
            builder.Append(".");
        }

        protected virtual void Append(StringBuilder builder, XmlComparisonDetails details)
        {
            builder.AppendFormat(
                "{0} at \"{1}\" ({2}:{3})",
                this.GetValueText(details.Value),
                details.XPath,
                details.LineNumber,
                details.LinePosition);
        }

        protected string GetValueText(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is XObject)
            {
                var name = ((XObject)value).GetName();

                if (name != null)
                {
                    value = name.ToString();
                }
            }

            var result = value.ToString();

            if (result.Length > 25)
            {
                result = result.Substring(0, 23) + "..";
            }

            if (value is string)
            {
                result = "\"" + result + "\"";
            }

            return result;
        }
    }
}