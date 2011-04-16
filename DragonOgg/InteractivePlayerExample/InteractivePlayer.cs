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

using DragonOgg.Interactive;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace InteractivePlayerExample
{
    /// <summary>
    /// A simple example of how to use the interactive player in DragonOgg.
    /// These features target interactive applications such as games.
    /// 
    /// This samples a short guitar rift and also a "boing" sound effect in the
    /// background.  It ends by playing several overlapping "boing"s.
    /// 
    /// The Thread.Sleep() calls wouldn't be required in most programs if the
    /// sounds are being played in response to something (such as collision
    /// events, button presses, etc).  Here, they're used simply to add delays
    /// between calls to Play(), otherwise the program would attempt to play 
    /// all sounds at the same time.
    /// </summary>
    /// 
    class InteractivePlayer
    {
        static void Main(string[] args)
        {
            // Setup OpenTK audio stuff
            AudioContext ac = new AudioContext();
            XRamExtension xram = new XRamExtension();

            // Load the .ogg files
            string guitarFile = "GuitarSample.ogg";
            AudioClip guitarClip = new AudioClip(guitarFile);

            string boingFile = "BoingSample.ogg";
            AudioClip boingClip = new AudioClip(boingFile);

            // Play it.  The first time a clip is played, a background thread is
            // launched to handle all audio.  Playing the clip again or playing
            // a new clip will use the already existing thread.  To avoid 
            // threading, construct your own AudioManager.
            guitarClip.Play();

            // Play the "boing" clip every 2 seconds during the guitar clip
            boingClip.Play();
            Thread.Sleep(2000);

            boingClip.Play();
            Thread.Sleep(2000);

            boingClip.Play();
            Thread.Sleep(2000);

            // Finally play some overlapping sounds to demonstrate a sound can
            // have more than one instance played at a time.
            boingClip.Play();
            Thread.Sleep(200);

            boingClip.Play();
            Thread.Sleep(200);

            boingClip.Play();
            Thread.Sleep(200);

            boingClip.Play();
            Thread.Sleep(200);

            // Give the last "boing" time to finish
            Thread.Sleep(800);
        }
    }
}
