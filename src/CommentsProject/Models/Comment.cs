namespace CommentsProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int ArticleId { get; set; }
        public string UserName { get; set; }
        public int? ParentId { get; set; }
    }
}
