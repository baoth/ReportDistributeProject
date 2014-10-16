using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Aspose.Cells;
using System.Data;
using System.Xml.Linq;
using System.Text;

namespace weixinreportviews.Model
{
    public class ExcelReader
    {
        #region 私有变量
        private Aspose.Cells.Workbook wb = null;
        private Aspose.Cells.Worksheet ws = null;
        private List<CellAreaReader> MergedCells = new List<CellAreaReader>();
        public class CellAreaReader
        {
            public CellAreaReader(CellArea Area)
            {
                this._area = Area;
                int temp=Area.EndRow - Area.StartRow;
                this._rowSpan = temp == 0 ? 0 : temp + 1;
                temp = Area.EndColumn - Area.StartColumn;
                this._colSpan = temp == 0 ? 0 : temp + 1;
            }

            private CellArea _area;

            public CellArea Area {get{return _area;}}

            private int _rowSpan = -1;

            public int RowSpan
            {
                get { return _rowSpan; }
            }

            private int _colSpan = -1;

            public int ColSpan
            {
                get { return _colSpan; }
            }

            public bool Exists(int rowIndex, int colIndex)
            {
                return this.Area.StartRow <= rowIndex && this.Area.EndRow >= rowIndex
                    && this.Area.StartColumn <= colIndex
                    && this.Area.EndColumn >= colIndex;
            }

