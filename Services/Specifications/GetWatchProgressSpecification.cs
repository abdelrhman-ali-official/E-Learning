using Domain.Contracts;
using Domain.Entities.ContentEntities;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class GetWatchProgressSpecification : Specifications<WatchProgress>
    {
        public GetWatchProgressSpecification(string userId, int contentId) 
            : base(p => p.UserId == userId && p.ContentId == contentId)
        {
        }
    }
}
