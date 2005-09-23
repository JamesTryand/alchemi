//
// Alchemi.Core.Manager.Storage.InMemoryManagerStorage.cs
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
using System.Collections;

using Alchemi.Core.Executor;

namespace Alchemi.Core.Manager.Storage
{
	/// <summary>
	/// Store all manager information in memory.
	/// 
	/// This type of storage is not persistent but usefull for testing or for running 
	/// lightweight managers.
	/// </summary>
	public class InMemoryManagerStorage : IManagerStorage
	{
		private ArrayList m_users;
		private ArrayList m_groups;

		public InMemoryManagerStorage()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region IManagerStorage Members

		public SystemSummary GetSystemSummary()
		{
			throw new Exception("Not implemented");
		}

		public void AddUsers(User[] users)
		{
			if (users == null)
			{
				return;
			}

			if (m_users == null)
			{
				m_users = new ArrayList();
			}

			m_users.AddRange(users);
		}

		public void UpdateUsers(User[] updates)
		{
			if (m_users == null || updates == null)
			{
				return;
			}

			for(int indexInList=0; indexInList<m_users.Count; indexInList++)
			{
				User userInList = (User)m_users[indexInList];

				foreach(User userInUpdates in updates)
				{
					if (userInList.Username == userInUpdates.Username)
					{
						userInList.Password = userInUpdates.Password;
						userInList.GroupId = userInUpdates.GroupId;
					}
				}
			}
		}

		public bool AuthenticateUser(SecurityCredentials sc)
		{
			if (sc == null || m_users == null)
			{
				return false;
			}

			for(int index=0; index<m_users.Count; index++)
			{
				User user = (User)m_users[index];

				if (user.Username == sc.Username && user.Password == sc.Password)
				{
					return true;
				}
			}

			return false;
		}

		public User[] GetUserList()
		{
			if (m_users == null)
			{
				return new User[0];
			}
			else
			{
				return (User[])m_users.ToArray(typeof(User));
			}
		}

		public void AddGroups(Group[] groups)
		{
			if (groups == null)
			{
				return;
			}

			if (m_groups == null)
			{
				m_groups = new ArrayList();
			}

			m_groups.AddRange(groups);
		}
		
		public Group[] GetGroups()
		{
			if (m_groups == null)
			{
				return new Group[0];
			}
			else
			{
				return (Group[])m_groups.ToArray(typeof(Group));
			}
		}

		public String AddExecutor(ExecutorInfo executor)
		{
			throw new Exception("Not implemented");
		}

		public void UpdateExecutor(ExecutorInfo executor)
		{
			throw new Exception("Not implemented");
		}

		public ExecutorInfo[] GetExecutors()
		{
			throw new Exception("Not implemented");
		}


		#endregion
	}
}
