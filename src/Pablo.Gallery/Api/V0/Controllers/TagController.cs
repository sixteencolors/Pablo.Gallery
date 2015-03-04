using System.Linq;
using System.Net;
using System.Web.Http;
using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic.Filters;

namespace Pablo.Gallery.Api.V0.Controllers
{
    public class TagController : ApiController
    {
		private readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet, EnableCors]
		public TagResult Index(int page = 0, int size = Global.DefaultPageSize) {
			var tags = from t in db.Tags orderby t.Name select t;
			var results = size > 0 ? tags.Skip(page * size).Take(size).AsEnumerable() : tags;
			return new TagResult {
				Tags = (from tag in results
						   select new TagSummary(tag)).ToList()
			};
		}

		[HttpGet, EnableCors]
		public TagDetail Index([FromUri(Name = "id")] string alias, int page = 0, int size = Global.DefaultPageSize) {
			var tag = db.Tags.FirstOrDefault(a => a.Slug == alias);
			if (tag == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new TagDetail(tag, page, size);
		}

		protected override void Dispose(bool disposing) {
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}

	}
}