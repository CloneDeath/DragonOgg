// Author:
//    Caleb Leak
//    caleb@embergames.net
//    www.EmberGames.net
// 
// Copyright (C) 2011 by Ember Games Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//

using System;
using System.Threading;

using DragonOgg.MediaPlayer;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace MediaPlayerExample
{
    /// <summary>
    /// A simple two item play list.  First plays the guitar sample and then 
    /// plays the "boing" sample.
    /// </summary>
    class MediaPlayerExample
    {
        static void Main(string[] args)
        {
            OggPlaylist playlist = new OggPlaylist();

            // Add the guitar sound
            OggFile guitarFile = new OggFile("GuitarSample.ogg");
            OggPlaylistFile guitarPlayList = new OggPlaylistFile(guitarFile, 0);
            playlist.Add(guitarPlayList);

            // Add the boing sound after the guitar
            OggFile boingFile = new OggFile("BoingSample.ogg");
            OggPlaylistFile boingPlayList = new OggPlaylistFile(boingFile, -1);
            playlist.Add(boingPlayList);

            OggPlayerFBN player = new OggPlayerFBN();

            // Keep playing until our playlist is empty
            while(true)
            {
                while (player.PlayerState != OggPlayerStatus.Waiting
                    && player.PlayerState != OggPlayerStatus.Stopped)
                {
                    Thread.Sleep(5);
                }

                if (playlist.GetNextFile() != null)
                {
                    playlist.CurrentFile.File.ResetFile();
                    player.SetCurrentFile(playlist.CurrentFile.File);
                    player.Play();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
