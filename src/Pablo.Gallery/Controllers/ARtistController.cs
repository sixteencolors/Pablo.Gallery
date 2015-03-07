using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Controllers
{
    public class ArtistController : Controller
    {
        readonly GalleryContext db = new GalleryContext();

        public ActionResult Detail(string artist)
        {
            var model = db.Artists.FirstOrDefault(a => a.Slug == artist);
            if (model == null)
                return new HttpNotFoundResult();
            return View(model);
        }

    }
}
