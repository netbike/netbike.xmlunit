namespace NetBike.XmlUnit
{
    using System.Linq;

    public delegate bool XmlCompareHandler(XmlCompareContext context);

    public static class XmlCompareHandling
    {
        public static bool Default(XmlCompareContext context)
        {
            return true;
        }

        public static bool StopWhenNotEquals(XmlCompareContext context)
        {
            return false;
        }

        public static bool StopWhenDifference(XmlCompareContext context)
        {
            return context.State == XmlComparisonState.Different;
        }

        public static XmlCompareHandler StopWhen(XmlComparisonState state)
        {
            return x => (int)x.State >= (int)state;
        }

        public static XmlCompareHandler Limit(int count)
        {
            return Limit(XmlComparisonState.Different, count);
        }

        public static XmlCompareHandler Limit(XmlComparisonState state, int count)
        {
            return x => x.Differences.Count(item => item.State == state) >= count;
        }
    }
}