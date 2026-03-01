using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetVisibleContentsSpecification : Specifications<Content>
    {
        public GetVisibleContentsSpecification(int? pageIndex = null, int? pageSize = null) 
            : base(c => c.IsVisible)
        {
            setOrderByDescending(c => c.CreatedAt);
            
            if (pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 && pageSize > 0)
            {
                ApplyPagination(pageIndex.Value, pageSize.Value);
            }
        }
    }
}
