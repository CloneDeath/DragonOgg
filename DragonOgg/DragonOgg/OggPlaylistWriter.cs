// 
//  OggPlaylistWriter.cs
//  
//  Author:
//       El Dragon <thedragon@the-dragons-nest.co.uk>
//  
//  Copyright (c) 2010 Matthew Harris
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace DragonOgg
{

	/// <summary>
	/// Interface for OggPlaylistWriters for various playlist file formats
	/// </summary>
	public interface OggPlaylistWriter
	{
		bool WriteFile(string Filename);
		OggPlaylist ReadFile(string Filename);
	}
	
	/// <summary>
	/// PlaylistWriter class for .M3U files
	/// </summary>
	public class OggPlaylistWriter_M3U : OggPlaylistWriter
	{
		public bool WriteFile (string Filename)
		{
			throw new System.NotImplementedException();
		}
		
		
		public OggPlaylist ReadFile (string Filename)
		{
			throw new System.NotImplementedException();
		}	
	}
	
	/// <summary>
	/// PlaylistWriter class for .PLS files
	/// </summary>
	public class OggPlaylistWriter_PLS : OggPlaylistWriter
	{
		public bool WriteFile (string Filename)
		{
			throw new System.NotImplementedException();
		}
		
		
		public OggPlaylist ReadFile (string Filename)
		{
			throw new System.NotImplementedException();
		}	
	}
}
