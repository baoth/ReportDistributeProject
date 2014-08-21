using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;
using QSmart.Core.Object;
using System.Data;
using System.Reflection;

namespace weixinreportviews.Model
{
    public static class General
    {
        public const string Token = "tiantian315";
        public const string AppId = "wxadcc12090e7138b3";
        public const string AppSecret = "722dfaf58953c47ffd9ac9548c727461";
        public const string authurl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxadcc12090e7138b3&redirect_uri=http%3A%2F%2Fwww.baoth.com%2fHome&response_type=code&scope=snsapi_base&state=123#wechat_redirect";

        public static string BaseDirector
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static DbSession CreateDbSession() { return new DbSessionInstance(); }

        /// <summary>
        /// 生成16位的订单号
        /// </summary>
        /// <param name="Prefix">订单号前缀(2位）</param>
        /// <returns>订单号</returns>
        public static string CreateOrderNumber(string Prefix)
        {
            Prefix = Prefix.ToUpper();
            if (Prefix.Length > 2) Prefix = Prefix.Substring(0, 2);
            string subfix = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            DateTime dt=DateTime.Now;
            return Prefix + dt.Year.ToString().Substring(2, 2) +
                string.Format("{0:D2}", dt.Month) + string.Format("{0:D2}", dt.Day) + subfix;
        }

        /// <summary>
        /// 根据HttpRequest创建模型实例
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="request">HttpRequest</param>
        /// <returns>模型实例</returns>
        public static T CreateInstance<T>(HttpRequestBase request) where T : new()
        {
            PropertyInfo[] pis = typeof(T).GetProperties();
            T obj = new T();
            for (int i = 0; i < pis.Length; i++)
            {
                PropertyInfo pi = pis[i];
                string value = string.IsNullOrEmpty(request[pi.Name]) ?
                    string.IsNullOrEmpty(request[pi.Name.ToLower()]) ?
                    string.IsNullOrEmpty(request[pi.Name.ToUpper()]) ? string.Empty : request[pi.Name.ToUpper()]
                    : request[pi.Name.ToLower()] : request[pi.Name];
                if (!string.IsNullOrEmpty(value))
                {
                    pi.SetValue(obj, ParseObj(pi.PropertyType, value), null);
                }
            }
            return obj;
        }

        /// <summary>
        /// 字符串值转换成类型值
        /// </summary>
        /// <param name="PropertyType">类型</param>
        /// <param name="value">字符串值</param>
        /// <returns>类型值</returns>
        public static object ParseObj(Type PropertyType, string value)
        {
            if (PropertyType == typeof(String))
            {
                return value;
            }
            else if (PropertyType == typeof(Int16) || PropertyType == typeof(Nullable<Int16>))
            {
                Int16 result = 0;
                Int16.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(Int32) || PropertyType == typeof(Nullable<Int32>))
            {
                Int32 result = 0;
                Int32.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(Int64) || PropertyType == typeof(Nullable<Int64>))
            {
                Int64 result = 0;
                Int64.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(float) || PropertyType == typeof(Nullable<float>))
            {
                float result = 0;
                float.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(double) || PropertyType == typeof(Nullable<double>))
            {
                double result = 0;
                double.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(decimal) || PropertyType == typeof(Nullable<decimal>))
            {
                decimal result = 0;
                decimal.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(Boolean) || PropertyType == typeof(Nullable<Boolean>))
            {
                bool result = false;
                value = value.ToLower();
                //加上是 和 1 hanyx 2012.5.30
                value = (value == "on" || value == "true" || value == "是" || value == "1" ? "true" : "false");
                Boolean.TryParse(value, out result);
                return result;
            }
            else if (PropertyType == typeof(Double) || PropertyType == typeof(Nullable<Double>))
            {
                double result = 0.0;
                Double.TryParse(value, out  result);
                return result;
            }
            else if (PropertyType == typeof(Array))
            {
                string[] arr = value.Split(',');
                return arr;
            }
            else if (PropertyType == typeof(String[]))
            {
                string[] arr = value.Split(',');
                return arr;
            }
            else if (PropertyType == typeof(DateTime) || PropertyType == typeof(Nullable<DateTime>))
            {
                DateTime dateTime = DateTime.Now;
                DateTime.TryParse(value, out dateTime);
                return dateTime;
            }
            else if (PropertyType == typeof(Guid) || PropertyType == typeof(Nullable<Guid>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = Guid.Empty.ToString();
                }
                return new Guid(value);
            }
            else if (PropertyType == typeof(byte[]))
            {
                return null;
            }
            else
            {
                throw new Exception("不支持的类型解析");
            }
        }

