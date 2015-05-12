namespace NetBike.XmlUnit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class XmlCustomAnalyzer : IXmlAnalyzer
    {
        private readonly List<XmlAnalyzeInfo> infos;
        private IXmlAnalyzer defaultAnalyzer;

        public XmlCustomAnalyzer()
            : this(XmlAnalyzer.Default)
        {
        }

        public XmlCustomAnalyzer(IXmlAnalyzer defaultAnalyzer)
        {
            if (defaultAnalyzer == null)
            {
                throw new ArgumentNullException("defaultAnalyzer");
            }

            this.defaultAnalyzer = defaultAnalyzer;
            this.infos = new List<XmlAnalyzeInfo>();
        }

        public XmlCustomAnalyzer SetDefault(XmlComparisonState comparisonState)
        {
            var analyzer = XmlAnalyzer.Constant(comparisonState);
            return this.SetDefault(analyzer);
        }

        public XmlCustomAnalyzer SetDefault(IXmlAnalyzer analyzer)
        {
            if (analyzer == null)
            {
                throw new ArgumentNullException("analyzer");
            }

            this.defaultAnalyzer = analyzer;
            return this;
        }

        public XmlCustomAnalyzer SetEqual(XmlComparisonType comparisonType, Predicate<XmlComparison> condition = null)
        {
            return this.SetState(XmlComparisonState.Equal, comparisonType, condition);
        }

        public XmlCustomAnalyzer SetSimilar(XmlComparisonType comparisonType, Predicate<XmlComparison> condition = null)
        {
            return this.SetState(XmlComparisonState.Similar, comparisonType, condition);
        }

        public XmlCustomAnalyzer SetDifferent(XmlComparisonType comparisonType, Predicate<XmlComparison> condition = null)
        {
            return this.SetState(XmlComparisonState.Different, comparisonType, condition);
        }

        public XmlCustomAnalyzer SetState(XmlComparisonState state, XmlComparisonType comparisonType, Predicate<XmlComparison> condition = null)
        {
            for (var i = 0; i < this.infos.Count; i++)
            {
                if (this.infos[i].State == state && this.infos[i].ComparisonType == comparisonType)
                {
                    this.infos[i].Condition = condition;
                    return this;
                }
            }

            this.infos.Add(new XmlAnalyzeInfo
            {
                State = state,
                ComparisonType = comparisonType,
                Condition = condition
            });
                                                                                                                                                                            
            return this;
        }

        public XmlComparisonState Analyze(XmlComparison comparison)
        {
            foreach (var info in this.infos)
            {
                if (info.ComparisonType == comparison.ComparisonType)
                {
                    if (info.Condition == null || info.Condition(comparison))
                    {
                        return info.State;
                    }
                }
            }

            return this.defaultAnalyzer.Analyze(comparison);
        }

        private sealed class XmlAnalyzeInfo
        {
            public XmlComparisonState State { get; set; }

            public XmlComparisonType ComparisonType { get; set; }

            public Predicate<XmlComparison> Condition { get; set; }
        }
    }
}