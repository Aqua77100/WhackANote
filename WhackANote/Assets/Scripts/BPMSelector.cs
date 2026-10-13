using UnityEngine;
using TMPro;

public class BPMSelector : MonoBehaviour
{
    [System.Serializable]
    public class BPMOption
    {
        public int bpm;
        public AudioClip metronomeClip;
    }

    [Header("References")]
    public TMP_Dropdown bpmDropdown;
    public Composer composer;

    [Header("BPM Options")]
    public BPMOption[] bpmOptions;

    private void Start()
    {
        // Clear anything currently in the dropdown
        bpmDropdown.ClearOptions();

        // Add each BPM option to the dropdown
        foreach (BPMOption option in bpmOptions)
        {
            bpmDropdown.options.Add(
                new TMP_Dropdown.OptionData(option.bpm + " BPM")
            );
        }

        // Refresh dropdown visually
        bpmDropdown.RefreshShownValue();

        // Listen for the user changing the dropdown
        bpmDropdown.onValueChanged.AddListener(ChangeBPM);

        // Set initial BPM
        ChangeBPM(bpmDropdown.value);
    }

    private void ChangeBPM(int optionIndex)
    {
        BPMOption selectedOption = bpmOptions[optionIndex];

        /*composer.SetBPM(
            selectedOption.bpm,
            selectedOption.metronomeClip
        );*/

        Debug.Log("BPM changed to: " + selectedOption.bpm);
    }
}