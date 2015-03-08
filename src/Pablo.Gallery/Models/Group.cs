using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Humanizer;

namespace Pablo.Gallery.Models {
	[Table("Group", Schema = GalleryContext.Schema)]
	public class Group
	{
	    private string _slug;
	    private string _name;

	    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

	    [MaxLength(256), Index("ux_Group_Name", IsUnique = true)]
	    public string Name
	    {
	        get { return _name; }
	        set
	        {
	            _name = value;
	            _slug = _name.Underscore().Dasherize();
	        }
	    }

	    [MaxLength(256), Index("ux_Alias_Slug", IsUnique = true)]
        public string Slug {
            get { return _slug; }
            set { _slug = value; }
        }
        
        [InverseProperty("Group")]
		public virtual ICollection<Pack> Packs { get; set; } 
	}
}