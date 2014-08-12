using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.Object;

namespace weixinreportviews.Model
{
    /// <summary>
    /// 公司账户信息类
    /// </summary>
    public class SS_CompanyAccount:QSmartEntity
    {
        private Guid _Id = Guid.NewGuid();
        [PrimaryKey]
        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Name = string.Empty;
        [StringMaxLength(50)]
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

        private Guid _Creator = Guid.NewGuid();

        public Guid Creator
        {
            get { return _Creator; }
            set { _Creator = value; }
        }

        private string _LoginKey = string.Empty;
        [StringMaxLength(15)]
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

        public string OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
        }
    }
}