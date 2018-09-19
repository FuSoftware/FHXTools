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

        public static void ExportRecherche()
        {
        }
    }
}
