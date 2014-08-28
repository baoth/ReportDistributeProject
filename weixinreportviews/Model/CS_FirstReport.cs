using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;
using QSmart.Core.DataBase;

namespace weixinreportviews.Model
{
    /// <summary>
    /// 报表模型
    /// </summary>
    public class CS_FirstReport : QSmartEntity
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
        #region 属性(仅作呈现用)
        [Ignore]
        public string CreateDateDisplay
        {
            get { return this.CreateDate.ToShortDateString(); }
        }
        [Ignore]
        public string StopedDisplay
        {
            get { return this.Stoped ? "是" : "否"; }
        }
        [Ignore]
        public string BuildedDisplay
        {
            get { return this.Builded ? "是" : "否"; }
        }
        #endregion

        private string _ReportKey = string.Empty;
        /// <summary>
        /// 关键字
        /// </summary>
        [StringMaxLength(20, VarCharType.nvarchar)]
        [Unique]
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

        private Guid _AccountId = Guid.Empty;

        public Guid AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; }
        }

        [Ignore]
        public string Url
        {
            get
            {
                return "ReportViews\\" + this.AccountId + "\\" + this.Id + ".html";
            }
        }

        public List<QObject> CreateDeleteCommand()
        {
            if (this.Id != Guid.Empty)
            {
                QSmartQuery QueryA = new QSmartQuery();

                QueryA.Tables.Add(new QSmartQueryTable { tableName = "CS_FirstReport" });

                QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "Id", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.Id }
                });

                return new List<QObject> { QueryA };
            }
            return null;
        }
    }
}