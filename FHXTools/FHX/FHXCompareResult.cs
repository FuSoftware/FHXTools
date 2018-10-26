using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXCompareResultList
    {
        Dictionary<string, FHXCompareResult> _results = new Dictionary<string, FHXCompareResult>();

        public FHXCompareResultList()
        {

        }

        public void Add(FHXObject parent, FHXParameter param, FHXCompareType type)
        {
            string rpath = param.RelativePath(parent);
            if (!_results.Keys.Contains(rpath))
            {
                _results.Add(rpath, new FHXCompareResult(parent, type, param.Value));
            }
            else
            {
                _results[rpath].NewValue = param.Value;
            }
        }

        public Dictionary<string, FHXCompareResult> Results
        {
            get
            {
                return this._results;
            }
        }

    }

    public enum FHXCompareType
    {
        DIFFERENT,
        IN,
        NOT_IN
    }

    public class FHXCompareResult
    {
        FHXCompareType _type;
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public FHXObject Parent { get; set; }
        public string Type
        {
            get
            {
                return _type.ToString();
            }
        }

        public FHXCompareResult(FHXObject parent, FHXCompareType type, string value = "")
        {
            this.OldValue = value;
            this.Parent = parent;
            this._type = type;
        }
    }
}
