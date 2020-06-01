using Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IFeedbackService : IService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="name"></param>
        /// <param name="companyID"></param>
        /// <param name="feedbackType"></param>
        /// <param name="content"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        (bool, string) AddFeedback(string IDCard, string name, string companyID, int feedbackType, string content, IFormFileCollection files);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="Time"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        PageResult<FeedbackDTO> GetFeedbackPage(int pageIndex, int pageSize, DateTime Time = new DateTime(), string content = "");
    }
}
