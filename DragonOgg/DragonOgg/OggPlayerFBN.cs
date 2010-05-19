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
	/// OggPlayerFBN class (Fixed Buffer Number)
	/// This class takes OggFile objects and outputs them in a threaded player
	/// using OpenAL (through the OpenTK wrapper)
	/// The FBN player has a fixed number of buffers - Use SetBufferInfo to configure them
	/// This is useful for background playing where memory is a significant issue.
	/// </summary>
	public class OggPlayerFBN : OggPlayer
	{

		private int m_UpdateDelay;						// Time to wait at the end of each buffer loop
		
		// Yummy events
		public event EventHandler StateChanged;
		public event EventHandler BufferUnderrun;
		public event EventHandler PlaybackStarted;
		public event EventHandler PlaybackFinished;
		public event EventHandler PlaybackTick;
	
		// OpenAL stuff
		private AudioContext m_Context;
		private uint m_Source;
		private ALError m_LastError;
		private uint[] m_Buffers;
		private int m_BufferCount;
		private int m_BufferSize;
				
		// Property exposure
		/// <summary>
		/// Current state of the player as an OggPlayerStatus enumeration. 
		/// Use OggUtilities.GetEnumString to convert into human-readable information
		/// </summary>
		public OggPlayerStatus PlayerState { get { return m_PlayerState; } }
		/// <summary>
		/// OggFile object representing the file currently loaded into the player
		/// </summary>
		public OggFile CurrentFile { get { return m_CurrentFile; } }
		/// <summary>
		/// The last error from the OpenAL subsystem as an ALError enumeration. 
		/// Use OggUtilities.GetEnumString to convert into human readable information
		/// </summary>
		public ALError LastALError { get { return m_LastError; } }
		/// <summary>
		/// The position in seconds of the current time being read from the file.
		/// The actual playing time may differ slightly, especially with large buffers
		/// </summary>
		public float TimeCurrent { get { return m_PlayingOffset; } }
		/// <summary>
		/// The length of the current file in seconds.
		/// </summary>
		public float TimeMax { get { return float.Parse(m_CurrentFile.GetQuickTag(OggTags.Length)); } }
		/// <summary>
		/// The amount of time to wait after each buffering pass. 
		/// Use on high-performance systems to reduce processor load by increasing the time between buffering passes. 
		/// WARNING: VERY LIKELY TO CAUSE STUTTERING: USE ONLY IF REALLY NEEDED
		/// The default is 10ms. Set lower if you are getting buffer under-runs and cannot increase the buffer count/size.
		/// </summary>
		public int UpdateDelay { get { return m_UpdateDelay; } set { m_UpdateDelay = value; } }
		/// <summary>
		/// The current size of each buffer block
		/// Use SetBufferInfo to change this value
		/// </summary>
		public int BufferSize { get { return m_BufferSize; } }
		/// <summary>
		/// The current number of buffer blocks
		/// Use SetBufferInfo to change this value
		/// </summary>
		public int BufferCount { get { return m_BufferCount; } }
		
		/// <summary>
		/// How much of the file has elapsed. Returns a float between 0 & 1
		/// </summary>
		public float FractionElapsed { get { float FE = m_PlayingOffset/float.Parse(m_CurrentFile.GetQuickTag(OggTags.Length)); if (FE>1) { return 1; } else if (FE<0) { return 0; } else { return FE; } } }
		
		/// <summary>
		/// Whether a tick event should be raised every TickInterval seconds of played audio
		/// </summary>
		public bool TickEnabled { get { return m_TickEnabled; } set { m_TickEnabled = value; } }
		
		/// <summary>
		/// The interval at which tick events should be raised if TickEnabled is true
		/// </summary>
		public float TickInterval { get { return m_TickInterval; } set { m_TickInterval = value; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		public OggPlayerFBN()
		{
			m_PlayerState = OggPlayerStatus.Waiting;
			
			m_UpdateDelay = 10;
			m_Context = new AudioContext();			// Initialise the AudioContext
			m_BufferCount = 32;
			m_BufferSize = 4096;
			m_Buffers = new uint[m_BufferCount];				// We're using four buffers so we always have a supply of data
			
			m_TickInterval = 1;			// Default tick is every second
			m_TickEnabled = false;		// Tick event is disabled by default
			
			// Create source
			AL.GenSource(out m_Source);
			
			// Configure the source listener
			AL.Source(m_Source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
			AL.Source(m_Source, ALSource3f.Velocity, 0.0f, 0.0f, 0.0f);
			AL.Source(m_Source, ALSource3f.Direction, 0.0f, 0.0f, 0.0f);
			AL.Source(m_Source, ALSourcef.RolloffFactor, 0.0f);
			AL.Source(m_Source, ALSourceb.SourceRelative, true);
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
		/// Dispose of the player safely
		/// </summary>
		public override void Dispose ()
		{
			this.Playback_Stop();
			AL.DeleteBuffers(m_Buffers);
			AL.DeleteSource(ref m_Source);
			if (m_Context!=null) { m_Context.Dispose(); m_Context = null; }
			if (m_CurrentFile!=null) { m_CurrentFile.Dispose(); m_CurrentFile = null; }
		}
		
		/// <summary>
		/// Destructor
		/// </summary>
		~OggPlayerFBN()
		{
			// Tidy up the OpenAL stuff
			AL.DeleteBuffers(m_Buffers);
			AL.DeleteSource(ref m_Source);
			if (m_Context!=null) { m_Context.Dispose(); m_Context = null; }
			if (m_CurrentFile!=null) { m_CurrentFile.Dispose(); m_CurrentFile = null; }
		}
				
		/// <summary>
		/// Set the current file. Only valid when the player is stopped or no file has been set
		/// </summary>
		/// <param name="NewFile">
		/// An <see cref="OggFile"/> object containg the file to set
		/// </param>
		public override bool  SetCurrentFile(OggFile NewFile)
		{
			// Check current state
			if (!((m_PlayerState==OggPlayerStatus.Stopped)||(m_PlayerState==OggPlayerStatus.Waiting))) { return false; }
			m_CurrentFile = NewFile;
			StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.UserRequest);
			return true;
		}
		
		/// <summary>
		/// Set the current file. Only valid when the player is stopped or no file has been set
		/// </summary>
		/// <param name="NewFilename">
		/// A <see cref="System.String"/> containing the path to the file to set
		/// </param>
		public override bool SetCurrentFile(string NewFilename)
		{
			return SetCurrentFile(new OggFile(NewFilename));	
		}
		
		/// <summary>
		/// Start playing the current file
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public override OggPlayerCommandReturn Playback_Play() 
		{
			// We can only play if we're stopped (this should also stop us trying to play invalid files as we'll then be 'Waiting' or 'Error' rather than stopped)
			if (m_PlayerState == OggPlayerStatus.Stopped)
			{	

				// Begin buffering
				StateChange(OggPlayerStatus.Buffering, OggPlayerStateChanger.UserRequest);

				// Create & Populate buffers
				for (int i=0;i<m_Buffers.Length;i++)
				{
					lock (OALLocker) {
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
				}
				lock (OALLocker)
				{
					// We've filled four buffers with data, give 'em to the source
					AL.SourceQueueBuffers(m_Source, m_Buffers.Length, m_Buffers);
					// Start playback
					AL.SourcePlay(m_Source);
				}
				m_LastTick = 0;
				m_LastError = ALError.NoError;
				// Spawn a new player thread
				StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
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
			bool Running = true; bool ReachedEOF = false; bool UnderRun = false;
			
			if (PlaybackStarted!=null) { PlaybackStarted(this, new EventArgs()); }
			while (Running)
			{
				// See what we're doing
				if (m_PlayerState==OggPlayerStatus.Playing)
				{
					// Check number of buffers
					int QueuedBuffers = 0;
					AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out QueuedBuffers);
					// EOF stuff
					if (ReachedEOF)
					{
						// We've come to the end of the file, just see if there are any buffers left in the queue
						if (QueuedBuffers>0) 
						{
							// We want to remove the buffers, so carry on to the usual playing section
						}
						else
						{
							lock (OALLocker)
							{
								// End of file & all buffers played, exit.
								Running = false;
								// Stop the output device if it isn't already
								if (AL.GetSourceState(m_Source)!=ALSourceState.Stopped) { AL.SourceStop(m_Source); }
								m_CurrentFile.ResetFile();	// Reset file's internal pointer
								// De-allocate all buffers
								for(int i = 0; i<m_Buffers.Length; i++)
								{
									AL.DeleteBuffer(ref m_Buffers[i]);	
								}
								m_Buffers = new uint[m_BufferCount];
							}
							// Set state stuff & return
							StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.EndOfFile);
							if (PlaybackFinished!=null) { PlaybackFinished(this, new EventArgs()); }
							return;
						}
					}
					
					// If the number of buffers is greater than 0 & the source isn't playing, poke it so it does
					if ((!ReachedEOF)&&(QueuedBuffers>0)&&(AL.GetError()==ALError.NoError))
					{
						if (AL.GetSourceState(m_Source) != ALSourceState.Playing)
						{
							AL.SourcePlay(m_Source);
						}
					}
					
					// Check for buffer underrun
					int ProcessedBuffers = 0; uint BufferRef=0;
					lock (OALLocker)
					{
						AL.GetSource(m_Source, ALGetSourcei.BuffersProcessed, out ProcessedBuffers);	
					}				
					if (ProcessedBuffers>=m_BufferCount)
					{
						UnderRun = true;
						if (BufferUnderrun!=null) { BufferUnderrun(this, new EventArgs()); }
					} else { UnderRun = false; }
					
					// Unbuffer any processed buffers
					while (ProcessedBuffers>0)
					{
						OggBufferSegment obs;
						lock (OALLocker)
						{
							// For each buffer thats been processed, reload and queue a new one
							AL.SourceUnqueueBuffers(m_Source, 1, ref BufferRef); 
							#if (DEBUG)
							if (AL.GetError()!=ALError.NoError) { Console.WriteLine("SourceUnqueueBuffers: ALError: " + OggUtilities.GetEnumString(AL.GetError())); }
							#endif
							if (ReachedEOF) { --ProcessedBuffers; continue; }	// If we're at the EOF loop to the next buffer here - we don't want to be trying to fill any more
							obs = m_CurrentFile.GetBufferSegment(m_BufferSize);	// Get chunk of tasty buffer data with the configured segment
						}
						// Check the buffer segment for errors
						if (obs.ReturnValue>0)
						{
							lock (OALLocker)
							{
								// No error, queue data
								AL.BufferData((int)BufferRef, m_CurrentFile.Format, obs.Buffer, obs.ReturnValue, obs.RateHz);
								#if (DEBUG)
								if (AL.GetError()!=ALError.NoError) { Console.WriteLine("BufferData: ALError: " + OggUtilities.GetEnumString(AL.GetError())); }
								#endif
								AL.SourceQueueBuffers(m_Source, 1, ref BufferRef);
								#if (DEBUG)
								if (AL.GetError()!=ALError.NoError) { Console.WriteLine("SourceQueueBuffers: ALError: " + OggUtilities.GetEnumString(AL.GetError())); }
								#endif
							}
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
								lock (OALLocker)
								{
									m_PlayerState = OggPlayerStatus.Error;
									AL.SourceStop(m_Source);
									Running = false;
								}
								if (PlaybackFinished!=null) { PlaybackFinished(this, new EventArgs()); }
								break;
							}
						}
						// Check for errors
						m_LastError = AL.GetError();
						if (m_LastError!= ALError.NoError)
						{
							StateChange(OggPlayerStatus.Error, OggPlayerStateChanger.Error);
							lock (OALLocker) { AL.SourceStop(m_Source); }
							Running = false;
							break;
						}
						
						--ProcessedBuffers;
					}
						
					// If we under-ran, restart the player
					if (UnderRun) { lock (OALLocker) { AL.SourcePlay(m_Source); } }
					
					// Do stuff with the time values & tick event
					m_PlayingOffset = m_CurrentFile.GetTime();
					if (m_TickEnabled)
					{
						if (m_PlayingOffset>=m_LastTick+m_TickInterval)
						{
							m_LastTick = m_PlayingOffset;
							if (PlaybackTick!=null) { PlaybackTick(this, new EventArgs()); }
						}
					}
				}
				else if (m_PlayerState==OggPlayerStatus.Seeking)
				{
					// Just wait for us to finish seeking
				}
				else if (m_PlayerState==OggPlayerStatus.Paused)
				{
					// Just wait for us to un-pause
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
		
		/// <summary>
		/// Stop playback. 
		/// Only valid if the player is playing or paused
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public override OggPlayerCommandReturn Playback_Stop()
		{
			if (!((m_PlayerState == OggPlayerStatus.Paused)||(m_PlayerState == OggPlayerStatus.Playing))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			// Stop the source and set the state to stop
			lock (OALLocker)
			{
				// Stop playing
				AL.SourceStop(m_Source);
				// See how many buffers are queued, and unqueue them
				int nBuffers;
				AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out nBuffers); 
				if (nBuffers>0) { AL.SourceUnqueueBuffers((int)m_Source,nBuffers); }
				// Reset the file object's internal location etc.
				m_CurrentFile.ResetFile();	
				// De-allocate all buffers
				for(int i = 0; i<m_Buffers.Length; i++)
				{
					AL.DeleteBuffer(ref m_Buffers[i]);	
				}
				m_Buffers = new uint[m_BufferCount];
			}
			m_LastTick = 0;
			// Set the new state
			StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.UserRequest);
			return OggPlayerCommandReturn.Success;
		}
		
		/// <summary>
		/// Pause playback
		/// Only valid if the player is playing
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public override OggPlayerCommandReturn Playback_Pause()
		{
			if (!(m_PlayerState == OggPlayerStatus.Playing)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			lock (OALLocker)
			{
				AL.SourcePause(m_Source);
			}
			StateChange(OggPlayerStatus.Paused, OggPlayerStateChanger.UserRequest);
			return OggPlayerCommandReturn.Success;
			
		}
		
		/// <summary>
		/// Unpause playback
		/// Only valid if the player is paused
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public override OggPlayerCommandReturn Playback_UnPause()
		{
			if (!(m_PlayerState == OggPlayerStatus.Paused)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			lock (OALLocker)
			{
				AL.SourcePlay(m_Source);
			}
			StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
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
		public override OggPlayerCommandReturn Playback_Seek(float RequestedTime)
		{
			if (!((m_PlayerState == OggPlayerStatus.Playing)||(m_PlayerState == OggPlayerStatus.Playing))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			OggPlayerCommandReturn retVal = OggPlayerCommandReturn.Error;
			StateChange(OggPlayerStatus.Seeking, OggPlayerStateChanger.UserRequest);
			lock (OALLocker)
			{
				AL.SourcePause(m_Source);
				retVal = m_CurrentFile.SeekToTime(RequestedTime);
				AL.SourcePlay(m_Source);
			}
			m_LastTick = RequestedTime - m_TickInterval;
			StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
			return retVal;
		}
	}
	
}
