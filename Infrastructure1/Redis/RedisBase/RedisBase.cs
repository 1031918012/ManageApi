using Infrastructure.Redis.Inite;
using ServiceStack.Redis;
using System;

namespace Infrastructure.Redis
{
    public abstract class RedisBase: IDisposable
    {

        public IRedisClient _iClient { get; private set; }
        /// <summary>
        /// 构造时完成链接的打开
        /// </summary>
        public RedisBase()
        {
            _iClient = RedisManager.GetClient();
        }

        //public static IRedisClient _iClient { get; private set; }
        //static RedisBase()
        //{
        //    _iClient = RedisManager.GetClient();
        //}


        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _iClient.Dispose();
                    _iClient = null;
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Transcation()
        {
            using (IRedisTransaction irt = this._iClient.CreateTransaction())
            {
                try
                {
                    irt.QueueCommand(r => r.Set("key", 20));
                    irt.QueueCommand(r => r.Increment("key", 1));
                    irt.Commit(); // 提交事务
                }
                catch (Exception ex)
                {
                    irt.Rollback();
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 清除全部数据 请小心
        /// </summary>
        public virtual void FlushAll()
        {
            _iClient.FlushAll();
        }

        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        public void Save()
        {
            _iClient.Save();//阻塞式save
        }

        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            _iClient.SaveAsync();//异步save
        }
    }
}
