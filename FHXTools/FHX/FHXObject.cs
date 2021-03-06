﻿using FHXTools.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FHXTools.FHX
{
    public class FHXObject
    {
        public List<FHXObject> Children { get; set; }
        public List<FHXParameter> Parameters { get; set; }

        public FHXObject Parent { get; set; }

        public string Type { get; set; }
        public string mName { get; set; }
        public string Name
        {
            get
            {
                if (this.mName != "" && this.mName != null) return this.mName;

                if (this.Type == "SIMPLE_IO_CHANNEL")
                {
                    return string.Format("CH{0}", this.GetParameter("POSITION").Value);
                }
                else if (this.Type == "SERIAL_IO_PORT")
                {
                    return string.Format("P{0}", this.GetParameter("POSITION").Value);
                }
                else if (this.Type == "SERIAL_IO_DEVICE")
                {
                    return string.Format("I{0}", this.GetParameter("INDEX").Value);
                }
                else if (this.Type == "SIMPLE_IO_CARD" || this.Type == "SERIAL_IO_CARD")
                {
                    return string.Format("{0}/C{1}", this.GetParameter("CONTROLLER").Value, this.GetParameter("CARD_SLOT").Value);
                }
                else
                {
                    bool hasName = this.Parameters.Any(s => s.Name == "NAME" || s.Name == "TAG");

                    if (hasName)
                    {

                        FHXParameter p = this.Parameters.Single(s => s.Name == "NAME" || s.Name == "TAG");
                        return p.Value;
                    }
                    else
                    {
                        return this.Type;
                    }
                }
            }
        }

        public FHXObject()
        {
            this.mName = "";
            this.Parameters = new List<FHXParameter>();
            this.Children = new List<FHXObject>();
        }

        public FHXObject(string type, string name = "")
        {
            this.mName = name;
            this.Type = type;
            this.Parameters = new List<FHXParameter>();
            this.Children = new List<FHXObject>();
        }

        public override string ToString()
        {
            return this.Path;
        }

        public void AddChild(FHXObject obj)
        {
            Children.Add(obj);
            obj.SetParent(this);
        }

        public void RemoveChild(FHXObject obj)
        {
            if(Children.Contains(obj)) Children.Remove(obj);
        }

        public void AddParameter(FHXParameter p)
        {
            Parameters.Add(p);
            p.SetParent(this);
        }

        public void SetParent(FHXObject parent)
        {
            if(parent == null && this.Parent != null)
            {
                this.Parent.RemoveChild(this);
            }
            this.Parent = parent;
        }

        public FHXParameter GetParameter(string name)
        {
            bool has = HasParameter(name);
            return has ? this.Parameters.Single(s => s.Name == name) : null;
        }

        public string GetParameterValue(string name)
        {
            FHXParameter p = GetParameter(name);
            return p == null ? "" : p.Value;
        }

        public string Path
        {
            get
            {
                string s = "";
                if (Parent != null)
                {
                    s += Parent.Path;
                }
                s += @"/" + this.Name;
                return s;
            }
        }

        public FHXObject GetRoot()
        {
            return (this.Parent == null) ? this : this.Parent.GetRoot();
        }

        public FHXObject GetParentFromType(string type)
        {
            if (this.Parent == null)
            {
                return null;
            }
            else if(this.Type == type)
            {
                return this;
            }
            else
            {
                return this.Parent.GetParentFromType(type);
            }
        }

        public TreeViewItem ToTreeViewItem(bool children, bool recursive)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = this.Name;
            item.Tag = this;

            if (children)
            {
                IEnumerable<FHXObject> query = Children.OrderBy(i => i.Name);
                foreach (FHXObject o in query)
                {
                    item.Items.Add(o.ToTreeViewItem(recursive, recursive));
                }
            }
            
            return item;
        }

        public List<FHXObject> GetAllChildren()
        {
            List<FHXObject> c = new List<FHXObject>();
            c.AddRange(this.Children);

            foreach(FHXObject o in Children)
            {
                c.AddRange(o.GetAllChildren());
            }

            return c;
        }

        public List<FHXParameter> GetAllParameters()
        {
            List<FHXObject> c = GetAllChildren();
            List<FHXParameter> p = new List<FHXParameter>();

            p.AddRange(this.Parameters);

            foreach (FHXObject o in c)
            {
                p.AddRange(o.Parameters);
            }

            return p;
        }

        public bool HasParameter(string name) => this.Parameters.Any(i => i.Name == name);

        public bool HasChild(string name) => this.Children.Any(i => i.mName == name);

        public FHXObject GetChild(string name, bool deep = false)
        {
            List<FHXObject> c = deep ? GetAllChildren() : Children;
            return c.Any(i => i.Name == name) ? c.Single(i => i.Name == name) : null;
        }

        public FHXObject GetChildFromPath(string path)
        {
            string[] hierarchy = path.Split('/');
            FHXObject current = this;

            foreach (string p in hierarchy)
            {
                current = current.GetChild(p);

                if (current == null) return null; //Child not found
            }

            return current; // Returns the last child from the search
        }

        public FHXParameter GetSingleParameterFromPath(string filter)
        {
            var p = this.GetAllParameters();
            if(p.Any(i => i.Path.Contains(filter)))
            {
                return p.Single(i => i.Path.Contains(filter));
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, List<FHXSearchResult>> SearchMultiple(string[] query)
        {
            List<FHXObject> os = GetAllChildren();
            List<FHXParameter> ps = GetAllParameters();

            Dictionary<string, List<FHXSearchResult>> results = new Dictionary<string, List<FHXSearchResult>>();
            foreach (var s in query)
            {
                results[s] = SearchSimple(s, os, ps);
            }
            return results;
        }

        public Dictionary<string, List<FHXSearchResult>> SearchMultipleParallel(string[] query)
        {
            List<FHXObject> os = GetAllChildren();
            List<FHXParameter> ps = GetAllParameters();

            Dictionary<string, List<FHXSearchResult>> results = new Dictionary<string, List<FHXSearchResult>>();
            Parallel.For(0, query.Length, (i) =>
            {
                results[query[i]] = SearchSimple(query[i], os, ps);
            });
            return results;
        }

        public List<FHXSearchResult> Search(string query)
        {
            return SearchSimple(query, GetAllChildren(), GetAllParameters());
        }

        public List<FHXSearchResult> SearchSimple(string query, List<FHXObject> os, List<FHXParameter> ps)
        {
            List<FHXSearchResult> res = new List<FHXSearchResult>();

            if(os != null)
            {
                os = os.Where(i => i.mName.Contains(query) || i.Path.Contains(query)).ToList();

                foreach (var o in os)
                {
                    res.Add(new FHXSearchResult(o));
                }
            }
            
            if(ps != null)
            {
                ps = ps.Where(i => i.Name.Contains(query) || i.Path.Contains(query) || i.Value.Contains(query)).ToList();

                foreach (var p in ps)
                {
                    res.Add(new FHXSearchResult(p));
                }
            }
            

            return res;

        }
    }
}
