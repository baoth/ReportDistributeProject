using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;

namespace weixinreportviews.Model
{
    public class WebSession:QSmartSession
    {
        public WebSession(string name) : base(name) { }
    }
}