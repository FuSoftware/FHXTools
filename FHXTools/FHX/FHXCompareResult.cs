using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXCompareResultList
    {
        Dictionary<string, List<FHXCompareResult>> _results = new Dictionary<string, List<FHXCompareResult>>();

        public FHXCompareResultList()
        {

        }

        public void Add(FHXObject parent, FHXParameter param, FHXCompareType type)
        {
            string rpath = param.RelativePath(parent);
            if (!_results.Keys.Contains(rpath))
            {
                _results.Add(rpath, new List<FHXCompareResult>());
            }

            _results[rpath].Add(new FHXCompareResult(parent, type, param.Value));
        }

        public Dictionary<string, List<FHXCompareResult>> Results
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
        public string Value { get; set; }
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
            this.Value = value;
            this.Parent = parent;
            this._type = type;
        }
    }
}
