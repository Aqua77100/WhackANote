using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Sequence Settings")]
    [Tooltip("Assign the mole game object in order (Index 0 = Mole 1, Index 2 = Mole 2, etc)")]
    public MoleStationaryController[] moles;

    [Tooltip("Order of mole indices to trigger (0-indexed: 0=Mole1, 1=Mole2, etc)")]
    public int[] sequence = new int[] { 0, 1, 2, 3, 4, 3 };

    [Header("Rhythm Settings")]
    public float bpm = 100f;
    [Tooltip("How long (in beats) the mole stays up for clicking")]
    public float activeWindowInBeats = 0.8f;

    private float SecondsPerBeat => 60f / bpm;

    [Header("Tutorial / Control Settings")]
    [Tooltip("If true, the sequencer will wait for TutorialManager to call StartTutorialSong() instead of auto-starting.")]
    public bool isTutorial = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Only auto-start if this is NOT a tutorial level
        if (!isTutorial)
        {
            StartCoroutine(PlaySequence());
        }
    }

    private IEnumerator PlaySequence(){
        int sequenceIndex = 0;

        while(true){
            float secondsPerBeat = SecondsPerBeat;
            int moleIndex = sequence[sequenceIndex];

            if (moleIndex >= 0 && moleIndex < moles.Length && moles[moleIndex] != null){
                float duration = secondsPerBeat * activeWindowInBeats;
                moles[moleIndex].PopUp(duration);
            }

            sequenceIndex = (sequenceIndex+1)%sequence.Length;

            yield return new WaitForSeconds(secondsPerBeat);
        }
    }

}
