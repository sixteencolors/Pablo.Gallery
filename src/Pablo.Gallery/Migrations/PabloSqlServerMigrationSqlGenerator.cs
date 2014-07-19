using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Npgsql;

namespace Pablo.Gallery.Migrations {
	public class PabloSqlServerMigrationSqlGenerator : NpgsqlMigrationSqlGenerator
	{
		protected override void Convert(MigrationOperation migrationOperation)
		{
			var operation = migrationOperation as AddFullTextIndexOperation;

			if (operation != null) {
				var sql = new StringBuilder();
				sql.Append("CREATE INDEX ");
				sql.Append(this.GetTableNameFromFullTableName(operation.Table) + "_" + operation.Name);
				sql.Append(" ON ");
				this.AppendTableName(operation.Table, sql);
				sql.Append(string.Format(" USING gin (to_tsvector('{0}', \"{1}\"))", operation.Dictionary, operation.Column));
				this.AddStatment(sql, false);
			}
		}
	}
}