        /// <summary>
        /// 客户登陆后台管理
        /// </summary>
        /// <param name="LoginKey">账户名</param>
        /// <param name="Password">密码</param>
        /// <returns>信息</returns>
        public static CustomerLoginInfo Login(string LoginKey, string Password)
        {
            CustomerLoginInfo result=new CustomerLoginInfo();
            DbSession session = General.CreateDbSession();
            result.Account = session.Retrieve<SS_CompanyAccount>("LoginKey", LoginKey);
            result.Error = !(result.Account != null && !result.Account.Stoped && Password == result.Account.Password);
            result.ErrorMsg = result.Account == null ? "账户不存在，请联系客服人员解决。" :
                result.Account.Stoped ? "账户已停用，请联系客服人员解决。" :
                result.Account.Password != Password ? "账户密码错误。" : string.Empty;
            return result;
        }

        /// <summary>
        /// 微信客户关注
        /// </summary>
        /// <param name="LoginKey">账户名</param>
        /// <returns>信息</returns>
        public static CustomerLoginInfo Attension(string LoginKey)
        {
            CustomerLoginInfo result = new CustomerLoginInfo();
            DbSession session = General.CreateDbSession();
            result.Account = session.Retrieve<SS_CompanyAccount>("LoginKey", LoginKey);
            result.Error = !(result.Account != null && !result.Account.Stoped);
            result.ErrorMsg = result.Account == null ? "您关注的账户不存在，请联系客服人员解决。" :
                result.Account.Stoped ? "您关注的账户已停用，请联系客服人员解决。" : string.Empty;
            return result;
        }
    }

    /// <summary>
    /// 客户登陆或关注返回信息
    /// </summary>
    public class CustomerLoginInfo
    {
        public SS_CompanyAccount Account { get; set; }
        public bool Error { get; set; }
        public string ErrorMsg { get; set; }
    }

    public abstract class DbSession: QSmartSession
    {
        protected DbSession(string name) : base(name) { }

