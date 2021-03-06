﻿using System;
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
            //"openid,productkind,accountid#openid,productkind,accountid"
            if (!string.IsNullOrEmpty(id))
            {
                List<string> ids = id.Split('#').ToList();
                var uinfo = Session[General.LogonSessionName] as CustomerLoginInfo;
                DbSession session = General.CreateDbSession();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        var arr = ids[i].Split(',') ;
                        CS_UserAttention entity = new CS_UserAttention {
                            OpenId = arr[0],
                            ProductKind= ProductKindEnum.微信第一表,
                            AccountId = uinfo.Account.Id
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
            
            if (!string.IsNullOrEmpty(entity.OpenId))
            {
                entity.ProductKind = ProductKindEnum.微信第一表;
                var uinfo = Session[General.LogonSessionName] as CustomerLoginInfo;
                entity.AccountId = uinfo.Account.Id;
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
        public JsonResult GetRowByFilter()
        {
            CS_UserAttention entity = General.CreateInstance<CS_UserAttention>(Request);
            if (!string.IsNullOrEmpty(entity.OpenId))
            {
                entity.ProductKind = ProductKindEnum.微信第一表;
                var uinfo = Session[General.LogonSessionName] as CustomerLoginInfo;
                entity.AccountId = uinfo.Account.Id;
                var winCore = General.CreateWeixinCore();
                var userInof = winCore.GetUserBaseInfo(entity.OpenId);

                entity.NickName = userInof.nickname;
                entity.HeadImgUrl = userInof.headimgurl;
                var dbSession = General.CreateDbSession();
                var cobj = entity.CreateQSmartObject();
                cobj.RetainColumn(new List<string>() { "NiceName", "HeadImgUrl", "AccountId", "ProductKind", "OpenId" });
                dbSession.Context.ModifyEntity(cobj);
                dbSession.Context.SaveChange();
                return Json(new { result = 0, HeadImgUrl = entity.HeadImgUrl, NickName = entity.NickName });
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
