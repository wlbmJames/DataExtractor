using DataAnalyzer.SearchRules;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DataAnalyzer
{
    public static class ResultHandler
    {
        public static ResultModel GetResultModel(ClassifiedDocument document)
        {
            var result = new ResultModel();
            result.DocClass = document.DocClass.Title;
            result.NumberOfPages = document.Document.Pages.Count;
            foreach (var rule in document.DocClass.DataRules)
            {
                if(rule is CharacterStringRule && rule.SearchResult != null && rule.SearchResult.IsFound)
                {
                    result.Data.Add(new NamedResult() { Title = rule.Title, Result = rule.SearchResult});
                }
                if(rule is RepeatingCSRule && rule.SearchResult != null && rule.SearchResult.IsFound)
                {
                    result.TableData.Add(new NamedResult() { Title = rule.Title, Result = rule.SearchResult });
                }
            }
            return result;
        }
        public static void SaveModel(List<ResultModel> models, string path)
        {
            var formatter = new XmlSerializer(typeof(List<ResultModel>));
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, models);
            }
        }
    }
}
