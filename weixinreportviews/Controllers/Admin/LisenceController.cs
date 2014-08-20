 using System.Linq;
 using System.Web;
 using System.Web.Mvc;
using weixinreportviews.Model;
using QSmart.Core.Object;
using System.Reflection;
using System;
using System.Collections.Generic;
using QSmart.Core.DataBase;
 
 namespace weixinreportviews.Controllers.Admin
 {
     public class LisenceController : Controller
     {
        /// <summary>
        /// 获取授权信息页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Model(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                DbSession session = General.CreateDbSession();
                SS_Lisence lisence = session.Retrieve<SS_Lisence>(
                    "Id", int.Parse(id));
                PropertyInfo[] pis = lisence.GetType().GetProperties();
                for (int i = 0; i < pis.Length; i++)
                {
                    ViewData.Add(pis[i].Name, pis[i].GetValue(lisence, null).ToString());
                }
            }
            return View("Model");
        }

        /// <summary>
        /// 新建或修改授权信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save()
        {
            try
            {              
                SS_Lisence lisence = General.CreateInstance<SS_Lisence>(Request);
                lisence.ExpiryDate = lisence.EffectiveDate.AddYears(lisence.EffectiveYear);
                DbSession session = General.CreateDbSession();
                if (lisence.Id != 0)
                {
                    session.Context.ModifyEntity(lisence.CreateQSmartObject());
                }
                else
                {
                    lisence.OrderNumber = General.CreateOrderNumber("LE");
                    session.Context.InsertEntity(lisence.CreateQSmartObject());
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
        /// 删除授权信息
        /// </summary>
        /// <param name="id">授权Id</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                List<string> ids = id.Split(',').ToList<string>();
                DbSession session = General.CreateDbSession();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        SS_Lisence entity = new SS_Lisence { Id = int.Parse(ids[i]) };
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

        /// <summary>
        /// 获取授权列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Grid()
        {
            return View("Index");
        }
        //
        // GET: /Lisence/

        /// <summary>
        /// 获取授权列表信息
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        public JsonResult GridDatas()
        {
            LisenceDataTablesParameter dtp = General.CreateInstance<LisenceDataTablesParameter>(Request);
            int totalcount = 0;
            DbSession session = General.CreateDbSession();
            var rows = session.PaginationRetrieve<SS_LisenceView>(dtp.iDisplayStart,
                dtp.iDisplayLength, dtp.GetFilters<SS_LisenceView>(),
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
        public JsonResult Validation(string col, string value)
        {
            DbSession session = General.CreateDbSession();
            return Json(new
            {
                result = session.Exists<SS_Lisence>(col, value) == true ? 0 : 1
            });
        }
 
    }

     public class LisenceDataTablesParameter : DataTablesParameter
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
                 else if (colName == "Stoped".ToLower())
                 {
                     result.Add(new QSmartQueryFilterCondition
                     {
                         Column = new QSmartQueryColumn { columnName = colName, dataType = typeof(int) },
                         Operator = QSmartOperatorEnum.equal,
                         Connector = QSmartConnectorEnum.and,
                         Values = new List<object> { this.exactSearch[i] }
                     });
                 }
                 else if (colName == "EffectiveDate".ToLower())
                 {
                     result.Add(new QSmartQueryFilterCondition
                     {
                         Column = new QSmartQueryColumn { columnName = "year(" + colName + ")", dataType = typeof(int) },
                         Operator = QSmartOperatorEnum.equal,
                         Connector = QSmartConnectorEnum.and,
                         Values = new List<object> { this.exactSearch[i] }
                     });
                 }
                 else if (colName == "StateCode".ToLower())
                 {
                     
                     int val = int.Parse(this.exactSearch[i]);
                     DateTime nowval = DateTime.Now;
                     switch (val)
                     {
                         case 0: //生效
                             subs = new List<QSmartQueryFilterCondition>();
                             subs.Add(new QSmartQueryFilterCondition
                             {
                                 Column = new QSmartQueryColumn { columnName = "EffectiveDate", dataType = typeof(DateTime) },
                                 Operator = QSmartOperatorEnum.lessequal,
                                 Connector = QSmartConnectorEnum.and,
                                 Values = new List<object> { nowval }
                             });
                             subs.Add(new QSmartQueryFilterCondition
                             {
                                 Column = new QSmartQueryColumn { columnName = "ExpiryDate", dataType = typeof(DateTime) },
                                 Operator = QSmartOperatorEnum.greatequal,
                                 Connector = QSmartConnectorEnum.and,
                                 Values = new List<object> { nowval }
                             });
                             break;
                         case 1: //失效
                             result.Add(new QSmartQueryFilterCondition
                             {
                                 Column = new QSmartQueryColumn { columnName = "ExpiryDate", dataType = typeof(DateTime) },
                                 Operator = QSmartOperatorEnum.less,
                                 Connector = QSmartConnectorEnum.and,
                                 Values = new List<object> { nowval }
                             });
                             break;
                         case 2: //未生效
                             result.Add(new QSmartQueryFilterCondition
                             {
                                 Column = new QSmartQueryColumn { columnName = "EffectiveDate", dataType = typeof(DateTime) },
                                 Operator = QSmartOperatorEnum.great,
                                 Connector = QSmartConnectorEnum.and,
                                 Values = new List<object> { nowval }
                             });
                             break;
                     }
                     
                 }
             }
             if (subs != null)
             {
                 if (result.Count == 0) result = subs;
                 else
                 {
                     foreach (QSmartQueryFilterCondition fc in subs)
                     {
                         result[result.Count - 1].Combins.Add(fc);
                     }
                 }
             }

             return result;
         }
     }
}
