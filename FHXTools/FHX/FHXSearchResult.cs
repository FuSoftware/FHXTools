using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    public class FHXSearchResult
    {
        FHXObject Source { get; set; }
        FHXParameter ParamSource { get; set; }

        public FHXSearchResult(FHXObject source)
        {
            this.Source = source;
        }

        public FHXSearchResult(FHXParameter paramSource)
        {
            this.ParamSource = paramSource;
        }

        public string Path
        {
            get
            {
                return this.ParamSource != null ? ParamSource.Path : this.Source.Path;
            }
        }

        public string Value
        {
            get
            {
                return (this.ParamSource != null) ? this.ParamSource.Value : this.Source.Name;
            }
        }
    }
}
