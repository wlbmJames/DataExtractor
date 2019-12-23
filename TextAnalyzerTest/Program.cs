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
            //var dir = @"D:\Projects\TessWrapper\tesseract";
            //System.IO.Directory.SetCurrentDirectory(dir);
            //var imageName = @"D:\Projects\TextLayerAnalyzer\Samples\Bupa\{imageName}.tif";
            var imageName = "2019-11-28_11-25-08_CE 2017-18 - 2_4";
            var document = GetDoc(imageName);
            double avgSize = 0;
            document.Pages[0].Words.ForEach(line => avgSize += line.Bound.Height);
            avgSize /= document.Pages[0].Words.Count;
            var dClassifier = new Classifier();
            dClassifier.AddClass(DocClassesCollector.Tawunia());
            dClassifier.AddClass(DocClassesCollector.Bupa());
            var docs = dClassifier.Classify(document);
            var analyzer = new Analyzer();
            var models = new List<ResultModel>();
            foreach (var doc in docs)
            {
                analyzer.AnalyzeDocument(doc);
                var model = ResultHandler.GetResultModel(doc);
                models.Add(model);
            }
            var resPath = $@"111.xml";
            //var resPath = $@"D:\Projects\TextLayerAnalyzer\{imageName}.xml";
            ResultHandler.SaveModel(models, resPath);
        }

        private static Document GetDoc(string imageName)
        {
            //var testpdf = $@"Tawuniya\{imageName}.pdf";
            var testpdf = $@"Bupa\{imageName}.tif";
            //var testpdf = @"Bupa\Claim Exp. (UY18).pdf";
            var path = System.IO.Path.Combine(SamplesFolder, testpdf);
            //var path = "111.tif";
            var extractor = new Extractor();
            var file = new File(path);
            var doc = extractor.GetDocument(file);
            return doc;
        }
    }
}
