using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
using System.Reflection;
using QSmart.Core.Object;
using QSmart.Core.DataBase;
using System.IO;

namespace weixinreportviews.Controllers.Customer.FirstReportProduct
{
    public class FReportMainController :BaseController
    {
        [HttpGet]
        public ActionResult Model(string id)
        {
            ViewData.Add("YLState", "disabled");
            if (!string.IsNullOrEmpty(id))
            {
                var session = General.CreateDbSession();
                var ent = session.Retrieve<CS_FirstReport>("Id", Guid.Parse(id));
                if (ent!=null) {
                    ViewData.Add("CreateUrl",PathTools.AddWebHeadAddress(ent.Url));
                    ViewData.Add("Id", ent.Id);
                    ViewData.Add("ReportKey", ent.ReportKey);
                    ViewData.Add("Title", ent.Title);
                    ViewData.Remove("YLState");
                }
            }
           return View("Model");
        }

        [HttpPost]
        public ActionResult Save()
        {
            try
            {
                var customerInfo = Session[General.LogonSessionName] as CustomerLoginInfo;
                var account = General.CreateInstance<CS_FirstReport>(Request);
                var session = General.CreateDbSession();
                var path = Request["CreateUrl"];
                if (account.Id != Guid.Empty)
                {
                    if (path.Contains(PathTools.TempPath))
                    {
                        ChangeAddress(path, account.Id, customerInfo.Account.LoginKey);
                    }
                    account.AccountId = customerInfo.Account.Id;
                    session.Context.ModifyEntity(account.CreateQSmartObject());
                }
                else
                {
                    account.Id = Guid.NewGuid();
                    ChangeAddress(path, account.Id, customerInfo.Account.Id.ToString());
                    account.AccountId = customerInfo.Account.Id;
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
                var customInfo = Session[General.LogonSessionName] as CustomerLoginInfo ;
                for (int i = 0; i < ids.Count; i++)
                {
                    if (!string.IsNullOrEmpty(ids[i]))
                    {
                        var entity = new CS_FirstReport { Id = Guid.Parse(ids[i]) };
                        session.Context.DeleteEntity(entity.CreateDeleteCommand());
                        var filePath = PathTools.SaveHtmlPath + "\\" + customInfo.Account.Id + "\\" + entity.Id + ".html";
                        if (System.IO.File.Exists(filePath))
                        {
                            paths.Add(filePath);
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
        /// <summary>
        /// 上传文件并生成html
        /// </summary>
        /// <returns></returns>
        public JsonResult Upload() 
        {
            //保存上传文件
            string path = string.Empty;
            FileInfo fi = null;
            string fname = Guid.NewGuid().ToString();
            try
            {
                foreach (string fid in Request.Files)
                {
                    if (Request.Files[fid] == null) continue;
                    
                    fi = new FileInfo(Request.Files[fid].FileName);
                    var pathDir=Path.Combine(PathTools.BaseDirector,PathTools.TempPath);
                    if (!System.IO.Directory.Exists(pathDir)) {
                        System.IO.Directory.CreateDirectory(pathDir);
                    }
                    path = Path.Combine(pathDir, fname + fi.Extension);
                    Request.Files[fid].SaveAs(path);

                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    err = 1,
                    message = "文件上传失败:" + ex.Message
                });
            }

            string htmlName = fname + ".html";
            try
            {
                if (fi.Extension.ToLower() == ".xls" || fi.Extension.ToLower() == ".xlsx")
                {
                    ExcelReportBuilder erb = new ExcelReportBuilder(PathTools.RPTemplatePath);
                    erb.Build(path, Path.Combine(PathTools.BaseDirector,
                            "temp",
                            htmlName));
                }
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    err = 1,
                    message = "生成html文件失败:" + ex.Message
                });
            }


            return Json(new { data = PathTools.AddWebHeadAddress(PathTools.TempPath+"\\" + htmlName) });
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
            var curtomInfo=Session[General.LogonSessionName] as CustomerLoginInfo;
            if (dtp.exactFilter == null)
            {
                dtp.exactSearch = new string[1];
                dtp.exactFilter = new string[1];
                dtp.exactSearch.SetValue(curtomInfo.Account.Id.ToString(), 0);
                dtp.exactFilter.SetValue("AccountId", 0);
            }
            else
            {
                dtp.exactSearch = dtp.exactSearch.Add(curtomInfo.Account.Id.ToString());
                dtp.exactFilter = dtp.exactFilter.Add("AccountId");
            }
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
                result = session.Exists<CS_FirstReport>(col,value) == true ? 0 : 1
            });
        }
         [HttpPost]
        public JsonResult ValidationEnt(string col, string value,Guid id)
        {
            var customerInfo = Session[General.LogonSessionName] as CustomerLoginInfo;
            DbSession session = General.CreateDbSession();
            var  result = session.ExistsEnt<CS_FirstReport>(col, value, new List<QSmartQueryFilterCondition>() { 
                new QSmartQueryFilterCondition(){
                     Column = new QSmartQueryColumn { columnName = "Id", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.unequal,
                    Connector = QSmartConnectorEnum.and,
                    Values = new List<object> { id }
                },
                new QSmartQueryFilterCondition(){
                     Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                    Operator = QSmartOperatorEnum.equal,
                    Connector = QSmartConnectorEnum.and,
                    Values = new List<object> { customerInfo.Account.Id }
                },
            });
            return Json(new
            {
                result=result? 0 : 1
            });
        }
        public string ChangeAddress(string path,Guid id,string companyId) 
        {
            
            var tempPath = PathTools.RemoveWebHeadAddress(path).Replace("\\", "//");
            tempPath = tempPath.Substring(2);
            var rePathDir = PathTools.GenerateHtmlPath + "//" + companyId;
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
            return result;
        }
    }

}
