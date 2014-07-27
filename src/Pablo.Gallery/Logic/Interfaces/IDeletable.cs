using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Logic.Interfaces {
	public interface IDeletable {
		bool IsDeleted { get; set; }
	}
}