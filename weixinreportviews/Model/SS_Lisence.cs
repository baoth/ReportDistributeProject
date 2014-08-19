using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;
using QSmart.Core.DataBase;

namespace weixinreportviews.Model
{

    public enum ProductKindEnum
    {
        微信第一表=0,
    }

    public class SS_Lisence:QSmartEntity
    {
        private int _Id = 0;
        [PrimaryKey]
        [AutoIncrement]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private Guid _AccountId = Guid.Empty;
        /// <summary>
        /// 账户Id
        /// </summary>
        public Guid AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; }
        }

        private ProductKindEnum _ProductKind = ProductKindEnum.微信第一表;
        /// <summary>
        /// 产品类型
        /// </summary>
        public ProductKindEnum ProductKind
        {
            get { return _ProductKind; }
            set { _ProductKind = value; }
        }

        private short _LisencePoint = 0;
        /// <summary>
        /// 授权点数
        /// </summary>
        public short LisencePoint
        {
            get { return _LisencePoint; }
            set { _LisencePoint = value; }
        }

        private DateTime _CreateDate = DateTime.Now;

        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }

        private Guid _Creator = Guid.Empty;

        public Guid Creator
        {
            get { return _Creator; }
            set { _Creator = value; }
        }

        private DateTime _EffectiveDate = DateTime.Now;
        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime EffectiveDate
        {
            get { return _EffectiveDate; }
            set { _EffectiveDate = value; }
        }

        private short _EffectiveYear = 0;
        /// <summary>
        /// 有效年
        /// </summary>
        public short EffectiveYear
        {
            get { return _EffectiveYear; }
            set { _EffectiveYear = value; }
        }

        private DateTime _ExpiryDate = DateTime.Now;
        /// <summary>
        /// 失效结束日期
        /// </summary>
        public DateTime ExpiryDate
        {
            get { return _ExpiryDate; }
            set { _ExpiryDate = value; }
        }

        private string _OrderNumber = string.Empty;
        [StringMaxLength(16)]
        [Unique]
        public string OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
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

        #region 属性(仅作呈现用)
        [Ignore]
        public string CreateDateDisplay
        {
            get { return this.CreateDate.ToShortDateString(); }
        }
        [Ignore]
        public string EffectiveDateDisplay
        {
            get { return this.EffectiveDate.ToShortDateString(); }
        }
        [Ignore]
        public string ExpiryDateDisplay
        {
            get { return this.ExpiryDate.ToShortDateString(); }
        }
        [Ignore]
        public string ProductKindDisplay
        {
            get { return this.ProductKind.ToString(); }
        }
        [Ignore]
        public string StopedDisplay
        {
            get { return this.Stoped ? "是" : "否"; }
        }
        #endregion

        public List<QObject> CreateDeleteCommand()
        {
            if (this.Id != 0)
            {
                QSmartQuery Query = new QSmartQuery();

                Query.Tables.Add(new QSmartQueryTable { tableName = typeof(SS_Lisence).Name });

                Query.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "Id", dataType = typeof(int) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.Id }
                });

                return new List<QObject> { Query };
            }
            return null;
        }
    }

    public class SS_LisenceView : SS_Lisence
    {
        private string _AccountName = string.Empty;

        public string AccountName
        {
            get { return _AccountName; }
            set { _AccountName = value; }
        }
    }
}