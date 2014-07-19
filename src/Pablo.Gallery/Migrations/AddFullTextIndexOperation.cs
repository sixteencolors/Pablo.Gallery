using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Migrations {
	public class AddFullTextIndexOperation : MigrationOperation{

		public string Table { get; private set; }
		public string Dictionary { get; private set; }
		public string Name { get; private set; }
		public string Column { get; private set; }

		/// <summary>
		/// Operation for adding a full text index to your database
		/// </summary>
		/// <param name="table">Name of the table to add the index to</param>
		/// <param name="name">Name of the index to add</param>
		/// <param name="dictionary">Dictionary to be used</param>
		public AddFullTextIndexOperation(string table, string name, string column, string dictionary) : base(new {})
		{
			Table = table;
			Dictionary = dictionary;
			Name = name;
			Column = column;
		}



		public override bool IsDestructiveChange {
			get { return false; }
		}
	}
}