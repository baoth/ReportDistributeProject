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
                List<string> ids = id.Split('#').ToList();
                DbSession session = General.CreateDbSession();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        var arr = ids[i].Split(',') ;
                        CS_UserAttention entity = new CS_UserAttention {
                            OpenId = arr[0],
                            ProductKind=(ProductKindEnum)int.Parse(arr[1]),
                            AccountId=Guid.Parse(arr[2])
                        };
                        session.Context.DeleteEntity(entity.CreateDeleteCommand());
                    }
                }

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
       [HttpPost]
        public ActionResult BindUpdate() 
        {
            CS_UserAttention entity = General.CreateInstance<CS_UserAttention>(Request);
            
            if (!string.IsNullOrEmpty(entity.OpenId) && (entity.AccountId != null && entity.AccountId != Guid.Empty))
            {
                //校验绑定信息
                if (entity.Binded)
                {
                    int validLisencePoint = General.RetrieveValidLisence(entity.AccountId, entity.ProductKind);
                    if (validLisencePoint <= 0)
                    {
                        return Json(new { result = 1, msg = "没有可用的授权！" });
                    }
                }
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
            var CurrentUserInfo = Session[General.LogonSessionName] as CustomerLoginInfo;
            if (CurrentUserInfo != null)
            {
               dtp.exactFilter=dtp.exactFilter.Add("AccountId");
               dtp.exactSearch=dtp.exactSearch.Add(CurrentUserInfo.Account.Id.ToString());
            }

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
        [HttpPost]
        public ActionResult GetRowByFilter()
        {
            CS_UserAttention entity = General.CreateInstance<CS_UserAttention>(Request);
            if (!string.IsNullOrEmpty(entity.OpenId) && (entity.AccountId != null && entity.AccountId != Guid.Empty))
            {
                DbSession session = General.CreateDbSession();
                QSmartQuery Query = new QSmartQuery();
                Query.Tables.Add(new QSmartQueryTable());
                Query.Tables[0].tableName = typeof(CS_UserAttention).Name;
                //OpenId
                Query.FilterConditions.Add(new QSmartQueryFilterCondition()
                {
                    Column = new QSmartQueryColumn() { columnName = "OpenId",dataType=typeof(string) },
                    Operator=QSmartOperatorEnum.equal,
                    Values=new List<object>{entity.OpenId},
                    Connector=QSmartConnectorEnum.and
                });
               
                //AccountId
                Query.FilterConditions.Add(new QSmartQueryFilterCondition()
                {
                    Column = new QSmartQueryColumn() { columnName = "AccountId", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { entity.AccountId },
                    Connector = QSmartConnectorEnum.and
                });
                //产品类型
                Query.FilterConditions.Add(new QSmartQueryFilterCondition()
                {
                    Column = new QSmartQueryColumn() { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                    Operator = QSmartOperatorEnum.equal,
                    Values = new List<object> { entity.ProductKind }                   
                });
               
                var results = session.Context.QueryEntity<CS_UserAttention>(Query);
                if (results != null && results.Count > 0)
                {
                    var row = results[0];
                    return Json(new { result = 0, HeadImgUrl = row.HeadImgUrl, NickName = row.NickName });
                }
               
            }
            return Json(new { result = 1 });
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
                    else if (colName == "AccountId".ToLower())
                    {
                         ////添加公司账号对应的ID，每个公司只能看到自己公司的用户
                        result.Add(new QSmartQueryFilterCondition
                        {
                            Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
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
