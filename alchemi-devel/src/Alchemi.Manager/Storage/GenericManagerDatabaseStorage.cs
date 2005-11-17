#region Alchemi copyright and license notice

/*
* Alchemi [.NET Grid Computing Framework]
* http://www.alchemi.net
* Title         :  GenericManagerDatabaseStorage.cs
* Project       :  Alchemi.Core.Manager.Storage
* Created on    :  30 August 2005
* Copyright     :  Copyright � 2005 The University of Melbourne
*                    This technology has been developed with the support of
*                    the Australian Research Council and the University of Melbourne
*                    research grants as part of the Gridbus Project
*                    within GRIDS Laboratory at the University of Melbourne, Australia.
* Author        :  Tibor Biro (tb@tbiro.com), Krishna Nadiminti (kna@csse.unimelb.edu.au)
* License       :  GPL
*                    This program is free software; you can redistribute it and/or
*                    modify it under the terms of the GNU General Public
*                    License as published by the Free Software Foundation;
*                    See the GNU General Public License
*                    (http://www.gnu.org/copyleft/gpl.html) for more 
details.
*
*/
#endregion

//using Advanced.Data.Provider;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Text;
using Alchemi.Core;
using Alchemi.Core.Manager;
using Alchemi.Core.Manager.Storage;
using Alchemi.Core.Owner;
using Alchemi.Core.Utility;

namespace Alchemi.Manager.Storage
{
	/// <summary>
	/// Implement generic relational database storage here
	/// This class should not be directly instantiated because it only contains a partial implementation
	/// 
	/// TODO: The Executors are updated very often so database updates will probably be very expensive. Change the update functions to only update data that actually changed since the last load.
	/// </summary>
	public abstract class GenericManagerDatabaseStorage : IManagerStorage
	{
		protected String m_connectionString;

		public GenericManagerDatabaseStorage(String connectionString)
		{
			m_connectionString = connectionString;
		}

		#region Generic database manipulation routines
		
		/// <summary>
		/// Run a stored procedure and return a data reader.
		/// The caller is responsible for closing the database connection.
		/// </summary>
		/// <param name="storedProcedure"></param>
		/// <returns></returns>
		protected OleDbDataReader RunSpReturnDataReader(String storedProcedure)
		{
			OleDbConnection connection = new OleDbConnection(m_connectionString);
			OleDbCommand command = new OleDbCommand();
			//AdpConnection connection = new AdpConnection(m_connectionString);
			//AdpCommand command = new AdpCommand();

			command.Connection = connection;
			command.CommandText = storedProcedure;
			command.CommandType = CommandType.StoredProcedure;
		
			connection.Open();

			// the connection must remain open until the reader is closed
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}

		protected OleDbDataReader RunSqlReturnDataReader(String sqlQuery)
		{
			OleDbConnection connection = new OleDbConnection(m_connectionString);
			OleDbCommand command = new OleDbCommand();

			command.Connection = connection;
			command.CommandText = sqlQuery;
			command.CommandType = CommandType.Text;
		
			connection.Open();

			// the connection must remain open until the reader is closed
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}

		/// <summary>
		/// Run a stored procedure and return a scalar.
		/// </summary>
		/// <param name="storedProcedure"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected object RunSpReturnScalar(String storedProcedure, params OleDbParameter[] parameters)
		{
			using(OleDbConnection connection = new OleDbConnection(m_connectionString))
			{
				OleDbCommand command = new OleDbCommand();

				command.Connection = connection;
				command.CommandText = storedProcedure;
				command.CommandType = CommandType.StoredProcedure;

				foreach (OleDbParameter parameter in parameters)
				{
					command.Parameters.Add(parameter);
				}
		
				connection.Open();

				return command.ExecuteScalar();
			}
		}

		protected void RunSql(String sqlQuery)
		{
			RunSql(sqlQuery, null);
		}

		protected void RunSql(String sqlQuery, params OleDbParameter[] parameters)
		{
			using(OleDbConnection connection = new OleDbConnection(m_connectionString))
			{
				OleDbCommand command = new OleDbCommand();

				command.Connection = connection;
				command.CommandText = sqlQuery;
				command.CommandType = CommandType.Text;

				if (parameters != null)
				{
					foreach(OleDbParameter parameter in parameters)
					{
						command.Parameters.Add(parameter);
					}
				}
		
				connection.Open();

				command.ExecuteNonQuery();
			}
		}

		protected object RunSqlReturnScalar(String sqlQuery)
		{
			return RunSqlReturnScalar(sqlQuery, null);
		}

