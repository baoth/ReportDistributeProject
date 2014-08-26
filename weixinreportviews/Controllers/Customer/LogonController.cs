using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;

namespace weixinreportviews.Controllers
{
    public class LogonController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginkey">账户</param>
        /// <param name="password">密码</param>        
        public ActionResult Index()
        {
           
            return View();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session[General.LogonSessionName] = null;
            return Json(new { result= 0});
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginkey">账户</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Login(string loginkey, string password)
        {
            CustomerLoginInfo logininfo = General.Login(loginkey, password);
            if (logininfo.Error == CustomerLoginErrorEnum.成功)
            {
                Session[General.LogonSessionName] = logininfo;               
            }
            return Json(new { error = (int)logininfo.Error });
        }
      
    }
}