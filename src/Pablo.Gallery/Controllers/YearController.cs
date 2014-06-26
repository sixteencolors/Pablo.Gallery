using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace Pablo.Gallery.Controllers
{
	public class YearController : Controller
	{
		public ActionResult Detail(int? year)
		{
            // I could not figure out a route to do this, so if the route has a null year, return the index
            // (needed for support of legacy Sixteen Colors URL's
		    return !year.HasValue ? View("Index") : View(year.Value);
		}

	    public ActionResult Index()
		{
			return View();
		}
	}
}
