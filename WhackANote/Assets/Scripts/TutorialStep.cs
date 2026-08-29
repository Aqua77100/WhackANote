using UnityEngine;

public enum StepTriggerType { TapAnywhere, TapTargetMole, MiniGameCompletion } // The different types needed for the tutorial

[System.Serializable]
public class TutorialStep
{
    public string stepName; // What is this step called?
    [TextArea(2, 4)] public string dialogueText; // What text for this step?
    public StepTriggerType triggerType; // What trigger type is it?
    public GameObject stepUIContainer; // Unique UI frame/cutout panel for this step
    public GameObject pulseCue; // Tap indicator object
    public int targetMoleIndex = -1; // -1 because its an array starting from 0
}