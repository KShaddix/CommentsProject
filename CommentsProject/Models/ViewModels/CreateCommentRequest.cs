namespace CommentsProject.Models.ViewModels
{
    public class CreateCommentRequest
    {
        public string Text { get; set; }
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public int? ParentId { get; set; }
    }
}
