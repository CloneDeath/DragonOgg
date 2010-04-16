// 
//  OggFile.cs
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
using System.IO;
using csvorbis;
using TagLib;
using OpenTK.Audio.OpenAL;

namespace DragonOgg
{
	

	/*
	 *	OggFile Class
	 *	Combines the csvorbis, System.IO and Taglib functionality into one class
	 *	Designed for use with OggPlayer or OggPlaylist
	 */
	/// <summary>
	/// Combines the csvorbis, System.IO and TagLib functionality into one class
	/// Use for editting tags, or in conjunction with OggPlayer for audio output or OggPlaylist for playlist reading/writing
	/// </summary>
	public class OggFile
	{
		
		private string m_Filename;			// Filename
		
		private VorbisFile m_CSVorbisFile; 	// CSVorbis file object
		private TagLib.File m_TagLibFile;	// TagLibSharp file object
		
		private int m_Streams;				// Number of Vorbis streams in the file
		private int m_Bitrate;				// ABR/NBR of the file
		private int m_LengthTime;			// Number of seconds in the file
		private Info[] m_Info;				// OggVorbis file info object					
		private ALFormat m_Format;			// Format of the file
		
		private const int _BIGENDIANREADMODE = 0;		// Big Endian config for read operation: 0=LSB;1=MSB
		private const int _WORDREADMODE = 1;			// Word config for read operation: 1=Byte;2=16-bit Short
		private const int _SGNEDREADMODE = 0;			// Signed/Unsigned indicator for read operation: 0=Unsigned;1=Signed
		private const int _SEGMENTLENGTH = 4096;		// Default number of segments to read if unspecified (Segment type is determined by _WORDREADMODE)
		
