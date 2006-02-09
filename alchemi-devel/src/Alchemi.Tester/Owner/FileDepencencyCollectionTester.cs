#region Alchemi copyright and license notice

/*
* Alchemi [.NET Grid Computing Framework]
* http://www.alchemi.net
* Title         :  FileDepencencyCollectionTester.cs
* Project       :  Alchemi.Tester.Owner
* Created on    :  08 February 2006
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

using Alchemi.Core.Owner;

using NUnit.Framework;

namespace Alchemi.Tester.Owner
{
	/// <summary>
	/// Summary description for FileDepencencyCollectionTester.
	/// </summary>
	[TestFixture]
	public class FileDepencencyCollectionTester
	{
		public FileDepencencyCollectionTester()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region "Contains Tests"

		[Test]
		public void ContatinsTestNullValue()
		{
			FileDependencyCollection collection = new FileDependencyCollection();

			bool result = collection.Contains(null);

			Assert.IsFalse(result);
		}

		[Test]
		public void ContatinsTestEmptyCollection()
		{
			FileDependencyCollection collection = new FileDependencyCollection();
			FileDependency fd = new FileDependencyTestFiller(@"c:\tst.txt");

			bool result = collection.Contains(fd);

			Assert.IsFalse(result);
		}

		[Test]
		public void ContatinsTestDependencyFound()
		{
			FileDependencyCollection collection = new FileDependencyCollection();
			FileDependency fd1 = new FileDependencyTestFiller(@"c:\test1.txt");
			FileDependency fd2 = new FileDependencyTestFiller(@"c:\test2.txt");

			collection.Add(fd1);
			collection.Add(fd2);

			bool result = collection.Contains(fd1);

			Assert.IsTrue(result);
		}

		[Test]
		public void ContatinsTestDependencyNotFound()
		{
			FileDependencyCollection collection = new FileDependencyCollection();
			FileDependency fd1 = new FileDependencyTestFiller(@"c:\test1.txt");
			FileDependency fd2 = new FileDependencyTestFiller(@"c:\test2.txt");
			FileDependency fd3 = new FileDependencyTestFiller(@"c:\test3.txt");

			collection.Add(fd1);
			collection.Add(fd2);

			bool result = collection.Contains(fd3);

			Assert.IsFalse(result);
		}

		[Test]
		public void ContatinsTestDependencyFoundCaseOnlyDifferent()
		{
			FileDependencyCollection collection = new FileDependencyCollection();
			FileDependency fd1 = new FileDependencyTestFiller(@"c:\test1.txt");
			FileDependency fd2 = new FileDependencyTestFiller(@"c:\test2.txt");
			FileDependency fd3 = new FileDependencyTestFiller(@"c:\TEST2.txt");

			collection.Add(fd1);
			collection.Add(fd2);

			bool result = collection.Contains(fd3);

			Assert.IsTrue(result);
		}

		#endregion

		#region "Add Tests"

		[Test]
		public void AddTestNullValue()
		{
			FileDependencyCollection collection = new FileDependencyCollection();

			try
			{
				collection.Add(null);

				Assert.IsTrue(false, "Adding a null value to the collection should throw an InvalidOretationException.");
			}
			catch (InvalidOperationException e)
			{
				Console.Write(e);
				// pass if we get here
				Assert.IsTrue(true);
			}
		}

		[Test]
		public void AddTestDuplicatedValue()
		{
			FileDependencyCollection collection = new FileDependencyCollection();
			FileDependency fd1 = new FileDependencyTestFiller(@"c:\test1.txt");
			FileDependency fd2 = new FileDependencyTestFiller(@"c:\test1.txt");

			collection.Add(fd1);

			try
			{
				collection.Add(fd2);

				Assert.IsTrue(false, "Adding a duplicate value to the collection should throw an InvalidOretationException.");
			}
			catch (InvalidOperationException e)
			{
				Console.Write(e);
				// pass if we get here
				Assert.IsTrue(true);
			}
		}

		[Test]
		public void AddTestSimpleScenario()
		{
			FileDependencyCollection collection = new FileDependencyCollection();
			FileDependency fd1 = new FileDependencyTestFiller(@"c:\test1.txt");
			FileDependency fd2 = new FileDependencyTestFiller(@"c:\test2.txt");

			collection.Add(fd1);
			collection.Add(fd2);

			Assert.IsTrue(collection.Contains(fd1));
			Assert.IsTrue(collection.Contains(fd2));
		}

		#endregion
	}
}
