using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Weixin.Core;
namespace weixinreportviews.Model
{
    public static class PathTools
    {
        public static string 静态报表路径
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RPPath"]; }
        }

        public static string WebRoot
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["webroot"]; }
        }
        public static string BaseDirector
        {
            get { return System.AppDomain.CurrentDomain.BaseDirectory; }
        }
        public static string GenerateHtmlPath
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RPHtml"]; }
        }
        public static string SaveHtmlPath 
        {
            get { return System.IO.Path.Combine(PathTools.BaseDirector, System.Configuration.ConfigurationManager.AppSettings["RPHtml"]); }
        }
        public static string RPTemplatePath
        {
            get { return System.IO.Path.Combine(PathTools.BaseDirector, System.Configuration.ConfigurationManager.AppSettings["RPTemplate"]); }
        }

        public static string AddWebHeadAddress(string path) 
        {
            if (string.IsNullOrEmpty(path)) return "";
            return System.IO.Path.Combine(PathTools.WebRoot, path.Replace("//","/"));
        }
        public static string RemoveWebHeadAddress(string path)
        {
            return path.Replace(PathTools.WebRoot,"");
        }

        public static string GetAbsolutePath(string filePath) 
        {
            return System.IO.Path.Combine(BaseDirector, filePath);
        }
    }
}
