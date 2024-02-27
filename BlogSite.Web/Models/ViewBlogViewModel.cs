using System.Collections.Generic;
using BlogSite.Data;

namespace BlogSite.Web.Models
{
    public class ViewBlogViewModel
    {
        public BlogPost Post { get; set; }
        public List<Comment> Comments { get; set; }
        public string CommenterName { get; set; }
    }
}