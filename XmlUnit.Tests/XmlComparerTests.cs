namespace NetBike.XmlUnit.Tests
{
    using System.Linq;
    using NetBike.XmlUnit;
    using NUnit.Framework;

    [TestFixture]
    public class XmlComparerTests
    {
        [Test]
        public void CompareElementsIsEqualTests()
        {
            var comparison = CompareItems("elements", "elements");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareElementsBySequenceIsEqualTests()
        {
            var comparer = new XmlComparer { NodeMatcher = XmlNodeMatcher.BySequence };
            var comparison = CompareItems(comparer, "elements", "elements");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareElementsWithDifferentOrderIsSimilarTests()
        {
            var comparison = CompareItems("elements", "elements-with-different-order");
            AssertComparison(
                comparison,
                XmlComparisonState.Similar, 
                new XmlComparisonType[]
                {
                    XmlComparisonType.NodeListSequence,
                    XmlComparisonType.NodeListSequence
                });
        }

        [Test]
        public void CompareElementsWithDifferentOrderAndStopAnalyzerIsDifferentTests()
        {
            var comparer = new XmlComparer { Analyzer = XmlAnalyzer.Constant(XmlComparisonState.Different) };
            var comparison = CompareItems(comparer, "elements", "elements-with-different-order");
            AssertComparison(
                comparison,
                XmlComparisonState.Different,
                new XmlComparisonType[]
                    {
                        XmlComparisonType.NodeListSequence,
                XmlComparisonType.NodeListSequence
            });
        }

        [Test]
        public void CompareElementsWithIgnoreCommentsIsEqualTest()
        {
            var comparer = new XmlComparer { IgnoreComments = true };
            var comparison = CompareItems(comparer, "elements", "elements-with-comment");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareElementsWithCommentsIsEqualTest()
        {
            var comparison = CompareItems("elements-with-comment", "elements-with-comment");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareElementsWithoutIgnoreCommentsIsDifferentTest()
        {
            var comparer = new XmlComparer { IgnoreComments = false };
            var comparison = CompareItems(comparer, "elements", "elements-with-comment");
            AssertComparison(
                comparison,
                XmlComparisonState.Different,
                new XmlComparisonType[]
                    {
                        XmlComparisonType.NodeList,
                XmlComparisonType.NodeList,
            });
        }

        [Test]
        public void CompareNamespacesIsEqualTest()
        {
            var comparison = CompareItems("ns", "ns");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareNamespacesWithDifferentPrefixesIsSimilarTest()
        {
            var comparison = CompareItems("ns", "ns-with-another-prefix");
            AssertComparison(
                comparison,
                XmlComparisonState.Similar,
                new XmlComparisonType[]
                    {
                        XmlComparisonType.NamespacePrefix
                    });
        }

        [Test]
        public void CompareWithoutNamespaceIsDifferentTest()
        {
            var comparison = CompareItems("ns", "ns-without-namespace");
            AssertComparison(
                comparison,
                XmlComparisonState.Different,
                new XmlComparisonType[]
                    {
                        XmlComparisonType.NamespaceUri,
                         XmlComparisonType.NamespaceUri
                    });
        }

        [Test]
        public void CompareAttributesIsEqualTest()
        {
            var comparison = CompareItems("attr", "attr");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareAttributesWithAnotherOrderIsEqualTest()
        {
            var comparison = CompareItems("attr", "attr-with-another-order");
            AssertComparison(comparison, XmlComparisonState.Equal);
        }

        [Test]
        public void CompareAttributesWithDifferentNamespaceIsDifferentTest()
        {
            var comparison = CompareItems("attr", "attr-with-namespace");
            AssertComparison(
                comparison,
                XmlComparisonState.Different,
                new XmlComparisonType[]
                    {
                        XmlComparisonType.NamespaceUri,
                        XmlComparisonType.NamespaceUri
                    });
        }

        [Test]
        public void Issue1CompareTest()
        {
            var comparer = new XmlComparer
            {
                NormalizeText = true,
                Analyzer = new XmlCustomAnalyzer()
                    .SetState(XmlComparisonState.Equal, XmlComparisonType.NamespacePrefix),
            };

            var comparison = CompareItems("issue-1-expected", "issue-1-actual");
            AssertComparison(
                comparison,
                XmlComparisonState.Different,
                new XmlComparisonType[]
                    {
                        XmlComparisonType.NodeListLookup
                    });
        }

        private static XmlComparisonResult CompareItems(string expectedItem, string actualItem)
        {
            return CompareItems(new XmlComparer(), expectedItem, actualItem);
        }

        private static XmlComparisonResult CompareItems(XmlComparer comparer, string expectedItem, string actualItem)
        {
            var expectedXml = XmlSamples.GetContent(expectedItem);
            var actualXml = XmlSamples.GetContent(actualItem);
            return comparer.Compare(expectedXml, actualXml);
        }

        private static void AssertComparison(XmlComparisonResult comparison, XmlComparisonState expectedState, XmlComparisonType[] expectedTypes = null)
        {
            Assert.IsNotNull(comparison);
            Assert.AreEqual(expectedState, comparison.State);

            if (expectedTypes != null)
            {
                var actualItems = comparison.Differences.ToArray();

                Assert.AreEqual(expectedTypes.Length, actualItems.Length);

                for (var i = 0; i < expectedTypes.Length; i++)
                {
                    Assert.AreEqual(expectedTypes[i], actualItems[i].Difference.ComparisonType);
                }
            }
        }
    }
}