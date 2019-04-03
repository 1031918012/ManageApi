using Domain;
using Infrastructure.Redis.RedisServer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ManageApi
{
    /// <summary>
    /// 
    /// </summary>
    public class RayPIToken
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="redisStringService"></param>
        public RayPIToken(IConfiguration configuration, RedisStringService redisStringService)
        {
            Configuration = configuration;
            _redisStringService = redisStringService;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }
        private RedisStringService _redisStringService { get; set; }
        /// <summary>
        /// 获取JWT字符串并存入缓存
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <param name="expiresSliding"></param>
        /// <param name="expiresAbsoulte"></param>
        /// <returns></returns>
        public string IssueJWT(User tokenModel, TimeSpan expiresSliding, TimeSpan expiresAbsoulte)
        {
            DateTime UTC = DateTime.UtcNow;
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,tokenModel.Sub),//Subject,
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//JWT ID,JWT的唯一标识
                new Claim(JwtRegisteredClaimNames.Iat, UTC.ToString(), ClaimValueTypes.Integer64),//Issued At，JWT颁发的时间，采用标准unix时间，用于验证过期
            };

            JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: "RayPI",//jwt签发者,非必须
            audience: tokenModel.Uname,//jwt的接收该方，非必须
            claims: claims,//声明集合
            expires: UTC.AddHours(12),//指定token的生命周期，unix时间戳格式,非必须
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Authentication:JwtBearer:SecurityKey"].ToString())), SecurityAlgorithms.HmacSha256));//使用私钥进行签名加密

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);//生成最后的JWT字符串

            //RayPIMemoryCache.AddMemoryCache(encodedJwt, tokenModel, expiresSliding, expiresAbsoulte);//将JWT字符串和tokenModel作为key和value存入缓存
            _redisStringService.Set(encodedJwt, tokenModel.ID.ToString(), expiresAbsoulte);
            
            return encodedJwt;
        }
    }
}
