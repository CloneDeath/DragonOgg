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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DragonOgg.Interactive;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace InteractivePlayerGUI
{
    /// <summary>
    /// A simple GUI to demonstrate DragonOgg.  Allows you to play two sounds:
    /// a guitar sample and a boing sample.
    /// </summary>
    public partial class InteractivePlayerForm : Form
    {
        AudioClip guitar;
        AudioClip boing;
        AudioClip externalClip;

        /// <summary>
        /// Initializes OpenAL and loads two sound clips.
        /// </summary>
        public InteractivePlayerForm()
        {
            // Setup OpenTK audio stuff
            AudioContext ac = new AudioContext();
            XRamExtension xram = new XRamExtension();

            // Load the .ogg files
            guitar = new AudioClip("GuitarSample.ogg");
            boing = new AudioClip("BoingSample.ogg");

            externalClip = null;

            InitializeComponent();
        }

        /// <summary>
        /// Plays the "boing" clip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlayBoing_Click(object sender, EventArgs e)
        {
            boing.Play();
        }

        /// <summary>
        /// Plays the guitar clip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlayGuitar_Click(object sender, EventArgs e)
        {
            guitar.Play();
        }

        /// <summary>
        /// Loads an external audio clip to play.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Filter = "Ogg files (*.ogg)|*.ogg|All files (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    externalClip = new AudioClip(fileDialog.FileName);
                    textBoxFile.Text = fileDialog.FileName;
                    buttonPlayExternal.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Plays the external audio clip if it's been loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlayExternal_Click(object sender, EventArgs e)
        {
            if(externalClip != null)
                externalClip.Play();
        }
    }
}
