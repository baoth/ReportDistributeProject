using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;
using QSmart.Core.Object;
using System.Data;
using System.Reflection;
using QSmart.Weixin.Core;
using System.Text.RegularExpressions;

namespace weixinreportviews.Model
{
    public static class General
    {
        public const string LogonSessionName = "userInfo";        
        public const string Token = "tiantian315";
        public const string AppId = "wxadcc12090e7138b3";
        public const string AppSecret = "722dfaf58953c47ffd9ac9548c727461";
        public const string authurl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxadcc12090e7138b3&redirect_uri=http%3A%2F%2Fwww.baoth.com%2fHome&response_type=code&scope=snsapi_base&state=123#wechat_redirect";

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
                if (!pi.CanWrite) continue;
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
            else if (PropertyType.IsEnum)
            {
                return int.Parse(value);
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
        public static CustomerLoginInfo LoginAdmin(string LoginKey, string Password) {
            if (LoginKey == "admin" && Password == "admin")
            {
                return new CustomerLoginInfo()
                {
                    Account = new SS_CompanyAccount { LoginKey = "admin" },

                    Error = CustomerLoginErrorEnum.管理账户
                };
            }
            return null;
        }
        /// <summary>
        /// 客户登陆后台管理
        /// </summary>
        /// <param name="LoginKey">账户名</param>
        /// <param name="Password">密码</param>
        /// <returns>信息</returns>
        public static CustomerLoginInfo Login(string LoginKey, string Password)
        {
            CustomerLoginInfo result=LoginAdmin(LoginKey, Password);
            if ( result!= null) return result;
            result=new CustomerLoginInfo();
            DbSession session = General.CreateDbSession();
            result.Account = session.Retrieve<SS_CompanyAccount>("LoginKey", LoginKey);
            if (result.Account == null) result.Error = CustomerLoginErrorEnum.账户不存在;
            else if (result.Account.Stoped) result.Error = CustomerLoginErrorEnum.账户停用;
            else if (result.Account.Password.Trim() != Password.Trim()) result.Error = CustomerLoginErrorEnum.密码错误;
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
            if (result.Account == null) result.Error = CustomerLoginErrorEnum.账户不存在;
            else if (result.Account.Stoped) result.Error = CustomerLoginErrorEnum.账户停用;
            return result;
        }

        /// <summary>
        /// 创建微信核心部件
        /// </summary>
        /// <returns></returns>
        public static WeixinCore CreateWeixinCore()
        {
            return new WeixinCore(General.AppId, General.AppSecret, General.Token, System.Configuration.ConfigurationManager.AppSettings);
        }

        /// <summary>
        /// 获取可用的授权个数
        /// </summary>
        /// <param name="AccountId">账户Id</param>
        /// <param name="ProductKind">产品类型</param>
        /// <returns></returns>
        public static int RetrieveValidLisence(Guid AccountId, ProductKindEnum ProductKind)
        {
            //select * from 
            //(select SUM(LisencePoint) as total from SS_Lisence where EffectiveDate<=datetimenow
            //and ExpiryDate>=datetimenow and AccountId=accountId and ProductKind=productkind and Stoped=false) a
            //,
            //(select count(OpendId) as usered from CS_BindUser where ProductKind=productkind and AccountId=accountid) b

            QSmartQuery QueryA = new QSmartQuery();
            QueryA.Tables.Add(new QSmartQueryTable());
            QueryA.Tables[0].tableName = typeof(SS_Lisence).Name;
            
            QSmartQueryColumn qqc=new QSmartQueryColumn();
            qqc.columnName = "LisencePoint";
            qqc.aliasName = "total";
            qqc.functions.Add(new QSmartQueryFunction { Function = QSmartFunctionEnum.sum });
            QueryA.SelectColumns.Add(qqc);

            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "EffectiveDate", dataType = typeof(DateTime)},
                Operator= QSmartOperatorEnum.lessequal,
                Values=new List<object>{DateTime.Now},
                Connector= QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ExpiryDate", dataType = typeof(DateTime) },
                Operator = QSmartOperatorEnum.greatequal,
                Values = new List<object> { DateTime.Now },
                Connector = QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { AccountId },
                Connector = QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { ProductKind },
                Connector = QSmartConnectorEnum.and
            });
            QueryA.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "Stoped", dataType = typeof(bool) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { false },
                Connector = QSmartConnectorEnum.and
            });

            QSmartQuery QueryB = new QSmartQuery();
            QueryB.Tables.Add(new QSmartQueryTable());
            QueryB.Tables[0].tableName = typeof(CS_BindUser).Name;
            QueryB.CountSetting.Effective = true;
            QueryB.CountSetting.AliasName = "usered";
            QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { AccountId },
                Connector = QSmartConnectorEnum.and
            });
            QueryB.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { ProductKind },
                Connector = QSmartConnectorEnum.and
            });

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
            DbSession session = General.CreateDbSession();
            DataTable dt = session.Context.QueryTable(Query);
            return Convert.ToInt32(dt.Rows[0][0]) - Convert.ToInt32(dt.Rows[0][1]);
        }

        /// <summary>
        /// 判断是否是手机浏览器发出的请求
        /// </summary>
        /// <param name="Request">httprequest</param>
        /// <returns></returns>
        public static bool PhoneBroswer(HttpRequestBase Request)
        {
            string u = Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
                return true;
            else
                return false;
        }
    }
    public static class ObjectExtend
    {
        public static string[] Add(this string [] a,string value){
            var length = 0;
            if (a != null)
            {
                length = a.Length;
            }
            var strNew = new string[length+ 1];
            for (int i = 0; i < length; i++)
            {
                strNew.SetValue(a[i], i);
            }
            strNew.SetValue(value, length);
            return strNew;
        }
    }
    /// <summary>
    /// 错误类型枚举
    /// </summary>
    public enum CustomerLoginErrorEnum
    {
        成功=0,
        账户不存在=1,
        账户停用=2,
        密码错误=3,
        管理账户=4,
    }

    /// <summary>
    /// 客户登陆或关注返回信息
    /// </summary>
    public class CustomerLoginInfo
    {
        public SS_CompanyAccount Account { get; set; }

        private CustomerLoginErrorEnum _Error = CustomerLoginErrorEnum.成功;

        public CustomerLoginErrorEnum Error
        {
            get { return _Error; }
            set { _Error = value; }
        }
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


        public T ExistsEnt<T>(string UniqueKeyName, object UniqueKeyValue) where T : QSmartEntity
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
            return results == null || results.Count == 0 ? null : results[0];
            
        }
        public bool ExistsEnt<T>(string UniqueKeyName, object UniqueKeyValue,List<QSmartQueryFilterCondition> listQFilter) where T : QSmartEntity
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(T).Name;
            Query.FilterConditions.AddRange(listQFilter);
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = UniqueKeyName, dataType = UniqueKeyValue.GetType() },
                Operator= QSmartOperatorEnum.equal,
                Values=new List<object>{UniqueKeyValue},
                Connector= QSmartConnectorEnum.and
            });
           
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