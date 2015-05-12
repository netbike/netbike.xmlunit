namespace NetBike.XmlUnit
{
    public interface IXmlComparisonFormatter
    {
        string ToString(XmlComparison comparison);

        string ToString(XmlDifference difference);

        string ToString(XmlComparisonResult comparisonResult);
    }
}