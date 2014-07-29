using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Exceptionless.Serialization;
using Humanizer;
using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic.Filters;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Api.V0.Controllers
{
    public class ArtistController : ApiController
    {
	    private readonly Models.GalleryContext db = new Models.GalleryContext();

	    [HttpGet, EnableCors]
	    public ArtistResult Index(int page = 0, int size = Global.DefaultPageSize)
	    {
		    var artists = from a in db.Artists orderby a.Alias select a;
		    var results = size > 0 ? artists.AsEnumerable().Skip(page*size).Take(size) : artists;
		    return new ArtistResult {Artists = (from artist in results select new ArtistSummary(artist.Alias, artist.Files.Count)).ToList()};
	    }

	    [HttpGet, EnableCors]
	    public ArtistDetail Index([FromUri(Name = "id")] string alias, int page = 0, int size = Global.DefaultPageSize)
	    {
		    var files = from f in db.Files
			    join fa in db.FileArtists on f.Id equals fa.FileId
			    join a in db.Artists on fa.ArtistId equals a.Id
			    where a.Slug == alias
			    select f;

		    var artist = db.Artists.FirstOrDefault(a => a.Slug == alias);
		    var results = files.OrderBy(f => f.Pack.Date).Skip(page*size).Take(size).AsEnumerable();
		    if (artist != null)
			    return new ArtistDetail
			    {
				    Alias = artist.Alias,
				    Files = (from f in results select new FileSummary(f)).ToArray()
			    };
			else return new ArtistDetail(); // TODO: Throw exception?
	    }

		protected override void Dispose(bool disposing) {
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}

    }
}
