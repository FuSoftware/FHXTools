using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using Xceed.Words.NET;

namespace FHXTools.FHX
{
    class FHXExporter
    {
        public static void ExportParameters(FHXObject obj, string file, bool recursive = true)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");

                sht.Cells[1, 1].Value = "Path";
                sht.Cells[1, 2].Value = "Value";

                List<FHXParameter> parameters = obj.GetAllParameters();
                int i = 2;

                foreach (var p in parameters)
                {
                    sht.Cells[i, 1].Value = p.Path;
                    sht.Cells[i, 2].Value = p.Value;
                    i++;
                }

                pkg.SaveAs(new FileInfo(file));
            }

        }

        public static void ExportRecherche(List<FHXSearchResult> results, string file)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");

                sht.Cells[1, 1].Value = "Path";
                sht.Cells[1, 2].Value = "Value";

                int i = 2;

                foreach (var r in results)
                {
                    sht.Cells[i, 1].Value = r.Path;
                    sht.Cells[i, 2].Value = r.Value;
                    i++;
                }

                pkg.SaveAs(new FileInfo(file));
            }
        }

        public static void ExportComparison(FHXCompareResultList results, string file)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");

                sht.Cells[1, 1].Value = "Key";
                sht.Cells[1, 2].Value = "Parent";
                sht.Cells[1, 3].Value = "Type";
                sht.Cells[1, 4].Value = "Value";

                int i = 2;

                foreach (var k in results.Results.Keys)
                {
                    foreach (var r in results.Results[k])
                    {
                        sht.Cells[i, 1].Value = k;
                        sht.Cells[i, 2].Value = r.Parent;
                        sht.Cells[i, 3].Value = r.Type;
                        sht.Cells[i, 4].Value = r.Value;
                        i++;
                    }

                }

                pkg.SaveAs(new FileInfo(file));
            }
        }

        public static void ExportObjectWord(FHXObject obj, string file)
        {
            using (var doc = DocX.Create(file))
            {
                Paragraph p = doc.InsertParagraph(obj.GetName());
                p.Alignment = Alignment.center;
  
                ExportObjectInDoc(obj, doc);
                doc.Save();
            }
        }

        private static void ExportObjectInDoc(FHXObject obj, DocX doc, int level = 0)
        {
            doc.InsertSection();
            Paragraph p = doc.InsertParagraph(obj.GetName()).Heading((HeadingType)level).Bold().UnderlineStyle(UnderlineStyle.singleLine);
            p.Alignment = Alignment.left;

            if(obj.Parameters.Count > 0)
            {
                Table t = doc.InsertTable(obj.Parameters.Count, 2);

                int i = 0;
                foreach (FHXParameter par in obj.Parameters)
                {
                    ExportParameterInDoc(par, t, i);
                    i++;
                }
            }
            

            foreach (FHXObject child in obj.Children)
            {
                ExportObjectInDoc(child, doc, level+1);
            }
        }

        private static void ExportParameterInDoc(FHXParameter par, Table t, int row)
        {
            for(var i = 0; i < 2; i++)
            {
                t.Rows[row].Cells[i].SetBorder(TableCellBorderType.Left, new Border(BorderStyle.Tcbs_single, BorderSize.two, 1, Color.Black));
                t.Rows[row].Cells[i].SetBorder(TableCellBorderType.Right, new Border(BorderStyle.Tcbs_single, BorderSize.two, 1, Color.Black));
                t.Rows[row].Cells[i].SetBorder(TableCellBorderType.Bottom, new Border(BorderStyle.Tcbs_single, BorderSize.two, 1, Color.Black));
                t.Rows[row].Cells[i].SetBorder(TableCellBorderType.Top, new Border(BorderStyle.Tcbs_single, BorderSize.two, 1, Color.Black));
            }
            

            t.Rows[row].Cells[0].Paragraphs.First().Append(par.Identifier);
            t.Rows[row].Cells[1].Paragraphs.First().Append(par.Value);
        }
    }
}
