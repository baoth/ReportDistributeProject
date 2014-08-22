using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
using weixinreportviews.Controllers.Admin;
using QSmart.Core.DataBase;
using System.Reflection;

namespace weixinreportviews.Controllers.Customer.FirstReportProduct
{
    public class FReportUserAttentionController : Controller
    {
        //
        // GET: /FRUserAttention/

        public ActionResult Index()
        {
            return View();
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
                    else if (colName == "AccountId".ToLower())
                    {
                        result.Add(new QSmartQueryFilterCondition
                        {
                            Column = new QSmartQueryColumn { columnName = colName, dataType = typeof(Guid) },
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
