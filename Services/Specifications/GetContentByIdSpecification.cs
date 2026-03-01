using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetContentByIdSpecification : Specifications<Content>
    {
        public GetContentByIdSpecification(int contentId) 
            : base(c => c.Id == contentId)
        {
        }
    }
}
