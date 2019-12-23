using System;

namespace DataAnalyzer
{
    public class Analyzer
    {
        public Analyzer()
        {

        }

        public void AnalyzeDocument(ClassifiedDocument document)
        {
            if (document.DocClass == null || document.Document == null || document.Document.Pages.Count == 0)
                throw new Exception("ftw");
            foreach (var page in document.Document.Pages)
            {
                for (int i = 0; i < document.DocClass.DataRules.Count; i++)
                {
                    var rule = document.DocClass.DataRules[i];
                    if (rule.SearchResult != null && rule.SearchResult.IsFound)
                        continue;
                    rule.Check(page, document.DocClass.PagesOnOriginalImage[page.Index]);
                }
            }
        }
    }
}