        /// <summary>
        /// 通过唯一键获取模型对象
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="UniqueKeyName">唯一键名称</param>
        /// <param name="UniqueKeyValue">唯一键值</param>
        /// <returns>模型对象</returns>
        public T Retrieve<T>(string UniqueKeyName, object UniqueKeyValue) where T : QSmartEntity
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(T).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition());
            Query.FilterConditions[0].Column = new QSmartQueryColumn();
            Query.FilterConditions[0].Column.columnName = UniqueKeyName;
            Query.FilterConditions[0].Column.dataType = UniqueKeyValue.GetType();
            Query.FilterConditions[0].Operator = QSmartOperatorEnum.equal;
            Query.FilterConditions[0].Values.Add(UniqueKeyValue);
            var results = this.Context.QueryEntity<T>(Query);
            return results.Count == 0 ? null : results[0];
        }

        /// <summary>
        /// 判断是否存在实例
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="UniqueKeyName">唯一键名称</param>
        /// <param name="UniqueKeyValue">唯一键值</param>
        /// <returns>true,存在 false,不存在</returns>
        public bool Exists<T>(string UniqueKeyName, object UniqueKeyValue) where T : QSmartEntity
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(T).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition());
            Query.FilterConditions[0].Column = new QSmartQueryColumn();
            Query.FilterConditions[0].Column.columnName = UniqueKeyName;
            Query.FilterConditions[0].Column.dataType = UniqueKeyValue.GetType();
            Query.FilterConditions[0].Operator = QSmartOperatorEnum.equal;
            Query.FilterConditions[0].Values.Add(UniqueKeyValue);
            DataTable dt = this.Context.QueryTable(Query);
            return dt.Rows.Count > 0 ? true : false;
        }

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="PageStart">起始行index</param>
        /// <param name="PageLength">一页需要显示多少条记录</param>
        /// <param name="Conditions">过滤条件</param>
        /// <param name="OrderBys">排序条件</param>
        /// <param name="TotalCount">返回总条数</param>
        /// <returns>数据集合</returns>
        public List<T> PaginationRetrieve<T>(int PageStart, int PageLength, List<QSmartQueryFilterCondition> Conditions
            ,Dictionary<QSmartQueryColumn, QSmartOrderByEnum> OrderBys,out int TotalCount)
            where T : QSmartEntity
        {
            TotalCount=0;

            QSmartQuery QueryA = new QSmartQuery();
            QueryA.Tables.Add(new QSmartQueryTable());
            QueryA.Tables[0].tableName = typeof(T).Name;
            QueryA.TopSetting.Effective = true;
            QueryA.TopSetting.Value = PageLength;
            QueryA.TopSetting.BeginValue = PageStart;
            QueryA.TopSetting.PrimaryKeyName = GetPrimaryKeyName<T>();
           
            if (Conditions != null && Conditions.Count > 0)
            {
                foreach (QSmartQueryFilterCondition fc in Conditions) QueryA.FilterConditions.Add(fc);
            }
            if (OrderBys != null && OrderBys.Count > 0)
            {
                foreach (QSmartQueryColumn qc in OrderBys.Keys) QueryA.OrderBys.Add(qc, OrderBys[qc]);
            }


            QSmartQuery QueryB = new QSmartQuery();
            QueryB.Tables.Add(new QSmartQueryTable());
            QueryB.Tables[0].tableName = typeof(T).Name;
            QueryB.CountSetting.Effective = true;
            QueryB.CountSetting.AliasName = "TotalCount";
            if (Conditions != null && Conditions.Count > 0)
            {
                foreach (QSmartQueryFilterCondition fc in Conditions) QueryB.FilterConditions.Add(fc);
            }

            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable
            {
                aliasName = "a",
                joinType = QSmartJoinEnum.comma,
                tableNameCreator = QueryA
            });

            Query.Tables.Add(new QSmartQueryTable
            {
                aliasName = "b",

                tableNameCreator = QueryB
            });
            DataTable dt = this.Context.QueryTable(Query);
            if (dt != null && dt.Rows.Count > 0) TotalCount = (int)dt.Rows[0]["TotalCount"];
            
            return this.Context.ConversionEntity<T>(dt);
        }

        /// <summary>
        /// 获取模型主键名称
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <returns>主键名称</returns>
        private string GetPrimaryKeyName<T>() where T : QSmartEntity
        {
            Type mtype = typeof(T);
            PropertyInfo[] infos = mtype.GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo pi = infos[i];
                object[] cas = pi.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
                if (cas.Count() > 0) return pi.Name;
            }
            return string.Empty;
        }
    }

    class DbSessionInstance : DbSession
    {
        public DbSessionInstance() : base("db") { }
    }

    public class DataTablesParameter
    {
        /// <summary>
        /// DataTable请求服务器端次数
        /// </summary> 
        public string sEcho { get; set; }

        /// <summary>
        /// 模糊过滤文本
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
        /// 所有排序列名称
        /// </summary>
        public string[] allSortCol { get; set; }

        /// <summary>
        /// 对应排序列排序方式
        /// </summary>
        public string[] allDir { get; set; }

        /// <summary>
        /// 所有需要模糊过滤的列
        /// </summary>
        public string[] allFilter { get; set; }

        /// <summary>
        /// 所有需要精确多虑的列
        /// </summary>
        public string[] exactFilter { get; set; }

        /// <summary>
        /// 所有需要精确多虑的列值（与exactFilter值一一对应）
        /// </summary>
        public string[] exactSearch { get; set; }

        public List<QSmartQueryFilterCondition> GetFilters<T>() where T : new()
        {
            Type mType = typeof(T);
            PropertyInfo[] pis = mType.GetProperties();
            List<QSmartQueryFilterCondition> exacts = this.ExactSearch(pis);
            List<QSmartQueryFilterCondition> fuzzys = this.FuzzySearch(pis);
            if (exacts == null || exacts.Count == 0) return fuzzys;
            if (fuzzys == null || fuzzys.Count ==0) return exacts;
            QSmartQueryFilterCondition combinitem = new QSmartQueryFilterCondition();
            combinitem.Connector = QSmartConnectorEnum.and;
            foreach (var item in fuzzys)
            {
                combinitem.Combins.Add(item);
            }
            exacts.Add(combinitem);
            return exacts;
        }

        /// <summary>
        /// 模糊过滤条件
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <returns>过滤条件</returns>
        protected List<QSmartQueryFilterCondition> FuzzySearch<T>() where T : new()
        {
            return FuzzySearch(typeof(T).GetProperties());
        }

        /// <summary>
        /// 模糊过滤条件
        /// </summary>
        /// <param name="pis">模型属性集合</param>
        /// <returns>过滤条件</returns>
        protected List<QSmartQueryFilterCondition> FuzzySearch(PropertyInfo[] pis)
        {
            if (string.IsNullOrEmpty(sSearch) || this.allFilter == null || this.allFilter.Length == 0) return null;
            List<QSmartQueryFilterCondition> result = new List<QSmartQueryFilterCondition>();
            for (int i = 0; i < this.allFilter.Length; i++)
            {
                string col = this.allFilter[i];
                PropertyInfo pi = pis.First(e => e.Name.ToLower() == col.ToLower());
                if (pi == null) continue;
                result.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = pi.Name, dataType = pi.PropertyType },
                    Operator = QSmartOperatorEnum.like,
                    Connector = QSmartConnectorEnum.or,
                    Values = new List<object> { "%" + sSearch + "%" }
                });
            }
            return result;
        }

        /// <summary>
        /// 精确过滤条件
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <returns>过滤条件</returns>
        protected virtual List<QSmartQueryFilterCondition> ExactSearch<T>() where T : new()
        {
            return ExactSearch(typeof(T).GetProperties());
        }

        /// <summary>
        /// 精确过滤条件
        /// </summary>
        /// <param name="pis">模型属性集合</param>
        /// <returns>过滤条件</returns>
        protected virtual List<QSmartQueryFilterCondition> ExactSearch(PropertyInfo[] pis)
        {
            if (this.exactFilter == null || this.exactSearch == null || this.exactFilter.Length == 0
                || this.exactSearch.Length == 0 || this.exactSearch.Length!= this.exactFilter.Length ) return null;

            List<QSmartQueryFilterCondition> result = new List<QSmartQueryFilterCondition>();
            for (int i = 0; i < this.exactFilter.Length; i++)
            {
                string col = this.exactFilter[i];
                PropertyInfo pi = pis.First(e => e.Name.ToLower() == col.ToLower());
                if (pi == null) continue;
                result.Add(new QSmartQueryFilterCondition
                {
                    Column = new QSmartQueryColumn { columnName = pi.Name, dataType = pi.PropertyType },
                    Operator = QSmartOperatorEnum.equal,
                    Connector = QSmartConnectorEnum.and,
                    Values = new List<object> { this.exactSearch[i] }
                });
            }
            return result;
        }

        /// <summary>
        /// 排序条件
        /// </summary>
        /// <returns></returns>
        public Dictionary<QSmartQueryColumn, QSmartOrderByEnum> GetOrderBys()
        {
            if (allSortCol == null || allSortCol.Length == 0) return null;
            Dictionary<QSmartQueryColumn, QSmartOrderByEnum> result = new Dictionary<QSmartQueryColumn, QSmartOrderByEnum>();
            for (int i = 0; i < allSortCol.Length; i++)
            {
                if (string.IsNullOrEmpty(allSortCol[i])) continue;
                result.Add(new QSmartQueryColumn { columnName = allSortCol[i] },
                     (QSmartOrderByEnum)Enum.Parse(typeof(QSmartOrderByEnum), allDir[i].ToLower()));
            }
            return result;
        }
    }

    public class LoginInfo
    {
        private bool _Error = false;
        /// <summary>
        /// 是否错误
        /// </summary>
        public bool Error
        {
            get { return _Error; }
            set { _Error = value; }
        }

        private string _ErrorMsg = string.Empty;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get { return _ErrorMsg; }
            set { _ErrorMsg = value; }
        }

        private SS_CompanyAccount _Account = null;

        public SS_CompanyAccount Account
        {
            get { return _Account; }
            set { _Account = value; }
        }
    }
}