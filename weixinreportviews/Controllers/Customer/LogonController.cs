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
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
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
            return Json(new { error = (int)logininfo.Error });
        }
      
    }
}