		protected object RunSqlReturnScalar(String sqlQuery, params OleDbParameter[] parameters)
		{
			using(OleDbConnection connection = new OleDbConnection(m_connectionString))
			{
				OleDbCommand command = new OleDbCommand();

				command.Connection = connection;
				command.CommandText = sqlQuery;
				command.CommandType = CommandType.Text;

				if (parameters != null)
				{
					foreach(OleDbParameter parameter in parameters)
					{
						command.Parameters.Add(parameter);
					}
				}
		
				connection.Open();

				return command.ExecuteScalar();
			}
		}

		#endregion

		#region IManagerStorage Members

		public SystemSummary GetSystemSummary()
		{
			throw new Exception("Not implemented");
		}

		public DataSet RunSqlReturnDataSet(string query)
		{
			DataSet result = null;
			using (OleDbConnection connection = new OleDbConnection(m_connectionString))
			{
				OleDbCommand command = new OleDbCommand();
				command.Connection = connection;
				command.CommandText = query;
				command.CommandType = CommandType.Text;
			
				connection.Open();
				OleDbDataAdapter da = new OleDbDataAdapter(command);
				result = new DataSet();
				da.Fill(result);
			}

			return result;
		}

		/// <summary>
		/// Add users to a database
		/// </summary>
		/// <param name="users"></param>
		public void AddUsers(UserStorageView[] users)
		{
			if (users == null)
			{
				return;
			}

			foreach (UserStorageView user in users)
			{
				String sqlQuery;
				
				sqlQuery = String.Format("insert usr values('{0}', '{1}', {2})", 
					user.Username.Replace("'", "''"), 
					user.Password.Replace("'", "''"), 
					user.GroupId);
				
				RunSql(sqlQuery);
			}
		}

		public void UpdateUsers(UserStorageView[] updates)
		{
			if (updates == null)
			{
				return;
			}

			foreach (UserStorageView user in updates)
			{
				String sqlQuery;
				
				sqlQuery = String.Format("update usr set password='{1}', grp_id={2} where usr_name='{0}'", 
					user.Username.Replace("'", "''"), 
					user.Password.Replace("'", "''"), 
					user.GroupId);
				
				RunSql(sqlQuery);
			}
		}

		public UserStorageView[] GetUsers()
		{
			ArrayList userList = new ArrayList();

			using(OleDbDataReader dataReader = RunSqlReturnDataReader("select usr_name, password, grp_id from usr"))
			{
				while(dataReader.Read())
				{
					String username = dataReader.GetString(dataReader.GetOrdinal("usr_name"));
					String password = dataReader.GetString(dataReader.GetOrdinal("password"));
					Int32 groupId = dataReader.GetInt32(dataReader.GetOrdinal("grp_id"));

					UserStorageView user = new UserStorageView(username, password, groupId);

					userList.Add(user);
				}
			}

			return (UserStorageView[])userList.ToArray(typeof(UserStorageView));
		}

		public bool AuthenticateUser(SecurityCredentials sc)
		{
			if (sc == null || sc.Username == null || sc.Password == null)
			{
				return false;
			}

			object userCount = RunSqlReturnScalar(String.Format("select count(*) as authenticated from usr where usr_name = '{0}' and password = '{1}'",
				sc.Username.Replace("'", "''"),
				sc.Password.Replace("'", "''")));

			return Convert.ToBoolean(userCount);
		}

		public void AddGroups(GroupStorageView[] groups)
		{
			if (groups == null)
			{
				return;
			}

			foreach (GroupStorageView group in groups)
			{
				String sqlQuery;
				
				sqlQuery = String.Format("insert grp(grp_id, grp_name) values({0}, '{1}')", 
					group.GroupId,
					group.GroupName.Replace("'", "''"));
				
				RunSql(sqlQuery);
			}
		}
		
		public GroupStorageView[] GetGroups()
		{
			ArrayList groupList = new ArrayList();

			using(OleDbDataReader dataReader = RunSqlReturnDataReader("select grp_id, grp_name from grp"))
			{
				while(dataReader.Read())
				{
					Int32 groupId = dataReader.GetInt32(dataReader.GetOrdinal("grp_id"));
					String groupName = dataReader.GetString(dataReader.GetOrdinal("grp_name"));

					GroupStorageView group = new GroupStorageView(groupId, groupName);

					groupList.Add(group);
				}
			}

			return (GroupStorageView[])groupList.ToArray(typeof(GroupStorageView));
		}

