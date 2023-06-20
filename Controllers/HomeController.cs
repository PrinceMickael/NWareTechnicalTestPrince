using BlogEngineNwareTechnologies.Controllers;
using BlogEngineNwareTechnologies.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index(string? CrudAction )
        {
            var crudController = new CrudController();
            var categoryList = crudController.ReadCategory();
            var postList = crudController.ReadPost();
            postList = removeUlteriorDatePost(postList);

            dynamic model = new System.Dynamic.ExpandoObject();
            model.Categories = categoryList;
            model.Posts = postList;
            model.CrudAction = CrudAction;

            return View(model);
        }

        private List<PostModel> removeUlteriorDatePost(List<PostModel> listAllPosts)
        {
            var listPostsAccepted = new List<PostModel>();

            foreach (var post in listAllPosts)
            {
                if (post.publicationDate <= DateTime.Now) { listPostsAccepted.Add(post); }
            }

            return listPostsAccepted;
        }

        public ActionResult CreateCategory()
        {
            return View();
        }

        public ActionResult UpdateCategory(string pTitle)
        {
            ViewBag.oldTitle = pTitle;
            return View();
        }

        public ActionResult CreatePost()
        {
            var crudController = new CrudController();
            var list = crudController.ReadCategory();
            ViewBag.CategoryList = list;

            return View();
        }

        public ActionResult UpdatePost(string pTitle)
        {

            //get category
            var crudController = new CrudController();
            var listPosts = crudController.ReadPost();

            foreach (var post in listPosts)
            {
                var list = crudController.ReadCategory();
                ViewBag.categoryList = list;

                if (post.title == pTitle)
                {
                    ViewBag.oldTitle = post.title;
                    ViewBag.oldCategory = post.category.title;
                    var date = post.publicationDate;
                    ViewBag.oldPublicationDate = date.ToShortDateString();
                    ViewBag.oldContent = post.content;

                }
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}