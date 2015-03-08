using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Api.ApiModels {
	[DataContract(Name = "tag")]
	public class TagSummary {
		public Tag Tag { get; set; }

		public TagSummary(Models.Tag tag)
		{
			Tag = tag;
		}

		[DataMember(Name="url")]
		public string Url { get { return "/tag/" + Tag.Slug; } set { } }

		[DataMember(Name="name")]
		public string Name { get { return Tag.Name; } set { }}

		[DataMember(Name="slug")]
		public string Slug { get { return Tag.Slug; } set { } }
	}

	[DataContract(Name = "tag")]
	public class TagDetail : TagSummary
	{
		public TagDetail(Tag tag, int page = 0, int size = Global.DefaultPageSize) : base(tag)
		{
			Files = (from ft in tag.Files
				select new FileSummary(ft.File)).Skip(page*size).Take(size);
		}

		[DataMember(Name = "files")]
		public IEnumerable<FileSummary> Files { get; set; } 
	}

	[DataContract(Name = "result")]
	public class TagResult
	{
		[DataMember(Name="tags")]
		public IEnumerable<TagSummary> Tags { get; set; } 
	}
    public class TagMeta {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}