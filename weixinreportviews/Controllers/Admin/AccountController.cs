using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using weixinreportviews.Model;
using System.Reflection;

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
                new SS_CompanyAccount {Id=Guid.NewGuid(), Name = "12311231", OrderNumber = "1", Phone = "123123", Address ="1"},
                 new SS_CompanyAccount {Id=Guid.NewGuid(), Name = "12311231", OrderNumber = "1", Phone = "123123", Address ="11"},
                  new SS_CompanyAccount {Id=Guid.NewGuid(), Name = "1231213", OrderNumber = "1", Phone = "123123", Address ="111"},
                   new SS_CompanyAccount {Id=Guid.NewGuid(), Name = "12311213", OrderNumber = "1", Phone = "123123", Address ="1111"},
                    new SS_CompanyAccount {Id=Guid.NewGuid(), Name = "1213123", OrderNumber = "1", Phone = "123123", Address ="11111"},
                     new SS_CompanyAccount {Id=Guid.NewGuid(), Name = "12131123", OrderNumber = "1", Phone = "123123", Address ="111111"},
            };
            var t = d.Select(e => new
            {
                e.Id,
                e.Name,
                e.OrderNumber,
                e.Phone,
                e.Address,
                e.CreateDate,
                e.LoginKey

            });
            var dp = ObjectExtend<DataTablesParameter>.BindToObject(Request);
            return Json(new {
                sEcho =dp.sEcho,// param.sEcho,
                iTotalRecords = 50,
                iTotalDisplayRecords = 50,
                aaData=d
            },JsonRequestBehavior.AllowGet) ;
        }


    }
    public class DataTablesParameter
    {
        public DataTablesParameter() { }
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
    public static class ObjectExtend<T> where T : new()
    {
        public static T BindToObject(HttpRequestBase request)
        {
            T t = new T();
            PropertyInfo[] arrProperty = t.GetType().GetProperties();
            foreach (PropertyInfo pi in arrProperty)
            {
                if (!string.IsNullOrEmpty(request[pi.Name]))
                {
                    object value = ParseObj(request[pi.Name], pi);
                    pi.SetValue(t, value, null);
                }
            }
            return t;
        }
        public static object ParseObj(string s, PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(String))
            {
                return s;
            }
            else if (pi.PropertyType == typeof(Int16) || pi.PropertyType == typeof(Nullable<Int16>))
            {
                Int16 result = 0;
                Int16.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(Int32) || pi.PropertyType == typeof(Nullable<Int32>))
            {
                Int32 result = 0;
                Int32.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(Int64) || pi.PropertyType == typeof(Nullable<Int64>))
            {
                Int64 result = 0;
                Int64.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(float) || pi.PropertyType == typeof(Nullable<float>))
            {
                float result = 0;
                float.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(double) || pi.PropertyType == typeof(Nullable<double>))
            {
                double result = 0;
                double.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(Nullable<decimal>))
            {
                decimal result = 0;
                decimal.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(Boolean) || pi.PropertyType == typeof(Nullable<Boolean>))
            {
                bool result = false;
                s = s.ToLower();
                //加上是 和 1 hanyx 2012.5.30
                s = (s == "on" || s == "true" || s == "是" || s == "1" ? "true" : "false");
                Boolean.TryParse(s, out result);
                return result;
            }
            else if (pi.PropertyType == typeof(Double) || pi.PropertyType == typeof(Nullable<Double>))
            {
                double result = 0.0;
                Double.TryParse(s, out  result);
                return result;
            }
            else if (pi.PropertyType == typeof(Array))
            {
                string[] arr = s.Split(',');
                return arr;
            }
            else if (pi.PropertyType == typeof(DateTime) || pi.PropertyType == typeof(Nullable<DateTime>))
            {
                DateTime dateTime = DateTime.Now;
                DateTime.TryParse(s, out dateTime);
                return dateTime;
            }
            else if (pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(Nullable<Guid>))
            {
                if (string.IsNullOrEmpty(s))
                {
                    s = Guid.Empty.ToString();
                }
                return new Guid(s);
            }
            else if (pi.PropertyType == typeof(byte[]))
            {
                return null;
            }
            else
            {
                throw new Exception("不支持的类型解析");
            }
        }
    }
}
