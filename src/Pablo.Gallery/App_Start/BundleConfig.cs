using System.Web.Optimization;
using Pablo.Gallery.Logic.Less;

namespace Pablo.Gallery
{
	public static class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.unobtrusive*",
				"~/Scripts/jquery.validate*"
			));

			bundles.Add(new ScriptBundle("~/bundles/js").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/colorbox/jquery.colorbox.js",
				"~/Scripts/bootstrap.js",
				"~/Scripts/imagesloaded.pkgd.js",
				"~/Scripts/jquery.parseparams.js",
				"~/Scripts/jsrender.js",
				"~/Scripts/Site.js",
				"~/Scripts/select2.js"
			));

			var bundle = new StyleBundle("~/bundles/css").Include(
			    "~/Content/colorbox.css",
				"~/Content/bootstrap.css",
				"~/Content/css/select2.css",
				"~/Content/Site.less"				
			);
			bundle.Transforms.Clear();
			bundle.Transforms.Add(new LessTransform());
			bundle.Transforms.Add(new CssMinify());
			bundles.Add(bundle);
            BundleTable.EnableOptimizations = true;

            //bundles.Add(new StyleBundle("~/Content/themes/base/css"));
        }

        public static void RegisterExternalBundles(BundleCollection bundles)
		{
			// bundles used for external viewer

			bundles.Add(new ScriptBundle("~/external/js-full").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/colorbox/jquery.colorbox.js",
				"~/Scripts/jsrender.js",
				"~/Scripts/External.js"
			));

			bundles.Add(new ScriptBundle("~/external/js").Include(
				"~/Scripts/External.js"
			));

			var bundle = new StyleBundle("~/external/css").Include(
				"~/Content/External.less"
			);
			bundle.Transforms.Clear();
			bundle.Transforms.Add(new LessTransform());
			bundle.Transforms.Add(new CssMinify());
			bundles.Add(bundle);

		}
	}
}