using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Humanizer;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Api.ApiModels {
	[DataContract(Name = "artist")]
	public class ArtistBase
	{
		[DataMember(Name = "alias")]
		public string Alias { get; set; }
	}
	[DataContract(Name = "artist")]
	public class ArtistSummary: ArtistBase {
		[DataMember(Name="files")]
		public int Files { get; set; }

		[DataMember(Name="url")]
		public string Url { get; set; }

		public ArtistSummary(string alias, int files)
		{
			Alias = alias;
			Files = files;
			Url = "/artist/" + alias.Underscore().Dasherize();
		}
	}

	[DataContract(Name = "artist")]
	public class ArtistDetail : ArtistBase
	{
		[DataMember(Name="Files")]
		public IEnumerable<FileSummary> Files { get; set; } 
	}

	[DataContract(Name = "result")]
	public class ArtistResult
	{
		[DataMember(Name = "artists")]
		public IEnumerable<ArtistSummary> Artists { get; set; }
	}
}