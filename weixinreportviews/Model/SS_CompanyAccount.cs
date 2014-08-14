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
        
    }
}