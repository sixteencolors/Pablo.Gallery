using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Controllers
{
    public class GalleryController : Controller
    {
        protected readonly GalleryContext db = new GalleryContext();

    }
}
