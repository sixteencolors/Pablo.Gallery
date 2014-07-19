using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Pablo.Gallery.Logic.Interceptors {
	/// <summary>
	/// Intercept query to replace with full text syntax. Taken from http://www.entityframework.info/Home/FullTextSearch
	/// </summary>
	public class FullTextSearchInterceptor : IDbCommandInterceptor {
		private const string FullTextPrefix = "-FTSPREFIX-";
		public static string Search(string search) {
			return string.Format("({0}{1})", FullTextPrefix, search);
		}
		public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
		}
		public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
		}
		public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
			RewriteFullTextQuery(command);
		}
		public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
		}
		public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
			RewriteFullTextQuery(command);
		}
		public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
		}
		public static void RewriteFullTextQuery(DbCommand cmd) {
			string text = cmd.CommandText;
			for (int i = 0; i < cmd.Parameters.Count; i++) {
				DbParameter parameter = cmd.Parameters[i];
				if (parameter.DbType.In(DbType.String, DbType.AnsiString, DbType.StringFixedLength, DbType.AnsiStringFixedLength)) {
					if (parameter.Value == DBNull.Value)
						continue;
					var value = (string)parameter.Value;
					if (value.IndexOf(FullTextPrefix) >= 0) {
						parameter.Size = 4096;
						parameter.DbType = DbType.AnsiStringFixedLength;
						value = value.Replace(FullTextPrefix, ""); // remove prefix we added n linq query
						value = value.Substring(1, value.Length - 2); // remove %% escaping by linq translator from string.Contains to sql LIKE
						parameter.Value = value;
						if (cmd.Connection is Npgsql.NpgsqlConnection)
						{
							value = value.Trim(new char[] {')', '('}); // remove enexplicable parenthetical
							parameter.Value = string.Format("'{0}'", value); // HACK: puts all string searches in quotes, may not work well with and/or queries
							// pgsql version that may need modification over time if we hit cases where it does not quite work right
							// the handling of the pgsql query could probably be a lot more elegant, particularly in its handling of 
							// multiple words, but I am not a pgsql fulltext expert
							cmd.CommandText = Regex.Replace(text,
								string.Format(
									@"""(\w*)"".""(\w*)""\s*LIKE\s*\(@{0}\)\s?", parameter.ParameterName),
								string.Format(@"to_tsvector('english', ""$2"") @@ to_tsquery(@{0})", parameter.ParameterName));
						}
						else
						{
							// Untested SQL Server version fromo original example
							cmd.CommandText = Regex.Replace(text,
								string.Format(
									@"\[(\w*)\].\[(\w*)\]\s*LIKE\s*@{0}\s?(?:ESCAPE N?'~')", parameter.ParameterName),
								string.Format(@"contains([$1].[$2], @{0})", parameter.ParameterName));
						}
						if (text == cmd.CommandText)
							throw new Exception("FTS was not replaced on: " + text);
						text = cmd.CommandText;
					}
				}
			}
		}
	}
}