using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;
using QSmart.Core.DataBase;

namespace weixinreportviews.Model
{
    /// <summary>
    /// 微信客户关注信息类
    /// </summary>
    public class CS_UserAttention
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
        /// <summary>
        /// 客户微信OpenId
        /// </summary>
        public string OpenId
        {
            get { return _OpenId; }
            set { _OpenId = value; }
        }

        private string _Name = string.Empty;
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private Guid _AccountId = Guid.Empty;

        public Guid AccountId
        {
            get { return _AccountId; }
            set { _AccountId = value; }
        }

        private DateTime _AttentionDate = DateTime.Now;

        public DateTime AttentionDate
        {
            get { return _AttentionDate; }
            set { _AttentionDate = value; }
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

        private bool _Binded = false;

        public bool Binded
        {
            get { return _Binded; }
            set { _Binded = value; }
        }

        public List<QObject> CreateDeleteCommand()
        {
            if (this.Id != 0)
            {
                QSmartQuery QueryA = new QSmartQuery();

                QueryA.Tables.Add(new QSmartQueryTable { tableName = typeof(CS_UserAttention).Name });

                QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "Id", dataType = typeof(int) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.Id }
                });

                QSmartQuery QueryB = new QSmartQuery();

                QueryB.Tables.Add(new QSmartQueryTable { tableName = typeof(CS_BindUser).Name });

                QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.Id }
                });

                return new List<QObject> { QueryA, QueryB };
            }
            return null;
        }
    }
}