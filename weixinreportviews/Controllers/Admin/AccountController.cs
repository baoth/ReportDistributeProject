using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;

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
            if (!string.IsNullOrEmpty(id))
            {
                DbSession session = General.CreateDbSession();
                SS_CompanyAccount account = session.Retrieve<SS_CompanyAccount>(
                    "Id", Guid.Parse(id));
            }
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
        public ActionResult GridIndex()
        {
            return View();
        }

        /// <summary>
        /// 获取账户列表信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GridDatas(int pageindex,int pagecount)
        {
//select a.*,b.* from

//(select top 1000 * from bx_main where guid not in (select top 10000 guid from bx_main order by docnum) order by docnum) a 
//, 
//(select count(*) as count1 from bx_main) b        
            return null;
        }


    }
}
