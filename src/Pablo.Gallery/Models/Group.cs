using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models {
	[Table("Group", Schema = GalleryContext.Schema)]
	public class Group {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		[MaxLength(256), Index("ux_Group_Name", IsUnique = true)]
		string Name { get; set; }
		[InverseProperty("Group")]
		public virtual ICollection<Pack> Packs { get; set; } 
	}
}