﻿using BlogEngineNwareTechnologies.Models;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Xml.Linq;
using WebApplication1.Models;
using static WebApplication1.Models.CrudActionModel;

namespace BlogEngineNwareTechnologies.Controllers
{
    public class CrudController : Controller
    {

        static readonly string CategoryDataFile = @".\Data\XmlFiles\CategoryData.xml";
        static readonly string PostsDataFile = @".\Data\XmlFiles\PostsData.xml";

        [HttpPost]
        public ActionResult CreateCategory(string pTitle)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(CategoryDataFile);
                XmlNode root = doc.SelectSingleNode("CATEGORIES");
                XmlElement category = doc.CreateElement("CATEGORY");
                root.AppendChild(category);

                XmlElement id = doc.CreateElement("ID");
                id.InnerText = doc.SelectNodes("CATEGORIES/CATEGORY").Count.ToString();
                category.AppendChild(id);

                XmlElement title = doc.CreateElement("TITLE");
                title.InnerText = pTitle;
                category.AppendChild(title);

                if (pTitle == null) 
                {
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Empty });
                }

                if (isCategoryValide(pTitle))
                {
                    doc.Save(CategoryDataFile);
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Create });
                }
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Exist });
            } catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Error });
            }
        }

        public List<CategoryModel> ReadCategory()
        {
            var categoryList = new List<CategoryModel>();

            using (var xmlReader = new StreamReader(CategoryDataFile))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                var CategoryNode = xmlDoc.SelectNodes("//CATEGORIES//CATEGORY");
                foreach (XmlNode subNode in CategoryNode)
                {
                    CategoryModel category;
                    category = new CategoryModel();
                    category.Id = int.Parse(subNode["ID"].InnerText);
                    category.title = subNode["TITLE"].InnerText;
                    categoryList.Add(category);
                }
            }

            return categoryList;

        }

        public ActionResult UpdateCategory(string oldTitle, string newTitle)
        {
            try
            {
                if (newTitle == null)
                {
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Empty });
                }

                var doc = XDocument.Load(CategoryDataFile);
                var target = doc.Root.Descendants("CATEGORY").FirstOrDefault(x => x.Element("TITLE").Value == oldTitle);
                var title = target.Descendants("TITLE").FirstOrDefault();
                title.Value = newTitle;

                if (isCategoryValide(newTitle))
                {
                    doc.Save(CategoryDataFile);
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Update });
                }

                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Exist });
            } catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Error });
            }
        }

        public ActionResult DeleteCategory(string oldTitle)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(CategoryDataFile);
                var root = doc.SelectSingleNode("CATEGORIES");
                var target = root.SelectSingleNode("CATEGORY[TITLE='" + oldTitle + "']");

                if (CategoryIsNotUse(oldTitle))
                {
                    root.RemoveChild(target);
                    doc.Save(CategoryDataFile);
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Delete });
                }
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Used });

            } catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Error });
            }
        }

        [HttpPost]
        public ActionResult CreatePost(string pTitle, string pCategory, string pDate, string pContent)
        {
            try
            {
                if (pTitle == null || pCategory == null || pDate == null || pContent == null)
                {
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Empty });
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(PostsDataFile);
                XmlNode root = doc.SelectSingleNode("POSTS");
                XmlElement post = doc.CreateElement("POST");
                root.AppendChild(post);

                XmlElement id = doc.CreateElement("ID");
                id.InnerText = doc.SelectNodes("POSTS/POST").Count.ToString(); ;
                post.AppendChild(id);

                XmlElement category = doc.CreateElement("CATEGORY");
                category.InnerText = pCategory;
                post.AppendChild(category);

                XmlElement title = doc.CreateElement("TITLE");
                title.InnerText = pTitle;
                post.AppendChild(title);

                XmlElement date = doc.CreateElement("PUBLICATIONDATE");
                date.InnerText = pDate;
                post.AppendChild(date);

                XmlElement content = doc.CreateElement("CONTENT");
                content.InnerText = pContent;
                post.AppendChild(content);


                if (isPostValide(pTitle, pCategory, pDate, pContent))
                {
                    doc.Save(PostsDataFile);
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Create });
                }
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Exist });

            } catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Error });
            }
        }

        public List<PostModel> ReadPost()
        {
            var postList = new List<PostModel>();
            var xmlFilePath = PostsDataFile;
            using (var xmlReader = new StreamReader(xmlFilePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                var listCategory = ReadCategory();

                var PostNode = xmlDoc.SelectNodes("//POSTS//POST");
                foreach (XmlNode subNode in PostNode)
                {
                    PostModel post = new PostModel()
                    {
                        Id = int.Parse(subNode["ID"].InnerText),
                        title = subNode["TITLE"].InnerText,
                        publicationDate = DateTime.Parse(subNode["PUBLICATIONDATE"].InnerText),
                        content = subNode["CONTENT"].InnerText
                    };

                    foreach (var category in listCategory)
                    {
                         if (category.title == subNode["CATEGORY"].InnerText)
                         {
                            post.category = new CategoryModel { Id = category.Id ,title = category.title };
                         }
                    }

                    postList.Add(post);
                }
                postList = postList.OrderBy(postList => postList.publicationDate).ToList();
                return postList;
            }
        }

        public ActionResult UpdatePost(string oldTitle, string newTitle, string newCategory, string newPublicationDate, string newContent)
        {
            try
            {

                if (newTitle == null || newCategory == null || newPublicationDate == null || newContent == null)
                {
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Empty });
                }

                var doc = XDocument.Load(PostsDataFile);

                var target = doc.Root.Descendants("POST").FirstOrDefault(x => x.Element("TITLE").Value == oldTitle);
                var title = target.Descendants("TITLE").FirstOrDefault();
                var category = target.Descendants("CATEGORY").FirstOrDefault();
                var publicationDate = target.Descendants("PUBLICATIONDATE").FirstOrDefault();
                var content = target.Descendants("CONTENT").FirstOrDefault();
                title.Value = newTitle;
                category.Value = newCategory;
                publicationDate.Value = newPublicationDate;
                content.Value = newContent;

                if (isCategoryValide(newTitle))
                {
                    doc.Save(PostsDataFile);
                    return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Update });
                }
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Exist });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Error });
            }
        }

        public ActionResult DeletePost(string oldTitle)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(PostsDataFile);

                var root = doc.SelectSingleNode("POSTS");
                var target = root.SelectSingleNode("POST[TITLE='" + oldTitle + "']");
                root.RemoveChild(target);

                doc.Save(PostsDataFile);

                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Delete });

            } catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { crudAction = ECrudAction.Error });
            }

        }

        private bool isCategoryValide(string pTitle)
        {
            //si existe deja
            var xml = new XmlDocument();
            xml.Load(CategoryDataFile);
            var categoryList = ReadCategory();
            foreach (var category in categoryList)
            {
                if (category.title == pTitle) return false;
            }

            return true;
        }

        private bool isPostValide(string pTitle, string pCategory, string pDate, string pContent)
        {


            return true;
        }

        private bool CategoryIsNotUse(string categoryTitleToDelete)
        {
            var list = ReadPost();
            foreach (var post in list)
            {
                // si la categorie a supprimer est utiliser par 1 post, ne pas supprimer
                if (categoryTitleToDelete == post.category.title) {  return false; }
            }

            return true;
        }

    }
}
