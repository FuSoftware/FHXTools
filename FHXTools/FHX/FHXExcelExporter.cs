using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace FHXTools.FHX
{
    class FHXExcelExporter
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
}
}
