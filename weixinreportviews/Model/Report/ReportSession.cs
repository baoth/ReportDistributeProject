using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QSmart.Core.DataBase;
using QSmart.Core.Object;
using QSmart.Weixin.Core;

namespace weixinreportviews.Model
{
    public class ReportBuilderSession
    {
        public ReportBuilderSession()  { }
        /// 获取生成器
        /// </summary>
        /// <param name="Id">生成器Id</param>
        /// <param name="FrameType">生成器类别枚举</param>
        /// <returns></returns>
        public ReportBuilder GetBuilder(Guid Id, ReportBuilderEnum FrameType)
        {
            switch (FrameType)
            {
                case ReportBuilderEnum.Excel文件创建框架:
                    return new SimpleReportBuilder(Id);
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