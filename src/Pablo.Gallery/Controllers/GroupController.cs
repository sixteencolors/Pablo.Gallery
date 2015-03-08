using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Controllers
{
    public class GroupController : Controller
    {
        readonly GalleryContext db = new GalleryContext();

        public ActionResult Detail(string group)
        {
            var model = db.Groups.FirstOrDefault(g => g.Slug == group);
            if (model == null)
                return new HttpNotFoundResult();
            return View(model);
        }

    }
}
