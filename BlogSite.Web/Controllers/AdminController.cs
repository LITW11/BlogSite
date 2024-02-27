using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogSite.Data;

namespace BlogSite.Web.Controllers
{
    public class AdminController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=BlogSite;Integrated Security=True";
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitPost(BlogPost post)
        {
            BlogDb db = new BlogDb(_connectionString);
            post.DateCreated = DateTime.Now;
            db.AddPost(post);
            return Redirect($"/home/viewblog?id={post.Id}");
        }
    }
}
