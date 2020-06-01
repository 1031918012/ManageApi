using Domain;
using Infrastructure;

namespace Repositories
{
    public class FeedbackRepository : EFRepository<Feedback>, IFeedbackRepository, IBaseRepository
    {
        public FeedbackRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
