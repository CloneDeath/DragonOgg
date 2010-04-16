//  
//  MiscStuff.cs
//  
//  Author:
//       matt <${AuthorEmail}>
// 
//  Copyright (c) 2010 matt
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace DragonOgg
{


	/// <summary>
	/// Enumeration of valid tags for reading and editting tags
	/// </summary>
	public enum OggTags { Title=0, Artist, Album, Genre, TrackNumber, HumanReadableBitrate, Length, HumanReadableLength, Filename }
	
	/// <summary>
	/// Enumeration of the return values from the OggFile.SetTag method
	/// </summary>
	public enum OggTagWriteCommandReturn { Success=0, InvalidValue, UnknownTag, ReadOnlyTag, Error}
	
	/// <summary>
	/// Buffer structure for passing data between the OggFile class and the OggPlayer class
	/// </summary>
	/// <value name="Buffer">The buffer data</value>
	/// <value name="BufferLength">Number of bytes *requested* by the GetBufferSegment command</value>
	/// <value name="ReturnValue">Number of bytes actually returned if succesful. 0 if EOF, -1 if Error</value>
	/// <value name="RateHz">Audio Sample Rate in Hz</value>
	public struct OggBufferSegment
	{
		public byte[] Buffer;
		public int BufferLength;	// Number of bytes requested (maximum size of Buffer)
		public int ReturnValue;	// The return value of the read operation
		public int RateHz;			// The nominal bitrate of the buffered data
	}
	
	/// <summary>
	/// Enumeration of the current player state of the OggPlayer Class
	/// </summary>
	public enum OggPlayerStatus { Waiting=0,Error,Stopped,Playing,Paused,Seeking,Buffering }
	
	/// <summary>
	/// Enumeration of the return values from various operations in the OggPlayer class
	/// </summary>
	public enum OggPlayerCommandReturn { Success=0, Error, OperationNotValid, ParameterNotValid, ValueOutOfRange, NoFile, InvalidCommandInThisPlayerState }
	
	static public class OggUtilities
	{
		/// <summary>
		/// Convert a number to a Human-Readable string with magnitude prefix between the number and unit
		/// Equivalent to MakeHumanReadable(Number, Unit, 1024).
		/// This function will round the final value to 2DP
		/// </summary>
		/// <param name="Number">
		/// A <see cref="System.Double"/> containg the value to be converted
		/// </param>
		/// <param name="Unit">
		/// A <see cref="System.String"/> containing the unit to be appended to the string after the magnitude prefix
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the converted string
		/// </returns>
		static public string MakeHumanReadable(double Number, string Unit)
		{
			return MakeHumanReadable(Number, Unit, 1024);
		}
		
		/// <summary>
		/// Convert a number to a Human-Readable string with magnitude prefix between the number and unit
		/// This function will roundthe final value to 2DP
		/// </summary>
		/// <param name="Number">
		/// A <see cref="System.Double"/> containg the value to be converted
		/// </param>
		/// <param name="Unit">
		/// A <see cref="System.String"/> containing the unit to be appended to the string after the magnitude prefix
		/// </param>
		/// <param name="Offset">
		/// A <see cref="System.Int32"/> containing the magnitude at which to move to the next magnitude prefix (Common values: 1000, 1024)
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the converted string
		/// </returns>
		static public string MakeHumanReadable(double Number, string Unit, int Offset)
		{
			string UnitPrefix = "";
			if (Number>Offset) { Number /= Offset; UnitPrefix = "k"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; } 	// kilo
			if (Number>Offset) { Number /= Offset; UnitPrefix = "M"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Mega
			if (Number>Offset) { Number /= Offset; UnitPrefix = "G"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Giga
			if (Number>Offset) { Number /= Offset; UnitPrefix = "T"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Tera
			if (Number>Offset) { Number /= Offset; UnitPrefix = "P"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Peta
			if (Number>Offset) { Number /= Offset; UnitPrefix = "E"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Exa
			if (Number>Offset) { Number /= Offset; UnitPrefix = "Z"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Zetta
			if (Number>Offset) { Number /= Offset; UnitPrefix = "Y"; } else { return Math.Round(Number, 2).ToString() + UnitPrefix + Unit; }	// Yotta
			return Math.Round(Number, 2).ToString() + UnitPrefix + Unit;
			
		}
		
		/// <summary>
		/// Convert a quantity of seconds to a string in HH:MM:SS format
		/// Will return the smallest number of segments (e.g. MM:SS if less than an hour or just SS if less than a minute)
		/// </summary>
		/// <param name="Number">
		/// A <see cref="System.Int32"/> containing the number of seconds
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the processed time
		/// </returns>
		static public string MakeHumanReadableTime(int Number)
		{
			int Hours = 0; int Minutes = 0; int Seconds = 0;
			// Do some fancy maffs ;)
			if (Number>(60*60)) { Hours = (Number - (Number % (60*60))) / (60*60); Number %= (60*60); }
			if (Number>60) { Minutes = (Number - (Number % 60)) / 60; Number %= 60; }
			Seconds = Number;
			string retVal;
			retVal = Seconds.ToString(); if ((Seconds<10)&&(Minutes>0)) { retVal = "0" + retVal; }
			if (Minutes>0) { retVal = Minutes.ToString() + ":" + retVal; if ((Minutes<10)&&(Hours>0)) { retVal = "0" + retVal; } }
			if (Hours>0) { if (Minutes<=0) { retVal = "00:" + retVal; } retVal = Hours.ToString() + ":" + retVal; }
			return retVal;
		}
		
		/// <summary>
		/// Converts an enumeration into a description string for display to a user
		/// </summary>
		/// <param name="opStatus">
		/// An <see cref="OggPlayerStatus"/> enumeration to interpret
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the description
		/// </returns>
		static public string GetEnumString(OggPlayerStatus opStatus)
		{
			switch (opStatus)
			{
			case OggPlayerStatus.Buffering: return "Buffering";
			case OggPlayerStatus.Error: return "Player Error";
			case OggPlayerStatus.Paused: return "Paused";
			case OggPlayerStatus.Playing: return "Playing";
			case OggPlayerStatus.Seeking: return "Seeking";
			case OggPlayerStatus.Stopped: return "Stopped";
			case OggPlayerStatus.Waiting: return "Waiting for file";
			default: return "Unknown Player Status Value";
			}
		}
		
		/// <summary>
		/// Converts an enumeration into a description string for display to a user
		/// </summary>
		/// <param name="opCommandReturn">
		/// An <see cref="OggPlayerCommandReturn"/> enumeration to interpret
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the description
		/// </returns>
		static public string GetEnumString(OggPlayerCommandReturn opCommandReturn)
		{
			switch (opCommandReturn)
			{
			case OggPlayerCommandReturn.Error: return "Error Executing Command";
			case OggPlayerCommandReturn.InvalidCommandInThisPlayerState: return "Command not valid in this player state";
			case OggPlayerCommandReturn.NoFile: return "No file assigned to the player";
			case OggPlayerCommandReturn.OperationNotValid: return "This operation is not valid";
			case OggPlayerCommandReturn.ParameterNotValid: return "A parameter was not valid";
			case OggPlayerCommandReturn.Success: return "Command executed successfully";
			case OggPlayerCommandReturn.ValueOutOfRange: return "A value was outside the valid range";
			default: return "Unknown Player Command Return Value";
			}
		}
		
		/// <summary>
		/// Converts an enumeration into a description string for display to a user
		/// </summary>
		/// <param name="oTag">
		/// An <see cref="OggTags"/> enumeration to interpret
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the description
		/// </returns>
		static public string GetEnumString(OggTags oTag)
		{
			switch (oTag)
			{
			case OggTags.Album: return "Album";
			case OggTags.Artist: return "Artist";
			case OggTags.Filename: return "Filename";
			case OggTags.Genre: return "Genre";
			case OggTags.HumanReadableBitrate: return "Bitrate";
			case OggTags.HumanReadableLength: return "Length";
			case OggTags.Length: return "Length in seconds";
			case OggTags.Title: return "Title";
			case OggTags.TrackNumber: return "Track Number";
			default: return "Unknown Tag Identity Value";
			}
		}
		
		/// <summary>
		/// Converts an enumeration into a description string for display to a user
		/// </summary>
		/// <param name="oTagWriteCommandReturn">
		/// An <see cref="OggTagWriteCommandReturn"/> enumeration to interpret
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the description
		/// </returns>
		static public string GetEnumString(OggTagWriteCommandReturn oTagWriteCommandReturn)
		{
			switch (oTagWriteCommandReturn)
			{
			case OggTagWriteCommandReturn.Error: return "Error Writing Tag";
			case OggTagWriteCommandReturn.InvalidValue: return "Value is invalid for this tag";
			case OggTagWriteCommandReturn.ReadOnlyTag: return "Tag is read-only";
			case OggTagWriteCommandReturn.Success: return "Tag written successfully";
			case OggTagWriteCommandReturn.UnknownTag: return "Tag ID not recognised";
			default: return "Unknown Tag Write Command Return Value";
			}
		}
	}

}
