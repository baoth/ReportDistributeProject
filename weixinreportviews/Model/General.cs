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

        /// <summary>
        /// 获取分页数据集合
        /// </summary>
        /// <typeparam name="T">模型类型</typeparam>
        /// <param name="PageIndex">当前页号</param>
        /// <param name="PageCount">一页需要显示多少条记录</param>
        /// <param name="Conditions">过滤条件</param>
        /// <param name="OrderBys">排序条件</param>
        /// <param name="TotalCount">返回总条数</param>
        /// <returns>数据集合</returns>
        public List<T> PaginationRetrieve<T>(int PageIndex, int PageCount, List<QSmartQueryFilterCondition> Conditions
            ,Dictionary<QSmartQueryColumn, QSmartOrderByEnum> OrderBys,out int TotalCount)
            where T : QSmartEntity
        {
            TotalCount=0;

            QSmartQuery QueryA = new QSmartQuery();
            QueryA.Tables.Add(new QSmartQueryTable());
            QueryA.Tables[0].tableName = typeof(T).Name;
            QueryA.TopSetting.Effective = true;
            QueryA.TopSetting.Value = PageCount;
            QueryA.TopSetting.BeginValue = (PageIndex-1) * PageCount;
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
        public DbSessionInstance() : base("") { }
    }
}