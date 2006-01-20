#region Alchemi copyright and license notice

/*
* Alchemi [.NET Grid Computing Framework]
* http://www.alchemi.net
* Title         :  SqlServerManagerDatabaseStorageTester.cs
* Project       :  Alchemi.Tester.Manager.Storage
* Created on    :  22 September 2005
* Copyright     :  Copyright � 2006 The University of Melbourne
*                    This technology has been developed with the support of
*                    the Australian Research Council and the University of Melbourne
*                    research grants as part of the Gridbus Project
*                    within GRIDS Laboratory at the University of Melbourne, Australia.
* Author        :  Tibor Biro (tb@tbiro.com)
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

using System;
using System.Configuration;

using Alchemi.Core.Manager.Storage;
using Alchemi.Manager.Storage;

using NUnit.Framework;

namespace Alchemi.Tester.Manager.Storage
{
	/// <summary>
	/// SQL Server specific tests
	/// Replace the managerStorage object with a SQL Server storage and 
	/// inherit all tests from the ManagerStorageTester
	/// </summary>
	[TestFixture]
	public class SqlServerManagerDatabaseStorageTester : ManagerStorageTester
	{
		private SqlServerManagerDatabaseStorage m_managerStorage;

		public override IManagerStorage ManagerStorage
		{
			get
			{
				return m_managerStorage;
			}
		}

		[SetUp]
		public void TestStartUp()
		{
			String connectionString = ConfigurationSettings.AppSettings["SqlTesterConnectionString"];
			connectionString = "Provider=SQLOLEDB;User ID=alchemi;Password=alchemi;Initial Catalog=AlchemiTester;Data Source=localhost;Connect Timeout=30";
			connectionString = string.Format(
				"user id={1};password={2};initial catalog={3};data source={0};Connect Timeout=5; Max Pool Size=5; Min Pool Size=5",
				"localhost",
				"alchemi",
				"alchemi",
				"AlchemiTester"
				);

			m_managerStorage = new SqlServerManagerDatabaseStorage(connectionString);

			m_managerStorage.SetUpStorage();
			m_managerStorage.InitializeStorageData();
		}

		[TearDown]
		public void TestShutDown()
		{
			m_managerStorage.TearDownStorage();
		}

		public SqlServerManagerDatabaseStorageTester()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
