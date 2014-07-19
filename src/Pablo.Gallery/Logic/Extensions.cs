using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.IO;
using System.Security.Principal;
using System.Linq;
using Microsoft.Ajax.Utilities;

namespace Pablo.Gallery
{
	public static class Extensions
	{
		public static bool In<T>(this T source, params T[] list) {
			return (list as IList<T>).Contains(source);
		}

		public static T WrapWebApiException<T>(this Controller controller, Func<T> action)
		{
			try
			{
				return action();
			}
			catch (System.Web.Http.HttpResponseException ex)
			{
				throw new System.Web.HttpException((int)ex.Response.StatusCode, null);
			}
		}

		public static Models.User CurrentUser(this IPrincipal principal)
		{
			using (var db = new Models.GalleryContext())
			{
				return db.Users.FirstOrDefault(r => r.UserName == principal.Identity.Name);
			}
		}

	}
}

