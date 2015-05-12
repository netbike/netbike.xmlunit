namespace NetBike.XmlUnit.Tests
{
    using System.IO;

    public static class XmlSamples
    {
        public static string GetContent(string name)
        {
            return File.ReadAllText("samples/" + name + ".xml");
        }

        public static string GetContentByFullName(string name)
        {
            return File.ReadAllText("samples/" + name);
        }

        public static string GetFullPathByName(string name)
        {
            return Path.GetFullPath("samples/" + name);
        }
    }
}