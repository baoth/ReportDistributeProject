using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
using System.Reflection;
using QSmart.Core.Object;
using QSmart.Core.DataBase;

namespace weixinreportviews.Controllers.Admin
{
 
    public class AccountController : BaseController
    {
        /// <summary>
        /// 获取账户信息页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Model(string id)
        {
            
            if (!string.IsNullOrEmpty(id))
            {
                DbSession session = General.CreateDbSession();
                SS_CompanyAccount account = session.Retrieve<SS_CompanyAccount>(
                    "Id", Guid.Parse(id));
                if (account != null)
                {
                    PropertyInfo[] pis = account.GetType().GetProperties();

                    for (int i = 0; i < pis.Length; i++)
                    {
                        var objvalue = pis[i].GetValue(account, null);
                        if (pis[i].PropertyType == typeof(DateTime) || pis[i].PropertyType == typeof(Nullable<DateTime>))
                        {                           
                            var value1 = objvalue == null ? "" : ((DateTime)objvalue).ToShortDateString();
                            ViewData.Add(pis[i].Name, value1);
                        }
                        else
                        {
                            var value = objvalue == null ? "" : objvalue.ToString();
                            ViewData.Add(pis[i].Name, value);
                        }
                    }
                }
            }
            return View("Model");
        }

        /// <summary>
        /// 新建或修改账户信息
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 删除账户信息
        /// </summary>
        /// <param name="id">账户Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {
            
            if (!string.IsNullOrEmpty(id))
            {
                List<string> ids = id.Split(',').ToList();
                DbSession session = General.CreateDbSession();
                List<string> listPath = new List<string>();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        SS_CompanyAccount entity = new SS_CompanyAccount { Id = Guid.Parse(ids[i]) };
                        session.Context.DeleteEntity(entity.CreateDeleteCommand());
                        listPath.Add(System.IO.Path.Combine(PathTools.SaveHtmlPath, ids[i]));
                    }
                }

                try
                {
                    session.Context.SaveChange();
                    foreach (var item in listPath)
                    {
                        if (System.IO.Directory.Exists(item))
                        System.IO.Directory.Delete(item, true);
                    }
                    return Json(new { result = 0 });
                }
                catch (Exception ex)
                {
                    return Json(new { result = 1, msg = ex.Message });
                }
            }
            return Json(new { result = 0 });
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

        /// <summary>
        /// 验证唯一键属性是否在数据库中存在
        /// </summary>
        /// <param name="col">唯一键属性名</param>
        /// <param name="value">唯一键属性值</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Validation(string col,string value)
        {
            DbSession session=General.CreateDbSession();
            return Json(new
            {
                result=session.Exists<SS_CompanyAccount>(col,value)==true?0:1
            });
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
