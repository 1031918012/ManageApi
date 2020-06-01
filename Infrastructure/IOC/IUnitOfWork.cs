namespace Infrastructure
{
    public interface IUnitOfWork
    {
        bool Commit();
    }
}
