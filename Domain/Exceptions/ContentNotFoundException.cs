namespace Domain.Exceptions
{
    public sealed class ContentNotFoundException : NotFoundException
    {
        public ContentNotFoundException(int contentId) 
            : base($"Content with ID {contentId} not found")
        {
        }
    }
}
