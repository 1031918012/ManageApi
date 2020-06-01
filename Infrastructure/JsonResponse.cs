using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class JsonResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string JsonContent { get; set; }
        public CodeEnum Code { get; set; }
    }
    public enum CodeEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        ok = 200,
        /// <summary>
        /// 生成新的资源
        /// </summary>
        Created = 201,
        /// <summary>
        /// 服务器已经收到请求，但还未进行处理，会在未来再处理，通常用于异步操作
        /// </summary>
        Accepted = 202,
        /// <summary>
        /// 资源已经不存在
        /// </summary>
        NoContent = 204,
        /// <summary>
        /// 永久重定向
        /// </summary>
        PermanentRedirection = 301,
        /// <summary>
        /// 暂时重定向|用于get
        /// </summary>
        TemporaryRedirection = 302,
        /// <summary>
        /// 暂时重定向|用于post，put，delete，
        /// </summary>
        SeeOther = 303,
        /// <summary>
        /// 服务器不理解客户端的请求，未做任何处理。
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// 用户未提供身份验证凭据，或者没有通过身份验证。
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// 用户通过了身份验证，但是不具有访问资源所需的权限。
        /// </summary>
        Forbidden = 403,
        /// <summary>
        /// 所请求的资源不存在，或不可用。
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// 用户已经通过身份验证，但是所用的 HTTP 方法不在他的权限之内。
        /// </summary>
        MethodNotAllowed = 405,
        /// <summary>
        /// 所请求的资源已从这个地址转移，不再可用。
        /// </summary>
        Gone = 410,
        /// <summary>
        /// 客户端要求的返回格式不支持。比如，API 只能返回 JSON 格式，但是客户端要求返回 XML 格式。
        /// </summary>
        UnsupportedMediaType = 415,
        /// <summary>
        /// 客户端上传的附件无法处理，导致请求失败。
        /// </summary>
        UnprocessableEntity = 422,
        /// <summary>
        /// 客户端的请求次数超过限额
        /// </summary>
        TooManyRequests = 429,
        /// <summary>
        /// 客户端请求有效，服务器处理时发生了意外
        /// </summary>
        InternalServerError = 500,
        /// <summary>
        ///服务器无法处理请求，一般用于网站维护状态
        /// </summary>
        ServiceUnavailable = 503
    }
}
