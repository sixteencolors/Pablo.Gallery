﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
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
		}

		[DataMember(Name = "url")]
        public string Url { get { return "pack/" + pack.Name; } set { } }

		[DataMember(Name = "previewUrl")]
		public string PreviewUrl { get { return pack.PreviewUrl(maxWidth: 320).TrimStart('~'); } set { } }

		[DataMember(Name = "previewWidth")]
		public int PreviewWidth { get { return Math.Min((pack.Thumbnail != null ? pack.Thumbnail.Width : null) ?? 160, 160); } set { } }

		[DataMember(Name = "previewHeight")]
		public int PreviewHeight
		{
			get
			{
				var height = (pack.Thumbnail != null ? pack.Thumbnail.Height : null) ?? 160;
				var width = pack.Thumbnail != null ? pack.Thumbnail.Width : null;
				if (width != null)
					height = width > 0 ? PreviewWidth * height / width.Value : 0;
				return height;
			}
			set { }
		}

		[DataMember(Name = "name")]
        public string Name { get { return pack.Name; } set { } }

		[DataMember(Name = "date")]
		[JsonConverter(typeof(DateOnlyConverter))]
        public DateTime? Date { get { return pack.Date; } set { } }

		[DataMember(Name = "groups")]
		public string[] Groups { get; set; }

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
		public PackDetail(Models.Pack p, int page = 0, int size = Global.DefaultPageSize)
			: base(p)
		{
			Files = (from f in p.Files
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
}