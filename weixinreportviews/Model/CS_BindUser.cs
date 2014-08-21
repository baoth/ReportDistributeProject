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
    public class CS_BindUser
    {
        private int _Id = 0;
        [PrimaryKey]
        [AutoIncrement]
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _OpenId = string.Empty;

        public string OpenId
        {
            get { return _OpenId; }
            set { _OpenId = value; }
        }

        private Guid _AccountId = Guid.Empty;

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

        private ProductKindEnum _ProductKind = ProductKindEnum.微信第一表;
        /// <summary>
        /// 产品类型
        /// </summary>
        public ProductKindEnum ProductKind
        {
            get { return _ProductKind; }
            set { _ProductKind = value; }
        }
    }
}