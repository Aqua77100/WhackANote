using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText; 
    public GameObject fullScreenBlocker;

    [Header("Game References")]
    public MoleStationaryController[] moles;
    public NewMonoBehaviourScript rhythmSequencer;

    [Header("Sequence Steps")] // Hopefully this means that it will be modular
    public List<TutorialStep> steps = new List<TutorialStep>();

    private int currentStepIndex = 0; // change number for whichever step you wish to have
    private bool waitingForInput = false; // Are we wating for the user to tap? --> will be used to move onto next scene hopefully

    private void Start()
    {
        ExecuteStep(0); // Excecute the steps from the beginning (0)
    }

    private void Update()
    {
        if (!waitingForInput) return;

        TutorialStep currentStep = steps[currentStepIndex];

        if (currentStep.triggerType == StepTriggerType.TapAnywhere) // If this step is the type to want a tap anywhere, wait for tap and advance forward
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                AdvanceStep();
            }
        }
    }

    public void ExecuteStep(int index)
    {
        if (index >= steps.Count) // if the index goes over the amount of steps we have, then this means that the tutorial is complete.
        {
            CompleteTutorial();
            return;
        }

        currentStepIndex = index; // current step is at index
        TutorialStep step = steps[currentStepIndex]; // cary out step at step index

        // Google suggestion (don't know if this actually works):
        // Disable all step-specific overlay containers
        foreach (var s in steps)
        {
            if (s.stepUIContainer != null) s.stepUIContainer.SetActive(false);
            if (s.pulseCue != null) s.pulseCue.SetActive(false);
        }

        // Enable target step UI & text
        if (step.stepUIContainer != null) step.stepUIContainer.SetActive(true);
        if (step.pulseCue != null) step.pulseCue.SetActive(true);
        if (dialogueText != null) dialogueText.text = step.dialogueText;

        // Mole target setup (for the first step)
        if (step.triggerType == StepTriggerType.TapTargetMole && step.targetMoleIndex >= 0)
        {
            moles[step.targetMoleIndex].PopUp(999f); // Keep mole up until hit (doesn't go down)
        }

        waitingForInput = true;
    }

    public void OnMoleTapped(int moleIndex) // If you hit the molle, then we need to advance step
    {
        if (!waitingForInput) return;

        TutorialStep currentStep = steps[currentStepIndex];
        if (currentStep.triggerType == StepTriggerType.TapTargetMole && currentStep.targetMoleIndex == moleIndex)
        {
            AdvanceStep();
        }
    }

    public void AdvanceStep()
    {
        waitingForInput = false; // No longer waiting for input (as it has been recieived), so reset for the next step
        ExecuteStep(currentStepIndex + 1); // Excecute next step
    }

    public void CompleteTutorial() // complete the tutorial?
    {
        PlayerPrefs.SetInt("tutorial_complete", 1); // set boolean to true
        PlayerPrefs.Save(); // Save
        SceneManager.LoadScene("Menu"); // Go back to menu
    }
}