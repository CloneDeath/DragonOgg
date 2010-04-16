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
	/*
	 * OggPlaylist class
	 * This object contains a collection of OggFile objects for use with an OggPlayer
	 * OggPlayer objects that are playing a playlist have additional capabilities related
	 * to track changing/seeking
	 * The OggPlaylist can also read and write M3U and PLS files
	 */

	public class OggPlaylist
	{

		public OggPlaylist ()
		{
		}
	}
	
	/*
	 * OggPlaylistFile class
	 * This object contains additional details about an OggFile for use within the OggPlaylist object
	 */
	public class OggPlaylistFile : IComparable
	{
		public OggFile File;	// The ogg file itself
		public bool Played;	// Whether the file has been played
		public int OrderNum;	// The order of the file within the playlist
		
		OggPlaylistFile(OggFile f, int Order)
		{
			File = f; OrderNum = Order;
		}
		
		// Compare the OrderNum
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
