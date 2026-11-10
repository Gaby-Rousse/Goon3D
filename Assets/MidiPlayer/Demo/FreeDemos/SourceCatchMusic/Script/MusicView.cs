using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;
using UnityEngine.Events;
using DemoMPTK;

namespace MPTKDemoCatchMusic
{
    /// <summary>
    /// MAin class for CatchMusic demo/
    ///     - Start MidiFilePlayer ad defined OnEventNotesMidi to catch each MIDI events (without playing them).
    ///     - Create Unity GameObject for each MIDI events (only NoteOn and PatchChange for instrument).
    /// </summary>
    public class MusicView : MonoBehaviour
    {
        public Camera Cam;
        public bool showGUI = true;
        //public float Speed = 1f;

        /// <summary>
        /// A prefab MidiFilePlayer must exists in the Unity Editor Hierarchy and must be associated in the MusicView GameObject inspector.
        /// In ths demo, the MidiFilePlayer is muted (Send MIDI ti the Synth disabled). 
        /// It is only used to receive MIDI events thanks to the MIDI sequencer and the event defined with OnEventNotesMidi (see bellow).
        /// </summary>
        public MidiFilePlayer midiFilePlayer;

        //https://stackoverflow.com/questions/64459671/unity-set-gui-box-background-color
        GUIStyle labelStyle;
        Texture2D consoleBackground;

        private void Awake()
        {
            // this variable is stored in the class
            // 1 pixel image, only 1 color to set
            consoleBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            consoleBackground.SetPixel(0, 0, new Color(0, 0, 0, 1f));
            consoleBackground.Apply(); // not sure if this is necessary

            // basically just create a copy of the "none style"
            // and then change the properties as desired
            labelStyle = new GUIStyle(GUIStyle.none);
            labelStyle.fontSize = 16;
            labelStyle.normal.textColor = Color.white;
            labelStyle.normal.background = consoleBackground;
            labelStyle.padding.left = 5;
        }


        void OnGUI()
        {
            if (!showGUI)
                return;
            int startx = 0;
            int starty = Screen.height - 50;
            int maxwidth = Screen.width;

            if (midiFilePlayer != null)
            {
                GUILayout.BeginArea(new Rect(startx, starty, maxwidth, 200));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Previous", ""), GUILayout.Width(150)))
                {
                    midiFilePlayer.MPTK_Previous();
                }
                if (GUILayout.Button(new GUIContent("Next", ""), GUILayout.Width(150)))
                {
                    midiFilePlayer.MPTK_Next();
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("MIDI '" + midiFilePlayer.MPTK_MidiName + (midiFilePlayer.MPTK_IsPlaying ? "' is playing" : " is not playing"), labelStyle);
                GUILayout.EndArea();
            }
        }
    }
}
