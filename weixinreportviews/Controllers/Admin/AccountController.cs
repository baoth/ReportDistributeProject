using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace weixinreportviews.Controllers.Admin
{
    public class AccountController : BaseController
    {
        /// <summary>
        /// 获取账户信息页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string id)
        {
            return View();
        }

        /// <summary>
        /// 保存账户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save()
        {
            return null;
        }

        /// <summary>
        /// 获取账户列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Grid()
        {
            return View();
        }

        /// <summary>
        /// 获取账户列表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GridDatas()
        {
            return null;
        }


    }
}
