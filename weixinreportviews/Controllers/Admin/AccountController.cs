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
        public ActionResult Model(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                DbSession session = General.CreateDbSession();
                SS_CompanyAccount account = session.Retrieve<SS_CompanyAccount>(
                    "Id", Guid.Parse(id));
            }
            return View("Model");
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
            return View("Index");
        }

        /// <summary>
        /// 获取账户列表信息
        /// </summary>
        /// <returns></returns>
       //[HttpPost]
        public JsonResult GridDatas()//int pageindex,int pagecount)

        {
            var d = new List<SS_CompanyAccount>() { 
                new SS_CompanyAccount { Name = "12311231", OrderNumber = "1", Phone = "123123", Address ="1"},
                 new SS_CompanyAccount { Name = "12311231", OrderNumber = "1", Phone = "123123", Address ="11"},
                  new SS_CompanyAccount { Name = "1231213", OrderNumber = "1", Phone = "123123", Address ="111"},
                   new SS_CompanyAccount { Name = "12311213", OrderNumber = "1", Phone = "123123", Address ="1111"},
                    new SS_CompanyAccount { Name = "1213123", OrderNumber = "1", Phone = "123123", Address ="11111"},
                     new SS_CompanyAccount { Name = "12131123", OrderNumber = "1", Phone = "123123", Address ="111111"},
            };
//select a.*,b.* from

//(select top 1000 * from bx_main where guid not in (select top 10000 guid from bx_main order by docnum) order by docnum) a 
//, 
//(select count(*) as count1 from bx_main) b        
            return Json(new {
                sEcho =1,// param.sEcho,
                iTotalRecords = 50,
                iTotalDisplayRecords = 50,
                aaData=d
            },JsonRequestBehavior.AllowGet) ;
        }


    }
    public class DataTableParameter
    {
        /// <summary>
        /// DataTable请求服务器端次数
        /// </summary> 
        public string sEcho { get; set; }

        /// <summary>
        /// 过滤文本
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// 每页显示的数量
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// 分页时每页跨度数量
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// 排序列的数量
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// 逗号分割所有的列
        /// </summary>
        public string sColumns { get; set; }
    }
}
