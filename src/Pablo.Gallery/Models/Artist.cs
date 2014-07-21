using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models {
	[Table("Artist", Schema = GalleryContext.Schema)]
	public class Artist {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[MaxLength(256), Index("ux_Alias_Name", IsUnique = true)]
		string Alias { get; set; }
		public virtual ICollection<Group> Groups { get; set; }
		[InverseProperty("Artists")]
		public virtual ICollection<File> Files { get; set; } 
	}
}