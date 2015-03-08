using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace Pablo.Gallery.Api.ApiModels
{
	class DateOnlyConverter : IsoDateTimeConverter
	{
		public DateOnlyConverter()
		{
			DateTimeFormat = "yyyy-MM-dd";
		}
	}

	[DataContract(Name = "pack")]
	public class PackSummary
	{
		Models.Pack pack;

		public PackSummary(Models.Pack pack)
		{
			this.pack = pack;

            // TODO: Figure out why trying to do this here causes an exception that does not bubble up when making the comparison to null if pack.Group is not null
            //if (pack.Group != null)
            //    Group = new GroupSummary(pack.Group);
               
		}

		[DataMember(Name = "url")]
		public string Url { get { return "/pack/" + pack.Name; } set { } }

		[DataMember(Name = "previewUrl")]
		public string PreviewUrl { get { return pack.PreviewUrl(maxWidth: 320).TrimStart('~'); } set { } }

		[DataMember(Name = "previewWidth")]
		public int? PreviewWidth { get { return pack.Thumbnail != null && pack.Thumbnail.Width != null ? (int?)Math.Min(pack.Thumbnail.Width.Value, 160) : null; } set { } }

		[DataMember(Name = "previewHeight")]
		public int? PreviewHeight
		{
			get
			{
				var height = pack.Thumbnail != null ? (int?)pack.Thumbnail.Height : null;
				var width = pack.Thumbnail != null ? (int?)pack.Thumbnail.Width : null;
				var previewWidth = PreviewWidth;
				if (height != null && width != null && previewWidth != null)
					return width > 0 ? previewWidth * height / width : 0;
				return null;
			}
			set { }
		}

		[DataMember(Name = "name")]
		public string Name { get { return pack.Name; } set { } }

		[DataMember(Name = "date")]
		[JsonConverter(typeof(DateOnlyConverter))]
		public DateTime? Date { get { return pack.Date; } set { } }

		[DataMember(Name = "group")]
		public GroupSummary Group { get; set; }

		[DataMember(Name = "fileName")]
		public string FileName { get { return pack.FileName; } set { } }

		[DataMember(Name = "thumbnail")]
		public FileSummary Thumbnail
		{
			get { return pack.Thumbnail != null ? new FileSummary(pack.Thumbnail) : null; }
			set { }
		}
	}

	[DataContract(Name = "pack")]
	public class PackDetail : PackSummary
	{
		public PackDetail(Models.Pack pack, int page = 0, int size = Global.DefaultPageSize)
			: base(pack)
		{
            if (pack.Group != null)
                Group = new GroupSummary(pack.Group);
            Files = (from f in pack.Files
			         orderby f.Order
			         select new FileSummary(f)).Skip(page * size).Take(size);
		}

		[DataMember(Name = "files")]
		public IEnumerable<FileSummary> Files { get; set; }
	}

	[DataContract(Name = "result")]
	public class PackResult
	{
		[DataMember(Name = "packs")]
		public IEnumerable<PackSummary> Packs { get; set; }
	}

    public class PackMetaUpdate
    {
        [DataMember(Name = "artist")]
        public ArtistMeta Artist { get; set; }

        [DataMember(Name = "tag")]
        public TagMeta Tag { get; set; }

        [DataMember(Name = "group")]
        public GroupMeta Group { get; set; }
    }
}