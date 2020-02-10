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
        //private static string SamplesFolder = @"D:\Projects\TextLayerAnalyzer\Samples";
        private static string SamplesFolder = @"D:\Projects\TextLayerAnalyzer\Samples\Ensol";
        static void Main(string[] args)
        {
            //var dir = @"D:\Projects\TessWrapper\tesseract";
            //System.IO.Directory.SetCurrentDirectory(dir);
            //var imageName = @"D:\Projects\TextLayerAnalyzer\Samples\Bupa\{imageName}.tif";
            var imageName = "2019-11-ээ";
            var document = GetDoc(imageName);
            var dClassifier = new Classifier();
            //dClassifier.AddClass(DocClassesCollector.TawuniaLegacy());
            //dClassifier.AddClass(DocClassesCollector.BupaLegacy());
            dClassifier.AddClass(DocClassesCollector.Act());
            var docs = dClassifier.Classify(document);
            var analyzer = new Analyzer();
            var models = new List<ResultModel>();
            foreach (var doc in docs)
            {
                analyzer.AnalyzeDocument(doc);
                var model = ResultHandler.GetResultModel(doc);
                models.Add(model);
            }
            var resPath = $@"112.xml";
            //var resPath = $@"D:\Projects\TextLayerAnalyzer\{imageName}.xml";
            ResultHandler.SaveModel(models, resPath);
        }

        private static Document GetDoc(string imageName)
        {
            //var testpdf = $@"Tawuniya\{imageName}.pdf";
            var testpdf = $@"{imageName}.pdf";
            //var testpdf = $@"Bupa\{imageName}.tif";
            //var testpdf = @"Bupa\Claim Exp. (UY18).pdf";
            var path = System.IO.Path.Combine(SamplesFolder, testpdf);
            //var path = "111.tif";
            var extractor = new Extractor();
            var file = new File(path);
            var doc = extractor.GetDocument(file, "rus");
            return doc;
        }
    }
}
