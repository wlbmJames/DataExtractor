using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATAPY.Document.Data.Core
{
    public class ProcessingDocumentAnalysis
    {
        public static Document SearchStaticText(List<Document> Documents)
        {
            Document Ret = new Document();
            int PagesCount = GetPagesCount(Documents);
            Ret.Pages.AddPages(PagesCount);
            var FirstDoc = Documents.First();
            for (int i = 0; i < PagesCount; i++) {
                Ret.Pages[i].Bound = FirstDoc.Pages[i].Bound;
                Ret.Pages[i].Words.AddRange(FirstDoc.Pages[i].Words);
            }
            for (int i = 1; i < Documents.Count; i++) {
                for (int j = 0; j < PagesCount; j++) {
                    var CommonPageWords = GetCommonWords(Ret.Pages[j].Words, Documents[i].Pages[j]);
                    Ret.Pages[j].Words.Clear();
                    Ret.Pages[j].Words.AddRange(CommonPageWords);
                }
            }
            return Ret;
        }
        private static int GetPagesCount(List<Document> Documents)
        {
            int Ret = Documents.First().Pages.Count;
            for (int i = 1; i < Documents.Count; i++) {
                if (Documents[i].Pages.Count != Ret) {
                    throw new ArgumentException("Documents in collection have different number of pages.");
                }
            }
            return Ret;
        }
        private static List<Word> GetCommonWords(List<Word> Page1Words, Page Page2)
        {
            List<Word> Ret = new List<Word>();
            foreach (var Word in Page1Words) {
                string Symbol;
                var FoundWord = Page2.GetWordForPoint(Word.Bound.GetCenterPoint(), out Symbol);
                if (FoundWord != null && FoundWord.Text == Word.Text) {
                    Ret.Add(Word);
                }
            }
            return Ret;
        }
    }
}
