using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;

namespace weixinreportviews.Model
{
    /// <summary>
    /// 报表模型
    /// </summary>
    public class CS_FirstReport
    {
        private Guid _Id = Guid.Empty;

        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Title = string.Empty;
        /// <summary>
        /// 报表名称
        /// </summary>
        [StringMaxLength(20, VarCharType.nvarchar)]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _ReportKey = string.Empty;
        /// <summary>
        /// 关键字
        /// </summary>
        [StringMaxLength(20, VarCharType.nvarchar)]
        public string ReportKey
        {
            get { return _ReportKey; }
            set { _ReportKey = value; }
        }

        private DateTime _CreateDate = DateTime.Now;
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }

        private bool _Builded = false;
        /// <summary>
        /// 是否已生成Html
        /// </summary>
        public bool Builded
        {
            get { return _Builded; }
            set { _Builded = value; }
        }

        private bool _Stoped = false;
        /// <summary>
        /// 是否停用
        /// </summary>
        public bool Stoped
        {
            get { return _Stoped; }
            set { _Stoped = value; }
        }

        /// <summary>
        /// html文件url
        /// </summary>
        [Ignore]
        public string Url
        {
            get { return string.Empty; }
            
        }
    }
}