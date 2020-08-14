using System.Collections.Generic;

namespace CommentsProject.Models
{
    public class FilteredModels<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int MaxPage { get; set; }
    }
}
