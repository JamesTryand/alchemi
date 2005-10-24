//
// Alchemi.Tester.Manager.Storage.ApplicationStorageViewTester.cs
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

using Alchemi.Core;
using Alchemi.Core.Manager.Storage;

using NUnit.Framework;

namespace Alchemi.Tester.Manager.Storage
{
	/// <summary>
	/// Test constrains devined on the ApplicationStorageView object.
	/// </summary>
	[TestFixture]
	public class ApplicationStorageViewTester
	{
		/// <summary>
		/// Try to get the TotalThreads value when it was not initialized
		/// It should throw an exception.
		/// </summary>
		[Test]
		public void TotalThreadsTestNotSet()
		{
			ApplicationStorageView application = new ApplicationStorageView("username1");

			try
			{
				Int32 result = application.TotalThreads;
			}
			catch
			{
				// exception expected
				return;
			}

			Assert.Fail("An exception was expected");
		}

		/// <summary>
		/// Try to get the UnfinishedThreads value when it was not initialized
		/// It should throw an exception.
		/// </summary>
		[Test]
		public void UnfinishedThreadsTestNotSet()
		{
			ApplicationStorageView application = new ApplicationStorageView("username1");

			try
			{
				Int32 result = application.UnfinishedThreads;
			}
			catch
			{
				// exception expected
				return;
			}

			Assert.Fail("An exception was expected");
		}
	
	}
}