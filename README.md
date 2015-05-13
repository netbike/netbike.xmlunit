NetBike.XmlUnit
===============

XML comparison and validation library.

* Comparison result may be the "equal", "similar" or "different".
* Useful details about all the differences: xpath, node, value and position.
* Сustomizable comparison handling and analysis of the differences.
* Constraints for NUnit testing.
* .NET 3.5

Install via [NuGet package](https://www.nuget.org/packages/NetBike.XmlUnit):

```
PM> Install-Package NetBike.XmlUnit
```

Example:

```csharp
var comparer = new XmlComparer
{
    NormalizeText = true,
    Analyzer = XmlAnalyzer.Custom()
        .SetEqual(XmlComparisonType.NodeListSequence)
        .SetSimilar(XmlComparisonType.NamespacePrefix),
    CompareHandler = XmlCompareHandling.Limit(10)
};

var result = comparer.Compare(expected, actual);

if (!result.IsEqual)
{
    foreach (var item in result.Differences)
    {
        Console.WriteLine("State: {0}", item.State);
        Console.WriteLine("Comparison: {1}", item.Comparison.ToString());
    }
}
```

NUnit Adapter
--------------

Install via [NuGet package](https://www.nuget.org/packages/NetBike.XmlUnit.NUnitAdapter):

```
PM> Install-Package NetBike.XmlUnit.NUnitAdapter
```

The test value must be the String, XNode, TextReader or Stream.

Example

```csharp
Assert.That(
    actual,
    IsXml.Equals(expected)
        .WithIgnore(XmlComparisonType.NamespacePrefix))
```

Contributors
------------

* [Anton Mastrakov](https://github.com/a-mastrakov)
* [Pavel Shkarin](https://github.com/a7b0)

Inspired by [XmlUnit](http://xmlunit.org).


License
-------

[MIT License](https://github.com/netbike/netbike.xmlunit/blob/master/LICENSE)
