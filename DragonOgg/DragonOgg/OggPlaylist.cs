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
using System.Collections;
using System.IO;

namespace DragonOgg
{

	/// <summary>
	/// Class to handle the playing and in-memory storage of playlists
	/// </summary>
	public class OggPlaylist : IEnumerable
	{
		
		private ArrayList m_FileHeap;		// Array of files in the playlist
		private bool m_Repeat;				// Whether the playlist should repeat
		private bool m_Random;				// Whether the playlist should use a random order for playing
		private int m_Position;			// Position of the playlist within the array
		private bool m_AutoOrder;			// Flag indicating whether the playlist automatically orders items on Add/Remove
		private Random m_RandomGenerator;	// Random number generator
		private OggPlayer m_Player;			// Player object for output
		private OggFile m_CurrentFile;		// Currently playing file
		private OggPlayerStatus m_PlaylistState;	// Player status enumeration indicating the state of the playlist
		
		/// <summary>
		/// Flag indicating whether the playlist will loop when it comes to the end
		/// Allows playing of already played files if RandomOrder is true
		/// </summary>
		public bool Repeat { get { return m_Repeat; } set { m_Repeat = value; } }
		/// <summary>
		/// Flag indicating whether the playlist will play files in the order dictated by OrderNum
		/// </summary>
		public bool RandomOrder { get { return m_Random; } set { m_Random = value; } }
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator) GetEnumerator();
		}
		
		public OggPlaylistEnumerator GetEnumerator()
		{
			return new OggPlaylistEnumerator(m_FileHeap);
		}
				
		public OggPlaylist ()
		{
			
			
		}
	}
	
	/// <summary>
	/// Enumerator class for the OggPlaylist
	/// </summary>
	public class OggPlaylistEnumerator : IEnumerator
	{
		
		private ArrayList m_Heap;
		private int m_Position;
		
		public bool MoveNext()
		{
			m_Position++;
			return (m_Position < m_Heap.Count);
		}
		
		public void Reset()
		{
			m_Position = -1;
		}
		
		object IEnumerator.Current { get { return Current; } }
		
		public OggFile Current { get { try { return (OggFile) m_Heap[m_Position]; } catch (IndexOutOfRangeException ex) { throw new InvalidOperationException(); } } }
		
		public OggPlaylistEnumerator(ArrayList Heap)
		{
			m_Position = -1;
			m_Heap = Heap;
		}
		
	}
	
	/// <summary>
	/// Contains an OggFile and additional information pertaining to it's position within a playlist
	/// </summary>
	public class OggPlaylistFile : IComparable
	{
		private OggFile m_File;			// The ogg file itself
		private bool m_Played;			// Whether the file has been played
		private int m_OrderNum;		// The order of the file within the playlist
		private string m_FileString;	// Full path to the file
		private bool m_Cached;			// Whether the file has been loaded
		
		/// <summary>
		/// The Ogg file.
		/// Note: Use the CacheFile & UnCacheFile operations to create/destroy this object to maintain internal consistency
		/// Setting OggPlaylistFile.File = null will fail silently (i.e. won't clear anything!)
		/// </summary>
		public OggFile File { get { return m_File; } set { if(value==null) { return; } m_File = value; } }
		public string Filename { get { return m_FileString; } }
		/// <summary>
		/// Flag indicating whether this file has been played since the last reset of playing flags
		/// </summary>
		public bool Played { get { return m_Played; } set { m_Played = value; } }
		/// <summary>
		/// Number indicating the order position of the file
		/// </summary>
		public int OrderNum { get { return m_OrderNum; } set { m_OrderNum = value; } }
		/// <summary>
		/// Flag indicating whether the OggFile has been opened & loaded.
		/// </summary>
		public bool Cached { get { return m_Cached; } }
		
		/// <summary>
		/// Constructor for the OggPlaylistFile object (cached)
		/// </summary>
		/// <param name="f">
		/// An <see cref="OggFile"/> containing the file associated with this OggPlaylistFile
		/// </param>
		/// <param name="Order">
		/// A <see cref="System.Int32"/> containing a value indicating it's order
		/// </param>
		OggPlaylistFile(OggFile f, int Order)
		{
			m_File = f; 
			m_OrderNum = Order; 
			m_Played = false; 
			m_Cached = true; 
			m_FileString = m_File.GetQuickTag(OggTags.Filename);
		}
		
		/// <summary>
		/// Constructor for the OggPlaylistFile object (non-cached)
		/// </summary>
		/// <param name="Path">
		/// A <see cref="System.String"/> containing the path to the file
		/// </param>
		/// <param name="Order">
		/// A <see cref="System.Int32"/> containg a value indicating it's order
		/// </param>
		OggPlaylistFile(string Path, int Order)
		{
			m_File = null;
			m_OrderNum = Order;
			m_Played = false;
			m_Cached = false;
			m_FileString = Path;
		}
		
		/// <summary>
		/// Deconstructor
		/// </summary>
		~OggPlaylistFile()
		{
			m_File = null;	
		}
		
		/// <summary>
		/// Cache the file. This creates an OggFile object for the file specified by OggPlaylistFile.FileString
		/// This will return false if the filepath is invalid or if any other error occurs during creation.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating whether the operation succeeded.
		/// </returns>
		public bool CacheFile()
		{
			if (!(System.IO.File.Exists(m_FileString))) { return false; }
			if (m_Cached) { UnCacheFile(); }
			try 
			{
				m_File = new OggFile(m_FileString);
				m_Cached = true;
			}
			catch (Exception ex)
			{
				#if (DEBUG)
				Console.WriteLine(DateTime.Now.ToString() + ": OggPlaylistFile.CacheFile: " + ex.Message);
				#endif
				return false;	
			}
			return true;
		}
		
		/// <summary>
		/// Uncache the file. Use this instead of OggPlaylistFile.File = null as it sets some internal flags
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating whether the operation succeeded.
		/// </returns>
		public bool UnCacheFile()
		{
			if (!(m_Cached)) { return true; }
			try
			{
				m_File = null;
				m_Cached = false;
			}
			catch (Exception ex)
			{
				#if (DEBUG)
				Console.WriteLine(DateTime.Now.ToString() + ": OggPlaylistFile.UnCacheFile: " + ex.Message);
				#endif
				return false;
			}
			return true;
			
		}
		
		/// <summary>
		/// Implementation of IComparable.CompareTo interface
		/// </summary>
		int IComparable.CompareTo(object obj)
		{
			if (typeof(object)!=typeof(OggPlaylistFile)) { throw new System.InvalidCastException("OggPlaylistFile:CompareTo obj not an OggPlaylistFile"); }
			OggPlaylistFile tmp = (OggPlaylistFile) obj;
			if (tmp.OrderNum>this.OrderNum) { return 1; }
			if (tmp.OrderNum<this.OrderNum) { return -1; }
			return 0;
		}
		

	}
}