            public bool IsInitailCell(int rowIndex, int colIndex)
            {
                return this.Area.StartRow == rowIndex && this.Area.StartColumn == colIndex;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath">excel文件路径</param>
        /// <param name="opensheet">打开工作页的名称或索引号</param>
        public ExcelReader(string filepath, object opensheet)
        {
            
            if (!string.IsNullOrEmpty(filepath))
            {

                this.wb = new Workbook(filepath);
                if (this.wb == null) return;


                if (opensheet.GetType() == typeof(int))
                {
                    int sheetindex = (int)opensheet;
                    this.ws = this.wb.Worksheets[sheetindex];
                }
                else
                {
                    string sheetname = opensheet.ToString();
                    this.ws = this.wb.Worksheets[sheetname];
                }
                if (this.ws == null) return;
                if (ws.Cells.MergedCells != null)
                {
                    foreach (CellArea ca in this.ws.Cells.MergedCells) this.MergedCells.Add(new CellAreaReader(ca));
                }
            }
            
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath">excel文件路径</param>
        public ExcelReader(string filepath) : this(filepath, 0) { }
        #endregion

        #region 公共方法
        /// <summary>
        /// 获取excel表html格式
        /// </summary>
        /// <returns></returns>
        public string HtmlTable()
        {
            XElement table = this.Table();
            StringBuilder output = new StringBuilder();
            output.Append(table + Environment.NewLine);
            return output.ToString();
        }
        /// <summary>
        /// 转换数据为XDocument元素
        /// </summary>
        /// <returns></returns>
        public XElement Table()
        {

            if (this.wb == null || this.ws == null) return null;

            
            XAttribute cellpadding = new XAttribute("cellpadding", 0);
            XAttribute cellspacing = new XAttribute("cellspacing", 0);
            XElement table = new XElement("table", cellpadding, cellspacing);

            int colCount = this.ws.Cells.MaxColumn;
            int rowCount = this.ws.Cells.MaxRow;
            for (int rowIndex = 0; rowIndex <= rowCount; rowIndex++)
            {
                XElement tr = new XElement("tr");
                for (int colIndex = 0; colIndex <= colCount; colIndex++)
                {

                    CellAreaReader car = this.MergedCells.Find(e => e.Exists(rowIndex, colIndex));
                    if (car == null)
                    {
                        XElement td = new XElement("td", this.ws.Cells[rowIndex, colIndex].StringValue);
                        this.SetDisplayStyle(td, this.ws.Cells[rowIndex, colIndex]);
                        tr.Add(td);
                    }
                    else if (car.IsInitailCell(rowIndex, colIndex))
                    {
                        XElement td = new XElement("td", this.ws.Cells[rowIndex, colIndex].StringValue);
                        this.SetDisplayStyle(td, this.ws.Cells[rowIndex, colIndex]);
                        if (car.ColSpan != 0)
                        {
                            XAttribute attr = new XAttribute("colspan", car.ColSpan);
                            td.Add(attr);
                        }
                        else
                        {
                            XAttribute attr = new XAttribute("rowspan", car.RowSpan);
                            td.Add(attr);
                        }
                        tr.Add(td);
                    }
                }
                table.Add(tr);
            }
            return table;
        }

        /// <summary>
        /// 输出为DataTable
        /// </summary>
        /// <param name="exportColumnName">第一行中的数据是否出口到数据表的列名 （默认false）</param>
        /// <returns></returns>
        public DataTable DataTable(bool exportColumnName=false)
        {
            return this.ws.Cells.ExportDataTable(0, 0, this.ws.Cells.MaxRow, this.ws.Cells.MaxColumn
                , exportColumnName);
        }
        #endregion

        #region 私有方法
        private void SetDisplayStyle(XElement ele, Aspose.Cells.Cell cell)
        {
            Aspose.Cells.Style style=cell.GetDisplayStyle();
            string stylestr = string.Empty;
            if (style.ForegroundColor.Name!="0")
            stylestr += string.Format("background-color:rgb({0},{1},{2});",
                style.ForegroundColor.R, style.ForegroundColor.G, style.ForegroundColor.B);
            stylestr +=string.Format("color:rgb({0},{1},{2});",
                style.Font.Color.R, style.Font.Color.G, style.Font.Color.B);
            stylestr += string.Format("font:{0} {1} {2}px auto {3},sans-serif;",
                style.Font.IsItalic ? "italic" : "normal", style.Font.IsBold ? "bold" : "normal",
                style.Font.Size, style.Font.Name);
            stylestr += string.Format("text-align:{0};", style.HorizontalAlignment.ToString().ToLower());
            stylestr += string.Format("height:{0}px;", this.ws.Cells.GetRowHeightPixel(cell.Row));
            stylestr += string.Format("width:{0}px;", this.ws.Cells.GetColumnWidthPixel(cell.Column));
            XAttribute attr = new XAttribute("style", stylestr);
            ele.Add(attr);
        }
        #endregion
    }


    public class ExcelSheetReader
    {
        #region 私有变量
        private Aspose.Cells.Worksheet ws = null;
        private List<CellAreaReader> MergedCells = new List<CellAreaReader>();
        public class CellAreaReader
        {
            public CellAreaReader(CellArea Area)
            {
                this._area = Area;
                int temp = Area.EndRow - Area.StartRow;
                this._rowSpan = temp == 0 ? 0 : temp + 1;
                temp = Area.EndColumn - Area.StartColumn;
                this._colSpan = temp == 0 ? 0 : temp + 1;
            }

            private CellArea _area;

            public CellArea Area { get { return _area; } }

            private int _rowSpan = -1;

            public int RowSpan
            {
                get { return _rowSpan; }
            }

            private int _colSpan = -1;

            public int ColSpan
            {
                get { return _colSpan; }
            }

            public bool Exists(int rowIndex, int colIndex)
            {
                return this.Area.StartRow <= rowIndex && this.Area.EndRow >= rowIndex
                    && this.Area.StartColumn <= colIndex
                    && this.Area.EndColumn >= colIndex;
            }

            public bool IsInitailCell(int rowIndex, int colIndex)
            {
                return this.Area.StartRow == rowIndex && this.Area.StartColumn == colIndex;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="worksheet">excel sheet</param>
        public ExcelSheetReader(Aspose.Cells.Worksheet worksheet)
        {
            this.ws = worksheet;
            if (this.ws == null) return;
            if (ws.Cells.MergedCells != null)
            {
                foreach (CellArea ca in this.ws.Cells.MergedCells) this.MergedCells.Add(new CellAreaReader(ca));
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 获取excel表html格式
        /// </summary>
        /// <returns></returns>
        public string HtmlTable()
        {
            XElement table = this.Table();
            StringBuilder output = new StringBuilder();
            output.Append(table + Environment.NewLine);
            return output.ToString();
        }
        /// <summary>
        /// 转换数据为XDocument元素
        /// </summary>
        /// <returns></returns>
        public XElement Table()
        {

            if (this.ws == null) return null;


            XAttribute cellpadding = new XAttribute("cellpadding", 0);
            XAttribute cellspacing = new XAttribute("cellspacing", 0);
            XElement table = new XElement("table", cellpadding, cellspacing);

            int colCount = this.ws.Cells.MaxColumn;
            int rowCount = this.ws.Cells.MaxRow;
            for (int rowIndex = 0; rowIndex <= rowCount; rowIndex++)
            {
                XElement tr = new XElement("tr");
                for (int colIndex = 0; colIndex <= colCount; colIndex++)
                {

                    CellAreaReader car = this.MergedCells.Find(e => e.Exists(rowIndex, colIndex));
                    if (car == null)
                    {
                        XElement td = new XElement("td", this.ws.Cells[rowIndex, colIndex].StringValue);
                        this.SetDisplayStyle(td, this.ws.Cells[rowIndex, colIndex]);
                        tr.Add(td);
                    }
                    else if (car.IsInitailCell(rowIndex, colIndex))
                    {
                        XElement td = new XElement("td", this.ws.Cells[rowIndex, colIndex].StringValue);
                        this.SetDisplayStyle(td, this.ws.Cells[rowIndex, colIndex]);
                        if (car.ColSpan != 0)
                        {
                            XAttribute attr = new XAttribute("colspan", car.ColSpan);
                            td.Add(attr);
                        }
                        else
                        {
                            XAttribute attr = new XAttribute("rowspan", car.RowSpan);
                            td.Add(attr);
                        }
                        tr.Add(td);
                    }
                }
                table.Add(tr);
            }
            return table;
        }

        /// <summary>
        /// 输出为DataTable
        /// </summary>
        /// <param name="exportColumnName">第一行中的数据是否出口到数据表的列名 （默认false）</param>
        /// <returns></returns>
        public DataTable DataTable(bool exportColumnName = false)
        {
            return this.ws.Cells.ExportDataTable(0, 0, this.ws.Cells.MaxRow, this.ws.Cells.MaxColumn
                , exportColumnName);
        }
        #endregion

        #region 私有方法

        private void SetDisplayStyle(XElement ele, Aspose.Cells.Cell cell)
        {
            Aspose.Cells.Style style = cell.GetDisplayStyle();
            string stylestr = string.Empty;
            if (style.ForegroundColor.Name != "0")
                stylestr += string.Format("background-color:rgb({0},{1},{2});",
                    style.ForegroundColor.R, style.ForegroundColor.G, style.ForegroundColor.B);
            stylestr += string.Format("color:rgb({0},{1},{2});",
                style.Font.Color.R, style.Font.Color.G, style.Font.Color.B);
            stylestr += string.Format("font:{0} {1} {2}px auto {3},sans-serif;",
                style.Font.IsItalic ? "italic" : "normal", style.Font.IsBold ? "bold" : "normal",
                style.Font.Size, style.Font.Name);
            stylestr += string.Format("text-align:{0};", style.HorizontalAlignment.ToString().ToLower());
            stylestr += string.Format("height:{0}px;", this.ws.Cells.GetRowHeightPixel(cell.Row));
            stylestr += string.Format("width:{0}px;", this.ws.Cells.GetColumnWidthPixel(cell.Column));
            XAttribute attr = new XAttribute("style", stylestr);
            ele.Add(attr);
        }
        #endregion
    }
}