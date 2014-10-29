using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using QSmart.Core.Object;
using System.IO;
using System.Data;
using System.Text;

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

    public class ExcelReportModul:Html5Frame
    {
        private int tabIndex=0;

        /// <summary>
        /// 页签
        /// </summary>
        public XElement Tabs = null;

        /// <summary>
        /// 页签内容
        /// </summary>
        public XElement Sheets = null;

        public ExcelReportModul(string frameurl)
            : base(frameurl)
        {
            Tabs = this.GetXElementById("tabs");
            Sheets = this.GetXElementById("scroller");
        }

        /// <summary>
        /// 添加页签项
        /// </summary>
        /// <param name="tabName">页签项名称</param>
        /// <returns>页签项</returns>
        public XElement AddTab(string tabName)
        {
            if (Tabs != null)
            {
                string classattr = tabIndex == 0 ? "tab_nav first" : "tab_nav";
                XAttribute tclass = new XAttribute("class", classattr);
                XAttribute tabindex = new XAttribute("tabindex", tabIndex);
                XElement li = new XElement("li", tclass, tabindex);
                XElement a = new XElement("a",tabName);
                li.Add(a);
                Tabs.Add(li);
                tabIndex++;
                return li;
            }

            return null;
        }
        /// <summary>
        /// 添加页签内容页
        /// </summary>
        /// <returns>页签内容页</returns>
        public XElement AddSheet()
        {
            if (Sheets != null)
            {
                XAttribute tclass = new XAttribute("class", "sheet");
                XElement div = new XElement("div", tclass);
                Sheets.Add(div);
                return div;
            }
            return null;
        }

        /// <summary>
        /// 添加页签内容页并赋予宽高
        /// </summary>
        /// <param name="width">页面宽</param>
        /// <param name="height">页面高</param>
        /// <returns></returns>
        public XElement AddSheet(double width,double height)
        {
            if (Sheets != null)
            {
                XAttribute tclass = new XAttribute("class", "sheet");
                XAttribute tstyle = new XAttribute("style", string.Format("width:{0}px;height:{1}px;",width,height));
                XElement div = new XElement("div", tclass,tstyle);
                Sheets.Add(div);
                return div;
            }
            return null;
        }
    }

    /// <summary>
    /// excel 转换成HTML
    /// </summary>
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

        public ExcelReportBuilder(string TemplatePath) { this._TemplatePath = TemplatePath; }


        /// <summary>
        /// 创建html
        /// </summary>
        /// <param name="filePath">参照文件完整路径</param>
        /// <param name="savePath">转换成html后保存的文件夹路径</param>
        /// <param name="savefileName">保存的文件名（无后缀）</param>
        /// <param name="isDelSource">是否删除参照文件</param>
        /// <returns></returns>
        public bool Build(string filePath, string savePath, string savefileName, bool isDelSource = true)
        {
            string templatePath = this.TemplatePath;
            try
            {
                ExcelReportModul mframe = new ExcelReportModul(templatePath);

                var wbook = new Aspose.Cells.Workbook(filePath);

                string chartdir = Path.Combine(savePath, savefileName);
                string savefile = Path.Combine(savePath, savefileName + ".html");

                if (System.IO.Directory.Exists(chartdir)) System.IO.Directory.Delete(chartdir);
                System.IO.Directory.CreateDirectory(chartdir);

                for (int i = 0; i < wbook.Worksheets.Count; i++)
                {
                    var wsheet = wbook.Worksheets[i];
                    ExcelSheetReader reader = new ExcelSheetReader(wsheet);
                    XElement table = reader.Table();
                    List<XElement> charts = reader.Charts(savePath, savefileName);
                    List<XElement> pictures = reader.Pictures(savePath, savefileName);
                    if (table == null && (charts == null || charts.Count == 0) && (pictures == null || pictures.Count == 0)) continue;
                    SheetClientArea scat = reader.GetTableClientArea();
                    SheetClientArea scac = reader.GetChartsClientArea();
                    SheetClientArea scap = reader.GetPicturesClientArea();

                    SheetClientArea area = scat.Max(scac).Max(scap);
                    
                    mframe.AddTab(wsheet.Name);
                    var framesheet = mframe.AddSheet(area.Width, area.Height);
                    if (framesheet != null)
                    {
                        if (table != null) framesheet.Add(table);
                        foreach (XElement xe in charts)
                        {
                            framesheet.Add(xe);
                        }
                        foreach (XElement xe in pictures)
                        {
                            framesheet.Add(xe);
                        }
                    }
                }
                mframe.Save(savefile);

                if (isDelSource)
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch(Exception ex)
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
    /// <summary>
    /// word 转换成HTML
    /// </summary>
    public class WordReportBuilder
    {

        private string _TemplatePath = string.Empty;
        /// <summary>
        /// 获取html模板完整路径
        /// </summary>
        public string TemplatePath
        {
            get { return _TemplatePath; }
        }

        public WordReportBuilder(string TemplatePath) { this._TemplatePath = TemplatePath; }
        private Aspose.Words.Document oDoc; 
        /// <summary>
        /// 创建html
        /// </summary>
        /// <param name="filePath">参照文件完整路径</param>
        /// <param name="savePath">转换成html后保存的完整路径</param>
        /// <param name="isDelSource">是否删除参照文件</param>
        /// <returns></returns>
        public bool Build(string filePath, string savePath, bool isDelSource = true)
        {
            string templatePath = this.TemplatePath;
            try
            {
                Html5Frame mframe = new Html5Frame(templatePath);//模板文件

                oDoc = new Aspose.Words.Document(filePath);
                oDoc.Save(savePath,Aspose.Words.SaveFormat.Html);

                try
                {
                    Html5Frame wordframe = new Html5Frame(savePath);
                    XElement bodyContent = (XElement)wordframe.body.FirstNode;
                    //XElement body=wordframe.body;

                    if (bodyContent != null)
                    {
                        XElement div = mframe.GetXElementById("table");
                        if (div != null) div.Add(bodyContent);
                    }
                    //删除word 转换的HTML
                    if (File.Exists(savePath))
                    {
                        File.Delete(savePath);
                    }
                    //word 转换后的HTML
                    mframe.Save(savePath);
                }
                catch { }
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
    }

    /// <summary>
    /// pdf 转换成HTML
    /// </summary>
    public class PdfReportBuilder
    {
        private string _TemplatePath = string.Empty;
        /// <summary>
        /// 获取html模板完整路径
        /// </summary>
        public string TemplatePath
        {
            get { return _TemplatePath; }
        }

        public PdfReportBuilder(string TemplatePath) { this._TemplatePath = TemplatePath; }
        private Aspose.Pdf.Document oDoc;

        /// <summary>
        /// 创建html
        /// </summary>
        /// <param name="filePath">参照文件完整路径</param>
        /// <param name="savePath">转换成html后保存的文件夹路径</param>
        /// <param name="savefileName">保存的文件名（无后缀）</param>
        /// <param name="isDelSource">是否删除参照文件</param>
        /// <returns></returns>
        public bool Build(string filePath, string savePath, string savefileName, bool isDelSource = true)
        {

            string templatePath = this.TemplatePath;
            string savefile = Path.Combine(savePath, savefileName + ".html");
            try
            {
                //Html5Frame mframe = new Html5Frame(templatePath);//模板文件

                oDoc = new Aspose.Pdf.Document(filePath);
                if (!System.IO.Directory.Exists(savePath)) System.IO.Directory.CreateDirectory(savePath);
                oDoc.Save(savefile, Aspose.Pdf.SaveFormat.Html);

                try
                {
                    System.Collections.ArrayList mytxt = new System.Collections.ArrayList();
                    StreamReader sr = new StreamReader(savefile, Encoding.UTF8);
                    while (!sr.EndOfStream)
                    {
                        string mline = sr.ReadLine();
                        mytxt.Add(mline);
                        if (mline.Trim() == "<head>")
                        {
                            mytxt.Add("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=0, minimum-scale=1.0, maximum-scale=1.0\"/>");
                            mytxt.Add("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
                            mytxt.Add("<meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black\" />");
                            mytxt.Add("<style type=\"text/css\">");
                            mytxt.Add("div{width:2000px;height:1000px;}");
                            mytxt.Add("</style>");
                        }

                    }
                    sr.Close();
                    StreamWriter writer = new StreamWriter(savefile);
                    for (int i = 0; i < mytxt.Count; i++)
                    {
                        writer.WriteLine(mytxt[i]);
                    }
                    writer.Close();

                    //string con = "";
                    //FileStream fs = new FileStream(savefile, FileMode.Open, FileAccess.Read);
                    //StreamReader sr = new StreamReader(fs);
                    //con = sr.ReadToEnd();
                    //con = con.Replace("&nbsp;", "<![CDATA[&nbsp;]]>");
                    //sr.Close();
                    //fs.Close();
                    //FileStream fs2 = new FileStream(savefile, FileMode.Open, FileAccess.Write);
                    //StreamWriter sw = new StreamWriter(fs2);
                    //sw.WriteLine(con);
                    //sw.Close();
                    //fs2.Close();
                    

                    //XDocument xdoc = XDocument.Load(savefile);
                    //var head = xdoc.Root.Element("head");
                    //XAttribute metaname1 = new XAttribute("name", "viewport");
                    //XAttribute metacount1 = new XAttribute("content", "width=device-width, initial-scale=1.0, user-scalable=0, minimum-scale=1.0, maximum-scale=1.0");
                    //XElement meata1 = new XElement("meta", metaname1, metacount1);
                    //head.Add(meata1);
                    //XAttribute metaname2 = new XAttribute("name", "apple-mobile-web-app-capable");
                    //XAttribute metacount2 = new XAttribute("content", "yes");
                    //XElement meata2 = new XElement("meta", metaname2, metacount2);
                    //head.Add(meata2);
                    //XAttribute metaname3 = new XAttribute("name", "apple-mobile-web-app-status-bar-style");
                    //XAttribute metacount3 = new XAttribute("content", "black");
                    //XElement meata3 = new XElement("meta", metaname3, metacount3);
                    //head.Add(meata3);
                    //xdoc.Save(savefile);

                    //fs = new FileStream(savefile, FileMode.Open, FileAccess.Read);
                    //sr = new StreamReader(fs);
                    //con = sr.ReadToEnd();
                    //con = con.Replace("<![CDATA[&nbsp;]]>", "&nbsp;");
                    //sr.Close();
                    //fs.Close();
                    //fs2 = new FileStream(savefile, FileMode.Open, FileAccess.Write);
                    //sw = new StreamWriter(fs2);
                    //sw.WriteLine(con);
                    //sw.Close();
                    //fs2.Close();


                    //Html5Frame pdfframe = new Html5Frame(savefile);
                    //XNode bodyContent = (XElement)pdfframe.body.FirstNode;

                    //if (bodyContent != null)
                    //{


                    //    XElement div = mframe.GetXElementById("scroller");
                    //    if (div != null && bodyContent != null)
                    //    {
                    //        do
                    //        {
                    //            div.Add(bodyContent);
                    //            bodyContent = bodyContent.NextNode;
                    //        } while (bodyContent != null);

                    //    }
                    //    var links = pdfframe.head.Elements("link");
                    //    foreach (XElement lnk in links)
                    //    {
                    //        mframe.head.Add(lnk);
                    //    }
                    //}

                    ////pdf 转换后的HTML
                    //mframe.Save(savefile);
                }
                catch { }
                if (isDelSource)
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}