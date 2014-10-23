using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;
using QSmart.Weixin.Core;
using QSmart.Core.Object;

namespace weixinreportviews.Model
{
    public class ProductGeneral
    {
                
        public static bool UserBinded(string OpenId, ProductKindEnum ProductKind)
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(CS_BindUser).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                Operator= QSmartOperatorEnum.equal,
                Values=new List<object>{OpenId},
                Connector= QSmartConnectorEnum.and
            });
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { ProductKind },
                Connector = QSmartConnectorEnum.and
            });
            DbSession session = General.CreateDbSession();
            var dt = session.Context.QueryTable(Query);
            return dt == null || dt.Rows.Count == 0 ? false : true;
        }

        public static CS_BindUser RetrieveUser(string OpenId, ProductKindEnum ProductKind)
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(CS_BindUser).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { OpenId },
                Connector = QSmartConnectorEnum.and
            });
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { ProductKind },
                Connector = QSmartConnectorEnum.and
            });
            DbSession session = General.CreateDbSession();
            List<CS_BindUser> users = session.Context.QueryEntity<CS_BindUser>(Query);
            return users.Count == 0 ? null : users[0];
        }

        public static CS_UserAttention RetrieveAttention(string OpenId, ProductKindEnum ProductKind, Guid AccountId)
        {
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(CS_BindUser).Name;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "OpenId", dataType = typeof(string) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { OpenId },
                Connector = QSmartConnectorEnum.and
            });
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "ProductKind", dataType = typeof(ProductKindEnum) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { ProductKind },
                Connector = QSmartConnectorEnum.and
            });
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { AccountId },
                Connector = QSmartConnectorEnum.and
            });
            DbSession session = General.CreateDbSession();
            List<CS_UserAttention> users = session.Context.QueryEntity<CS_UserAttention>(Query);
            return users.Count == 0 ? null : users[0];
        }

        public static List<CS_FirstReport> RetrieveSuitableReports(CS_BindUser User)
        {
            DbSession session=General.CreateDbSession();
            QSmartQuery Query = new QSmartQuery();
            Query.Tables.Add(new QSmartQueryTable());
            Query.Tables[0].tableName = typeof(CS_FirstReport).Name;
            Query.TopSetting.Effective = true;
            Query.TopSetting.Value = 5;
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "AccountId", dataType = typeof(Guid) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { User.AccountId },
                Connector = QSmartConnectorEnum.and
            });
            Query.FilterConditions.Add(new QSmartQueryFilterCondition
            {
                Column = new QSmartQueryColumn { columnName = "Stoped", dataType = typeof(bool) },
                Operator = QSmartOperatorEnum.equal,
                Values = new List<object> { false },
                Connector = QSmartConnectorEnum.and
            });
            Query.OrderBys.Add(new QSmartQueryColumn { columnName = "CreateDate" }, QSmartOrderByEnum.desc);
            return session.Context.QueryEntity<CS_FirstReport>(Query);
        }

        public static string CreateFirstReportNews(string ToUserName, string FromUserName, List<CS_FirstReport> Reports)
        {
            ReplyWeixinNewsMessage rnm = new ReplyWeixinNewsMessage();
            rnm.FromUserName = FromUserName;
            rnm.ToUserName = ToUserName;
            rnm.CreateTime = WeixinCoreExtension.GetTimeStamp(DateTime.Now);
            foreach (CS_FirstReport rp in Reports)
            {
                ArticleItem item = new ArticleItem();
                item.Title = rp.Title;
                item.Description = rp.Title;
                item.Url = string.Format("{0}/{1}", PathTools.WebRoot, rp.Url);//e7d8e8e441cb44bda292b2a59df46000logo.jpg");//rp.Url);
                item.PicUrl = string.Format("{0}/{1}", PathTools.WebRoot,rp.PicUrl);
                rnm.Articles.Add(item);
            }
            ArticleItem itemhistory = new ArticleItem();
            itemhistory.Title = "历史浏览";
            itemhistory.Description = "历史浏览";
            itemhistory.Url = string.Empty;
            rnm.Articles.Add(itemhistory);
            return rnm.GetReplyMessage();
        }

        public static bool ApplyLisence(string Content,WeixinUserInfo info)
        {
            string msg = Content.ToLower().Trim();
            if (msg.Length <= 3) return false;
            string LoginKey = msg.Substring(3, msg.Length - 3);
            ProductKindEnum pkind = ProductKindEnum.无;
            DbSession session = General.CreateDbSession();
            if (msg.StartsWith("fp:")) //天下第一表产品申请授权
            {
                pkind = ProductKindEnum.微信第一表;
            }
            if (pkind == ProductKindEnum.无) return false;

            SS_CompanyAccount account = session.Retrieve<SS_CompanyAccount>("LoginKey", LoginKey);
            if (account == null) return false;

            CS_UserAttention atten = ProductGeneral.RetrieveAttention(info.openid, pkind, account.Id);
            if (atten != null) return true;
            atten = new CS_UserAttention();
            atten.AccountId=account.Id;
            atten.AttentionDate = DateTime.Now;
            atten.HeadImgUrl = info.headimgurl;
            atten.NickName = info.nickname;
            atten.OpenId = info.openid;
            atten.ProductKind = pkind;
            session.Context.InsertEntity(atten.CreateQSmartObject());
            session.Context.SaveChange();
            return true;
        }
    }
}