		public void AddGroupPermission(Int32 groupId, Permission permission)
		{
			String sqlQuery;
			
			// in case there is a duplicate remove the permission first
			sqlQuery = String.Format("delete grp_prm where grp_id={0} and prm_id={1}", 
				groupId,
				(Int32)permission);

			RunSql(sqlQuery);

			sqlQuery = String.Format("insert grp_prm(grp_id, prm_id) values({0}, {1})", 
				groupId,
				(Int32)permission);
				
			RunSql(sqlQuery);
		}

		public Permission[] GetGroupPermissions(Int32 groupId)
		{
			ArrayList permissions = new ArrayList();

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(String.Format("select prm_id from grp_prm where grp_id={0}", groupId)))
			{
				while(dataReader.Read())
				{
					Permission permission = (Permission)dataReader.GetInt32(dataReader.GetOrdinal("prm_id"));

					permissions.Add(permission);
				}
			}

			return (Permission[])permissions.ToArray(typeof(Permission));
		}

		public bool CheckPermission(SecurityCredentials sc, Permission perm)
		{
			String query = String.Format("select count(*) as permitted from usr inner join grp on grp.grp_id = usr.grp_id inner join grp_prm on grp_prm.grp_id = grp.grp_id inner join prm on prm.prm_id = grp_prm.prm_id where usr.usr_name = '{0}' and prm.prm_id >= {1}", 
				sc.Username.Replace("'", "''"),
				(int)perm);

			return Convert.ToBoolean(RunSqlReturnScalar(query));
		}

		public String AddExecutor(ExecutorStorageView executor)
		{
			if (executor == null)
			{
				return null;
			}

			String executorId = Guid.NewGuid().ToString();

			RunSql(String.Format("insert into executor(executor_id, is_dedicated, is_connected, usr_name) values ('{0}', {1}, {2}, '{3}')",
				executorId,
				Convert.ToInt16(executor.Dedicated),
				Convert.ToInt16(executor.Connected),
				executor.Username.Replace("'", "''")
				));

			UpdateExecutorPingTime(executorId, executor.PingTime);

			UpdateExecutorHostAddress(executorId, executor.HostName, executor.Port);

			UpdateExecutorCpuUsage(executorId, executor.MaxCpu, executor.CpuUsage, executor.AvailableCpu, executor.TotalCpuUsage);

			UpdateExecutorAdditionalInformation(executorId, executor.MaxMemory, executor.MaxDisk, executor.NumberOfCpu, executor.Os, executor.Architecture);

			return executorId;
		}

		private void UpdateExecutorPingTime(String executorId, DateTime pingTime)
		{
			OleDbParameter dateTimeParameter = new OleDbParameter("@ping_time", pingTime);
			
			if (pingTime != DateTime.MinValue)
			{
				RunSql(String.Format("update executor set ping_time=? where executor_id='{0}'",
					executorId
					), 
					dateTimeParameter);
			}
			else
			{
				RunSql(String.Format("update executor set ping_time=null where executor_id='{0}'",
					executorId
					));
			}
		}

		private void UpdateExecutorHostAddress(String executorId, String hostName, Int32 port)
		{
			RunSql(String.Format("update executor set host='{1}', port={2} where executor_id='{0}'",
				executorId,
				hostName.Replace("'", "''"),
				port
				));
		}

		private void UpdateExecutorCpuUsage(String executorId, Int32 maxCpu, Int32 cpuUsage, Int32 availableCpu, float totalCpuUsage)
		{
			RunSql(String.Format("update executor set cpu_max={1}, cpu_usage={2}, cpu_avail={3}, cpu_totalusage={4} where executor_id='{0}'",
				executorId,
				maxCpu,
				cpuUsage,
				availableCpu,
				totalCpuUsage
				));
		}

		private void UpdateExecutorDetails(String executorId, bool dedicated, bool connected, String userName)
		{
			RunSql(String.Format("update executor set is_dedicated='{1}', is_connected='{2}', usr_name='{3}' where executor_id='{0}'",
				executorId,
				Convert.ToInt16(dedicated),
				Convert.ToInt16(connected),
				userName.Replace("'", "''")
				));
		}
		
		private void UpdateExecutorAdditionalInformation(String executorId, float maxMemory, float maxDisk, Int32 numberOfCpu, String os, String architecture)
		{
			RunSql(String.Format("update executor set mem_max = {1}, disk_max = {2}, num_cpus = {3}, os = '{4}', arch = '{5}' where executor_id='{0}'",
				executorId,
				maxMemory, 
				maxDisk, 
				numberOfCpu, 
				os.Replace("'", "''"), 
				architecture.Replace("'", "''")
				));
		}

