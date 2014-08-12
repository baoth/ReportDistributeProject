using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;
using QSmart.Core.Object;

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
    }

    public abstract class DbSession: QSmartSession
    {
        protected DbSession(string name) : base(name) { }

        /// <summary>
        /// 通过主键获取模型对象
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="PrimaryKeyName">主键名称</param>
        /// <param name="PrimaryKeyValue">主键值</param>
        /// <returns>模型对象</returns>
        public T Retrieve<T>(string PrimaryKeyName, object PrimaryKeyValue) where T : QSmartEntity
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(T).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition());
            Query.FilterConditions[0].Column = new QSmartQueryColumn();
            Query.FilterConditions[0].Column.columnName = PrimaryKeyName;
            Query.FilterConditions[0].Column.dataType = PrimaryKeyValue.GetType();
            Query.FilterConditions[0].Operator = QSmartOperatorEnum.equal;
            Query.FilterConditions[0].Values.Add(PrimaryKeyValue);
            var results = this.Context.QueryEntity<T>(Query);
            return results.Count == 0 ? null : results[0];
        }
    }

    class DbSessionInstance : DbSession
    {
        public DbSessionInstance() : base("") { }
    }
}