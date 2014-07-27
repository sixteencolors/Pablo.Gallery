using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Pablo.Gallery.Logic.Interfaces;

namespace Pablo.Gallery.Models {
	[Table("Artist", Schema = GalleryContext.Schema)]
	public class Artist {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[MaxLength(256), Index("ux_Alias_Name", IsUnique = true)]
		public string Alias { get; set; }
		public virtual ICollection<Group> Groups { get; set; }
		public virtual ICollection<FileArtist> Files { get; set; }
	}

}