		public void UpdateExecutor(ExecutorStorageView executor)
		{
			if (executor == null || executor.ExecutorId == null || executor.ExecutorId.Length == 0)
			{
				return;
			}

			UpdateExecutorDetails(executor.ExecutorId, executor.Dedicated, executor.Connected, executor.Username);

			UpdateExecutorPingTime(executor.ExecutorId, executor.PingTime);

			UpdateExecutorHostAddress(executor.ExecutorId, executor.HostName, executor.Port);

			UpdateExecutorCpuUsage(executor.ExecutorId, executor.MaxCpu, executor.CpuUsage, executor.AvailableCpu, executor.TotalCpuUsage);
		}

		public ExecutorStorageView[] GetExecutors()
		{
			return GetExecutors(TriStateBoolean.Undefined);
		}

		public ExecutorStorageView[] GetExecutors(TriStateBoolean dedicated)
		{
			return GetExecutors(dedicated, TriStateBoolean.Undefined);
		}

		public ExecutorStorageView[] GetExecutors(TriStateBoolean dedicated, TriStateBoolean connected)
		{
			StringBuilder query = new StringBuilder();
			bool whereSet = false;

			query.Append("select executor_id, is_dedicated, is_connected, ping_time, host, port, usr_name, cpu_max, cpu_usage, cpu_avail, cpu_totalusage, mem_max, disk_max, num_cpus, os, arch from executor");
			
			if(dedicated != TriStateBoolean.Undefined)
			{
				if (!whereSet)
				{
					query.Append(" where ");
					whereSet = true;
				}
				else
				{
					query.Append(" and ");
				}

				query.AppendFormat(" is_dedicated = {0}", dedicated == TriStateBoolean.True ? 1 : 0);
			}

			if(connected != TriStateBoolean.Undefined)
			{
				if (!whereSet)
				{
					query.Append(" where ");
					whereSet = true;
				}
				else
				{
					query.Append(" and ");
				}

				query.AppendFormat(" is_connected = {0}", connected == TriStateBoolean.True ? 1 : 0);
			}
		
			using(OleDbDataReader dataReader = RunSqlReturnDataReader(query.ToString()))
			{
				return DecodeExecutorFromDataReader(dataReader);
			}
		}

		public ExecutorStorageView GetExecutor(String executorId)
		{
			using(OleDbDataReader dataReader = RunSqlReturnDataReader(String.Format("select executor_id, is_dedicated, is_connected, ping_time, host, port, usr_name, cpu_max, cpu_usage, cpu_avail, cpu_totalusage, mem_max, disk_max, num_cpus, os, arch from executor where executor_id='{0}'",
					  executorId)))
			{
				ExecutorStorageView[] executors = DecodeExecutorFromDataReader(dataReader);

				if (executors == null || executors.Length == 0)
				{
					return null;
				}
				else
				{
					return executors[0];
				}
			}
		}

