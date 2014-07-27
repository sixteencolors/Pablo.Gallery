using Pablo.Gallery.Api.ApiModels;
using Pablo.Gallery.Logic;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Pablo.Gallery.Logic.Converters;
using Pablo.Gallery.Logic.Extractors;
using Pablo.Gallery.Models;
using Pablo.Gallery.Logic.Filters;
using Pablo.Gallery.Logic.Selectors;
using System.Web;
using System.Collections.Generic;

namespace Pablo.Gallery.Api.V0.Controllers
{
	public class PackController : ApiController
	{
		private readonly Models.GalleryContext db = new Models.GalleryContext();

		[HttpGet]
		public PackResult Index(int? year = null, string query = null, int page = 0, int size = Global.DefaultPageSize)
		{
			IQueryable<Pack> packs = from p in db.Packs
				orderby p.Date descending
				select p;
			if (year != null)
				packs = packs.Where(pack => pack.Date.Value.Year == year);
			if (!string.IsNullOrEmpty(query))
				packs = packs.Where(pack => pack.FileName.ToLower().Contains(query.ToLower()));
			var results = size > 0 ? packs.Skip(page*size).Take(size).AsEnumerable() : packs;
			return new PackResult
			{
				Packs = (from pack in results
					select new PackSummary(pack)).ToList()
			};
		}

		[HttpGet, ActionName("Index")]
		public HttpResponseMessage Download([FromUri(Name = "id")] string packName, string format)
		{
			var pack = db.Packs.FirstOrDefault(r => r.Name == packName);
			if (pack == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			if (format != "download")
				throw new HttpResponseException(HttpStatusCode.Forbidden);

			var packArchiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, pack.NativeFileName);
			var content = new StreamContent(System.IO.File.OpenRead(packArchiveFileName));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			//content.Headers.ContentLength = pack.FileSize;
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = Path.GetFileName(pack.NativeFileName)
			};
			var response = new HttpResponseMessage {Content = content};
			response.Headers.CacheControl = new CacheControlHeaderValue
			{
				MaxAge = new TimeSpan(0, 10, 0),
				Public = true
			};
			return response;
		}

		[HttpGet, EnableCors]
		public PackDetail Index([FromUri(Name = "id")] string packName, int page = 0, int size = Global.DefaultPageSize)
		{
			var pack = db.Packs.FirstOrDefault(p => p.Name == packName);
			if (pack == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new PackDetail(pack, page, size);
		}

		private static string GetMediaType(string format)
		{
			switch (format.ToUpperInvariant())
			{
				case "PNG":
					return "image/png";
				case "GIF":
					return "image/gif";
				case "JPG":
				case "JPEG":
					return "image/jpeg";
				case "TIFF":
					return "image/tiff";
				case "MP3":
					return "audio/mpeg";
				case "OGG":
				case "OGA":
					return "application/ogg";
				default:
					return "application/octet-stream";
			}
		}

		[HttpPost, HttpPut, HttpDelete, EnableCors]
		[Authorize]
		public FileDetail Index([FromUri(Name = "id")] string pack, [FromUri(Name = "path")] string name, [FromUri] Artist artist)
		{
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == pack && r.Name == name);
			if (file != null)
			{
				var artistRecord = db.Artists.FirstOrDefault(a => a.Alias == artist.Alias) ?? db.Artists.Add(new Artist {Alias = artist.Alias});
				var fileArtist = file.Artists.FirstOrDefault(fa => fa.Artist.Id == artistRecord.Id && fa.FileId == file.Id);

				if (fileArtist == null && Request.Method != HttpMethod.Delete)
					db.FileArtists.Add(new FileArtist { ArtistId = artistRecord.Id, FileId = file.Id });
				else if (fileArtist != null && fileArtist.IsDeleted && Request.Method != HttpMethod.Delete)
					fileArtist.IsDeleted = false;
				else if (fileArtist != null && Request.Method == HttpMethod.Delete)
					fileArtist.IsDeleted = true;

				db.SaveChanges();
				file = db.Files.FirstOrDefault(f => f.Id == file.Id);
			}
			return new FileDetail(file);
			// do nothing if the relationship already exists and is not deleted
		}

