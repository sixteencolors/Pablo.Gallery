﻿using System.Web.Mvc;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Pablo.Gallery.Controllers
{
    public class PackController : GalleryController
	{

		public ActionResult Index(string query = null, int? year = null)
		{
            ViewBag.Params = new {
                Year = year,
                Query = query
            };
            return View();
		}

		public ActionResult Detail(string pack)
		{
			var model = db.Packs.FirstOrDefault(p => p.Name == pack);
			if (model == null)
				return new HttpNotFoundResult();
			return View(model);
		}

		public ActionResult File(string pack, string file)
		{
			var model = db.Files.FirstOrDefault(r => r.Pack.Name == pack && r.Name == file);
			if (model == null)
				return new HttpNotFoundResult();
			return View(model);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}
