﻿using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Pablo.Gallery
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Year",
				url: "year/{year}",
                defaults: new { controller = "Year", action = "Detail" },
                constraints: new { year = @"^\d{4}$" }
			);

			routes.MapRoute(
				name: "Pack",
				url: "pack/{pack}",
				defaults: new { controller = "Pack", action = "Detail" }
			);

		    routes.MapRoute(
		        name: "Artist",
		        url: "artist/{artist}",
		        defaults: new {controller = "Artist", action = "Detail"}
		    );

            // Legacy Sixteen Colors route
		    routes.MapRoute(
		        name: "Search",
		        url: "search/",
		        defaults: new {controller = "File", action = "Index"}
		    );

			routes.MapRoute(
				name: "File",
				url: "pack/{pack}/{*file}",
				defaults: new { controller = "Pack", action = "File" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}