using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Service
{
    public class FeedbackService : BaseService, IFeedbackService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFYUServerClient _fYUServerClient;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="alarmSettingRepository"></param>
        public FeedbackService(IFeedbackRepository feedbackRepository, IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IFYUServerClient fYUServerClient) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _feedbackRepository = feedbackRepository;
            _fYUServerClient = fYUServerClient;
        }
        /// <summary>
        /// 添加反馈建议
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="name"></param>
        /// <param name="companyID"></param>
        /// <param name="feedbackType"></param>
        /// <param name="content"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public (bool, string) AddFeedback(string IDCard, string name, string companyID, int feedbackType, string content, IFormFileCollection files)
        {
            if (string.IsNullOrEmpty(content))
            {
                return (false, "请输入您遇到的问题");
            }
            var companyName = _fYUServerClient.GetCusName(companyID);
            if (!companyName.Item1)
            {
                return (false, "网络错误，请稍后重试");
            }
            Feedback feedback = new Feedback
            {
                CompanyName = companyName.Item2,
                CompanyID = companyID,
                Content = content,
                CreateTime = DateTime.Now,
                FeedbackType = (FeedbackEnum)feedbackType,
                IDCard = IDCard,
                Name = name,
            };
            List<string> path = new List<string>();
            foreach (IFormFile item in files)
            {
                try
                {
                    var fileType = "." + item.FileName.Split(".").LastOrDefault();
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string filename = Guid.NewGuid().ToString();
                    string pathone = $"{Directory.GetCurrentDirectory()}/wwwroot/images/{date}/{filename}{fileType}";
                    string filePath = Path.GetDirectoryName(pathone);
                    if (!Directory.Exists(filePath))
                    {
                        if (filePath != null)
                        {
                            Directory.CreateDirectory(filePath);
                        }
                    }
                    using (var os = new FileStream(pathone, FileMode.Create, FileAccess.ReadWrite))
                    {
                        item.CopyTo(os);
                        os.Close();
                    }
                    var link = AppConfig.WeChat.EnvironmentHost + $"/images/{date}/{filename}{fileType}";
                    path.Add(link);
                }
                catch (Exception)
                {
                    return (false, "图片部分处理错误");
                }
            }
            feedback.Path = JsonConvert.SerializeObject(path);
            _attendanceUnitOfWork.Add(feedback);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "服务器内部错误");
            }
            return (true, "反馈成功");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageResult<FeedbackDTO> GetFeedbackPage(int pageIndex, int pageSize, DateTime Time = new DateTime(), string content = "")
        {
            Expression<Func<Feedback, bool>> exp = s => true;
            if (Time != new DateTime())
            {
                exp = exp.And(s => s.CreateTime.Year == Time.Year && s.CreateTime.Month == Time.Month);
            }
            if (!string.IsNullOrEmpty(content))
            {
                exp = exp.And(s => s.Content.Contains(content));
            }
            var feedbackQuery = _feedbackRepository.EntityQueryable(exp).OrderByDescending(s => s.CreateTime);
            var feedbackQueryList = _feedbackRepository.GetByPage(pageIndex, pageSize, feedbackQuery);
            return PageMap<FeedbackDTO, Feedback>(feedbackQueryList);
        }
    }
}