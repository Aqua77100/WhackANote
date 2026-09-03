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
    [Tooltip("Assign your main Dialogue Box background panel here so it can be explicitly hidden")]
    public GameObject mainDialogueBox;
    public TextMeshProUGUI instructionText;

    [Header("Game References")]
    public MoleStationaryController[] moles;
    public NewMonoBehaviourScript rhythmSequencer;

    [Header("Sequence Steps")] // Hopefully this means that it will be modular
    public List<TutorialStep> steps = new List<TutorialStep>();

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private int currentStepIndex = 0; // change number for whichever step you wish to have
    private bool waitingForInput = false; // Are we wating for the user to tap? --> will be used to move onto next scene hopefully
    private bool ignoreTapThisFrame = false; // Prevents 1 tap from triggering 2 steps simultaneously

    private void Start()
    {
        // Mute and stop music completely at start
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
            backgroundMusic.mute = true;
            backgroundMusic.loop = false; // False so it actually ends
        }

        ExecuteStep(0);
    }

    private void Update()
    {
        // Handling the dialogue taps:
        if (!waitingForInput) return;

        if (ignoreTapThisFrame) //(this was because it would skip to the last one for some reason)
        {
            ignoreTapThisFrame = false;
            return;
        }

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
        // Hide all the UI containers first
        HideAllTutorialUI();

        if (index >= steps.Count) // if the index goes over the amount of steps we have, then this means that the tutorial is complete.
        {
            CompleteTutorial();
            return;
        }

        currentStepIndex = index; // current step is at index
        TutorialStep step = steps[currentStepIndex]; // cary out step at step index

        // Hide ALL UI, unmute, and start audio+gameplay when mini-game starts
        if (step.triggerType == StepTriggerType.MiniGameCompletion)
        {
            StartMiniGamePhase();
            waitingForInput = false;
            return;
        }

        // show dialogue & UI for the tutorial step
        if (mainDialogueBox != null) mainDialogueBox.SetActive(true);
        if (step.stepUIContainer != null) step.stepUIContainer.SetActive(true);
        if (step.pulseCue != null) step.pulseCue.SetActive(true);
        if (dialogueText != null) dialogueText.text = step.dialogueText;
        if (instructionText != null) instructionText.text = step.instructionText;

        // DIALOGUE BOX LOGIC
        bool hasCustomBox = step.customDialogueBox != null;

        // Turn ON custom box IF it exists --> otherwise = turn ON default box
        if (hasCustomBox)
        {
            step.customDialogueBox.SetActive(true);
            if (mainDialogueBox != null) mainDialogueBox.SetActive(false); // Ensure original is hidden
        }
        else
        {
            if (mainDialogueBox != null) mainDialogueBox.SetActive(true);
        }

        // Assign text to whichever box is active
        TextMeshProUGUI activeDialogueText = (hasCustomBox && step.customDialogueText != null) ? step.customDialogueText : dialogueText;
        if (activeDialogueText != null) activeDialogueText.text = step.dialogueText;

        TextMeshProUGUI activeInstructionText = (hasCustomBox && step.customInstructionText != null) ? step.customInstructionText : instructionText;
        if (activeInstructionText != null) activeInstructionText.text = step.instructionText;

        // Show the other step-specific overlays & cues
        if (step.stepUIContainer != null) step.stepUIContainer.SetActive(true);
        if (step.pulseCue != null) step.pulseCue.SetActive(true);

        if (step.triggerType == StepTriggerType.TapTargetMole && step.targetMoleIndex >= 0)
        {
            moles[step.targetMoleIndex].PopUp(999f); // keep this up indefinitely (until you tap)
        }

        waitingForInput = true;
    }



    public void OnMoleTapped(int moleIndex) // If you hit the mole, then we need to advance step
    {
        if (!waitingForInput) return;

        TutorialStep currentStep = steps[currentStepIndex];
        if (currentStep.triggerType == StepTriggerType.TapTargetMole && currentStep.targetMoleIndex == moleIndex)
        {
            ignoreTapThisFrame = true; // Prevent step 1 from taking this tap (for some reason it kept doing that)
            AdvanceStep();
        }
    }

    public void AdvanceStep()
    {
        waitingForInput = false; // No longer waiting for input (as it has been recieived), so reset for the next step
        ExecuteStep(currentStepIndex + 1); // Excecute next step
    }

    public void StartMiniGamePhase()  // Call this when advancing to the mini-game step
    {
        // Unhide and play audio ONLY when mini-game starts
        if (backgroundMusic != null)
        {
            backgroundMusic.mute = false;
            backgroundMusic.time = 0f;
            backgroundMusic.Play();
        }

        if (rhythmSequencer != null)
        {
            rhythmSequencer.StartCoroutine("PlaySequence");
        }
    }

    private void HideAllTutorialUI() 
    {
        if (mainDialogueBox != null) mainDialogueBox.SetActive(false);
        if (fullScreenBlocker != null) fullScreenBlocker.SetActive(false);

        // Stop and hide all active moles so they don't trigger "MISS" on game over
        if (moles != null)
        {
            foreach (var mole in moles)
            {
                if (mole != null)
                {
                    mole.HideAndStop();
                }
            }
        }

        foreach (var s in steps)
        {
            if (s.customDialogueBox != null) s.customDialogueBox.SetActive(false);
            if (s.stepUIContainer != null) s.stepUIContainer.SetActive(false);
            if (s.pulseCue != null) s.pulseCue.SetActive(false);
        }
    }


    public void CompleteTutorial() // complete the tutorial   
    {
        HideAllTutorialUI(); // Turns off the lingering text box

        if (fullScreenBlocker != null)
            fullScreenBlocker.SetActive(false); // this also doesn't pop up i think

        PlayerPrefs.SetInt("tutorial_complete", 1); // set boolean to true
        PlayerPrefs.Save(); // Save
    }
}