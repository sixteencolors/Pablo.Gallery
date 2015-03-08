using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic.Filters;

namespace Pablo.Gallery.Api.V0.Controllers
{
    public class ArtistController : ApiController
    {
	    private readonly Models.GalleryContext db = new Models.GalleryContext();

	    [HttpGet, EnableCors]
	    public ArtistResult Index(int page = 0, int size = Global.DefaultPageSize)
	    {
	        var query = Request.GetQueryNameValuePairs().FirstOrDefault(q => q.Key == "query");
		    var artists = from a in db.Artists
                          orderby a.Alias
                          where string.IsNullOrEmpty(query.Value) || a.Alias.Contains(query.Value)
                          select a;
			var results = size > 0 ? artists.Skip(page * size).Take(size).AsEnumerable() : artists;
		    return new ArtistResult
		    {
			    Artists = (from artist in results 
						   select new ArtistSummary(artist)).ToList()
		    };
	    }

	    [HttpGet, EnableCors]
	    public ArtistDetail Index([FromUri(Name = "id")] string alias, int page = 0, int size = Global.DefaultPageSize)
	    {
		    var artist = db.Artists.FirstOrDefault(a => a.Slug == alias);
		    if (artist == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
		    return new ArtistDetail(artist, page, size);
	    }

		protected override void Dispose(bool disposing) {
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}

    }
}