		private ExecutorStorageView[] DecodeExecutorFromDataReader(OleDbDataReader dataReader)
		{
			ArrayList executors = new ArrayList();

			while(dataReader.Read())
			{
				// in SQL the executor ID is stored as a GUID so we use GetValue instead of GetString in order to maximize compatibility with other databases
				String executorId = dataReader.GetValue(dataReader.GetOrdinal("executor_id")).ToString(); 

				bool dedicated = dataReader.GetBoolean(dataReader.GetOrdinal("is_dedicated"));
				bool connected = dataReader.GetBoolean(dataReader.GetOrdinal("is_connected"));
				DateTime pingTime = dataReader.IsDBNull(dataReader.GetOrdinal("ping_time")) ? DateTime.MinValue : dataReader.GetDateTime(dataReader.GetOrdinal("ping_time"));
				String hostname = dataReader.GetString(dataReader.GetOrdinal("host"));
				Int32 port = dataReader.IsDBNull(dataReader.GetOrdinal("port")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("port"));
				String username = dataReader.GetString(dataReader.GetOrdinal("usr_name"));
				Int32 maxCpu = dataReader.IsDBNull(dataReader.GetOrdinal("cpu_max")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("cpu_max"));
				Int32 cpuUsage = dataReader.IsDBNull(dataReader.GetOrdinal("cpu_usage")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("cpu_usage"));
				Int32 availableCpu = dataReader.IsDBNull(dataReader.GetOrdinal("cpu_avail")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("cpu_avail"));
				float totalCpuUsage = dataReader.IsDBNull(dataReader.GetOrdinal("cpu_totalusage")) ? 0 : (float)dataReader.GetDouble(dataReader.GetOrdinal("cpu_totalusage"));

				float maxMemory = dataReader.IsDBNull(dataReader.GetOrdinal("mem_max")) ? 0 : (float)dataReader.GetDouble(dataReader.GetOrdinal("mem_max"));;
				float maxDisk = dataReader.IsDBNull(dataReader.GetOrdinal("disk_max")) ? 0 : (float)dataReader.GetDouble(dataReader.GetOrdinal("disk_max"));
				Int32 numberOfCpu = dataReader.IsDBNull(dataReader.GetOrdinal("num_cpus")) ? 0 : dataReader.GetInt32(dataReader.GetOrdinal("num_cpus"));
				String os = dataReader.IsDBNull(dataReader.GetOrdinal("os")) ? "" : dataReader.GetString(dataReader.GetOrdinal("os"));
				String architecture = dataReader.IsDBNull(dataReader.GetOrdinal("arch")) ? "" : dataReader.GetString(dataReader.GetOrdinal("arch"));

				ExecutorStorageView executor = new ExecutorStorageView(
					executorId,
					dedicated,
					connected,
					pingTime,
					hostname,
					port,
					username,
					maxCpu,
					cpuUsage,
					availableCpu,
					totalCpuUsage,
					maxMemory,
					maxDisk,
					numberOfCpu,
					os,
					architecture
					);

				executors.Add(executor);
			}

			return (ExecutorStorageView[])executors.ToArray(typeof(ExecutorStorageView));
		}

		public String AddApplication(ApplicationStorageView application)
		{
			if (application == null)
			{
				return null;
			}

			String applicationId = Guid.NewGuid().ToString();

			OleDbParameter dateTimeParameter = new OleDbParameter("@time_created", application.TimeCreated);

			RunSql(String.Format("insert into application(application_id, state, time_created, is_primary, usr_name) values ('{0}', {1}, ?, '{2}', '{3}')",
				applicationId,
				(int)application.State,
				Convert.ToInt16(application.Primary),
				application.Username.Replace("'", "''")
				), 
				dateTimeParameter);

			return applicationId;
		}

		public void UpdateApplication(ApplicationStorageView updatedApplication)
		{
			if (updatedApplication == null || updatedApplication.ApplicationId == null || updatedApplication.ApplicationId.Length == 0)
			{
				return;
			}

			OleDbParameter dateTimeParameter = new OleDbParameter("@time_created", updatedApplication.TimeCreated);

			RunSql(String.Format("update application set state = {1}, time_created = ?, is_primary = '{2}', usr_name = '{3}' where application_id = '{0}'",
				updatedApplication.ApplicationId,
				(int)updatedApplication.State,
				Convert.ToInt16(updatedApplication.Primary),
				updatedApplication.Username.Replace("'", "''")
				), 
				dateTimeParameter);
		}

		public ApplicationStorageView[] GetApplications()
		{
			return GetApplications(false);
		}

		public ApplicationStorageView[] GetApplications(bool populateThreadCount)
		{
			ArrayList applications = new ArrayList();

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(String.Format("select application_id, state, time_created, is_primary, usr_name from application")))
			{
				while(dataReader.Read())
				{
					// in SQL the application ID is stored as a GUID so we use GetValue instead of GetString in order to maximize compatibility with other databases
					String applicationId = dataReader.GetValue(dataReader.GetOrdinal("application_id")).ToString(); 

					ApplicationState state = (ApplicationState)dataReader.GetInt32(dataReader.GetOrdinal("state"));
					DateTime timeCreated = dataReader.GetDateTime(dataReader.GetOrdinal("time_created"));
					bool primary = dataReader.GetBoolean(dataReader.GetOrdinal("is_primary"));
					String username = dataReader.GetString(dataReader.GetOrdinal("usr_name"));

					ApplicationStorageView application = new ApplicationStorageView(
						applicationId,
						state,
						timeCreated,
						primary,
						username
						);

					if (populateThreadCount)
					{
						Int32 totalThreads;
						Int32 unfinishedThreads;

						GetApplicationThreadCount(application.ApplicationId, out totalThreads, out unfinishedThreads);

						application.TotalThreads = totalThreads;
						application.UnfinishedThreads = unfinishedThreads;
					}

					applications.Add(application);
				}
			}

