using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace NetBike.XmlUnit.Tests
{
    public class TestBase
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        }
    }
}
