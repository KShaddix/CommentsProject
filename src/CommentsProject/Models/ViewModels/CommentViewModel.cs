using System.Collections.Generic;

namespace CommentsProject.Models.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public int? ParentId { get; set; }
        public List<CommentViewModel> ChildComments { get; set; }
    }
}
