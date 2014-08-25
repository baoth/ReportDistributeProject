using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;

namespace weixinreportviews.Model
{
    /// <summary>
    /// 客户绑定信息微信Id类
    /// </summary>
    public class CS_BindUser : QSmartEntity
    {
        private string _OpenId = string.Empty;
        /// <summary>
        /// 客户微信OpenId
        /// </summary>
        [PrimaryKey]
        [StringMaxLength(40, VarCharType.nvarchar)]
        public string OpenId
        {
            get { return _OpenId; }
            set { _OpenId = value; }
        }

        private ProductKindEnum _ProductKind = ProductKindEnum.微信第一表;
        /// <summary>
        /// 产品类型
        /// </summary>
        [PrimaryKey]
        public ProductKindEnum ProductKind
        {
            get { return _ProductKind; }
            set { _ProductKind = value; }
        }

        private Guid _AccountId = Guid.Empty;
        [PrimaryKey]
        public Guid AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; }
        }

        private DateTime _BindDate = DateTime.Now;

        public DateTime BindDate
        {
            get { return _BindDate; }
            set { _BindDate = value; }
        }

    }
}