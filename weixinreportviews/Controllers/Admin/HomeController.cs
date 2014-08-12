using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace weixinreportviews.Controllers
{
    public class HomeController : BaseController
    {
        //主页123123

        public ActionResult Index()
        {
            return View();
        }
        //二级主页
        public ActionResult ChildIndex()
        {
            ViewData["controller"] = Request["C"];
            return View("CIndex");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
