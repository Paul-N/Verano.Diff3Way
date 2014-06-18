using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way.Tests
{
    internal class Utils
    {
        public static string[] GetStringsOfResource(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "MyCompany.MyProduct.MyFile.txt";

            string[] result;

            using (Stream stream = assembly.GetManifestResourceStream(path))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd().Split(new []{Environment.NewLine}, StringSplitOptions.None);
            }

            return result;
        }

        public static string CombinePath(string basePath, string fileName, TestFile type)
        {
            switch (type)
            {
                case TestFile.Parent:
                    return string.Format("{0}.{1}.{2}", basePath, fileName, "parent");
                case TestFile.First:
                    return string.Format("{0}.{1}.{2}", basePath, fileName, "1st");
                case TestFile.Second:
                    return string.Format("{0}.{1}.{2}", basePath, fileName, "2nd");
                case TestFile.MergeOld:
                    return string.Format("{0}.{1}.{2}", basePath, fileName, "mold");
                default:
                    throw new ArgumentException("Invalid arg");
            }
        }
    }

    internal enum TestFile { Parent, First, Second, MergeOld }
}
