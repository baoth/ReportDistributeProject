using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
using System.Reflection;
using QSmart.Core.Object;
using QSmart.Core.DataBase;

namespace weixinreportviews.Controllers.Customer.FirstReportProduct
{
    public class FReportMainController :BaseController
    {
        [HttpGet]
        public ActionResult Model(string id)
        {
           return View("Model");
        }

        [HttpPost]
        public ActionResult Save()
        {
            try
            {
                SS_CompanyAccount account = General.CreateInstance<SS_CompanyAccount>(Request);
                DbSession session = General.CreateDbSession();
                if (account.Id != Guid.Empty)
                {
                    account.ModifyDate = DateTime.Now;
                    session.Context.ModifyEntity(account.CreateQSmartObject());
                }
                else
                {
                    account.Id = Guid.NewGuid();
                    account.OrderNumber = General.CreateOrderNumber("AC");
                    session.Context.InsertEntity(account.CreateQSmartObject());
                }
                session.Context.SaveChange();
                return Json(new { result = 0 });
            }
            catch (Exception ex)
            {
                return Json(new { result = 1, msg = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            return Json(new { result = 0 });
        }
        public JsonResult Upload() 
        {

            return Json(new { data=""});
        }
        /// <summary>
        /// 获取账户列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Grid()
        {
            return View("Index");
        }

        /// <summary>
        /// 获取账户列表信息
        /// </summary>
        /// <returns></returns>
       //[HttpPost]
        public JsonResult GridDatas()
        {
            AccountDataTablesParameter dtp = General.CreateInstance<AccountDataTablesParameter>(Request);
            int totalcount = 0;
            DbSession session = General.CreateDbSession();
            var rows = session.PaginationRetrieve<SS_CompanyAccount>(dtp.iDisplayStart,
                dtp.iDisplayLength, dtp.GetFilters<SS_CompanyAccount>(),
                dtp.GetOrderBys(), out totalcount);
            return Json(new
            {
                sEcho = dtp.sEcho,// param.sEcho,
                iTotalRecords = totalcount,
                iTotalDisplayRecords = totalcount,
                aaData = rows
            }, JsonRequestBehavior.AllowGet);
        }

       
    }
    public class AccountDataTablesParameter : DataTablesParameter
    {
        protected override List<QSmartQueryFilterCondition> ExactSearch(PropertyInfo[] pis)
        {
            if (this.exactFilter == null || this.exactSearch == null || this.exactFilter.Length == 0
               || this.exactSearch.Length == 0 || this.exactSearch.Length != this.exactFilter.Length) return null;

            List<QSmartQueryFilterCondition> result = new List<QSmartQueryFilterCondition>();
            for (int i = 0; i < this.exactFilter.Length; i++)
            {
                var colName = this.exactFilter[i].ToLower();

                if (colName == "Stoped".ToLower())
                {
                    result.Add(new QSmartQueryFilterCondition
                    {
                        Column = new QSmartQueryColumn { columnName = colName, dataType = typeof(int) },
                        Operator = QSmartOperatorEnum.equal,
                        Connector = QSmartConnectorEnum.and,
                        Values = new List<object> { this.exactSearch[i] }
                    });
                }
                else if (colName == "CreateDate".ToLower())
                {
                    result.Add(new QSmartQueryFilterCondition
                    {
                        Column = new QSmartQueryColumn { columnName = "year(" + colName + ")", dataType = typeof(int) },
                        Operator = QSmartOperatorEnum.equal,
                        Connector = QSmartConnectorEnum.and,
                        Values = new List<object> { this.exactSearch[i] }
                    });
                }
            }
            return result;
        }
    }

}
