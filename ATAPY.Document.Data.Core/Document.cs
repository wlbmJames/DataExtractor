using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ATAPY.Document.Data.Core
{
    public enum E_SourceFormat
    {
        XPS,
        PDF,
        HOCR
    }
    public class Document
    {
        public static char[] GLYPH_SYMBOLS_WITH_NO_GEOMETRY = { ' ', ' ' };
        public double Scale
        {
            get;
            set;
        }
        public Document()
        {
            this.Pages = new Pages(this);
            this.Settings = new DataProcessingSettings();
        }
        public Pages Pages
        {
            get;
            private set;
        }
        public ATAPY.Common.IO.File SourceFile
        {
            get;
            set;
        }
        public DataProcessingSettings Settings
        {
            get;
            set;
        }
        public E_SourceFormat SourceFormat
        {
            get;
            set;
        }
        public void SearchTables()
        {
            int StartPage = 1;
            int EndPage = this.Pages.Count;
            if (Settings.TableSearch.StartPage > 1) {
                StartPage = Settings.TableSearch.StartPage;
                if (StartPage < 1 || StartPage > this.Pages.Count) {
                    throw new ArgumentException("Start Page number is out of document pages count bounds.");
                }
            }
            if (Settings.TableSearch.EndPage != ATAPY.Common.Const.INT_UNINITIALIZED) {
                EndPage = Settings.TableSearch.EndPage;
                if (EndPage < 1 || EndPage > this.Pages.Count) {
                    throw new ArgumentException("End Page number is out of document pages count bounds.");
                }
            }
            if (EndPage < StartPage) {
                throw new ArgumentException("End Page number is less than Start Page number.");
            }
            bool CanHaveNoHeaders = false;
            int DesirecColumnsCount = 0;
            for (int i = StartPage - 1; i < EndPage; i++) {
                Page Page = Pages[i];
                Page.UpdateSettings(this.Settings.TableSearch);
                Page.Settings.CanHaveNoHeaders = CanHaveNoHeaders;
                Page.Settings.DesiredColumnsCount = DesirecColumnsCount;
                bool FooterFound;
                Page.SearchTables(out FooterFound);
                /*if (FooterFound && Settings.TableSearch.IsMultipageTable) {
                    break;
                }*/
                if (Page.Tables.Count > 0) {
                    CanHaveNoHeaders = this.Settings.TableSearch.CanHaveNoHeaders;
                    DesirecColumnsCount = Page.Tables[0].Data.Columns.Count;
                }
            }
        }
        public void DeleteTables()
        {
            foreach (var Page in this.Pages) {
                Page.Tables.Clear();
            }
        }
        public DataTable GetMergedTablesData()
        {
            return ProcessingTableSearch.MergeTables(this.Pages);
        }
        public Table GetMergeTable()
        {
            var Data = ProcessingTableSearch.MergeTables(this.Pages);
            Table Ret = new Table();
            Ret.Data = Data;
            return Ret;
        }
    }
}
