using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class AttendanceRecordMessageService : IAttendanceRecordMessageService
    {
        private readonly IClockRecordRepository _clockRecordRepository;
        private readonly IFYUServerClient _fYUServerClient;
        private readonly IWeChatMiddleware _weChatMiddleware;
        private readonly ILogger<AttendanceRecordService> _logger;

        public AttendanceRecordMessageService(IClockRecordRepository clockRecordRepository, ILogger<AttendanceRecordService> logger, IFYUServerClient fYUServerClient, IWeChatMiddleware weChatMiddleware)
        {
            _clockRecordRepository = clockRecordRepository;
            _fYUServerClient = fYUServerClient;
            _weChatMiddleware = weChatMiddleware;
            _logger = logger;
        }

        /// <summary>
        /// 发送所悟消息
        /// </summary>
        /// <param name="time"></param>
        /// <param name="Records"></param>
        public void ErrorRecordWechatMessage(DateTime time, List<ErrorMessageDTO> Records)
        {
            string accesstoken = string.Empty;
            try
            {
                accesstoken = _fYUServerClient.GetAccessToken();//获取accesstoken
                if (string.IsNullOrEmpty(accesstoken)) return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "异常打卡提醒获取泛员网accesstoken报错");
                return;
            }
            Dictionary<string, string> openids = new Dictionary<string, string>();
            List<string> idcards = new List<string>();
            try
            {

                idcards = Records.Select(t => t.IDCard.ToUpper()).ToList();
                openids = _fYUServerClient.GetOpenIdRange(idcards);//身份证获取openid
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "异常打卡提醒获取泛员网openid接口报错{0}", idcards);
                return;
            }
            foreach (var item in Records)
            {
                //string openid = "o1BNj1Vh0nBIHA5RltHx1G2E1iQ4";
                string openid = string.Empty;
                openids.TryGetValue(item.IDCard.ToUpper(), out openid);
                if (string.IsNullOrEmpty(openid))
                {
                    continue;
                }
                var TemplateData = "您昨日（" + time.ToString("yyyy年MM月dd日") + "）的考勤有异常，点击查看详情。|" + item.Name + "|" + time.ToString("yyyy年MM月dd日") + "|" + "缺卡" + "|如有疑问，请与贵公司的HR进行联系哦~😘。";
                var url = AppConfig.WeChat.WeChatUri.TempUrl + "/" + item.IDCard;
                var res = _weChatMiddleware.SendTemplateMessage(new WeChatTemplateMessage(openid, AppConfig.WeChat.WeChatConst.ErrTempID, url, TemplateData), accesstoken, TemplateData);
            }
        }

        /// <summary>
        /// 发送打卡提醒
        /// </summary>
        /// <param name="time"></param>
        /// <param name="clockMessage"></param>
        public void ClockMessage(DateTime time, List<ClockMessageDTO> clockMessage)
        {
            if (!clockMessage.Any())
            {
                return;
            }
            var ClockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate == time.Date && s.ClockType == clockMessage.FirstOrDefault().ClockType).ToList();
            Dictionary<string, string> openids = new Dictionary<string, string>();
            string accesstoken = string.Empty;
            List<string> idcards = new List<string>();
            try
            {
                accesstoken = _fYUServerClient.GetAccessToken();//获取accesstoken
                if (string.IsNullOrEmpty(accesstoken)) return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "考勤打卡提醒获取泛员网accesstoken报错");
                return;
            }
            try
            {
                idcards = clockMessage.Select(t => t.IDCard.ToUpper()).ToList();
                openids = _fYUServerClient.GetOpenIdRange(idcards);//身份证获取openid
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "考勤打卡提醒获取泛员网openid接口报错{0}", idcards);
                return;
            }
            foreach (var item in clockMessage)
            {
                if (ClockRecord.Where(s => s.IDCard == item.IDCard).Any())
                {
                    continue;
                }
                //string openid = "o1BNj1Vh0nBIHA5RltHx1G2E1iQ4";
                string openid = string.Empty;
                openids.TryGetValue(item.IDCard.ToUpper(), out openid);
                if (string.IsNullOrEmpty(openid))
                {
                    continue;
                }
                var TemplateData = "您好，您有一条考勤打卡提醒|" + item.Name + "|" + time.ToString("yyyy年MM月dd日") + "|" + item.ClockType.GetDescription() + "未打卡|" + item.ClockType.GetDescription() + "时间为" + time.ToString("yyyy年MM月dd日") + item.Worktime.ToString("HH:mm") + "请不要忘记打卡哦~😘。";
                var url = AppConfig.WeChat.WeChatUri.TempMessageUrl + "/" + item.IDCard;
                var res = _weChatMiddleware.SendTemplateMessage(new WeChatTemplateMessage(openid, AppConfig.WeChat.WeChatConst.TempID, url, TemplateData), accesstoken, TemplateData);
            }
        }
    }
}

