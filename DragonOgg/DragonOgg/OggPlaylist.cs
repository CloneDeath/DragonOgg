// 
//  OggPlaylist.cs
//  
//  Author:
//       dragon@the-dragons-nest.co.uk
// 
//  Copyright (c) 2010 Matthew Harris
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace DragonOgg
{

	/// <summary>
	/// Class to handle the playing and in-memory storage of playlists
	/// </summary>
	public class OggPlaylist
	{

		public OggPlaylist ()
		{
		}
	}
	
	/// <summary>
	/// Contains an OggFile and additional information pertaining to it's position within a playlist
	/// </summary>
	public class OggPlaylistFile : IComparable
	{
		private OggFile m_File;	// The ogg file itself
		private bool m_Played;	// Whether the file has been played
		private int m_OrderNum;	// The order of the file within the playlist
		
		public OggFile File { get { return m_File; } set { m_File = value; } }
		public bool Played { get { return m_Played; } set { m_Played = value; } }
		public int OrderNum { get { return m_OrderNum; } set { m_OrderNum = value; } }
		
		/// <summary>
		/// Constructor for the OggPlaylistFile object
		/// </summary>
		/// <param name="f">
		/// An <see cref="OggFile"/> containing the file associated with this OggPlaylistFile
		/// </param>
		/// <param name="Order">
		/// A <see cref="System.Int32"/> containg a value indicating it's order
		/// </param>
		OggPlaylistFile(OggFile f, int Order)
		{
			m_File = f; m_OrderNum = Order; m_Played = false;
		}
		
		/// <summary>
		/// Implementation of IComparable.CompareTo interface
		/// </summary>
		int IComparable.CompareTo (object obj)
		{
			if (typeof(object)!=typeof(OggPlaylistFile)) { throw new System.InvalidCastException("OggPlaylistFile:CompareTo obj not an OggPlaylistFile"); }
			OggPlaylistFile tmp = (OggPlaylistFile) obj;
			if (tmp.OrderNum>this.OrderNum) { return 1; }
			if (tmp.OrderNum<this.OrderNum) { return -1; }
			return 0;
		}
		

	}
}
