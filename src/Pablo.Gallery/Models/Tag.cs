using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Humanizer;

namespace Pablo.Gallery.Models {
	[Table("Tag", Schema = GalleryContext.Schema)]
	public class Tag {
		private string _slug;
		private string _name;

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(256), Index("ux_Tag_Name", IsUnique = true)]
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				_slug = _name.Underscore().Dasherize();
			}
		}

		public virtual ICollection<FileTag> FileTags { get; set; }

		[MaxLength(256), Index("ux_Tag_Slug", IsUnique = true)]
		public string Slug
		{
			get { return _slug; }
			set { _slug = value; }
		}
	}
}