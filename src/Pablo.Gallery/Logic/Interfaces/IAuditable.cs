using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Logic.Interfaces {
	public interface IAuditable {
		int CreatedById { get; set; }
		DateTime CreatedDateTime { get; set; }
		int EditedById { get; set; }
		DateTime EditedDateTime { get; set; }
	}
}