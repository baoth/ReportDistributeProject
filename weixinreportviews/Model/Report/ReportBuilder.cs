using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using QSmart.Core.Object;
using System.IO;
using System.Data;

namespace weixinreportviews.Model
{

    /// <summary>
    /// html xml框架
    /// </summary>
    public class Html5Frame
    {
        private XDocument doc = null;

        public XNamespace ns
        {
            get { return doc.Root.Name.Namespace; }
        }

        private XElement _html = null;

        public XElement html
        {
            get
            {
                if (_html == null) _html = doc.Root;
                return _html;
            }
        }

        private XElement _head = null;

        public XElement head
        {
            get
            {
                if (_head == null) _head = doc.Root.Element(ns + "head");
                return _head;
            }
        }

        private XElement _body = null;

        public XElement body
        {
            get
            {
                if (_body == null) _body = doc.Root.Element(ns + "body");
                return _body;
            }
        }

        public Html5Frame(string frameurl)
        {
            doc = XDocument.Load(frameurl);
        }

        public void AddLink(string href)
        {
            this.head.Add(new XElement(ns + "link", new XAttribute("rel", "stylesheet"),
                new XAttribute("type", "text/css"),
                new XAttribute("href", href)
                ));
        }

        public void RemoveLink(string href)
        {
            var xeles = this.head.Elements(this.ns + "link");
            if (xeles == null) return;
            foreach (XElement xele in xeles)
            {
                if (xele.Attribute("href").Value.ToLower() == href.ToLower())
                {
                    xele.Remove();
                    break;
                }
            }
        }

        public void AddScript(string src)
        {
            this.head.Add(new XElement(ns + "script", new XAttribute("type", "text/javascript"),
                new XAttribute("src", src)
                ));
        }

        public void RemoveScript(string src)
        {
            var xeles = this.head.Elements(ns + "script");
            if (xeles == null) return;
            foreach (XElement xele in xeles)
            {
                if (xele.Attribute("src").Value.ToLower() == src.ToLower())
                {
                    xele.Remove();
                    break;
                }
            }
        }

        public XElement GetXElementById(string selectorId)
        {
            XElement result = null;
            QueryById(selectorId, this.body.Elements(), ref result);
            return result;
        }

        public void Save(string path) { this.doc.Save(path); }

        private void QueryById(string selectorId,IEnumerable<XElement> xeles , ref XElement result)
        {
            result = null;
            if (xeles == null) return;
            foreach (XElement xele in xeles)
            {
                var xattrs = xele.Attributes("id");
                foreach (XAttribute xattr in xattrs)
                {
                    if (xattr.Value.ToLower() == selectorId.ToLower())
                    {
                        result = xele;
                        return;
                    }
                }
                QueryById(selectorId, xele.Elements(), ref result);
                if (result != null) return;
            }
        }
    }

    public class ExcelReportBuilder
    {

        private string _TemplatePath=string.Empty;
        /// <summary>
        /// 获取html模板完整路径
        /// </summary>
        public string TemplatePath
        {
          get { return _TemplatePath; }
        }

        public ExcelReportBuilder(string TemplatePath)  { this._TemplatePath=TemplatePath;}

        /// <summary>
        /// 创建html
        /// </summary>
        /// <param name="filePath">参照文件完整路径</param>
        /// <param name="savePath">转换成html后保存的完整路径</param>
        /// <param name="isDelSource">是否删除参照文件</param>
        /// <returns></returns>
        public bool Build(string filePath,string savePath,bool isDelSource=true)
        {
            string templatePath = this.TemplatePath;
            try
            {
                Html5Frame mframe = new Html5Frame(templatePath);
                //XElement xele = mframe.GetXElementById("title");
                //if (xele != null) xele.Value = string.IsNullOrEmpty(this.Title) ? "" : this.Title;
                //xele = mframe.GetXElementById("date");
                //if (xele != null) xele.Value = this.BuildDate==null?string.Empty:((DateTime)this.BuildDate).ToShortDateString();

                XElement columns = mframe.GetXElementById("columns");

                ExcelReader reader = new ExcelReader(filePath);
                XElement table = reader.Table();
                if (table != null)
                {
                    XElement div = mframe.GetXElementById("table");
                    if (div != null) div.Add(table);
                }
                //if (System.IO.File.Exists(this.HtmlFilePath)) System.IO.File.Delete(this.HtmlFilePath);
                mframe.Save(savePath);
                if (isDelSource)
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        
        private void AddColumn(XElement container, string field, string title, string align)
        {
            XElement xele = new XElement("th",
                new XAttribute("data-options", string.Format("field:'{0}',align:'{1}'", field, align)));
            xele.Value = title;
            container.Add(xele);
        }

        private void AddRow(XElement container, object[] values)
        {
            XElement row = new XElement("tr");
            foreach (object val in values)
            {
                row.Add(new XElement("td", val));
            }
            container.Add(row);
        }


    }
}