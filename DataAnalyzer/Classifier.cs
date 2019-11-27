using ATAPY.Document.Data.Core;
using DataAnalyzer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.SearchRules;

namespace DataAnalyzer
{
    public class Classifier
    {
        List<DocClass> _classes;
        DocStartedObject _docStartedObject;
        public Classifier()
        {
            _docStartedObject = new DocStartedObject();
            _classes = new List<DocClass>();
        }
        public bool AddClass(string title, int maxPagesCount)
        {
            var dClass = _classes.FirstOrDefault(cl => cl.Title == title);
            if (dClass != default)
                return false;
            _classes.Add(new DocClass(title, maxPagesCount));
            return true;
        }
        public bool AddClass(DocClass docClass)
        {
            _classes.Add(docClass);
            return true;
        }
        public List<ClassifiedDocument> Classify(Document document)
        {
            var result = new List<ClassifiedDocument>();
            ClassifiedDocument currentDoc = new ClassifiedDocument();
            foreach (var page in document.Pages)
            {
                if(_docStartedObject.DocStarted)
                {
                    if (CheckFooter(page))
                    {
                        currentDoc.Document.Pages.Add(page);
                        _docStartedObject.DocStarted = false;
                        _docStartedObject.ClassStarted = null;
                    }
                    else
                    {
                        var dClass = CheckHeader(page);
                        if (dClass != null)
                        {
                            _docStartedObject.DocStarted = true;
                            _docStartedObject.ClassStarted = dClass;
                            currentDoc = new ClassifiedDocument();
                            currentDoc.DocClass = dClass;
                            currentDoc.Document = new Document();
                            currentDoc.Document.Pages.Add(page);
                            result.Add(currentDoc);
                        }
                        else
                        {
                            currentDoc.Document.Pages.Add(page);
                        }
                    }

                }
                else
                {
                    var dClass = CheckHeader(page);
                    if (dClass != null)
                    {
                        _docStartedObject.DocStarted = true;
                        _docStartedObject.ClassStarted = dClass;
                        currentDoc = new ClassifiedDocument();
                        currentDoc.DocClass = dClass;
                        currentDoc.Document = new Document();
                        currentDoc.Document.Pages.Add(page);
                        result.Add(currentDoc);
                    }
                }
            }
            return result;
        }
        private bool CheckFooter(Page page)
        {
            if (!_docStartedObject.DocStarted)
                throw new Exception("Document was not started!");
            if (_docStartedObject.ClassStarted.FooterRules.Count < 1)
                return false;
            foreach (var rule in _docStartedObject.ClassStarted.FooterRules)
            {
                rule.Check(page);
                var result = rule.SearchResult;
                if (!ResultSuccess(result) && rule.RuleBinding == RuleBinding.Required
                    || ResultSuccess(result) && rule.RuleBinding == RuleBinding.Prohibited)
                    return false;

            }
            return true;
        }

        private bool ResultSuccess(SearchResult result)
        {
            return result.IsFound;
        }

        private DocClass CheckHeader(Page page)
        {
            foreach (var dClass in _classes)
            {
                foreach (var rule in dClass.HeaderRules)
                {
                    rule.Check(page);
                    var result = rule.SearchResult;
                    if (!ResultSuccess(result) && rule.RuleBinding == RuleBinding.Required
                        || ResultSuccess(result) && rule.RuleBinding == RuleBinding.Prohibited)
                        return null;
                }
                return (DocClass)dClass.Clone();
            }
            return null;
        }
    }
}
