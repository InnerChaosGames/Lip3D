using UnityEngine;
using Meta.WitAi.TTS.Utilities;

public class SimpleTTS : MonoBehaviour
{
    public TTSSpeaker ttsSpeaker;

    void Start()
    {
        // Test la pornire
        Speak("The Columns of the Ulpia Traiana Sarmisegetuza Forum, situated in the Roman Forum at Ulpia Traiana Sarmisegetuza featured an open central plaza enveloped by porticos. These covered galleries were structurally supported by column arrays. These columns served as a primary architectural component, providing both monumentality and spatial functionality to the civic center");
    }

    public void Speak(string text)
    {
        if (ttsSpeaker != null)
        {
            ttsSpeaker.Speak(text);
        }
        else
        {
            Debug.LogError("TTSSpeaker nu este setat!");
        }
    }
}