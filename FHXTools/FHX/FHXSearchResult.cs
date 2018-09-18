using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHXTools.FHX
{
    class FHXSearchResult
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
                if (this.ParamSource != null)
                {
                    return this.ParamSource.Path();
                }
                else
                {
                    return this.Source.Path();
                }
            }
        }

        public string Value
        {
            get
            {
                if (this.ParamSource != null)
                {
                    return this.ParamSource.Value;
                }
                else
                {
                    return this.Source.GetName();
                }
            }
        }
    }
}
