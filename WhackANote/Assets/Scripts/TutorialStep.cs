using UnityEngine;
using TMPro;

public enum StepTriggerType { TapAnywhere, TapTargetMole, MiniGameCompletion } // The different types needed for the tutorial

[System.Serializable]
public class TutorialStep
{
    [Header("Custom UI Panel")]
    public GameObject customDialogueBox; // The unique UI box for this step (with custom size/position/images)
    //The custom ones are option, otherwise the default is selected, may just get rid of the deafult one if we need to lighten it up
    public TextMeshProUGUI customDialogueText; // Text specifically inside this box
    public TextMeshProUGUI customInstructionText; // Instruction text specifically inside this box

    [Header("Main/Basic Components")]
    public string stepName; // What is this step called?
    [TextArea(2, 4)] public string dialogueText; // What text for this step?
    public StepTriggerType triggerType; // What trigger type is it?
    public GameObject stepUIContainer; // Unique UI frame/cutout panel for this step
    public GameObject pulseCue; // Tap indicator object
    [TextArea(2, 4)] public string instructionText; // E.g. 'Tap to continue'
    public int targetMoleIndex = -1; // -1 because its an array starting from 0
}