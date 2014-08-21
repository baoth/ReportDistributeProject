using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;
using QSmart.Core.DataBase;

namespace weixinreportviews.Model
{
    /// <summary>
    /// 公司账户信息类
    /// </summary>
    public class SS_CompanyAccount:QSmartEntity
    {
        #region 属性
        private Guid _Id = Guid.Empty;
        [PrimaryKey]
        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Name = string.Empty;
        [StringMaxLength(50)]
        [Unique]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Address = string.Empty;
        [StringMaxLength(100)]
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _Phone = string.Empty;
        [StringMaxLength(15)]
        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        private bool _Stoped = false;

        public bool Stoped
        {
            get { return _Stoped; }
            set { _Stoped = value; }
        }

        private DateTime _CreateDate = DateTime.Now;

        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
        }

        private DateTime? _ModifyDate;

        public DateTime? ModifyDate
        {
            get { return _ModifyDate; }
            set { _ModifyDate = value; }
        }

        private Guid _Creator = Guid.Empty;

        public Guid Creator
        {
            get { return _Creator; }
            set { _Creator = value; }
        }

        private string _LoginKey = string.Empty;
        [StringMaxLength(15)]
        [Unique]
        public string LoginKey
        {
            get { return _LoginKey; }
            set { _LoginKey = value; }
        }

        private string _Password = string.Empty;
        [StringMaxLength(15)]
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _OrderNumber = string.Empty;
        [StringMaxLength(16)]
        [Unique]
        public string OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
        }
        #endregion

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
        #endregion

        public List<QObject> CreateDeleteCommand()
        {
            if (this.Id != Guid.Empty)
            {
                QSmartQuery QueryA = new QSmartQuery();

                QueryA.Tables.Add(new QSmartQueryTable { tableName = typeof(SS_CompanyAccount).Name });

                QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = "Id", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { this.Id }
                });

                QSmartQuery QueryB = new QSmartQuery();

                QueryB.Tables.Add(new QSmartQueryTable { tableName = typeof(SS_Lisence).Name });

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

        /// <summary>
        /// 获取产品可用授权个数
        /// </summary>
        /// <returns>可用授权个数</returns>
        public int GetLisenceValidPoint(ProductKindEnum ProductKind)
        {
            //select * from
            //(select sum(LisencePoint) as t from SS_Lisence where ProductKind=ProductKind
            //and EffectiveDate<=datetime.now and ExpiryDate>=datetime.now and Stoped=0 and AccountId=Id) a
            //,
            //(select count(OpenId) as u from CS_BindUser where ProductKind=ProductKind and AccountId=Id) b
            QSmartQuery QueryA = new QSmartQuery();
            QueryA.Tables.Add(new QSmartQueryTable());
            QueryA.Tables[0].tableName = typeof(SS_Lisence).Name;
            QSmartQueryColumn selectcol=new QSmartQueryColumn();
            selectcol.columnName="LisencePoint";
            selectcol.aliasName="t";
            selectcol.functions.Add(new QSmartQueryFunction{ Function= QSmartFunctionEnum.sum});
            QueryA.SelectColumns.Add(selectcol);
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(int) },
                Values=new List<object>{ProductKind},
                Operator= QSmartOperatorEnum.equal,
                Connector= QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "EffectiveDate", dataType = typeof(DateTime) },
                Values = new List<object> { DateTime.Now },
                Operator = QSmartOperatorEnum.lessequal,
                Connector = QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ExpiryDate", dataType = typeof(DateTime) },
                Values = new List<object> { DateTime.Now },
                Operator = QSmartOperatorEnum.greatequal,
                Connector = QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "Stoped", dataType = typeof(bool) },
                Values = new List<object> { false },
                Operator = QSmartOperatorEnum.equal,
                Connector = QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                Values = new List<object> { this.Id },
                Operator = QSmartOperatorEnum.equal,
                Connector = QSmartConnectorEnum.and
            });

            QSmartQuery QueryB = new QSmartQuery();
            QueryB.Tables.Add(new QSmartQueryTable());
            QueryB.Tables[0].tableName = typeof(CS_BindUser).Name;
            QueryB.CountSetting.Effective = true;
            QueryB.CountSetting.CountAttributeName = "OpenId";
            QueryB.CountSetting.AliasName = "u";
            QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(int) },
                Values = new List<object> { ProductKind },
                Operator = QSmartOperatorEnum.equal,
                Connector = QSmartConnectorEnum.and
            });
            QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                Values = new List<object> { this.Id },
                Operator = QSmartOperatorEnum.equal,
                Connector = QSmartConnectorEnum.and
            });


            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable
            {
                aliasName = "a",
                joinType = QSmartJoinEnum.comma,
                tableNameCreator = QueryA
            });

            Query.Tables.Add(new QSmartQueryTable
            {
                aliasName = "b",

                tableNameCreator = QueryB
            });
            DbSession session=General.CreateDbSession();
            System.Data.DataTable dt = session.Context.QueryTable(Query);
            return ((int)dt.Rows[0][0]) - ((int)dt.Rows[0][1]);
        }
    }
}