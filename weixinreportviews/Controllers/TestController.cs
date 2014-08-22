using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;

namespace weixinreportviews.Controllers
{
    public class TestController : Controller
    {
        //三级1
        public ActionResult Grid()
        {
            return View("connext");
        }
        //三级2
        public ActionResult Model()
        {
            
            return View("connext1");
        }

        public void CreateTable()
        {
            try
            {
                //DbSession session = General.CreateDbSession();
                //session.Context.CreateTable<CS_UserAttention>();
                //session.Context.SaveChange();
                Response.Write("=====创建完成=======");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }
    }
}