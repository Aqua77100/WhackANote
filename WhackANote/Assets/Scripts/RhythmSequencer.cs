using UnityEngine;
using System.Collections;
using System.IO;

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

    [Header("Custom Level Settings")]
    public bool loadCustomLevel = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (isTutorial)
            return;

        // Only load a saved level in the custom gameplay scene.
        if (loadCustomLevel)
        {
            if (!LoadCustomLevel())
                return;
        }

        if (sequence == null || sequence.Length == 0)
        {
            Debug.LogError("Level sequence is empty!");
            return;
        }

        StartCoroutine(PlaySequence());
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

    // private IEnumerator PlaySequence()
    // {
    //     int sequenceIndex = 0;

    //     // Loop continuously for main game but run ONCE through the array for tutorial
    //     while (true)
    //     {
    //         float secondsPerBeat = SecondsPerBeat;
    //         int moleIndex = sequence[sequenceIndex];

    //         if (moleIndex >= 0 && moleIndex < moles.Length && moles[moleIndex] != null)
    //         {
    //             float duration = secondsPerBeat * activeWindowInBeats;
    //             moles[moleIndex].PopUp(duration);
    //         }

    //         sequenceIndex++;

    //         // If we reached the end of the tutorial sequence, stop and advance tutorial!
    //         if (sequenceIndex >= sequence.Length)
    //         {
    //             if (isTutorial)
    //             {
    //                 // Signal tutorial manager that song is done
    //                 TutorialManager tutorial = Object.FindAnyObjectByType<TutorialManager>();
    //                 if (tutorial != null) tutorial.AdvanceStep();

    //                 yield break; // Exit loop
    //             }

    //             sequenceIndex = 0; // Loop around for normal levels
    //         }

    //         yield return new WaitForSeconds(secondsPerBeat);
    //     }
    // }


    private bool LoadCustomLevel()
    {
        string fileName = PlayerPrefs.GetString(
            "SelectedCreatedLevelFile", ""
        );

        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("No custom level was selected!");
            return false;
        }

        string filePath = Path.Combine(
            Application.persistentDataPath,
            "CreatedLevels",
            Path.GetFileName(fileName)
        );

        if (!File.Exists(filePath))
        {
            Debug.LogError("Custom level not found: " + filePath);
            return false;
        }

        try
        {
            string json = File.ReadAllText(filePath);

            SavedLevelData data =
                JsonUtility.FromJson<SavedLevelData>(json);

            if (data == null ||
                data.sequence == null ||
                data.sequence.Length == 0 ||
                data.bpm <= 0)
            {
                Debug.LogError("Invalid custom level data!");
                return false;
            }

            // Apply the saved level's sequence and BPM.
            sequence = data.sequence;
            bpm = data.bpm;

            Debug.Log(
                "Loaded level: " + data.levelName +
                " | BPM: " + bpm +
                " | Sequence: " + string.Join(", ", sequence)
            );

            return true;
        }
        catch (System.Exception exception)
        {
            Debug.LogError("Failed to load level: " + exception.Message);
            return false;
        }
    }


}
