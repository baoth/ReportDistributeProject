using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
using weixinreportviews.Controllers.Admin;
using QSmart.Core.DataBase;
using System.Reflection;
using QSmart.Core.Object;

namespace weixinreportviews.Controllers.Customer.FirstReportProduct
{
    public class FReportUserAttentionController : BaseController
    {
        //
        // GET: /FRUserAttention/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Save()
        {
            try
            {
                CS_UserAttention account = General.CreateInstance<CS_UserAttention>(Request);
                DbSession session = General.CreateDbSession();
                if (account.OpenId != string.Empty)
                {
                    session.Context.ModifyEntity(account.CreateQSmartObject());
                }
                else
                {                   
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
        public ActionResult BindUpdate() 
        {
            CS_UserAttention entity = General.CreateInstance<CS_UserAttention>(Request);
            if (!string.IsNullOrEmpty(entity.OpenId) && entity.ProductKind != null && (entity.AccountId != null && entity.AccountId != Guid.Empty))
            {
                DbSession session = General.CreateDbSession();
                if (entity.Binded) session.Context.AttachEntity(entity.CreateBindCommand());
                else session.Context.AttachEntity(entity.CreateUnBindCommand());
                try
                {
                    session.Context.SaveChange();
                    return Json(new { result = 0 });
                }
                catch (Exception ex)
                {
                    return Json(new { result = 1, msg = ex.Message });
                }
            }
            return Json(new { result = 0 });
        }
        public JsonResult GridDatas()
        {
            FReportUserDataTablesParameter dtp = General.CreateInstance<FReportUserDataTablesParameter>(Request);
            int totalcount = 0;
            DbSession session = General.CreateDbSession();
            var rows = session.PaginationRetrieve<CS_UserAttention>(dtp.iDisplayStart,
                dtp.iDisplayLength, dtp.GetFilters<CS_UserAttention>(),
                dtp.GetOrderBys(), out totalcount);
            return Json(new
            {
                sEcho = dtp.sEcho,// param.sEcho,
                iTotalRecords = totalcount,
                iTotalDisplayRecords = totalcount,
                aaData = rows
            }, JsonRequestBehavior.AllowGet);            
        }
        public class FReportUserDataTablesParameter : DataTablesParameter
        {
            protected override List<QSmartQueryFilterCondition> ExactSearch(PropertyInfo[] pis)
            {
                if (this.exactFilter == null || this.exactSearch == null || this.exactFilter.Length == 0
                   || this.exactSearch.Length == 0 || this.exactSearch.Length != this.exactFilter.Length) return null;

                List<QSmartQueryFilterCondition> result = new List<QSmartQueryFilterCondition>();
                List<QSmartQueryFilterCondition> subs = null;
                for (int i = 0; i < this.exactFilter.Length; i++)
                {
                    var colName = this.exactFilter[i].ToLower();
                    if (colName == "ProductKind".ToLower())
                    {
                        result.Add(new QSmartQueryFilterCondition
                        {
                            Column = new QSmartQueryColumn { columnName = colName, dataType = typeof(int) },
                            Operator = QSmartOperatorEnum.equal,
                            Connector = QSmartConnectorEnum.and,
                            Values = new List<object> { this.exactSearch[i] }
                        });
                    }
                    else if (colName == "Binded".ToLower())
                    {
                        result.Add(new QSmartQueryFilterCondition
                        {
                            Column = new QSmartQueryColumn { columnName = colName, dataType = typeof(int) },
                            Operator = QSmartOperatorEnum.equal,
                            Connector = QSmartConnectorEnum.and,
                            Values = new List<object> { this.exactSearch[i] }
                        });
                    }
                   
                }
                if (subs != null)
                {
                    if (result.Count == 0) result = subs;
                    else
                    {
                        QSmartQueryFilterCondition combinitem = new QSmartQueryFilterCondition();
                        foreach (QSmartQueryFilterCondition fc in subs)
                        {
                            combinitem.Combins.Add(fc);
                        }
                        combinitem.Connector = QSmartConnectorEnum.and;
                        result.Add(combinitem);
                    }
                }

                return result;
            }
        }

    }
}
