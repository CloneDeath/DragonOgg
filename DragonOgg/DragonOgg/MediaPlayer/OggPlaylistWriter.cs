using System;

namespace DragonOgg.MediaPlayer
{

	/// <summary>
	/// Interface for OggPlaylistWriters for various playlist file formats
	/// </summary>
	public static class OggPlaylistWriter
	{
		public static bool WriteFile(string Filename, OggPlaylistFormat Format, OggPlaylist Playlist)
		{
			throw new NotImplementedException();
		}
		
		public static OggPlaylist ReadFile(string Filename, OggPlaylistFormat Format)
		{
			throw new NotImplementedException();
		}
		
		#region "PLS"
		private static bool WriteFilePLS(string Filename, OggPlaylist Playlist)
		{
			throw new NotImplementedException();
		}
		private static OggPlaylist ReadFilePLS(string Filename)
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region "M3U"
		private static bool WriteFileM3U(string Filename, OggPlaylist Playlist)
		{
			throw new NotImplementedException();
		}
		private static OggPlaylist ReadFileM3U(string Filename)
		{
			throw new NotImplementedException();	
		}
		#endregion
	}
	
}
