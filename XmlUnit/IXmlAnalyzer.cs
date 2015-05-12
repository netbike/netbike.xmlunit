namespace NetBike.XmlUnit
{
    public interface IXmlAnalyzer
    {
        XmlComparisonState Analyze(XmlComparison comparison);
    }
}