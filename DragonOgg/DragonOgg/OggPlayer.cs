// 
//  OggPlayer.cs
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
	/// Abstraction for all OggPlayers to ensure standardisation of player formats
	/// </summary>
	public abstract class OggPlayer : IDisposable
	{

		protected OggPlayerStatus m_PlayerState; 		// Player state
		protected OggFile m_CurrentFile;				// Currently active file
		
		protected float m_PlayingOffset;				// Current time in playback
		protected float m_BufferOffset;				// Current time in buffer

		protected bool m_TickEnabled;				// Tick control flag
		protected float m_TickInterval;				// Interval between tick events
		protected float m_LastTick;					// Last tick
		
		public event OggPlayerStateChangedHandler StateChanged; 
		public event OggPlayerMessageHandler PlayerMessage;
		public event OggPlayerTickHandler Tick;
		
		protected static readonly object StateLocker = new object();
		protected static object OALLocker = new object();
		
		protected void StateChange(OggPlayerStatus NewState) { StateChange(NewState, OggPlayerStateChanger.Internal); }
		protected void StateChange(OggPlayerStatus NewState, OggPlayerStateChanger Reason)
		{
			if (StateChanged!=null) { StateChanged(this, new OggPlayerStateChangedArgs(m_PlayerState, NewState, Reason)); }
			#if (DEBUG)
				Console.WriteLine(DateTime.Now.ToLongTimeString() + "\tOggPlayer::StateChange -- From: " + OggUtilities.GetEnumString(m_PlayerState) + " -- To: " + OggUtilities.GetEnumString(NewState));
			#endif
			m_PlayerState = NewState;
		}
		
		protected void SendMessage(OggPlayerMessageType Message) { SendMessage(Message, null); }
		protected void SendMessage(OggPlayerMessageType Message, object Params)
		{
			if (PlayerMessage!=null) { PlayerMessage(this, new OggPlayerMessageArgs(Message, Params)); }
			#if (DEBUG)
				Console.WriteLine(DateTime.Now.ToLongTimeString() + "\tOggPlayer::SendMessage -- Message: " + OggUtilities.GetEnumString(Message));
			#endif
		}
			
		public abstract OggPlayerCommandReturn Playback_Play();
		public abstract OggPlayerCommandReturn Playback_Stop();
		public abstract OggPlayerCommandReturn Playback_Pause();
		public abstract OggPlayerCommandReturn Playback_UnPause();
		public abstract OggPlayerCommandReturn Playback_Seek(float SeekTime);
		
		public abstract bool SetCurrentFile(string FileName);
		public abstract bool SetCurrentFile(OggFile File);

		public abstract void Dispose();
			
	}
	
	#region "Events"
	/// <summary>
	/// Event handler for changes in OggPlayer state
	/// </summary>
	public delegate void OggPlayerStateChangedHandler(object sender, OggPlayerStateChangedArgs e);
	
	/// <summary>
	/// Event arguments for OggPlayer StateChanged events
	/// </summary>
	public class OggPlayerStateChangedArgs : EventArgs
	{
		private OggPlayerStatus m_OldState; 
		private OggPlayerStatus m_NewState;
		private OggPlayerStateChanger m_Changer;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eOldState">
		/// Original state of the player as an <see cref="OggPlayerStatus"/> enumeration
		/// </param>
		/// <param name="eNewState">
		/// New state of the player as an <see cref="OggPlayerStatus"/> enumeration
		/// </param>
		/// <param name="eChanger">
		/// Reason for the change in state as an <see cref="OggPlayerStateChanger"/> enumeration
		/// </param>
		public OggPlayerStateChangedArgs(OggPlayerStatus eOldState, OggPlayerStatus eNewState, OggPlayerStateChanger eChanger)
		{
			m_OldState = eOldState; m_NewState = eNewState; m_Changer = eChanger;	
		}
		
		/// <summary>
		/// Original state
		/// </summary>
		public OggPlayerStatus OldState { get { return m_OldState; } }
		/// <summary>
		/// New state
		/// </summary>
		public OggPlayerStatus NewState { get { return m_NewState; } }
		/// <summary>
		/// Reason for the change in state
		/// </summary>
		public OggPlayerStateChanger Changer { get { return m_Changer; } }
	}
	
	/// <summary>
	/// Event handler for messages from an OggPlayer
	/// </summary>
	public delegate void OggPlayerMessageHandler(object sender, OggPlayerMessageArgs e);
	
	/// <summary>
	/// Event arguments for OggPlayer Message events
	/// </summary>
	public class OggPlayerMessageArgs : EventArgs
	{
		private OggPlayerMessageType m_Message;
		private object m_Params;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eMessage">
		/// Type of message as an <see cref="OggPlayerMessageType"/> enumerator
		/// </param>
		public OggPlayerMessageArgs(OggPlayerMessageType eMessage)
		{
			m_Message = eMessage; m_Params = null;	
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eMessage">
		/// Type of message as an <see cref="OggPlayerMessageType"/> enumerator
		/// </param>
		/// <param name="eParams">
		/// Message parameter(s). Type depends on the type of message
		/// </param>
		public OggPlayerMessageArgs(OggPlayerMessageType eMessage, object eParams)
		{
			m_Message = eMessage; m_Params = eParams;
		}
		
		/// <summary>
		/// Type of message sent
		/// </summary>
		public OggPlayerMessageType Message { get { return m_Message; } }
		/// <summary>
		/// Parameter(s) for this message. Content depends on the type of message
		/// </summary>
		public object Params { get { return m_Params; } }
	}
	
	/// <summary>
	/// Event handler for player tick events
	/// </summary>
	public delegate void OggPlayerTickHandler(object sender, OggPlayerTickArgs e);
	
	/// <summary>
	/// Event arguments for OggPlayer Tick events
	/// </summary>
	public class OggPlayerTickArgs : EventArgs
	{
		private float m_PlaybackTime;
		private float m_BufferedTime;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ePlaybackTime">
		/// Current position in seconds of the audio output process as a <see cref="System.Single"/>
		/// </param>
		/// <param name="eBufferedTime">
		/// Current position in seconds of the buffer process as a <see cref="System.Single"/>
		/// </param>
		public OggPlayerTickArgs(float ePlaybackTime, float eBufferedTime)
		{
			m_PlaybackTime = ePlaybackTime; m_BufferedTime = eBufferedTime;
		}
		
		/// <summary>
		/// Current position in seconds of the audio output process
		/// </summary>
		public float PlaybackTime { get { return m_PlaybackTime; } }
		
		/// <summary>
		/// Current position in seconds of the buffer process
		/// </summary>
		public float BufferedTime { get { return m_BufferedTime; } }
	}
	#endregion
}
