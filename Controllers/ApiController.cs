using BlogEngineNwareTechnologies.Controllers;
using BlogEngineNwareTechnologies.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class ApiController : Controller
    {
        [Route("Posts")]
        public JsonResult GetPosts()
        {
            CrudController crudController = new CrudController();
            var posts = crudController.ReadPost();
            if (posts.Count == 0 )
            {
                Response.StatusCode = 204;
                return Json(posts);
            }
            else
            {
                Response.StatusCode = 200;
                return Json(posts);
            }
        }

        [Route("Posts/{ID}")]
        public JsonResult GetPostsById(int id)
        {
            CrudController crudController = new CrudController();
            var posts = crudController.ReadPost();

            foreach ( var post in posts )
            {
                if ( post.Id == id )
                {
                    Response.StatusCode = 200;
                    return Json(post);
                }
            }
            return Json(Response.StatusCode = 404);
        }

        [Route("Category")]
        public JsonResult GetCategory()
        {
            CrudController crudController = new CrudController();
            var category = crudController.ReadCategory();
            if (category.Count == 0)
            {
                Response.StatusCode = 204;
                return Json(category);
            }
            else
            {
                Response.StatusCode = 200;
                return Json(category);
            }
        }

        [Route("Category/{ID}")]
        public JsonResult GetCategoryById(int id)
        {
            CrudController crudController = new CrudController();
            var categories = crudController.ReadCategory();

            foreach (var category in categories)
            {
                if (category.Id == id)
                {
                    Response.StatusCode = 200;
                    return Json(category);
                }
            }
            return Json(Response.StatusCode = 404);
        }

        [Route("Category/{ID}/Posts")]
        public ActionResult GetPostsByCategory(int id) 
        {
            CrudController crudController = new CrudController();
            var categories = crudController.ReadCategory();
            var posts = crudController.ReadPost();

            var postsList = new List<PostModel>();
            postsList = posts.Where(x => x.category.Id == id).ToList();

            if (postsList.Count == 0)
            {
                return Json(Response.StatusCode = 204);
            }
            else
            {
                Response.StatusCode = 200;
                return Json(postsList);
            }
        }

    }
}
