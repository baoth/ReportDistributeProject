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
                    }
                }
                else if (baseinfo != null && !baseinfo.IsEvent)
                {
                    switch (baseinfo.MsgType)
                    {
                        case MsgType.text:
                            var tm = (AcceptWeixinTextMessage)baseinfo.CreateAcceptMessage();
                            if (string.IsNullOrEmpty(tm.Content)) return Content("");
                            return Content("");
                        case MsgType.voice:
                            return Content("");

                    }
                }
            }
            return Content("");
        }
    }
}
