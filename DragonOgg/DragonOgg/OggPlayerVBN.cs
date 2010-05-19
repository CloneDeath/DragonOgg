// 
//  OggPlayerVBN.cs
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
	/// OggPlayerVBN class (Variable Buffer Number)
	/// This class takes OggFile objects and outputs them in a threaded player
	/// using OpenAL (through the OpenTK wrapper)
	/// The VBN player has a variable number of buffers
	/// This will load the song into the player as quickly as possible.
	/// This is useful for networked files as the actual period of reading is very
	/// short relative to the length of the file. It is less suitable for situations
	/// where memory is short or where spikes in processor demand when a file is first started cannot be tolerated.
	/// </summary>
	public class OggPlayerVBN : OggPlayer
	{

		public OggPlayerVBN()
		{
			
		}
		
		~OggPlayerVBN()
		{
			
		}
		
		public override OggPlayerCommandReturn Playback_Play ()
		{
			throw new System.NotImplementedException();
		}
		
		public override OggPlayerCommandReturn Playback_Seek (float SeekTime)
		{
			throw new System.NotImplementedException();
		}
		
		public override OggPlayerCommandReturn Playback_Stop ()
		{
			throw new System.NotImplementedException();
		}
		
		public override OggPlayerCommandReturn Playback_UnPause ()
		{
			throw new System.NotImplementedException();
		}
		
				public override OggPlayerCommandReturn Playback_Pause ()
		{
			throw new System.NotImplementedException();
		}
		
		public override bool SetCurrentFile (OggFile File)
		{
			throw new System.NotImplementedException();
		}
		
		public override bool SetCurrentFile (string FileName)
		{
			throw new System.NotImplementedException();
		}
		
		public override void Dispose ()
		{
			throw new System.NotImplementedException();
		}
		
		
	}
}
