using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class SpeakerData
{
    public string id;
    public string label;
    public TMP_FontAsset font;
    public int affection;
    public List<Status> statuses = new();

    [System.Serializable]
    public struct Status
    {
        public enum EMOTION
        {
            NEUTRAL,
            ANGRY,
            HAPPY,
            SAD,
        }
        public EMOTION emotion;
        public Sprite icon;
        public AudioClip audioClip;
    }
}