			return (ApplicationStorageView[])applications.ToArray(typeof(ApplicationStorageView));
		}

		public ApplicationStorageView[] GetApplications(String userName, bool populateThreadCount)
		{
			ArrayList applications = new ArrayList();

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(String.Format("select application_id, state, time_created, is_primary, usr_name from application where usr_name = '{0}'",
					  userName.Replace("'", "''"))))
			{
				while(dataReader.Read())
				{
					// in SQL the application ID is stored as a GUID so we use GetValue instead of GetString in order to maximize compatibility with other databases
					String applicationId = dataReader.GetValue(dataReader.GetOrdinal("application_id")).ToString(); 

					ApplicationState state = (ApplicationState)dataReader.GetInt32(dataReader.GetOrdinal("state"));
					DateTime timeCreated = dataReader.GetDateTime(dataReader.GetOrdinal("time_created"));
					bool primary = dataReader.GetBoolean(dataReader.GetOrdinal("is_primary"));
					String username = dataReader.GetString(dataReader.GetOrdinal("usr_name"));

					ApplicationStorageView application = new ApplicationStorageView(
						applicationId,
						state,
						timeCreated,
						primary,
						username
						);

					if (populateThreadCount)
					{
						Int32 totalThreads;
						Int32 unfinishedThreads;

						GetApplicationThreadCount(application.ApplicationId, out totalThreads, out unfinishedThreads);

						application.TotalThreads = totalThreads;
						application.UnfinishedThreads = unfinishedThreads;
					}

					applications.Add(application);
				}
			}

			return (ApplicationStorageView[])applications.ToArray(typeof(ApplicationStorageView));
		}

		public ApplicationStorageView GetApplication(String applicationId)
		{
			using(OleDbDataReader dataReader = RunSqlReturnDataReader(String.Format("select application_id, state, time_created, is_primary, usr_name from application where application_id='{0}'", applicationId)))
			{
				if(dataReader.Read())
				{
					ApplicationState state = (ApplicationState)dataReader.GetInt32(dataReader.GetOrdinal("state"));
					DateTime timeCreated = dataReader.GetDateTime(dataReader.GetOrdinal("time_created"));
					bool primary = dataReader.GetBoolean(dataReader.GetOrdinal("is_primary"));
					String username = dataReader.GetString(dataReader.GetOrdinal("usr_name"));

					ApplicationStorageView application = new ApplicationStorageView(
						applicationId,
						state,
						timeCreated,
						primary,
						username
						);

					return application;
				}
				else
				{
					return null;
				}
			}
		}

		public Int32 AddThread(ThreadStorageView thread)
		{
			if (thread == null)
			{
				return -1;
			}

			OleDbParameter timeStartedParameter = new OleDbParameter("@time_started", thread.TimeStarted);
			if (!thread.TimeStartedSet)
			{
				timeStartedParameter.Value = DBNull.Value;
			}

			OleDbParameter timeFinishedParameter = new OleDbParameter("@time_finished", thread.TimeFinished);
			if (!thread.TimeFinishedSet)
			{
				timeFinishedParameter.Value = DBNull.Value;
			}

			OleDbParameter executorIdParameter;
			
			if (thread.ExecutorId != null)
			{
				executorIdParameter = new OleDbParameter("@executor_id", thread.ExecutorId);
			}
			else
			{
				executorIdParameter = new OleDbParameter("@executor_id", DBNull.Value);
			}

			object threadIdObject = RunSqlReturnScalar(String.Format("insert into thread(application_id, executor_id, thread_id, state, time_started, time_finished, priority, failed) values ('{0}', ?, {2}, {3}, ?, ?, {4}, '{5}')",
				thread.ApplicationId,
				thread.ExecutorId,
				thread.ThreadId,
				(int)thread.State,
				thread.Priority,
				Convert.ToInt16(thread.Failed)
				), 
				executorIdParameter, timeStartedParameter, timeFinishedParameter);

			return Convert.ToInt32(threadIdObject);
		}

		public void UpdateThread(ThreadStorageView updatedThread)
		{
			if (updatedThread == null)
			{
				return;
			}

			OleDbParameter timeStartedParameter = new OleDbParameter("@time_started", updatedThread.TimeStarted);
			if (!updatedThread.TimeStartedSet)
			{
				timeStartedParameter.Value = DBNull.Value;
			}

			OleDbParameter timeFinishedParameter = new OleDbParameter("@time_finished", updatedThread.TimeFinished);
			if (!updatedThread.TimeFinishedSet)
			{
				timeFinishedParameter.Value = DBNull.Value;
			}

			OleDbParameter executorIdParameter;
			
			if (updatedThread.ExecutorId != null)
			{
				executorIdParameter = new OleDbParameter("@executor_id", updatedThread.ExecutorId);
			}
			else
			{
				executorIdParameter = new OleDbParameter("@executor_id", DBNull.Value);
			}

			RunSql(String.Format("update thread set application_id = '{1}', executor_id = ?, thread_id = {3}, state = {4}, time_started = ?, time_finished = ?, priority = {5}, failed = '{6}' where internal_thread_id = {0}",
				updatedThread.InternalThreadId,
				updatedThread.ApplicationId,
				updatedThread.ExecutorId,
				updatedThread.ThreadId,
				(int)updatedThread.State,
				updatedThread.Priority,
				Convert.ToInt16(updatedThread.Failed)
				), 
				executorIdParameter, timeStartedParameter, timeFinishedParameter);
		}

		public ThreadStorageView GetThread(String applicationId, Int32 threadId)
		{
			StringBuilder query = new StringBuilder();

			query.AppendFormat("select internal_thread_id, application_id, executor_id, thread_id, state, time_started, time_finished, priority, failed from thread where application_id='{0}' and thread_id={1}",
				applicationId,
				threadId);

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(query.ToString()))
			{
				ThreadStorageView[] threads = DecodeThreadFromDataReader(dataReader);

				if (threads.Length > 0)
				{
					return threads[0];
				}
				else
				{
					return null;
				}
			}
		}

		public ThreadStorageView[] GetThreads(params ThreadState[] findStates)
		{
			return GetThreads(null, findStates);
		}

		public ThreadStorageView[] GetThreads(String findApplicationId, params ThreadState[] findStates)
		{
			StringBuilder query = new StringBuilder();

			query.AppendFormat("select internal_thread_id, application_id, executor_id, thread_id, state, time_started, time_finished, priority, failed from thread");

			if (findApplicationId != null || (findStates != null && findStates.Length > 0))
			{
				query.Append(" where ");
			}

			// build the query based on the passed in variables
			if (findApplicationId != null)
			{
				query.AppendFormat("application_id='{0}'",
					findApplicationId);

			}

			if (findStates != null && findStates.Length > 0)
			{
				if (findApplicationId != null)
				{
					query.Append(" and ");
				}

				query.Append(" state in ");
				query.Append("(");

				for(int index = 0; index < findStates.Length; index++)
				{
					ThreadState state = findStates[index];

					if (index > 0)
					{
						query.Append(",");
					}
					query.Append((int)state);
				}

				query.Append(")");
			}
			

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(query.ToString()))
			{
				return DecodeThreadFromDataReader(dataReader);
			}
		}

		public ThreadStorageView[] GetExecutorThreads(String executorId, params ThreadState[] findStates)
		{
			StringBuilder query = new StringBuilder();

			query.AppendFormat("select internal_thread_id, application_id, executor_id, thread_id, state, time_started, time_finished, priority, failed from thread");

			// build the query based on the passed in variables
			query.AppendFormat(" where executor_id='{0}'",
				executorId);

			if (findStates != null && findStates.Length > 0)
			{
				query.Append(" and state in ");
				query.Append("(");

				for(int index = 0; index < findStates.Length; index++)
				{
					ThreadState state = findStates[index];

					if (index > 0)
					{
						query.Append(",");
					}
					query.Append((int)state);
				}

				query.Append(")");
			}

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(query.ToString()))
			{
				return DecodeThreadFromDataReader(dataReader);
			}
		}

		public ThreadStorageView[] GetExecutorThreads(bool dedicatedExecutor, params ThreadState[] findStates)
		{
			StringBuilder query = new StringBuilder();

			query.AppendFormat("select internal_thread_id, application_id, thread.executor_id, thread_id, state, time_started, time_finished, priority, failed from thread inner join executor on (thread.executor_id = executor.executor_id) where is_dedicated = {0}",
				dedicatedExecutor ? "1" : "0");

			if (findStates != null && findStates.Length > 0)
			{
				query.Append(" and state in ");
				query.Append("(");

				for(int index = 0; index < findStates.Length; index++)
				{
					ThreadState state = findStates[index];

					if (index > 0)
					{
						query.Append(",");
					}
					query.Append((int)state);
				}

				query.Append(")");
			}

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(query.ToString()))
			{
				return DecodeThreadFromDataReader(dataReader);
			}

		}

		public ThreadStorageView[] GetExecutorThreads(bool dedicatedExecutor, bool connectedExecutor, params ThreadState[] findStates)
		{
			StringBuilder query = new StringBuilder();

			query.AppendFormat("select internal_thread_id, application_id, thread.executor_id, thread_id, state, time_started, time_finished, priority, failed from thread inner join executor on (thread.executor_id = executor.executor_id) where is_dedicated = {0} and is_connected = {1}",
				dedicatedExecutor ? "1" : "0",
				connectedExecutor ? "1" : "0");

			if (findStates != null && findStates.Length > 0)
			{
				query.Append(" and state in ");
				query.Append("(");

				for(int index = 0; index < findStates.Length; index++)
				{
					ThreadState state = findStates[index];

					if (index > 0)
					{
						query.Append(",");
					}
					query.Append((int)state);
				}

				query.Append(")");
			}

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(query.ToString()))
			{
				return DecodeThreadFromDataReader(dataReader);
			}
		}

		private ThreadStorageView[] DecodeThreadFromDataReader(OleDbDataReader dataReader)
		{
			if (dataReader == null)
			{
				return new ThreadStorageView[0];
			}

			ArrayList threads = new ArrayList();

			while(dataReader.Read())
			{
				Int32 internalThreadId = dataReader.GetInt32(dataReader.GetOrdinal("internal_thread_id"));

				// in SQL the application ID is stored as a GUID so we use GetValue instead of GetString in order to maximize compatibility with other databases
				String applicationId = dataReader.GetValue(dataReader.GetOrdinal("application_id")).ToString(); 
				String executorId = dataReader.IsDBNull(dataReader.GetOrdinal("executor_id")) ? null : dataReader.GetValue(dataReader.GetOrdinal("executor_id")).ToString();

				Int32 threadId = dataReader.GetInt32(dataReader.GetOrdinal("thread_id"));
				ThreadState state = (ThreadState)dataReader.GetInt32(dataReader.GetOrdinal("state"));

				// for lack of a better default set the dates to DateTime.MinValue.
				DateTime timeStarted = dataReader.IsDBNull(dataReader.GetOrdinal("time_started")) ? DateTime.MinValue: dataReader.GetDateTime(dataReader.GetOrdinal("time_started"));
				DateTime timeFinished = dataReader.IsDBNull(dataReader.GetOrdinal("time_finished")) ? DateTime.MinValue : dataReader.GetDateTime(dataReader.GetOrdinal("time_finished"));

				Int32 priority = dataReader.GetInt32(dataReader.GetOrdinal("priority"));
				bool failed = dataReader.IsDBNull(dataReader.GetOrdinal("failed")) ? false : dataReader.GetBoolean(dataReader.GetOrdinal("failed"));

				ThreadStorageView thread = new ThreadStorageView(
					internalThreadId,
					applicationId,
					executorId,
					threadId,
					state,
					timeStarted,
					timeFinished,
					priority,
					failed
					);

				threads.Add(thread);
			}

			return (ThreadStorageView[])threads.ToArray(typeof(ThreadStorageView));  
		}


		public void GetApplicationThreadCount(String applicationId, out Int32 totalThreads, out Int32 unfinishedThreads)
		{
			totalThreads = unfinishedThreads = 0;

			using(OleDbDataReader dataReader = RunSqlReturnDataReader(String.Format("select state from thread where application_id = '{0}'",
					  applicationId)))
			{
				while(dataReader.Read())
				{
					Int32 state = dataReader.GetInt32(dataReader.GetOrdinal("state"));

					totalThreads ++;

					if (state == 0 || state == 1 || state == 2)
					{
						unfinishedThreads ++;
					}

				}
			}

		}

		public Int32 GetApplicationThreadCount(String applicationId, ThreadState threadState)
		{
			object threadCount = RunSqlReturnScalar(String.Format("select count(*) from thread where application_id='{0}' and state = {1}",
				applicationId,
				(int)threadState));

			return Convert.ToInt32(threadCount);
		}

		public Int32 GetExecutorThreadCount(String executorId, params ThreadState[] threadState)
		{
			if (executorId == null || threadState == null || threadState.Length == 0)
			{
				return 0;
			}
			
			StringBuilder query = new StringBuilder();

			query.AppendFormat("select count(*) from thread where executor_id='{0}' and state in ", 
				executorId);
			
			query.Append("(");

			for(int index = 0; index < threadState.Length; index++)
			{
				ThreadState state = threadState[index];

				if (index > 0)
				{
					query.Append(",");
				}
				query.Append((int)state);
			}

			query.Append(")");

			object threadCount = RunSqlReturnScalar(query.ToString());

			return Convert.ToInt32(threadCount);

		}

		#endregion
	}
}
