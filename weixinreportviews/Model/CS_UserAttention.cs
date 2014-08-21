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
    public class CS_UserAttention:QSmartEntity
    {
        private string _OpenId = string.Empty;
        /// <summary>
        /// 客户微信OpenId
        /// </summary>
        [PrimaryKey]
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
        [PrimaryKey]
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

        private bool _Binded = false;

        public bool Binded
        {
            get { return _Binded; }
            set { _Binded = value; }
        }

        public List<QObject> CreateDeleteCommand()
        {
            if (string.IsNullOrEmpty(this.OpenId) && this.ProductKind!=null && this.AccountId!=Guid.Empty)
            {
                QSmartQuery QueryA = new QSmartQuery();

                QueryA.Tables.Add(new QSmartQueryTable { tableName = typeof(CS_UserAttention).Name });

                QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.OpenId }
                });
                QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.ProductKind }
                });
                QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.AccountId }
                });

                QSmartQuery QueryB = new QSmartQuery();

                QueryB.Tables.Add(new QSmartQueryTable { tableName = typeof(CS_BindUser).Name });

                QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.OpenId }
                });
                QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.ProductKind }
                });
                QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.AccountId }
                });
                return new List<QObject> { QueryA, QueryB };
            }
            return null;
        }

        public List<QObject> CreateBindCommand()
        {
            this.Binded = true;
            QSmartQuery QueryA = new QSmartQuery();

            QueryA.Tables.Add(new QSmartQueryTable { tableName = typeof(CS_UserAttention).Name });
            QueryA.SelectColumns.Add(new QSmartQueryColumn
            {
                columnName = "Binded",
                dataType=typeof(bool),
                DefaultValue=1
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { this.OpenId }
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { this.ProductKind }
            });
            QueryA.ObjectState = QSmartObjectState.Update;

            CS_BindUser buser = new CS_BindUser();
            buser.AccountId = this.AccountId;
            buser.ProductKind = this.ProductKind;
            buser.BindDate = DateTime.Now;
            buser.AccountId = this.AccountId;
            QObject QueryB = buser.CreateQSmartObject();
            QueryB.ObjectState = QSmartObjectState.New;
            return new List<QObject> { QueryA, QueryB };
        }

        public List<QObject> CreateUnBindCommand()
        {
            this.Binded = false;
            QSmartQuery QueryA = new QSmartQuery();

            QueryA.Tables.Add(new QSmartQueryTable { tableName = typeof(CS_UserAttention).Name });
            QueryA.SelectColumns.Add(new QSmartQueryColumn
            {
                columnName = "Binded",
                dataType = typeof(bool),
                DefaultValue = 0
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { this.OpenId }
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { this.ProductKind }
            });
            QueryA.ObjectState = QSmartObjectState.Update;

            CS_BindUser buser = new CS_BindUser();
            buser.OpenId = this.OpenId;
            buser.ProductKind = this.ProductKind;
            buser.AccountId = this.AccountId;
            QSmartObject QueryB = buser.CreateQSmartObject();
            QueryB.RetainColumn(new List<string> { "OpenId", "ProductKind", "AccountId" });
            QueryB.ObjectState = QSmartObjectState.Delete;
            return new List<QObject> { QueryA, QueryB };
        }
    }
}