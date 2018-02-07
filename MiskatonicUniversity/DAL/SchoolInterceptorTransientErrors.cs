﻿using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using MiskatonicUniversity.Logging;

namespace MiskatonicUniversity.DAL
{
	public class SchoolInterceptorTransientErrors : DbCommandInterceptor
	{
		private int _counter = 0;
		private ILogger _logger = new Logger();

		// you can also override NonQueryExecuting and ScalarExecuting methods to check other query types' connection resiliency
		public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			bool throwTransientErrors = false;
			if (command.Parameters.Count > 0 && command.Parameters[0].Value.ToString() == "%Throw%")
			{
				throwTransientErrors = true;
				// replaces "Throw" with "an" so some students will be found and returned
				command.Parameters[0].Value = "%an%";
				command.Parameters[1].Value = "%an%";
			}

			if (throwTransientErrors && _counter < 4)
			{
				_logger.Information("Returning transient error for command: {0}", command.CommandText);
				_counter++;
				interceptionContext.Exception = CreateDummySqlException();
			}
		}

		private SqlException CreateDummySqlException()
		{
			// the instance of SQL Server you attempted to connect to doesn't support encryption
			var sqlErrorNumber = 20;

			var sqlErrorCtor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 7).Single();
			var sqlError = sqlErrorCtor.Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 });

			var errorCollection = Activator.CreateInstance(typeof(SqlErrorCollection), true);
			var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
			addMethod.Invoke(errorCollection, new[] { sqlError });

			var sqlExceptionCtor = typeof(SqlException).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 4).Single();
			var sqlException = (SqlException)sqlExceptionCtor.Invoke(new object[] { "Dummy", errorCollection, null, Guid.NewGuid() });

			return sqlException;
		}
	}
}