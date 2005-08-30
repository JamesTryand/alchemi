//
// Alchemi.Core.Manager.Storage.SqlServerManagerDatabaseStorage.cs
//
// Author:
//   Tibor Biro (tb@tbiro.com)
//
// Copyright (C) 2005 Tibor Biro (tb@tbiro.com)
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by 
// the Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License 
// along with this program; if not, write to the Free Software 
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Data;

using Advanced.Data.Provider;

namespace Alchemi.Core.Manager.Storage
{
	/// <summary>
	/// Override some generic database calls with SQL Server specific calls.
	/// This is usually done for performance reasons.
	/// </summary>
	public class SqlServerManagerDatabaseStorage : GenericManagerDatabaseStorage
	{
		private String m_connectionString;

		public SqlServerManagerDatabaseStorage(String connectionString)
		{
			m_connectionString = connectionString;
		}

		#region IManagerStorage Members

		/// <summary>
		/// GetSystemSummary implementation for Sql Server.
		/// This implementation uses a stored procedure to extract the data.
		/// </summary>
		/// <returns></returns>
		public new SystemSummary GetSystemSummary()
		{
			AdpConnection connection = new AdpConnection(m_connectionString);
			AdpCommand command = new AdpCommand();

			command.Connection = connection;
			command.CommandText = "Admon_SystemSummary";
			command.CommandType = CommandType.StoredProcedure;

			using (AdpDataReader dataReader = command.ExecuteReader())
			{
				if (dataReader.Read())
				{
					String maxPower;
					Int32 totalExecutors;
					Int32 powerUsage;
					Int32 powerAvailable;
					String powerTotalUsage;
					Int32 unfinishedThreads;

					maxPower = dataReader.GetString(dataReader.GetOrdinal("max_power"));
					totalExecutors = dataReader.GetInt32(dataReader.GetOrdinal("total_executors"));
					powerUsage = dataReader.GetInt32(dataReader.GetOrdinal("power_usage"));
					powerAvailable = dataReader.GetInt32(dataReader.GetOrdinal("power_avail"));
					powerTotalUsage = dataReader.GetString(dataReader.GetOrdinal("power_totalusage"));
					unfinishedThreads = dataReader.GetInt32(dataReader.GetOrdinal("unfinished_threads"));

					return new SystemSummary(
						maxPower, 
						totalExecutors,
						powerUsage,
						powerAvailable,
						powerTotalUsage,
						unfinishedThreads);
				}
			}

			return null;
		}

		#endregion

	}
}