		/// <summary>
		/// The format of the current file in an ALFormat enumeration
		/// </summary>
		public ALFormat Format { get { return m_Format; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Filename">
		/// A <see cref="System.String"/> containing the path to the Ogg Vorbis file this instance represents
		/// </param>
		public OggFile (string Filename)
		{
			// Check that the file exists
			if (!(System.IO.File.Exists(Filename))) { throw new FileNotFoundException("Unable to load new OggFile", Filename); }
			// Load the relevant objects
			m_Filename = Filename;
			m_CSVorbisFile = new VorbisFile(m_Filename);
			m_TagLibFile = TagLib.File.Create(m_Filename);
			
			// Populate some other info shizzle and do a little bit of sanity checking
			m_Streams = m_CSVorbisFile.streams();
			if (m_Streams<=0) { throw new Exception("File doesn't contain any logical bitstreams"); }
			// Assuming <0 is for whole file and >=0 is for specific logical bitstreams
			m_Bitrate = m_CSVorbisFile.bitrate(-1);
			m_LengthTime = (int)m_CSVorbisFile.time_total(-1);
			// Figure out the ALFormat of the stream
			m_Info = m_CSVorbisFile.getInfo();	// Get the info of the first stream, assuming all streams are the same? Dunno if this is safe tbh
			if (m_Info[0] == null) { throw new Exception("Unable to determine Format{FileInfo.Channels} for first bitstream"); }
			if (m_TagLibFile.Properties.AudioBitrate==16) {
				m_Format = (m_Info[0].channels)==1 ? ALFormat.Mono16 : ALFormat.Stereo16; // This looks like a fudge, but I've seen it a couple of times (what about the other formats I wonder?)
			}
			else 
			{
				m_Format = (m_Info[0].channels)==1 ? ALFormat.Mono8 : ALFormat.Stereo8;
			}
		}		
		
		/// <summary>
		/// Retrieve the value of a Tag from the Ogg Vorbis file
		/// </summary>
		/// <param name="TagID">
		/// A <see cref="OggTags"/> indicating which tag to read
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the data in the tag
		/// </returns>
		public string GetTag(OggTags TagID)
		{
			switch (TagID)
			{
			case OggTags.Title: return m_TagLibFile.Tag.Title;
			case OggTags.Artist: return m_TagLibFile.Tag.FirstPerformer;
			case OggTags.Album: return m_TagLibFile.Tag.Album;
			case OggTags.Genre: return m_TagLibFile.Tag.FirstGenre;
			case OggTags.TrackNumber: return m_TagLibFile.Tag.Track.ToString();
			case OggTags.Filename: return m_Filename;
			case OggTags.HumanReadableBitrate: return OggUtilities.MakeHumanReadable(m_Bitrate, "bit/s");
			case OggTags.Length: return m_LengthTime.ToString();
			case OggTags.HumanReadableLength: return OggUtilities.MakeHumanReadableTime(m_LengthTime);
			default: return null;
			}
			
		}
		
		/// <summary>
		/// Set the value of a tag in the Ogg Vorbis file (THIS FUNCTION WRITES TO DISK)
		/// </summary>
		/// <param name="TagID">
		/// A <see cref="OggTags"/> indicating which tag to change
		/// </param>
		/// <param name="Value">
		/// A <see cref="System.String"/> containing the value to write
		/// </param>
		/// <returns>
		/// A <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn SetTag(OggTags TagID, string Value)
		{
			switch (TagID)
			{
			case OggTags.Title: m_TagLibFile.Tag.Title = Value; break;
			case OggTags.Artist: m_TagLibFile.Tag.Performers = new string[] { Value }; break;
			case OggTags.Album: m_TagLibFile.Tag.Album = Value; break;
			case OggTags.Genre: m_TagLibFile.Tag.Genres = new string[] { Value }; break;
			case OggTags.TrackNumber: m_TagLibFile.Tag.Track = uint.Parse(Value); break;
			case OggTags.Filename: return OggTagWriteCommandReturn.ReadOnlyTag;
			case OggTags.HumanReadableBitrate: return OggTagWriteCommandReturn.ReadOnlyTag;
			case OggTags.Length: return OggTagWriteCommandReturn.ReadOnlyTag;
			default: return OggTagWriteCommandReturn.UnknownTag;
			}
			try { m_TagLibFile.Save(); } catch (Exception ex) { return OggTagWriteCommandReturn.Error; }
			return OggTagWriteCommandReturn.Success;
		}
		
		/// <summary>
		/// Get the next segment of Ogg data decoded into PCM format
		/// </summary>
		/// <param name="SegmentLength">
		/// A <see cref="System.Int32"/> indicating the number of bytes of data to request. 
		/// Defaults to 4096 if set to 0. 
		/// Use the ReturnValue property of the result to discover how many bytes are actually returned
		/// </param>
		/// <returns>
		/// Am <see cref="OggBufferSegment"/> containing the returned data
		/// </returns>
		public OggBufferSegment GetBufferSegment(int SegmentLength)
		{
			if (SegmentLength<=0) { SegmentLength = _SEGMENTLENGTH; }	// If segment length is invalid, use default segment length
			OggBufferSegment retVal; // Declare the buffer segment structure
			retVal.BufferLength = SegmentLength;
			retVal.Buffer = new Byte[retVal.BufferLength];	// Init buffer
			retVal.ReturnValue = m_CSVorbisFile.read(retVal.Buffer, retVal.BufferLength, _BIGENDIANREADMODE, _WORDREADMODE, _SGNEDREADMODE, null);
			retVal.RateHz = m_TagLibFile.Properties.AudioSampleRate; //m_Info[0].rate;
			return retVal;
		}
		
		/// <summary>
		/// Reset the OggFile (reload from disk).
		/// Useful if tags have changed externally, or to reset the internal position pointer to replay the file from the beginning
		/// SeekToTime(0) is the prefered method of moving the internal pointer to the beginning however however
		/// </summary>		
		public void ResetFile()
		{
			try
			{
				m_CSVorbisFile = null;
				m_CSVorbisFile = new VorbisFile(m_Filename);	// No point reloading anything else 'cos it shouldn't have changed	
				m_TagLibFile = null;
				m_TagLibFile = TagLib.File.Create(m_Filename);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to reload OggFile [" + m_Filename + "]", ex);	
			}
		}
		
		/// <summary>
		/// Attempt to change the internal position pointer to a new value within the file
		/// </summary>
		/// <param name="Seconds">
		/// A <see cref="System.Single"/> indicating what time to seek to
		/// </param>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn SeekToTime(float Seconds)
		{
			if(!(m_CSVorbisFile.seekable())) { return OggPlayerCommandReturn.OperationNotValid; }
			if(Seconds>m_CSVorbisFile.time_total(-1)) { return OggPlayerCommandReturn.ValueOutOfRange; }
			if(Seconds<0) { return OggPlayerCommandReturn.ValueOutOfRange; }
			if(m_CSVorbisFile.time_seek(Seconds)!=0) { return OggPlayerCommandReturn.Error; }
			return OggPlayerCommandReturn.Success;
		}
		
		/// <summary>
		/// Return current position of the internal pointer in seconds
		/// </summary>
		/// <returns>
		/// A <see cref="System.Single"/> indicating the position of the internal pointer in seconds
		/// </returns>
		public float GetTime()
		{
			return m_CSVorbisFile.time_tell();
		}
		
	}

}
