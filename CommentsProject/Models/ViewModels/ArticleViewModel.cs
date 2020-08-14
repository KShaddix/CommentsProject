namespace CommentsProject.Models.ViewModels
{
    public class ArticleViewModel
    {
        public Article Article { get; set; }
        public FilteredModels<CommentViewModel> Comments { get; set; }
        public ErrorViewModel Error { get; set; }
    }
}
