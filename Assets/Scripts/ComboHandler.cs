using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using UnityEditor;

public enum InputAction { L, H };

public class InputRec
{
    public InputRec(InputAction _action, float _timeStamp)
    {
        action = _action;
        timeStamp = _timeStamp;
        deltaTime = 0;
    }
    public InputAction action;
    public float timeStamp;
    public float deltaTime;
};
public struct ComboRec
{
    public ComboRec(ComboStep _action, float _timeStamp)
    {
        comboStep = _action;
        timeStamp = _timeStamp;
    }
    public ComboStep comboStep;
    public float timeStamp;
};
public class ComboHandler : MonoBehaviour
{
    [SerializeField] int MaxActions = 3;
    [SerializeField] float inputLifetime = 1;
    [SerializeField] ComboSequence[] availableSqeuences;
    List<ComboSequence> activePossiblitlities = new List<ComboSequence>();
    List<ComboRec> tempSequenceWithTimeStamp;
    List<InputAction> movesActive = new List<InputAction>();
    internal ComboStep comboStepActive;
    List<InputRec> inputHistory;
    bool sequenceStarted;
    int sequenceIncrementor;
    Animator animator;
    int lastSequenceNumber = -1;
    float elapsedSequenceTime;
    bool canTakeInput;
    string lastAttackSequence = "";

    private void Start()
    {
        animator = GetComponent<Animator>();
        inputHistory = new List<InputRec>(MaxActions);
        tempSequenceWithTimeStamp = new List<ComboRec>(MaxActions);
    }
    private void Update()
    {
        canTakeInput = sequenceStarted ? false : true;
        if (!canTakeInput)
            CheckIfCanTakeInput();
        if (canTakeInput)
            TakeInput();
        CheckInputValidity();
        GetInputBufferDeltaTimes();
        RegisterValidAttacks();
        ExecuteQueuedAttacks();
        //Clear After Sequence Ends
        //AnalyzeInput();
        //ExecuteInput();
    }
    void CheckIfCanTakeInput()
    {
        var input = inputHistory[sequenceIncrementor - 1];
        var step = comboStepActive;
        if (input.action == step.action && input.deltaTime >= step.minChainTime && input.deltaTime <= step.maxChainTime)
            canTakeInput = true;
        else
            canTakeInput = false;
    }
    void TakeInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            inputHistory.Add(new InputRec(InputAction.L, Time.time));

        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            inputHistory.Add(new InputRec(InputAction.H, Time.time));
        }
    }
    void CheckInputValidity()
    {
        while (inputHistory.Count > MaxActions)
        {
            inputHistory.RemoveAt(0);
        }
        //for (int i = 0; i < inputHistory.Count; i++)
        //{
        //    if (Time.time - inputHistory[i].timeStamp > inputLifetime)
        //    {
        //        inputHistory.RemoveAt(i);
        //        i--;
        //    }
        //}
    }
    void GetInputBufferDeltaTimes()
    {
        for (int i = 0; i < inputHistory.Count; i++)
        {
            if (i == 0)
                inputHistory[i].deltaTime = Time.time - inputHistory[i].timeStamp;
            else
                inputHistory[i].deltaTime = inputHistory[i].timeStamp - inputHistory[i - 1].timeStamp;
        }
    }
    void ProcessInputToGetPossibleSequences()
    {
        if (lastSequenceNumber != sequenceIncrementor)
        {
            lastSequenceNumber = sequenceIncrementor;
            if (inputHistory.Count == 1)
            {
                activePossiblitlities.Clear();
                for (int i = 0; i < availableSqeuences.Length; i++)
                {
                    if (inputHistory[0].action == availableSqeuences[i].sequenceSteps[0].action)
                    {
                        activePossiblitlities.Add(availableSqeuences[i]);
                    }
                }
            }
            else
            {
                bool noMatch = false;
                List<ComboSequence> tempPossibilities = new List<ComboSequence>();
                for (int i = 0; i < activePossiblitlities.Count; i++)
                {
                    for (int j = 0; j < sequenceIncrementor; j++)
                    {
                        if (inputHistory[j].action != activePossiblitlities[i].sequenceSteps[j].action)
                        {
                            noMatch = true;
                        }
                    }
                    if (!noMatch)
                    {
                        tempPossibilities.Add(activePossiblitlities[i]);
                    }
                    else
                        noMatch = false;
                }
                activePossiblitlities = tempPossibilities;
            }
        }

    }
    void RegisterValidAttacks()
    {
        if (inputHistory.Count > 0)
        {
            if (!sequenceStarted && sequenceIncrementor == 0)
            {
                sequenceIncrementor++;
                sequenceStarted = true;
                movesActive.Add(inputHistory[0].action);
                ProcessInputToGetPossibleSequences();
                comboStepActive = activePossiblitlities[0].sequenceSteps[0];

            }
            else
            {
                for (int i = 0; i < activePossiblitlities.Count; i++)
                {
                    if (inputHistory.Count > sequenceIncrementor)
                    {
                        var input = inputHistory[sequenceIncrementor];
                        var currentStep = activePossiblitlities[i].sequenceSteps[sequenceIncrementor - 1];
                        var nextStep = activePossiblitlities[i].sequenceSteps[sequenceIncrementor];
                        if (input.action == nextStep.action && input.deltaTime >= currentStep.minChainTime && input.deltaTime <= currentStep.maxChainTime)
                        {
                            sequenceIncrementor++;
                            movesActive.Add(nextStep.action);
                            comboStepActive = nextStep;
                            break;
                        }
                    }
                }
                ProcessInputToGetPossibleSequences();
            }

        }

    }

    void ExecuteQueuedAttacks()
    {
        if (movesActive.Count > 0)
        {
            animator.SetBool("Attacking", true);
            EventManager.TriggerEvent("Attacking", true);
            string attackSequence = "";
            foreach (var item in movesActive)
            {
                attackSequence += item.ToString();
            }
            if (attackSequence != "" && attackSequence != lastAttackSequence)
            {
                Log.Print(attackSequence);
                lastAttackSequence = attackSequence;
                animator.SetTrigger(attackSequence);
                EventManager.TriggerEvent("Attack Will Land In", comboStepActive.animationClip.events[0].time);
            Debug.Log(comboStepActive.animationClip.events[0].time);
            }
            if (movesActive.Count == 4)
            {
                ResetSequence();
            }
        }
        else
        {
            lastAttackSequence = "";
            animator.SetBool("Attacking", false);
            EventManager.TriggerEvent("Attacking", false);
            Log.ClearAfterDelay(1);
        }
    }
    public void ResetSequence()
    {
        sequenceIncrementor = 0;
        sequenceStarted = false;
        lastSequenceNumber = -1;
        lastAttackSequence = "";
        movesActive.Clear();
        comboStepActive = null;
        inputHistory.Clear();
        activePossiblitlities.Clear();
        ResetAllTriggers();
    }


    private void ResetAllTriggers()
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }
}
