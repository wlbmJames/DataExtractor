using System;
using ATAPY.Common.IO;
using ATAPY.Document.Data.Core;
using TextExtractor;
using DataAnalyzer;
using System.Collections.Generic;

namespace ImageAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dir = @"C:\ProgramData\EasyData\EasySeparate\Profiles\Tesseract\ImageAnalyzer";
            //System.IO.Directory.SetCurrentDirectory(dir);
            if (args.Length != 2)
                 throw new Exception("");
            var pathToImage = args[0];
            var pathToResultXml = args[1];
            var document = GetDoc(pathToImage);
            var dClassifier = new Classifier();
            dClassifier.AddClass(DocClassesCollector.TawuniaLegacy());
            dClassifier.AddClass(DocClassesCollector.BupaLegacy());
            var docs = dClassifier.Classify(document);
            var analyzer = new Analyzer();
            var models = new List<ResultModel>();
            foreach (var doc in docs)
            {
                analyzer.AnalyzeDocument(doc);
                var model = ResultHandler.GetResultModel(doc);
                models.Add(model);
            }
            ResultHandler.SaveModel(models, pathToResultXml);
        }
        private static Document GetDoc(string pathToImage)
        {
            var extractor = new Extractor();
            var file = new File(pathToImage);
            var doc = extractor.GetDocument(file);
            return doc;
        }
    }
}
