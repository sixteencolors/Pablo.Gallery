using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Pablo.Gallery.Models;

namespace Pablo.Gallery.Api.ApiModels
{
	[DataContract(Name = "file")]
	public class FileSummary
	{
		readonly Models.File file;

		public FileSummary(Models.File file)
		{
			this.file = file;
		}

		[DataMember(Name = "url")]
		public string Url { get { return "/pack/" + file.Path; } set { } }

		[DataMember(Name = "downloadUrl")]
		public string DownloadUrl { get { return file.DownloadUrl().TrimStart('~'); } set { } }

		[DataMember(Name = "previewUrl")]
		public string PreviewUrl { get { return file.PreviewUrl(maxWidth: 320).TrimStart('~'); } set { } }

		[DataMember(Name = "previewWidth")]
		public int? PreviewWidth { get { return file.Width != null ? (int?)Math.Min(file.Width.Value, 160) : null; } set { } }

		[DataMember(Name = "previewHeight")]
		public int? PreviewHeight
		{
			get
			{
				var height = file.Height;
				var width = file.Width;
				var previewWidth = PreviewWidth;
				if (height != null && width != null && previewWidth != null)
					return width > 0 ? previewWidth * height / width : 0;
				return null;
			}
			set { }
		}

		[DataMember(Name = "pack")]
		public string Pack { get { return file.Pack.Name; } set { } }

		[DataMember(Name = "path")]
		public string Path { get { return Logic.Scanner.NormalizedPath(System.IO.Path.GetDirectoryName(file.NativeFileName)); } set { } }

		[DataMember(Name = "fileName")]
		public string FileName { get { return System.IO.Path.GetFileName(file.NativeFileName); } set { } }

		[DataMember(Name = "name")]
		public string Name { get { return file.Name; } set { } }

		[DataMember(Name = "format")]
		public string Format { get { return file.Format; } set { } }

		[DataMember(Name = "type")]
		public string Type { get { return file.Type; } set { } }

		[DataMember(Name = "displayUrl")]
		public string DisplayUrl { get { return file.DisplayUrl().TrimStart('~'); } set { } }

		[DataMember(Name = "displayType")]
		public string DisplayType { get { return file.DisplayType(); } set { } }
	}

	[DataContract(Name = "file")]
	public class FileDetail : FileSummary
	{
		public FileDetail(Models.File file)
			: base(file)
		{
			Year = file.Pack.Date.Value.Year;
		}

		[DataMember(Name = "year")]
		public int Year { get; set; }
	}
}