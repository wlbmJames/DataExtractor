using ATAPY.Common.IO;
using TextExtractor;

namespace TextAnalyzerTest
{
    class Program
    {
        private static string SamplesFolder = @"D:\Projects\TextLayerAnalyzer\Samples";
        static void Main(string[] args)
        {
            GetDoc();
        }

        private static  void GetDoc()
        {
            var testpdf = @"Tawuniya\15168743 - Kingdom 2017 - 2018.pdf";
            var path = System.IO.Path.Combine(SamplesFolder, testpdf);
            var importer = new Importer();
            var file = new File(path);
            var doc = importer.LoadFromFile(file);
        }
    }
}