		[HttpPost, HttpPut, HttpDelete, EnableCors]
		[Authorize]
		public FileDetail Index([FromUri(Name = "id")] string pack, [FromUri(Name = "path")] string name, [FromUri] Tag tag) {
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == pack && r.Name == name);
			if (file != null) {
				var tagRecord = db.Tags.FirstOrDefault(t => t.Name == tag.Name) ?? db.Tags.Add(new Tag { Name = tag.Name });
				var fileTag = file.Tags.FirstOrDefault(ft => ft.Tag.Id == tagRecord.Id && ft.FileId == file.Id);

				if (fileTag == null && Request.Method != HttpMethod.Delete)
					db.FileTags.Add(new FileTag { TagId = tagRecord.Id, FileId = file.Id });
				else if (fileTag != null && fileTag.IsDeleted && Request.Method != HttpMethod.Delete)
					fileTag.IsDeleted = false;
				else if (fileTag != null && Request.Method == HttpMethod.Delete)
					fileTag.IsDeleted = true;

				db.SaveChanges();
				file = db.Files.FirstOrDefault(f => f.Id == file.Id);
			}
			return new FileDetail(file);
			// do nothing if the relationship already exists and is not deleted
		}

		[HttpGet, EnableCors]
		public FileDetail Index([FromUri(Name = "id")] string pack, [FromUri(Name = "path")] string name)
		{
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == pack && r.Name == name);
			if (file == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);
			return new FileDetail(file);
		}

		static async Task GetStream(Stream outStream, string packArchiveFileName, Models.File file, Converter converter, ConvertInfo convertInfo)
		{
			using (outStream)
			{
				convertInfo.ExtractFile = async destFile =>
				{
					var extractor = ExtractorFactory.GetFileExtractor(packArchiveFileName);
					await extractor.ExtractFile(packArchiveFileName, file.FileName, destFile);
				};
				using (var stream = await converter.Convert(convertInfo))
				{
					stream.CopyTo(outStream);
				}
			}
		}

		static async Task GetRawStream(Stream outStream, string packArchiveFileName, Models.File file)
		{
			using (outStream)
			{
				var extractor = ExtractorFactory.GetFileExtractor(packArchiveFileName);
				using (var stream = await extractor.ExtractFile(packArchiveFileName, file.FileName))
				{
					stream.CopyTo(outStream);
				}
			}
		}

		[HttpGet]
		public HttpResponseMessage Index([FromUri(Name = "id")]string packName, [FromUri(Name = "path")] string name, string format)
		{
			var file = db.Files.FirstOrDefault(r => r.Pack.Name == packName && r.Name == name);
			if (file == null)
				throw new HttpResponseException(HttpStatusCode.NotFound);

			var download = string.Equals(format, "download", StringComparison.OrdinalIgnoreCase);

			// TODO: support multiple archives
			var packArchiveFileName = Path.Combine(Global.SixteenColorsArchiveLocation, file.Pack.NativeFileName);

			var convertInfo = new ConvertInfo
			{
				Pack = file.Pack,
				FileName = file.NativeFileName,
				InputFormat = file.Format,
				InputType = file.Type,
				OutFileName = file.NativeFileName + "." + format,
				Properties = Request.RequestUri.ParseQueryString()
			};
			// check if we can convert this file
			var converter = ConverterFactory.GetConverter(convertInfo);

			HttpContent content;
			if (download || converter == null)
			{
				content = new PushStreamContent((s, hc, t) => GetRawStream(s, packArchiveFileName, file));
			}
			else
			{
				// prepare for conversion, parse request options
				converter.Prepare(convertInfo);

				content = new PushStreamContent((s, hc, t) => GetStream(s, packArchiveFileName, file, converter, convertInfo));
			}

			var mediaType = GetMediaType(format);
			content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
			var disposition = mediaType == "application/octet-stream" ? "attachment" : "inline";
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue(disposition)
			{
				FileName = Path.GetFileName(convertInfo.OutFileName)
			};

			var response = new HttpResponseMessage { Content = content };
			response.Headers.CacheControl = new CacheControlHeaderValue
			{
				MaxAge = new TimeSpan(0, 10, 0),
				Public = true
			};
			return response;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				db.Dispose();
			base.Dispose(disposing);
		}
	}
}
