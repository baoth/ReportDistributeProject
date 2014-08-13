using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;

namespace weixinreportviews.Model
{
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
        public string OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
        }
    }
}