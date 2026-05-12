using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.Core
{
    public class AudioData : MonoBehaviour
    {
        public List<AudioClip> audio;

        public const string MUSIC_MENUS = "MainMenuMusic";
        public const string MUSIC_INGAME = "IngameMusic";
        public const string BUTTON_DOWN = "ButtonDown";
        public const string BUTTON_UP = "ButtonUp";
    }
}