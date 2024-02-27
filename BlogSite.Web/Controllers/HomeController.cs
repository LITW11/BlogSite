using BlogSite.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogSite.Data;

namespace BlogSite.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=BlogSite;Integrated Security=True";

        public IActionResult Index(int page)
        {
            if (page <= 0)
            {
                page = 1;
            }
            int amountPerPage = 3;

            var db = new BlogDb(_connectionString);
            var vm = new HomePageViewModel();
            int skip = (page - 1) * amountPerPage;
            vm.Posts = db.GetPosts(skip, amountPerPage);

            if (page > 1)
            {
                vm.NewerPage = page - 1;
            }

            int lastOneOnCurrentPage = skip + amountPerPage;
            int total = db.GetPostsCount();
            if (lastOneOnCurrentPage < total)
            {
                vm.OlderPage = page + 1;
            }


            return View(vm);
        }

        public ActionResult ViewBlog(int id)
        {
            if (id == 0)
            {
                return Redirect("/"); //if no id was sent in, redirect to home page
            }
            var db = new BlogDb(_connectionString);
            var vm = new ViewBlogViewModel();
            var post = db.GetPost(id);
            if (post == null)
            {
                return Redirect("/"); //id not found in db
            }
            vm.Post = post;
            vm.Comments = db.GetComments(id);
            vm.CommenterName = Request.Cookies["commenter-name"];
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddComment(Comment comment)
        {
            var db = new BlogDb(_connectionString);
            comment.DateCreated = DateTime.Now;
            db.AddComment(comment);
            Response.Cookies.Append("commenter-name", comment.Name);
            return Redirect($"/home/viewblog?id={comment.PostId}");
        }

        public ActionResult MostRecent()
        {
            var db = new BlogDb(_connectionString);
            int id = db.GetMostRecentId();
            return Redirect($"/home/viewblog?id={id}");
        }
    }
}
