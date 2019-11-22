using ATAPY.Common.IO;
using TextExtractor;

namespace TextAnalyzerTest
{
    class Program
    {
        private static string SamplesFolder = @"D:\Projects\TessWrapper\uncompressed";
        //private static string SamplesFolder = @"D:\Projects\TextLayerAnalyzer\Samples";
        static void Main(string[] args)
        {
            GetDoc();
        }

        private static  void GetDoc()
        {
            //var testpdf = @"Tawuniya\15168743 - Kingdom 2017 - 2018.pdf";
            var testpdf = @"N354512126         5451 0212F003UF3020070     AAL H0  U63.pdf";
            var path = System.IO.Path.Combine(SamplesFolder, testpdf);
            var extractor = new Extractor();
            var file = new File(path);
            var doc = extractor.GetDocument(file);
        }
    }
}
