using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;

namespace Pablo.Gallery.Models {
	[Table("Artist", Schema = GalleryContext.Schema)]
	public class Artist {
		private string _slug;
		private string _alias;

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[MaxLength(256), Index("ux_Alias_Name", IsUnique = true)]
		public string Alias
		{
			get { return _alias; }
			set
			{
				_alias = value;
				_slug = _alias.Underscore().Dasherize();
			}
		}

		[MaxLength(256), Index("ux_Alias_Slug", IsUnique = true)]
		public string Slug
		{
			get { return _slug; }
			set { _slug = value; }
		}

		public virtual ICollection<FileArtist> Files { get; set; }
	}

}