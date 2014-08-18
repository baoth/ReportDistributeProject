using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace weixinreportviews.Controllers.Admin
{
    public class LisenceController : Controller
    {
        public ActionResult Grid()
        {
            return View("Index");
        }
        //
        // GET: /Lisence/

        public ActionResult Index()
        {
            return View();
        }

    }
}
