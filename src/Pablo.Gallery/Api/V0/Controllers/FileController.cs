using Pablo.Gallery.Api.ApiModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Pablo.Gallery.Logic.Interceptors;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class FileController : ApiController
	{
		readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public IEnumerable<FileSummary> Index(string format = null, string type = null, string query = null, int page = 0, int pageSize = Global.DefaultPageSize, string keyword = null)
		{
			var s = FullTextSearchInterceptor.Search(keyword);
			var files = db.QueryFiles(format, type, query).Where(f => f.Content.Text.Contains(s));
			return (from f in files.Skip(page*pageSize).Take(pageSize).AsEnumerable()
				select new FileSummary(f)).ToArray();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}
