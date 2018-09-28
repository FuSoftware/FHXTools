using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXDatabaseBuilder
    {
        List<KeyValuePair<string, int>> Columns;
        List<KeyValuePair<string, Func<FHXObject, FHXParameter>>> Conditions;


        public FHXDatabaseBuilder()
        {
            this.Conditions = DefaultConditions();
            this.Columns = DefaultColumns();
        }

        public void SetColumns(List<KeyValuePair<string, int>> Columns)
        {
            this.Columns = Columns;
        }

        public void BuildModules(List<FHXObject> modules, string file)
        {
            using (var pkg = new ExcelPackage())
            {
                var wbk = pkg.Workbook;
                var sht = wbk.Worksheets.Add("Parameters");

                foreach(var col in Columns)
                {
                    sht.Cells[1, col.Value].Value = col.Key;
                }

                int i = 2;
                foreach(var module in modules)
                {
                    BuildModule(module, sht, i);
                    i++;
                }

                pkg.SaveAs(new FileInfo(file));
            }
        }

        private void BuildModule(FHXObject module, ExcelWorksheet sht, int line)
        {
            foreach(var cond in Conditions)
            {
                try
                {
                    FHXParameter p = cond.Value(module);
                    if (p != null)
                    {
                        int col = Columns.Single(i => i.Key == cond.Key).Value;
                        sht.Cells[line, col].Value = p.Value;
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);
                }
                
            }
        }

        public void SetConditions(List<KeyValuePair<string, Func<FHXObject, FHXParameter>>> Conditions)
        {
            this.Conditions = Conditions;
        }

        private static List<KeyValuePair<string, int>> DefaultColumns()
        {
            List<KeyValuePair<string, int>> Columns = new List<KeyValuePair<string, int>>();
            Columns.Add(new KeyValuePair<string, int>("Repere", 1));
            Columns.Add(new KeyValuePair<string, int>("Description", 2));
            Columns.Add(new KeyValuePair<string, int>("P", 3));
            Columns.Add(new KeyValuePair<string, int>("I", 4));
            Columns.Add(new KeyValuePair<string, int>("D", 5));
            Columns.Add(new KeyValuePair<string, int>("ECHELLE_HAUTE", 6));
            Columns.Add(new KeyValuePair<string, int>("ECHELLE_BASSE", 7));
            Columns.Add(new KeyValuePair<string, int>("UNITES", 8));
            Columns.Add(new KeyValuePair<string, int>("AI_IN", 9));
            Columns.Add(new KeyValuePair<string, int>("PIN_IN", 10));
            Columns.Add(new KeyValuePair<string, int>("PID_IN", 11));
            Columns.Add(new KeyValuePair<string, int>("PID_OUT", 12));
            Columns.Add(new KeyValuePair<string, int>("AO_OUT", 13));
            Columns.Add(new KeyValuePair<string, int>("TYPE", 14));
            Columns.Add(new KeyValuePair<string, int>("SUB_TYPE", 15));
            Columns.Add(new KeyValuePair<string, int>("PRIMARY_CONTROL", 16));
            Columns.Add(new KeyValuePair<string, int>("CONTROLLER", 17));
            Columns.Add(new KeyValuePair<string, int>("AREA", 18));

            return Columns;
        }

        private static List<KeyValuePair<string, Func<FHXObject, FHXParameter>>> DefaultConditions()
        {
            List<KeyValuePair<string, Func<FHXObject, FHXParameter>>> Conditions = new List<KeyValuePair<string, Func<FHXObject, FHXParameter>>>();

            Func<FHXObject, FHXParameter> Repere = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("TAG");
            });

            Func<FHXObject, FHXParameter> Description = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("DESCRIPTION");
            });

            Func<FHXObject, FHXParameter> Gain = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("PID1/GAIN");
            });

            Func<FHXObject, FHXParameter> Reset = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("PID1/RESET");
            });

            Func<FHXObject, FHXParameter> Rate = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("PID1/RATE");
            });

            Func<FHXObject, FHXParameter> ScaleHigh = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                var p = o.GetSingleParameterFromPath("PID1/PV_SCALE.EU100");
                if (p == null)
                {
                    p = o.GetSingleParameterFromPath("AI1/OUT_SCALE.EU100");
                }
                return p;
            });

            Func<FHXObject, FHXParameter> ScaleLow = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                var p = o.GetSingleParameterFromPath("PID1/PV_SCALE.EU0");
                if(p == null)
                {
                    p = o.GetSingleParameterFromPath("AI1/OUT_SCALE.EU0");
                }
                return p;
            });

            Func<FHXObject, FHXParameter> Units = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                var p = o.GetSingleParameterFromPath("PID1/PV_SCALE.UNITS");
                if (p == null)
                {
                    p = o.GetSingleParameterFromPath("AI1/OUT_SCALE.UNITS");
                }
                return p;
            });

            Func<FHXObject, FHXParameter> AI_IN = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("AI1/IO_IN");
            });

            Func<FHXObject, FHXParameter> PIN_IN = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("PIN1/IO_IN");
            });

            Func<FHXObject, FHXParameter> PID_IN = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("PID1/IO_IN");
            });

            Func<FHXObject, FHXParameter> PID_OUT = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("PID1/IO_OUT");
            });

            Func<FHXObject, FHXParameter> AO_OUT = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetSingleParameterFromPath("AO1/IO_OUT");
            });

            Func<FHXObject, FHXParameter> TYPE = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("TYPE");
            });

            Func<FHXObject, FHXParameter> PRIMARY_CONTROL = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("PRIMARY_CONTROL_DISPLAY");
            });

            Func<FHXObject, FHXParameter> CONTROLLER = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("CONTROLLER");
            });

            Func<FHXObject, FHXParameter> AREA = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("PLANT_AREA");
            });

            Func<FHXObject, FHXParameter> SUB_TYPE = new Func<FHXObject, FHXParameter>((FHXObject o) =>
            {
                return o.GetParameter("SUB_TYPE");
            });

            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("Repere", Repere));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("Description", Description));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("P", Gain));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("I", Reset));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("D", Rate));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("ECHELLE_HAUTE", ScaleHigh));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("ECHELLE_BASSE", ScaleLow));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("UNITES", Units));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("AI_IN", AI_IN));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("PIN_IN", PIN_IN));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("PID_IN", PID_IN));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("PID_OUT", PID_OUT));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("AO_OUT", AO_OUT));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("TYPE", TYPE));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("PRIMARY_CONTROL", PRIMARY_CONTROL));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("CONTROLLER", CONTROLLER));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("AREA", AREA));
            Conditions.Add(new KeyValuePair<string, Func<FHXObject, FHXParameter>>("SUB_TYPE", SUB_TYPE));

            return Conditions;
        }
    }
}
