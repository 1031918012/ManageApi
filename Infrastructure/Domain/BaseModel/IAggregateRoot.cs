using Infrastructure;

namespace Domain
{
    /// <summary>
    /// 聚合根接口
    /// </summary>
    public interface IAggregateRoot : IEntity, IBaseRepository
    {
    }
}
