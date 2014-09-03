using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;

namespace weixinreportviews.Controllers.Customer.FirstReportProduct
{
    public class FReportHomeController : Controller
    {
        //
        // GET: /FReportHome/

        public ActionResult Index()
        {
            if (Session[weixinreportviews.Model.General.LogonSessionName] != null)
            {
                var obj = (CustomerLoginInfo)Session[weixinreportviews.Model.General.LogonSessionName];
                ViewData["Name"] = obj.Account.LoginKey;
                ViewData["Id"] = obj.Account.Id;
            }
            if (General.PhoneBroswer(Request))
            {
                return View("MobileIndex");
            }
            else
            {
                return View();
            }
        }

    }
}
