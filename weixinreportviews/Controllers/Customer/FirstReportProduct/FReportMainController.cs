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
            if (!string.IsNullOrEmpty(id))
            {
                var session = General.CreateDbSession();
                var ent = session.Retrieve<CS_FirstReport>(
                    "Id", Guid.Parse(id));
                if (ent!=null) {
                    ViewData.Add("CreateUrl",PathTools.AddWebHeadAddress(ent.CreateUrl));
                    ViewData.Add("Id", ent.Id);
                    ViewData.Add("ReportKey", ent.ReportKey);
                    ViewData.Add("Title", ent.Title);
                    ViewData.Add("eState", "disabled");
                }
            }
           return View("Model");
        }

        [HttpPost]
        public ActionResult Save()
        {
            try
            {
                var account = General.CreateInstance<CS_FirstReport>(Request);
                var session = General.CreateDbSession();
              
                if (account.Id != Guid.Empty)
                {
                    account.CreateUrl = ChangeAddress(account.CreateUrl,account.Id);
                    session.Context.ModifyEntity(account.CreateQSmartObject());
                }
                else
                {
                    account.Id = Guid.NewGuid();
                    account.CreateUrl = ChangeAddress(account.CreateUrl,account.Id);
                    account.CreateDate = DateTime.Now;
                    account.Stoped = false;
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

            if (!string.IsNullOrEmpty(id))
            {
                List<string> ids = id.Split(',').ToList();
                DbSession session = General.CreateDbSession();
                List<string> paths = new List<string>();
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        var entity = new CS_FirstReport { Id = Guid.Parse(ids[i]) };
                        session.Context.DeleteEntity(entity.CreateDeleteCommand());
                        if (!string.IsNullOrEmpty(entity.CreateUrl))
                        {
                            paths.Add(PathTools.GetAbsolutePath(entity.CreateUrl));
                        }
                    }
                }

                try
                {
                    session.Context.SaveChange();
                    foreach (var item in paths)
	                {
                        System.IO.File.Delete(item);
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
        public ActionResult UploadView ()
        {
           return View("Upload");
        }
        public JsonResult Upload() 
        {
            var files = Request.Files;
            var path =System.IO.Path.Combine(General.BaseDirector,"temp");
            var fileKey = Guid.NewGuid();
            var filePath = "";
            for (int i = 0; i < files.Count; i++)
            {
                var httpFile = files[i]; ;
                if (httpFile.ContentLength > 0) {
                    if (!System.IO.Directory.Exists(path)) {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    filePath = System.IO.Path.Combine(path, httpFile.FileName);
                    httpFile.SaveAs(filePath);
                }
                ReportBuilderSession rbs = new ReportBuilderSession();
                var rb = rbs.GetBuilder(fileKey, ReportBuilderEnum.Excel文件创建框架);
                if (rb != null)
                {
                    try
                    {
                        if (!rb.Build(filePath))
                        {
                            return Json(new
                            {
                                err = 1,
                                message = "读取数据文件出错！"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                      
                        return Json(new
                        {
                            err = 1,
                            message = ex.Message
                        });
                    }
                }
                filePath=rb.HtmlUrl;
            }
            return Json(new { data =PathTools.AddWebHeadAddress(filePath.Replace("\\","//")) });
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
            var dtp = General.CreateInstance<FirstReportDataTablesParameter>(Request);
            int totalcount = 0;
            DbSession session = General.CreateDbSession();
            var rows = session.PaginationRetrieve<CS_FirstReport>(dtp.iDisplayStart,
                dtp.iDisplayLength, dtp.GetFilters<CS_FirstReport>(),
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
        public JsonResult Validation(string col, string value)
        {
            DbSession session = General.CreateDbSession();
            return Json(new
            {
                result = session.Exists<CS_FirstReport>(col, value) == true ? 0 : 1
            });
        }
        public string ChangeAddress(string path,Guid id) 
        {
            CustomerLoginInfo customerInfo= Session[General.LogonSessionName] as CustomerLoginInfo;
            var tempPath = PathTools.RemoveWebHeadAddress(path).Replace("\\", "//");
            tempPath = tempPath.Substring(2);
            var rePathDir = PathTools.GenerateHtmlPath+"//"+customerInfo.Account.LoginKey;
            var newPathDir = System.IO.Path.Combine(PathTools.BaseDirector, rePathDir);
            if (!System.IO.File.Exists(newPathDir)) {
                System.IO.Directory.CreateDirectory(newPathDir);
            }
            var copyPath = System.IO.Path.Combine(PathTools.BaseDirector, tempPath);
            System.IO.File.Copy(copyPath,System.IO.Path.Combine(newPathDir,id.ToString()+".html"), true);
            System.IO.File.Delete(copyPath);
            return rePathDir +"//"+ id.ToString() + ".html";

        }
    }
    public class FirstReportDataTablesParameter : DataTablesParameter
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
