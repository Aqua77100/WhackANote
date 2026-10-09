using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [Header("Preview")]
    public composerMole[] moles;

    public float playbackBPM = 120f;
    [Header("Recording Timer")]
    public TextMeshProUGUI recordingTimerText;

    private float recordingTime = 0f;   
    [Header("Record Button")]
    public TMP_Text recordButtonText;

    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    public int countdownSeconds = 3;

    private bool isCountingDown = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isRecording)
            return;

        // Update recording stopwatch
        recordingTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(recordingTime / 60f);
        int seconds = Mathf.FloorToInt(recordingTime % 60f);

        if (recordingTimerText != null)
        {
            recordingTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Recording slot timer
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
        recordingTime = 0f;
        currentNote = -1;
        isRecording = true;

        if (recordingTimerText != null)
        {
            recordingTimerText.text = "00:00:000";
        }

        if (recordButtonText != null)
        {
            recordButtonText.text = "Stop Recording";
        }

        PlayMetronome();

        Debug.Log("Recording started");
    }

    public void StopRecording()
    {
        if (!isRecording)
            return;

        // Finish the current slot without another metronome click
        FinishCurrentSlot(false);

        isRecording = false;

        // Stop the metronome
        if (metronomeAudio != null)
        {
            metronomeAudio.Stop();
        }

        if (recordButtonText != null)
        {
            recordButtonText.text = "Record";
        }

        Debug.Log("Recording stopped");

        Debug.Log("Sequence: " +
                string.Join(", ", recordedSequence));
    }

    private void FinishCurrentSlot(bool playMetronome = true)
    {
        recordedSequence.Add(currentNote);

        Debug.Log(
            "Slot " +
            (recordedSequence.Count - 1) +
            ": " +
            currentNote
        );

        currentNote = -1;

        if (playMetronome)
        {
            PlayMetronome();
        }
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

    public void SetBPM(int newBPM)
    {
        playbackBPM = newBPM;

        Debug.Log("Playback BPM set to: " + playbackBPM);
    }

    public void PlayCreation()
    {  
        if (recordedSequence.Count == 0)
        {
            Debug.Log("Nothing has been recorded yet.");
            return;
        }

        StartCoroutine(PlayRecordedSequence());
    }

    private IEnumerator PlayRecordedSequence()
    {
        float secondsPerBeat = 60f / playbackBPM;

        Debug.Log("Playing creation at " + playbackBPM + " BPM");

        foreach (int moleIndex in recordedSequence)
        {
            // -1 means a rest
            if (moleIndex >= 0 && moleIndex < moles.Length)
            {
                moles[moleIndex].PlayPreview();
            }

            yield return new WaitForSeconds(secondsPerBeat);
        }

        Debug.Log("Finished playing creation.");
    }

    public void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else if (!isCountingDown)
        {
            StartCoroutine(CountdownThenRecord());
        }
    }

    private IEnumerator CountdownThenRecord()
    {
        isCountingDown = true;

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        for (int count = countdownSeconds; count > 0; count--)
        {
            if (countdownText != null)
                countdownText.text = count.ToString();

            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        isCountingDown = false;

        StartRecording();
    }
}