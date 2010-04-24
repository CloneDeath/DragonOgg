//
//  OggPlayer.cs
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
using System.Threading;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace DragonOgg
{
	
	/// <summary>
	/// OggPlayer class
	/// This class takes OggFile objects and outputs them in a threaded player
	/// using OpenAL (through the OpenTK wrapper)
	/// </summary>
	public class OggPlayer
	{

		private OggPlayerStatus m_PlayerState;			// Current state flag
		private OggFile m_CurrentFile;					// Currently active file
		private float m_TimeOffset;					// Current time within the file
		private int m_UpdateDelay;						// Time to wait at the end of each buffer loop
		
		// Yummy events
		public event EventHandler StateChanged;
		public event EventHandler BufferUnderrun;
		public event EventHandler PlaybackStarted;
		public event EventHandler PlaybackFinished;
		
		// SourceControl object for thread safety
		static readonly object SourceControl = new object();
	
		// OpenAL stuff
		private AudioContext m_Context;
		private uint m_Source;
		private ALError m_LastError;
		private uint[] m_Buffers;
		private int m_BufferCount;
		private int m_BufferSize;
				
		// Property exposure
		public OggPlayerStatus PlayerState { get { return m_PlayerState; } }
		public OggFile CurrentFile { get { return m_CurrentFile; } }
		public ALError LastALError { get { return m_LastError; } }
		public float TimeCurrent { get { return m_TimeOffset; } }
		public float TimeMax { get { return float.Parse(m_CurrentFile.GetQuickTag(OggTags.Length)); } }
		public int UpdateDelay { get { return m_UpdateDelay; } set { m_UpdateDelay = value; } }
		public int BufferSize { get { return m_BufferSize; } }
		public int BufferCount { get { return m_BufferCount; } }
				
		/// <summary>
		/// Constructor
		/// </summary>
		public OggPlayer ()
		{
			m_PlayerState = OggPlayerStatus.Waiting;
			
			m_UpdateDelay = 10;
			m_Context = new AudioContext();			// Initialise the AudioContext
			m_BufferCount = 32;
			m_BufferSize = 4096;
			m_Buffers = new uint[m_BufferCount];				// We're using four buffers so we always have a supply of data
		}
		
		/// <summary>
		/// Function for configuring buffer settings
		/// </summary>
		/// <param name="NumberOfBuffers">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="BufferSize">
		/// A <see cref="System.Int32"/>
		/// </param>
		public OggPlayerCommandReturn SetBufferInfo(int NumberOfBuffers, int BufferSize)
		{
			if (!((m_PlayerState==OggPlayerStatus.Stopped)||(m_PlayerState==OggPlayerStatus.Waiting))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			m_BufferCount = NumberOfBuffers;
			m_BufferSize = BufferSize;
			m_Buffers = new uint[m_BufferCount];
			return OggPlayerCommandReturn.Success;
		}
		
		
		/// <summary>
		/// Destructor
		/// </summary>
		~OggPlayer()
		{
			// Tidy up the OpenAL stuff
			AL.DeleteBuffers(m_Buffers);
			AL.DeleteSource(ref m_Source);
			m_Context.Dispose();	
		}
		
		// State change handler
		private void SetState(OggPlayerStatus State)
		{
			m_PlayerState = State;
			if (StateChanged!=null) { StateChanged(this, new EventArgs()); }
		}
		
		/// <summary>
		/// Set the current file. Only valid when the player is stopped or no file has been set
		/// </summary>
		/// <param name="NewFile">
		/// An <see cref="OggFile"/> object containg the file to set
		/// </param>
		public void SetCurrentFile(OggFile NewFile)
		{
			// Check current state
			if (!((m_PlayerState==OggPlayerStatus.Stopped)||(m_PlayerState==OggPlayerStatus.Waiting))) {
				throw new Exception("Unable to change file while player is running");
			}
			m_CurrentFile = NewFile;
			SetState(OggPlayerStatus.Stopped);
		}
		
		/// <summary>
		/// Set the current file. Only valid when the player is stopped or no file has been set
		/// </summary>
		/// <param name="NewFilename">
		/// A <see cref="System.String"/> containing the path to the file to set
		/// </param>
		public void SetCurrentFile(string NewFilename)
		{
			SetCurrentFile(new OggFile(NewFilename));	
		}
		
		/// <summary>
		/// Start playing the current file
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn Playback_Play() 
		{
			// We can only play if we're stopped (this should also stop us trying to play invalid files as we'll then be 'Waiting' or 'Error' rather than stopped)
			if (m_PlayerState == OggPlayerStatus.Stopped)
			{	
				// Create source
				AL.GenSource(out m_Source);
				// Begin buffering
				SetState(OggPlayerStatus.Buffering);
				// Configure the source listener
				AL.Source(m_Source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
				AL.Source(m_Source, ALSource3f.Velocity, 0.0f, 0.0f, 0.0f);
				AL.Source(m_Source, ALSource3f.Direction, 0.0f, 0.0f, 0.0f);
				AL.Source(m_Source, ALSourcef.RolloffFactor, 0.0f);
				AL.Source(m_Source, ALSourceb.SourceRelative, true);
				// Populate buffers
				for (int i=0;i<m_Buffers.Length;i++)
				{
					OggBufferSegment obs = m_CurrentFile.GetBufferSegment(0);
					if (obs.ReturnValue>0)
					{
						// Create a buffer
						AL.GenBuffer(out m_Buffers[i]);
						// Fill this buffer
						AL.BufferData((int)m_Buffers[i], m_CurrentFile.Format, obs.Buffer, obs.ReturnValue, obs.RateHz);
					} 
					else 
					{
						throw new Exception("Read error or EOF within initial buffer segment");
					}
				}
				// We've filled four buffers with data, give 'em to the source
				AL.SourceQueueBuffers(m_Source, m_Buffers.Length, m_Buffers);
				// Start playback
				AL.SourcePlay(m_Source);
				// Spawn a new player thread
				SetState(OggPlayerStatus.Playing);
				new Thread(new ThreadStart(Player_Thread)).Start();
				return OggPlayerCommandReturn.Success; 
			}
			// If we're paused we'll be nice to the user and automatically call the Playback_UnPause function, which they should have done in the first place
			else if (m_PlayerState == OggPlayerStatus.Paused)
			{
				Playback_UnPause();
				return OggPlayerCommandReturn.Success;
			}
			else if (m_PlayerState == OggPlayerStatus.Waiting)
			{
				return OggPlayerCommandReturn.NoFile;
			}
			else
			{
				return OggPlayerCommandReturn.InvalidCommandInThisPlayerState;
			}
		}
		
		// Player thread
		private void Player_Thread()
		{
			bool Running = true; bool ReachedEOF = false;
			if (PlaybackStarted!=null) { PlaybackStarted(this, new EventArgs()); }
			while (Running)
			{
				// See what we're doing
				if (m_PlayerState==OggPlayerStatus.Playing)
				{
					lock (SourceControl)
					{
						if (ReachedEOF)
						{
							// We've come to the end of the file, just see if there are any buffers left in the queue
							int QueuedBuffers = 0;
							AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out QueuedBuffers);
							if (QueuedBuffers>0) 
							{
								// We want to remove the buffers, so carry on to the usual playing section
							}
							else
							{
								// End of file & all buffers played, exit.
								Running = false;
								SetState(OggPlayerStatus.Stopped);
								if (AL.GetSourceState(m_Source)!=ALSourceState.Stopped) { AL.SourceStop(m_Source); }
								if (PlaybackFinished!=null) { PlaybackFinished(this, new EventArgs()); }
								return;
							}
						}
						int ProcessedBuffers = 0; uint BufferRef=0;
						AL.GetSource(m_Source, ALGetSourcei.BuffersProcessed, out ProcessedBuffers);
						bool UnderRun = false;
						// Check for buffer underrun
						if (ProcessedBuffers>=m_BufferCount)
						{
							UnderRun = true;
							if (BufferUnderrun!=null) { BufferUnderrun(this, new EventArgs()); }
						}
						while (ProcessedBuffers>0)
						{
							// For each buffer thats been processed, reload and queue a new one
							AL.SourceUnqueueBuffers(m_Source, 1, ref BufferRef);
							if (ReachedEOF) { --ProcessedBuffers; continue; }	// If we're at the EOF loop to the next buffer here - we don't want to be trying to fill any more
							OggBufferSegment obs = m_CurrentFile.GetBufferSegment(m_BufferSize);	// Get chunk of tasty buffer data with the configured segment
							// Check the buffer segment for errors
							if (obs.ReturnValue>0)
							{
								// No error, queue data
								AL.BufferData((int)BufferRef, m_CurrentFile.Format, obs.Buffer, obs.ReturnValue, obs.RateHz);
								AL.SourceQueueBuffers(m_Source, 1, ref BufferRef);
							}
							else
							{
								if (obs.ReturnValue==0)
								{
									// End of file
									ReachedEOF = true;
									break;
								}
								else
								{
									// Something went wrong with the read
									m_PlayerState = OggPlayerStatus.Error;
									AL.SourceStop(m_Source);
									Running = false;
									if (PlaybackFinished!=null) { PlaybackFinished(this, new EventArgs()); }
									break;
								}
							}
							// Check for errors
							m_LastError = AL.GetError();
							if (m_LastError!= ALError.NoError)
							{
								SetState(OggPlayerStatus.Error);
								AL.SourceStop(m_Source);
								Running = false;
								break;
							}
							
							--ProcessedBuffers;
						}
						// If we under-ran, restart the player
						if (UnderRun) { AL.SourcePlay(m_Source); }
					}
					m_TimeOffset = m_CurrentFile.GetTime();
				}
				else if (m_PlayerState==OggPlayerStatus.Seeking)
				{
					// Wait a short time then loop round again and see if we're still seeking
					Thread.Sleep(100);
				}
				else if (m_PlayerState==OggPlayerStatus.Paused)
				{
					// Wait a short time then loop round again and see if we're still paused
					Thread.Sleep(100);
				}
				else 
				{
					// Some other state, abort the playback 'cos we shouldn't
					// be in the Player_Thread in this case
					Running = false;
				}
				// Allow other shizzle to execute
				Thread.Sleep(m_UpdateDelay);
			}
		}
		
		// Convert a source state to a string
		private string SSTS(ALSourceState SS)
		{
			switch (SS)
			{
			case ALSourceState.Initial : return "Initial";
			case ALSourceState.Paused : return "Paused";
			case ALSourceState.Playing : return "Playing";
			case ALSourceState.Stopped : return "Stopped";
			default: return "Unknown";
			}
		}
		
		/// <summary>
		/// Stop playback. 
		/// Only valid if the player is playing or paused
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn Playback_Stop()
		{
			if (!((m_PlayerState == OggPlayerStatus.Paused)||(m_PlayerState == OggPlayerStatus.Playing))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			// Stop the source and set the state to stop
			lock (SourceControl)
			{
				// Stop playing
				AL.SourceStop(m_Source);
				// See how many buffers are queued, and unqueue them
				int nBuffers;
				AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out nBuffers); 
				if (nBuffers>0) { AL.SourceUnqueueBuffers((int)m_Source,nBuffers); }
				// Reset the file object's internal location etc.
				m_CurrentFile.ResetFile();
				// Set the new state
				SetState(OggPlayerStatus.Stopped);
			}
			return OggPlayerCommandReturn.Success;
		}
		
		/// <summary>
		/// Pause playback
		/// Only valid if the player is playing
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn Playback_Pause()
		{
			if (!(m_PlayerState == OggPlayerStatus.Playing)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			lock (SourceControl)
			{
				AL.SourcePause(m_Source);
				SetState(OggPlayerStatus.Paused);
			}
			return OggPlayerCommandReturn.Success;
			
		}
		
		/// <summary>
		/// Unpause playback
		/// Only valid if the player is paused
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn Playback_UnPause()
		{
			if (!(m_PlayerState == OggPlayerStatus.Paused)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			lock (SourceControl)
			{
				AL.SourcePlay(m_Source);
				SetState(OggPlayerStatus.Playing);
			}
			return OggPlayerCommandReturn.Success;
		}
		
		/// <summary>
		/// Seek to a time
		/// Only valid if the player is playing or paused
		/// </summary>
		/// <param name="RequestedTime">
		/// A <see cref="System.Single"/> indicating the position in seconds within the file to seek to
		/// </param>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn Playback_Seek(float RequestedTime)
		{
			if (!((m_PlayerState == OggPlayerStatus.Playing)||(m_PlayerState == OggPlayerStatus.Playing))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			OggPlayerCommandReturn retVal = OggPlayerCommandReturn.Error;
			lock (SourceControl)
			{
				SetState(OggPlayerStatus.Seeking);
				AL.SourcePause(m_Source);
				retVal = m_CurrentFile.SeekToTime(RequestedTime);
				AL.SourcePlay(m_Source);
				SetState(OggPlayerStatus.Playing);
			}
			return retVal;
		}
	}
	
}
