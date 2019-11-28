using ATAPY.Common.IO;
using ATAPY.Document.Data.Core;
using TextExtractor;
using DataAnalyzer;
using System.Collections.Generic;

namespace TextAnalyzerTest
{
    class Program
    {
        //private static string SamplesFolder = @"D:\Projects\TessWrapper\uncompressed";
        private static string SamplesFolder = @"D:\Projects\TextLayerAnalyzer\Samples";
        static void Main(string[] args)
        {
            var document = GetDoc();
            var dClassifier = new Classifier();
            dClassifier.AddClass(DocClassesCollector.Tawunia());
            var docs = dClassifier.Classify(document);
            var analyzer = new Analyzer();
            var models = new List<ResultModel>();
            foreach (var doc in docs)
            {
                analyzer.AnalyzeDocument(doc);
                var model = ResultHandler.GetResultModel(doc);
                models.Add(model);
            }
            var resPath = @"D:\Projects\TextLayerAnalyzer\TawunyaResult.xml";
            ResultHandler.SaveModel(models, resPath);
        }

        private static Document GetDoc()
        {
            var testpdf = @"Tawuniya\15168743 - Kingdom 2017 - 2018.pdf";
            //var testpdf = @"N354512126         5451 0212F003UF3020070     AAL H0  U63.pdf";
            var path = System.IO.Path.Combine(SamplesFolder, testpdf);
            var extractor = new Extractor();
            var file = new File(path);
            var doc = extractor.GetDocument(file);
            return doc;
        }
    }
}
