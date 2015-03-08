using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Api.ApiModels {
	[DataContract(Name = "artist")]
	public class ArtistSummary
	{
		public Artist Artist;

		public ArtistSummary(Artist artist)
		{
			Artist = artist;
		}

		[DataMember(Name = "url")]
		public string Url { get { return "/artist/" + Artist.Slug; } set { } }

		[DataMember(Name = "alias")]
		public string Alias { get { return Artist.Alias; } set { } }

		[DataMember(Name = "slug")]
		public string Slug { get { return Artist.Slug; } set { } }
	}

	[DataContract(Name = "artist")]
	public class ArtistDetail : ArtistSummary
	{
		public ArtistDetail(Artist artist, int page = 0, int size = Global.DefaultPageSize):base(artist)
		{
			Files = (from fa in artist.Files
				select new FileSummary(fa.File)).Skip(page*size).Take(size);
		}

		[DataMember(Name = "files")]
		public IEnumerable<FileSummary> Files { get; set; }
	}

	[DataContract(Name = "result")]
	public class ArtistResult
	{
		[DataMember(Name = "artists")]
		public IEnumerable<ArtistSummary> Artists { get; set; }
	}

    public class ArtistMeta {
        [DataMember(Name = "alias")]
        public string Alias { get; set; }
    }

}