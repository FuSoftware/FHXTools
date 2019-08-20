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
                sht.Cells[1, 4].Value = "OldValue";
                sht.Cells[1, 5].Value = "NewValue";

                int i = 2;

                foreach (var k in results.Results.Keys)
                {
                    var r = results.Results[k];
                    sht.Cells[i, 1].Value = k;
                    sht.Cells[i, 2].Value = r.Parent;
                    sht.Cells[i, 3].Value = r.Type;
                    sht.Cells[i, 4].Value = r.OldValue;
                    sht.Cells[i, 5].Value = r.NewValue;
                    i++;

                }

                pkg.SaveAs(new FileInfo(file));
            }
        }

        public static void ExportBulkEdit(List<FHXParameter> data, string file)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");
                
                List<string> headers = new List<string>();
                Dictionary<string, Dictionary<string, string>> table = new Dictionary<string, Dictionary<string, string>>();

                int i = 2;

                foreach (var param in data)
                {
                    string area = "";
                    string tag = "";
                    FHXObject module = param.Module;
                    if (module != null)
                    {
                        area = module.GetParameter("PLANT_AREA").Value;
                        tag = module.GetParameter("TAG").Value;
                        string id = module == null ? param.Parent.Name + "." + param.Name : param.RelativePath(module);

                        if (!headers.Contains(id)) headers.Add(id);
                        if (!table.ContainsKey(tag)) table.Add(tag, new Dictionary<string, string>());

                        if (!table[tag].ContainsKey(id)) table[tag].Add(id, param.Value);
                        else table[tag][id] = param.Value;
                    }
                }

                sht.Cells[1, 1].Value = "Module";

                for (int j = 0; j < headers.Count; j++)
                {
                    sht.Cells[1, j+2].Value = headers[j];
                }

                foreach (string k in table.Keys) //For each modules
                {
                    sht.Cells[i, 1].Value = k;

                    for (int j=0;j<headers.Count;j++)
                    {
                        int h = j + 2;
                        if (table[k].ContainsKey(headers[j]))
                        {
                            sht.Cells[i, h].Value = table[k][headers[j]];
                        }
                        else
                        {
                            sht.Cells[i, h].Value = "<NULL>";
                        }
                    }
                    i++;
                }
                pkg.SaveAs(new FileInfo(file));
            }
        }

        public static void ExportParameterList(List<FHXParameter> data, string file)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");

                sht.Cells[1, 1].Value = "Path";
                sht.Cells[1, 2].Value = "Area";
                sht.Cells[1, 3].Value = "Module";
                sht.Cells[1, 4].Value = "Parameter";
                sht.Cells[1, 5].Value = "Value";

                int i = 2;

                foreach (var param in data)
                {
                    string area = "";
                    string tag = "";
                    FHXObject module = param.Module;

                    if(module != null)
                    {
                        area = module.GetParameter("PLANT_AREA").Value;
                        tag = module.GetParameter("TAG").Value;
                    }

                    sht.Cells[i, 1].Value = param.Path;
                    sht.Cells[i, 2].Value = area;
                    sht.Cells[i, 3].Value = tag;
                    sht.Cells[i, 4].Value = module == null ? param.Parent.Name + "." + param.Name : param.RelativePath(module);
                    sht.Cells[i, 5].Value = param.Value;
                    i++;
                }

                pkg.SaveAs(new FileInfo(file));
            }
        }

        public static void ExportReferences(List<FHXReference> data, string file)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");

                sht.Cells[1, 1].Value = "Source";
                sht.Cells[1, 2].Value = "Source Value";
                sht.Cells[1, 3].Value = "Usage";
                sht.Cells[1, 4].Value = "Usage Value";

                int i = 2;

                foreach (var reference in data)
                {

                    sht.Cells[i, 1].Value = reference.Source.Path;
                    sht.Cells[i, 2].Value = reference.Source.Value;
                    sht.Cells[i, 3].Value = reference.Usage.Path;
                    sht.Cells[i, 3].Value = reference.Usage.Value;
                    i++;
                }

                pkg.SaveAs(new FileInfo(file));
            }
        }

        public static void ExportObjectWord(FHXObject obj, string file)
        {
            using (var doc = DocX.Create(file))
            {
                Paragraph p = doc.InsertParagraph(obj.Name);
                p.Alignment = Alignment.center;
  
                ExportObjectInDoc(obj, doc);
                doc.Save();
            }
        }

        private static void ExportObjectInDoc(FHXObject obj, DocX doc, int level = 0)
        {
            doc.InsertSection();
            Paragraph p = doc.InsertParagraph(obj.Name).Heading((HeadingType)level).Bold().UnderlineStyle(UnderlineStyle.singleLine);
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
            

            t.Rows[row].Cells[0].Paragraphs.First().Append(par.Name);
            t.Rows[row].Cells[1].Paragraphs.First().Append(par.Value);
        }

        public static void ExportFHXToFile(FHXObject obj, string path)
        {
            File.WriteAllText(path, ExportFHX(obj));
        }

        public static string ExportFHX(FHXObject obj)
        {
            string s = "";

            s += obj.Type + " ";

            foreach (var param in obj.Parameters.Where(i => i.Mandatory))
            {
                string v = param.VariableType == "string" ? string.Format("\"{0}\"", param.Value) : param.Value;
                s += string.Format("{0}={1} ", param.Name, v);
            }

            s += "\n{\n";

            if (obj.Type == "ATTRIBUTE_INSTANCE")
            {
                s += "VALUE\n{\n";
            }

            foreach (var param in obj.Parameters.Where(i => i.Mandatory == false))
            {
                string v = param.VariableType == "string" ? string.Format("\"{0}\"", param.Value) : param.Value;
                if (param.Value != "") s += string.Format("{0}={1}\n", param.Name, v);
            }

            foreach (var child in obj.Children)
            {
                s += ExportFHX(child);
            }

            if (obj.Type == "ATTRIBUTE_INSTANCE")
            {
                s += "}\n";
            }

            s += "}\n";

            return s;
        }
    }
}
