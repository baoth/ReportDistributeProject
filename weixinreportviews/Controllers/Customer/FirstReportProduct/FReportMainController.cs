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
                    ViewData.Add("CreateUrl", PathTools.AddWebHeadAddress(ent.Url).Replace("\\", "/"));
                    ViewData.Add("Id", ent.Id);
                    
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
                var ent = General.CreateInstance<CS_FirstReport>(Request);
                var session = General.CreateDbSession();
                var path = Request["CreateUrl"];
                var logo = Request["Logo"];
                if (ent.Id != Guid.Empty)
                {
                    if (path.Contains(PathTools.TempPath))
                    {
                        ChangeAddress(path, ent.Id, customerInfo.Account.Id.ToString(),logo);
                    }
                    ent.AccountId = customerInfo.Account.Id;
                    session.Context.ModifyEntity(ent.CreateQSmartObject());
                }
                else
                {
                    ent.Id = Guid.NewGuid();
                    ChangeAddress(path, ent.Id, customerInfo.Account.Id.ToString(),logo);
                    ent.AccountId = customerInfo.Account.Id;
                    ent.CreateDate = DateTime.Now;
                    ent.Stoped = false;
                    session.Context.InsertEntity(ent.CreateQSmartObject());
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
                        var dir = Path.Combine(PathTools.SaveHtmlPath, customInfo.Account.Id.ToString().Replace("-", ""), entity.Id.ToString().Replace("-", ""));
                        //var filePath = PathTools.SaveHtmlPath + "\\" + customInfo.Account.Id.ToString().Replace("-", "") + "\\" + entity.Id.ToString().Replace("-", "") + ".html";
                        //if (System.IO.File.Exists(filePath))
                        //{
                        //    paths.Add(filePath);
                        //}
                        if (System.IO.Directory.Exists(dir))
                        {
                            paths.Add(dir);
                        }
                    }
                }

                try
                {
                    session.Context.SaveChange();
                    foreach (var item in paths)
	                {
                        //System.IO.File.Delete(item);
                        System.IO.Directory.Delete(item,true);
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
            string fname = Guid.NewGuid().ToString().Replace("-","");
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
            string tempFullPath = string.Empty;
            try
            {
                //if (fi.Extension.ToLower() == ".xls" || fi.Extension.ToLower() == ".xlsx")
                //{
                //    ExcelReportBuilder erb = new ExcelReportBuilder(PathTools.RPTemplatePath);
                //    erb.Build(path, Path.Combine(PathTools.BaseDirector,
                //            "temp",
                //            htmlName));

                //    tempFullPath = Path.Combine(PathTools.TempPath,htmlName);
                //}
                if (fi.Extension.ToLower() == ".xls" || fi.Extension.ToLower() == ".xlsx")
                {
                    htmlName = "x" + htmlName;
                    //ExcelReportBuilder erb = new ExcelReportBuilder(PathTools.RPTemplatePath);
                    ExcelReportBuilderEx erb = new ExcelReportBuilderEx(PathTools.RPTemplatePath);
                    string savePath = Path.Combine(PathTools.BaseDirector,
                            "temp/" + "x" + fname,
                            htmlName);
                    //erb.Build(path, savePath);

                    erb.Build(path, Path.Combine(PathTools.BaseDirector,
                            "temp/" + "x" + fname), "x" + fname);
                            

                    tempFullPath = Path.Combine(PathTools.TempPath, "x" + fname, htmlName);
                }
                else if (fi.Extension.ToLower() == ".doc" || fi.Extension.ToLower()==".docx")
                {
                    htmlName = "w" + htmlName;
                    WordReportBuilder erb = new WordReportBuilder(PathTools.RPTemplatePath);
                    string savePath=Path.Combine(PathTools.BaseDirector,
                            "temp/" + "w"+fname,
                            htmlName);
                    erb.Build(path,savePath);

                    tempFullPath = Path.Combine(PathTools.TempPath,"w"+fname, htmlName);
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
            return Json(new { data = PathTools.AddWebHeadAddress(tempFullPath) });
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult Upload2()
        {
            //保存上传文件
            string path = string.Empty;
            FileInfo fi = null;
            string fname = Guid.NewGuid().ToString().Replace("-","");
            string fullname=string.Empty;
            try
            {
                foreach (string fid in Request.Files)
                {
                    if (Request.Files[fid] == null) continue;

                    fi = new FileInfo(Request.Files[fid].FileName);
                    var pathDir = Path.Combine(PathTools.BaseDirector, PathTools.TempPath);
                    if (!System.IO.Directory.Exists(pathDir))
                    {
                        System.IO.Directory.CreateDirectory(pathDir);
                    }
                    path = Path.Combine(pathDir, fname + "logo" + fi.Extension);
                    fullname=fname + "logo" + fi.Extension;
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

            return Json(new { err = 0, name = fullname});
        }
        /// <summary>
        /// 获取账户列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Grid()
        {
            System.Diagnostics.Trace.WriteLine("Grid");
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
        //public string ChangeAddress(string path,Guid id,string companyId) 
        //{
            
        //    var tempPath = PathTools.RemoveWebHeadAddress(path).Replace("\\", "//");
        //    tempPath = tempPath.Substring(2);
        //    var rePathDir = PathTools.GenerateHtmlPath + "//" + companyId;
        //    var newPathDir = System.IO.Path.Combine(PathTools.BaseDirector, rePathDir);
        //    if (!System.IO.File.Exists(newPathDir)) {
        //        System.IO.Directory.CreateDirectory(newPathDir);
        //    }
        //    var copyPath = System.IO.Path.Combine(PathTools.BaseDirector, tempPath);
        //    System.IO.File.Copy(copyPath,System.IO.Path.Combine(newPathDir,id.ToString()+".html"), true);
        //    System.IO.File.Delete(copyPath);

        //    //公司下的文件
        //    List<string> fileNameList=new List<string> ();
        //    DirectoryInfo dInfo = new DirectoryInfo(newPathDir);
        //    GetAllFileNameById(dInfo, id.ToString(), ref fileNameList);


        //    return rePathDir +"//"+ id.ToString() + ".html";

        //}

         public string ChangeAddress(string path, Guid id, string companyId,string logo)
         {
             var fullPathDir = string.Empty;
             var tempPath = PathTools.RemoveWebHeadAddress(path).Replace("\\", "//");
            tempPath = tempPath.Substring(1);
             var rePathDir = PathTools.GenerateHtmlPath + "//" + companyId.Replace("-","");
             var newPathDir = System.IO.Path.Combine(PathTools.BaseDirector, rePathDir);
             //if (!System.IO.File.Exists(newPathDir))
             if(!System.IO.Directory.Exists(newPathDir))
             {
                 System.IO.Directory.CreateDirectory(newPathDir);
             }
             
             //logo迁移
             if (!string.IsNullOrEmpty(logo))
             {
                 var copypath = System.IO.Path.Combine(PathTools.BaseDirector, PathTools.TempPath,logo);
                 if (System.IO.File.Exists(copypath))
                 {
                     var toPath = System.IO.Path.Combine(newPathDir, id.ToString().Replace("-","") + "logo.jpg");
                     System.IO.File.Copy(copypath, toPath, true);
                     System.IO.File.Delete(copypath);
                 }
             }
             
             var tempFullFileName = tempPath.Substring(tempPath.LastIndexOf('/')+1);
             var tempType = tempFullFileName.Substring(0, 1);
             var tempFileName = tempFullFileName.Substring(0, tempFullFileName.LastIndexOf("."));  
             
             if ((tempType=="x" || tempType == "w") && tempFileName.Length > 32)//word excel 转换
             {
                 var copyPath = System.IO.Path.Combine(PathTools.BaseDirector, tempPath.Substring(0, tempPath.LastIndexOf('/'))).Replace("/", "\\");//.Substring(0, tempPath.LastIndexOf('/') - 1).Replace("/","\\"), tempFileName
                 var toPath=System.IO.Path.Combine(newPathDir, id.ToString().Replace("-",""));
                 var saveFileName = id.ToString().Replace("-", "") + ".html";
                 CopyFolder(copyPath,toPath,saveFileName);
                // System.IO.Directory.Delete(copyPath);
                 DirectoryInfo di = new DirectoryInfo(copyPath);
                 di.Delete(true);
                 fullPathDir = System.IO.Path.Combine(toPath,saveFileName);
             }
             else
             {
                 var copyPath = System.IO.Path.Combine(PathTools.BaseDirector, tempPath);
                 System.IO.File.Copy(copyPath, System.IO.Path.Combine(newPathDir, id.ToString().Replace("-","") + ".html"), true);
                 System.IO.File.Delete(copyPath);
                 fullPathDir = System.IO.Path.Combine(newPathDir, id.ToString().Replace("-","") + ".html");
             }



             return fullPathDir;

         }
        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        /// <param name="saveFileName"></param>
         private static void CopyFolder(string fromPath, string toPath,string saveFileName)
         {
             if (!Directory.Exists(toPath))
                 Directory.CreateDirectory(toPath);

             // 子文件夹
             foreach (string sub in Directory.GetDirectories(fromPath))
                 CopyFolder(sub + "\\", toPath + "\\" + Path.GetFileName(sub) + "\\",saveFileName);

             // 文件
             foreach (string file in Directory.GetFiles(fromPath))
             {                 
                 var fileName = Path.GetFileName(file);
                 string extension = GetExtension(fileName);
                 if (extension == ".html")
                 {                    
                     System.IO.File.Copy(file,Path.Combine(toPath,saveFileName), true);
                 }
                 else
                 {
                     System.IO.File.Copy(file, Path.Combine(toPath, Path.GetFileName(file)), true);
                 }
             }
               
         }

         private static string GetExtension(string fileName)
         {
             return fileName.Substring(fileName.LastIndexOf('.'));
         }
      
        public void GetAllFileNameById(DirectoryInfo dd,string id,ref List<string> fileNameList)
        {
            FileInfo[] allfile = dd.GetFiles(id+".*");
            foreach (FileInfo tt in allfile)
            {
                fileNameList.Add(tt.Name);
            }
            DirectoryInfo[] direct = dd.GetDirectories();
            foreach (DirectoryInfo dirTemp in direct)
            {
                GetAllFileNameById(dirTemp, id, ref fileNameList);
            }
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
