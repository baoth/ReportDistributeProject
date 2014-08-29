using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
namespace weixinreportviews.Controllers
{
    public class BaseController : Controller
    {
        protected override void  OnActionExecuted(ActionExecutedContext filterContext)
        {

            base.OnActionExecuted(filterContext);
            //得到用户登录的信息
            var CurrentUserInfo = Session[General.LogonSessionName] as CustomerLoginInfo ;
            //判断用户是否为空
            if (CurrentUserInfo == null)
            {
                Response.Redirect("/Logon/Index");
            }
            //Response.Redirect("")
        }
        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
    }

    
}