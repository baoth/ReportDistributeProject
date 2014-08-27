using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;
using QSmart.Core.Object;
using QSmart.Weixin.Core;

namespace weixinreportviews.Model
{
    public class ReportBuilderSession:QSmartSession
    {
        public ReportBuilderSession(string name) : base(name) { }

        public string GetReportNews(string ToUserName,string FromUserName,string filter)
        {
            filter = filter.ToLower();
            List<ReportBuilder> builders = this.GetBuilders();
            if (string.IsNullOrEmpty(filter))
            {
                builders = builders.FindAll(e => e.Builded == true);
            }
            else
            {
                builders = builders.FindAll(e => e.Builded == true && e.KeyWord.ToLower().Contains(filter) == true);
            }
            if (builders.Count == 0)
            {
                ReplyWeixinTextMessage rtm = new ReplyWeixinTextMessage();
                rtm.ToUserName = ToUserName;
                rtm.FromUserName = FromUserName;
                rtm.CreateTime = WeixinCoreExtension.GetTimeStamp(DateTime.Now);
                rtm.Content = "系统提示：" + "没有匹配的报表被发现。";
                return rtm.GetReplyMessage();
            }
            ReplyWeixinNewsMessage rnm = new ReplyWeixinNewsMessage();
            rnm.FromUserName = FromUserName;
            rnm.ToUserName = ToUserName;
            rnm.CreateTime = WeixinCoreExtension.GetTimeStamp(DateTime.Now);
            foreach (ReportBuilder rb in builders)
            {
                ArticleItem item = new ArticleItem();
                item.Title = rb.Title;
                item.Description = rb.Title;
                item.Url = string.Format("http://{0}{1}",WeixinAdaptor.WebRoot,rb.HtmlUrl);
                rnm.Articles.Add(item);                
            }
            return rnm.GetReplyMessage();
        }

        /// <summary>
        /// 获取所有生成器信息
        /// </summary>
        /// <returns>生成器信息</returns>
        public List<ReportBuilder> GetBuilders()
        {
            List<ReportBuilder> results = new List<ReportBuilder>();
            results.AddRange(GetBuilders<SimpleReportBuilder>());
            return results;
        }

        /// <summary>
        /// 根据类型获取生成器信息
        /// </summary>
        /// <param name="rbenum">类型</param>
        /// <returns>生成器集合</returns>
        public List<ReportBuilder> GetBuilders(ReportBuilderEnum rbenum)
        {
            switch (rbenum)
            {
                case ReportBuilderEnum.Excel文件创建框架:
                    return GetBuilders<SimpleReportBuilder>().ToList<ReportBuilder>();
                default:
                    return new List<ReportBuilder>();
                
            }
        }

        /// <summary>
        /// 获取生成器信息
        /// </summary>
        /// <typeparam name="T">生成器类型</typeparam>
        /// <returns>生成器信息</returns>
        public List<T> GetBuilders<T>() where T : ReportBuilder
        {
            //select * from table
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(T).Name;
            return this.Context.QueryEntity<T>(Query);
        }

        /// <summary>
        /// 获取生成器
        /// </summary>
        /// <typeparam name="T">生成器类型</typeparam>
        /// <param name="Id">生成器Id</param>
        /// <returns></returns>
        public T GetBuilder<T>(string Id) where T : ReportBuilder
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(T).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition());
            Query.FilterConditions[0].Column = new QSmartQueryColumn();
            Query.FilterConditions[0].Column.columnName = "Id";
            Query.FilterConditions[0].Column.dataType = typeof(Guid);
            Query.FilterConditions[0].Operator = QSmartOperatorEnum.equal;
            Query.FilterConditions[0].Values.Add(Guid.Parse(Id));
            var results = this.Context.QueryEntity<T>(Query);
            return results.Count == 0 ? null : results[0];
        }

        /// <summary>
        /// 获取生成器
        /// </summary>
        /// <param name="Id">生成器Id</param>
        /// <param name="FrameType">生成器类别枚举</param>
        /// <returns></returns>
        public ReportBuilder GetBuilder(string Id, ReportBuilderEnum FrameType)
        {
            switch (FrameType)
            {
                case ReportBuilderEnum.Excel文件创建框架:
                    return GetBuilder<SimpleReportBuilder>(Id);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 创建生成器
        /// </summary>
        /// <param name="FrameType">生成器类别枚举</param>
        /// <returns></returns>
        public ReportBuilder CreateBuilder(ReportBuilderEnum FrameType)
        {
            switch (FrameType)
            {
                case ReportBuilderEnum.Excel文件创建框架:
                    return new SimpleReportBuilder();
                default:
                    return null;
            }
        }
    }
}