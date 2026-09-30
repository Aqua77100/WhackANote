using System.Collections.Generic;
using UnityEngine;

public class Composer : MonoBehaviour
{
    public static Composer Instance;

    [Header("Song Settings")]
    public float bpm = 100f;

    // 1 = one slot per beat
    // 2 = eighth-note slots
    // 4 = sixteenth-note slots
    public int subdivisions = 1;

    [Header("Metronome")]
    public AudioSource metronomeAudio;

    private List<int> recordedSequence = new List<int>();

    public bool isRecording = false;

    private float secondsPerSlot;
    private float timer;

    // -1 means nothing has been pressed during this slot
    private int currentNote = -1;

    public bool IsRecording => isRecording;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isRecording)
            return;

        timer += Time.deltaTime;

        if (timer >= secondsPerSlot)
        {
            FinishCurrentSlot();

            timer -= secondsPerSlot;
        }
    }

    public void StartRecording()
    {
        recordedSequence.Clear();

        secondsPerSlot = (60f / bpm) / subdivisions;

        timer = 0f;
        currentNote = -1;
        isRecording = true;

        PlayMetronome();

        Debug.Log("Recording started");
    }

    public void StopRecording()
    {
        if (!isRecording)
            return;

        FinishCurrentSlot();

        isRecording = false;

        Debug.Log("Recording stopped");

        Debug.Log("Sequence: " +
                  string.Join(", ", recordedSequence));
    }

    private void FinishCurrentSlot()
    {
        recordedSequence.Add(currentNote);

        Debug.Log(
            "Slot " +
            (recordedSequence.Count - 1) +
            ": " +
            currentNote
        );

        currentNote = -1;

        PlayMetronome();
    }

    public void RecordMole(int moleIndex)
    {
        if (!isRecording)
            return;

        currentNote = moleIndex;

        Debug.Log("Mole pressed: " + moleIndex);
    }

    private void PlayMetronome()
    {
        if (metronomeAudio != null)
        {
            metronomeAudio.Play();
        }
    }

    public int[] GetSequence()
    {
        return recordedSequence.ToArray();
    }

    /*public void SetBPM(int newBPM, AudioClip newMetronomeClip)
    {
        bpm = newBPM;

        if (metronomeAudio != null)
        {
            metronomeAudio.clip = newMetronomeClip;
        }

        Debug.Log("SongRecorder BPM: " + bpm);
    }*/
}