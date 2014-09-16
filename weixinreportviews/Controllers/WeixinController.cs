using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QSmart.Weixin.Core;
using weixinreportviews.Model;
using System.Xml.Linq;

namespace weixinreportviews.Controllers
{
    public class WeixinController : Controller
    {
        [HttpGet]
        [ActionName("Index")]
        public ActionResult GetIndex(string signature, string timestamp, string nonce, string echostr)
        {
            WeixinCore wc = General.CreateWeixinCore();

            if (wc.Check(signature, timestamp, nonce, wc.Token))
            {
                return Content(echostr);//返回随机字符串则表示验证通过
            }
            else
            {
                return Content("");
            }
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult PostIndex(string signature, string timestamp, string nonce, string echostr)
        {
            WeixinCore wc = General.CreateWeixinCore();
            if (!wc.Check(signature, timestamp, nonce, General.Token))
            {
                return Content("参数错误！");
            }
            if (Request.InputStream != null)
            {
                XDocument doc = XDocument.Load(Request.InputStream);
                WeixinMessageBaseInfo baseinfo = WeixinMessageBaseInfo.Create(doc);

                if (baseinfo != null && baseinfo.IsEvent)
                {
                    switch (baseinfo.EventType)
                    {
                        case EventType.subscribe:
                            return Content("");
                        case EventType.click: //点击菜单
                            if (baseinfo.EventKey.ToLower() == "firstreport") //报表菜单
                            {
                                var user = ProductGeneral.RetrieveUser(baseinfo.FromUserName, ProductKindEnum.微信第一表);
                                if (user==null)
                                {
                                    return Content(WeixinMessage(baseinfo,"系统提示：" + "您当前没有获得使用该功能授权。请联系您的管理员！"));
                                }
                                var reports = ProductGeneral.RetrieveSuitableReports(user);
                                return Content(ProductGeneral.CreateFirstReportNews(baseinfo.FromUserName,
                                    baseinfo.ToUserName,reports));
                            }
                            return Content("");
                        default:
                            return Content("");
                    }
                }
                else if (baseinfo != null && !baseinfo.IsEvent)
                {
                    switch (baseinfo.MsgType)
                    {
                        case MsgType.text: //注册
                            var tm = (AcceptWeixinTextMessage)baseinfo.CreateAcceptMessage();
                            if (string.IsNullOrEmpty(tm.Content)) return Content("");
                            WeixinUserInfo userinfo = wc.GetUserBaseInfo(baseinfo.FromUserName);
                            bool result = ProductGeneral.ApplyLisence(tm.Content, userinfo);
                            if (result)
                            {
                                return Content(WeixinMessage(baseinfo, "系统提示：请求已提交,请等待审核!"));
                            }
                            else
                            {
                                return Content(WeixinMessage(baseinfo, "系统提示：请求提交被拒绝，请联系系统管理员!"));
                            }
                        default:
                            return Content("");
                    }
                }
            }
            return Content("");
        }

        private string WeixinMessage(WeixinMessageBaseInfo info, string msg)
        {
            ReplyWeixinTextMessage rtm = new ReplyWeixinTextMessage();
            rtm.ToUserName = info.FromUserName;
            rtm.FromUserName = info.ToUserName;
            rtm.CreateTime = WeixinCoreExtension.GetTimeStamp(DateTime.Now);
            rtm.Content = msg;
            return rtm.GetReplyMessage();
        }

    }
}
