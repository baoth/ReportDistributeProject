using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace weixinreportviews.Controllers
{
    public class TestController : Controller
    {
        //三级1
        public ActionResult Index1()
        {
            return View("connext");
        }
        //三级2
        public ActionResult Index2()
        {
            
            return View("connext1");
        }
    }
}