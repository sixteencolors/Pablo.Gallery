using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Pablo.Gallery.Logic.Interfaces;

namespace Pablo.Gallery.Models {
	[Table("File_Tag", Schema = GalleryContext.Schema)]
	public class FileTag : IVersionable, IAuditable, IDeletable {
		public int FileId { get; set; }
		public int TagId { get; set; }
		public File File { get; set; }
		public Tag Tag { get; set; }
		public int Version { get; set; }
		public int CreatedById { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public int EditedById { get; set; }
		public DateTime EditedDateTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}