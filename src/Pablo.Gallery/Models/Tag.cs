using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models {
	[Table("Tag", Schema = GalleryContext.Schema)]
	public class Tag {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(256), Index("ux_Tag_Name", IsUnique = true)]
		public string Name { get; set; }
		public virtual ICollection<FileTag> FileTags { get; set; }
	}
}