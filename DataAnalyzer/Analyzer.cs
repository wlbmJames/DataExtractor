using System;
using ATAPY.Document.Data.Core;
using DataAnalyzer.Core;

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
                
                foreach (var rule in document.DocClass.DataRules)
                {
                    if (rule.SearchResult != null && rule.SearchResult.IsFound)
                        continue;
                    rule.Check(page, page.Index);
                }
            }
        }
    }
}
