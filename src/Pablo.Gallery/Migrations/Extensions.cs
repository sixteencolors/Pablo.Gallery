using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Ajax.Utilities;
using Npgsql;

namespace Pablo.Gallery.Migrations {
	public static class Extensions {
		public static void AddFullTextIndex(this DbMigration migration, string table, string name, string column, string dictionary = null)
		{
			((IDbMigration)migration).AddOperation(new AddFullTextIndexOperation(table, name, column, dictionary));
		}
	}
}