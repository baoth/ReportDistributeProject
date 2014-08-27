using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Weixin.Core;
namespace weixinreportviews.Model
{
    public static class WeixinAdaptor
    {
        public const string Token = "tiantian315";
        public const string AppId = "wxadcc12090e7138b3";
        public const string AppSecret = "722dfaf58953c47ffd9ac9548c727461";
        public const string authurl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxadcc12090e7138b3&redirect_uri=http%3A%2F%2Fwww.baoth.com%2fHome&response_type=code&scope=snsapi_base&state=123#wechat_redirect";

        public static string 静态报表路径
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RP静态报表路径"]; }
        }

        public static string WebRoot
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["webroot"]; }
        }

        public static string BaseDirector
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static ReportBuilderSession CreateReportSession()
        {
            return new ReportBuilderSession("localdb");
        }

        public static WeixinCore CreateWeixinCore()
        {
            return new WeixinCore(WeixinAdaptor.AppId, WeixinAdaptor.AppSecret, WeixinAdaptor.Token, System.Configuration.ConfigurationManager.AppSettings);
        }

        public static WebSession CreateWebSession()
        {
            return new WebSession("localdb");
        }

        
    }
}