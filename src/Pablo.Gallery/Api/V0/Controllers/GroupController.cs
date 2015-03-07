using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic.Filters;

namespace Pablo.Gallery.Api.V0.Controllers
{
    public class GroupController : ApiController
    {
        private readonly Models.GalleryContext db = new Models.GalleryContext();
        [System.Web.Http.HttpGet, EnableCors]
        public GroupResult Index(int page = 0, int size = Global.DefaultPageSize) {
            var groups = from g in db.Groups orderby g.Name select g;
            var results = size > 0 ? groups.Skip(page * size).Take(size).AsEnumerable() : groups;
            return new GroupResult {
                Groups = (from @group in results
                           select new GroupSummary(@group)).ToList()
            };
        }

        [System.Web.Http.HttpGet, EnableCors]
        public GroupDetail Index([FromUri(Name = "id")] string alias, int page = 0, int size = Global.DefaultPageSize) {
            var group = db.Groups.FirstOrDefault(a => a.Slug == alias);
            if (group == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return new GroupDetail(group, page, size);
        }

        protected override void Dispose(bool disposing) {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

    }
}
