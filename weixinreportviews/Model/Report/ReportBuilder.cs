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
    /// 报表生成器类型
    /// </summary>
    public enum ReportBuilderEnum
    {
        /// <summary>
        /// Excel文件创建框架
        /// </summary>
        Excel文件创建框架,
    }

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

    public abstract class ReportBuilder : QSmartEntity
    {
        #region 构造函数
        protected ReportBuilder()
        {
            this.Id = Guid.NewGuid();
            this.Builded = false;
            this.CreateDate = DateTime.Now;
            this.Title = string.Empty;
            this.KeyWord = string.Empty;
        }
        #endregion

        public Guid Id { get; set; }
        /// <summary>
        /// 报表名称
        /// </summary>
        [StringMaxLength(20, VarCharType.nvarchar)]
        public string Title { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [StringMaxLength(20, VarCharType.nvarchar)]
        public string KeyWord { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 生成html日期
        /// </summary>
        public DateTime? BuildDate { get; set; }

        /// <summary>
        /// 页面显示用
        /// </summary>
        [Ignore]
        public string BuildDateDisplay
        {
            get
            {
                if (this.BuildDate == null) return string.Empty;
                return String.Format("{0:G}", this.BuildDate);
            }
        }

        /// <summary>
        /// 是否已生成
        /// </summary>
        public bool Builded { get; set; }

        /// <summary>
        /// 生成的html文件路径
        /// </summary>
        [Ignore]
        public string HtmlFilePath
        {
            get
            {
                return WeixinAdaptor.BaseDirector + WeixinAdaptor.静态报表路径
                            + this.Id + ".html";
            }
        }

        /// <summary>
        /// html网址相对url(不带注入www.baoth.com等)
        /// </summary>
        [Ignore]
        public string HtmlUrl
        {
            get
            {
                return WeixinAdaptor.静态报表路径 + this.Id + ".html";
            }
        }

        [Ignore]
        public abstract ReportBuilderEnum Type { get; }

        /// <summary>
        /// 页面显示用
        /// </summary>
        [Ignore]
        public string TypeName { get { return this.Type.ToString(); } }

        /// <summary>
        /// 生成Html文件
        /// </summary>
        /// <param name="frameurl">模板框架url</param>
        /// <returns>true,生成成功 false,生成失败</returns>
        public abstract bool Build(string frameurl);

        /// <summary>
        /// 生成Html文件
        /// </summary>
        /// <returns>true,生成成功 false,生成失败</returns>
        public abstract bool Build();

        /// <summary>
        /// 删除生成的html文件
        /// </summary>
        /// <returns>true 成功 false 失败</returns>
        public bool DeleteBuilded()
        {
            if (System.IO.File.Exists(this.HtmlFilePath)) System.IO.File.Delete(this.HtmlFilePath);
            this.Builded = false;
            return true;
        }

        /// <summary>
        /// 删除附属的文件或数据
        /// </summary>
        /// <returns></returns>
        public abstract bool DeleteAttachData();

        /// <summary>
        /// 从request加载数据
        /// </summary>
        /// <param name="Request"></param>
        public abstract void SetDataFromHttpRequest(HttpRequestBase Request);
    }

    public class SimpleReportBuilder : ReportBuilder
    {

        public SimpleReportBuilder() : base() { }

        /// <summary>
        /// 类型
        /// </summary>
        [Ignore]
        public override ReportBuilderEnum Type
        {
            get { return ReportBuilderEnum.Excel文件创建框架; }
        }

        /// <summary>
        /// 数据文件路径
        /// </summary>
        [Ignore]
        public string DataFileUrl
        {
            get
            {
                return WeixinAdaptor.BaseDirector + System.Configuration.ConfigurationManager.AppSettings["RPExcelFile"]
                               + this.Id;
            }
        }

        public override bool DeleteAttachData()
        {
            if (File.Exists(this.DataFileUrl + ".xls")) File.Delete(this.DataFileUrl + ".xls");
            if (File.Exists(this.DataFileUrl + ".xlsx")) File.Delete(this.DataFileUrl + ".xlsx");
            return true;
        }

        public override bool Build(string frameurl)
        {
            try
            {
                this.BuildDate = DateTime.Now;
                Html5Frame mframe = new Html5Frame(frameurl);
                XElement xele = mframe.GetXElementById("title");
                if (xele != null) xele.Value = string.IsNullOrEmpty(this.Title) ? "" : this.Title;
                xele = mframe.GetXElementById("date");
                if (xele != null) xele.Value = this.BuildDate==null?string.Empty:((DateTime)this.BuildDate).ToShortDateString();

                XElement columns = mframe.GetXElementById("columns");

                ExcelReader reader = null;
                if (File.Exists(this.DataFileUrl + ".xls"))
                {
                    reader = new ExcelReader(this.DataFileUrl + ".xls");
                }
                else if (File.Exists(this.DataFileUrl + ".xlsx"))
                {
                    reader = new ExcelReader(this.DataFileUrl + ".xlsx");
                }
                XElement table = reader.Table();
                if (table != null)
                {
                    XElement div = mframe.GetXElementById("table");
                    if (div != null) div.Add(table);
                }


                ////读取excel数据
                //ExcelReaderEx eReader = new ExcelReaderEx();
                //if (File.Exists(this.DataFileUrl + ".xls"))
                //{
                //    if (!eReader.Load(this.DataFileUrl + ".xls")) return false;
                //}
                //else if (File.Exists(this.DataFileUrl + ".xlsx"))
                //{
                //    if (!eReader.Load(this.DataFileUrl + ".xlsx")) return false;
                //}
                //else { return false; }
               

                //try
                //{

                //    if (columns != null)
                //    {
                //        XElement tr = new XElement(mframe.ns + "tr");
                //        columns.Add(tr);
                //        foreach (DataColumn col in eReader.Columns)
                //        {
                //            this.AddColumn(tr, col.ColumnName, col.ColumnName, col.ExtendedProperties["align"].ToString());
                //        }
                //    }

                //    XElement rows = mframe.GetXElementById("rows");
                //    if (rows != null)
                //    {
                //        foreach (DataRow row in eReader.Rows)
                //        {

                //            this.AddRow(rows, row.ItemArray);
                //        }
                //    }
                //}
                //catch
                //{
                //    return false;
                //}


                if (System.IO.File.Exists(this.HtmlFilePath)) System.IO.File.Delete(this.HtmlFilePath);
                mframe.Save(this.HtmlFilePath);
                this.Builded = true;
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public override bool Build()
        {
            string frameName = "RP" + this.TypeName;
            string filepath = WeixinAdaptor.BaseDirector + System.Configuration.ConfigurationManager.AppSettings[frameName];
            return Build(filepath);
        }

        public override void SetDataFromHttpRequest(HttpRequestBase Request)
        {
            this.Title = Request["Title"];
            this.KeyWord = Request["KeyWord"];
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