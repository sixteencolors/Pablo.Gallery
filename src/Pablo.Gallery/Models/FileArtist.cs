using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pablo.Gallery.Logic.Interfaces;

namespace Pablo.Gallery.Models {
	[Table("File_Artist", Schema = GalleryContext.Schema)]
	public class FileArtist : IVersionable, IAuditable, IDeletable {
		public virtual int FileId { get; set; }
		public virtual int ArtistId { get; set; }
		public virtual File File { get; set; }
		public virtual Artist Artist { get; set; }
		public int Version { get; set; }
		public int CreatedById { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public int EditedById { get; set; }
		public DateTime EditedDateTime { get; set; }
		public bool IsDeleted { get; set; }
	}
}