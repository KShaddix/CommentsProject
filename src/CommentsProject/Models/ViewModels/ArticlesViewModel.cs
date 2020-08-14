namespace CommentsProject.Models.ViewModels
{
    public class ArticlesViewModel
    {
        public FilteredModels<Article> Articles { get; set; }
        public ErrorViewModel Error { get; set; }
    }
}
