using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

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
    List<InputRec> inputHistory;
    bool sequenceStarted;
    int sequenceIncrementor;
    Animator animator;
    int lastSequenceNumber = -1;
    float elapsedSequenceTime;
    private void Start()
    {
        animator = GetComponent<Animator>();
        inputHistory = new List<InputRec>(MaxActions);
        tempSequenceWithTimeStamp = new List<ComboRec>(MaxActions);
    }
    private void Update()
    {
        TakeInput();
        CheckInputValidity();
        GetInputBufferDeltaTimes();
        RegisterValidAttacks();
        ExecuteQueuedAttacks();
        //Clear After Sequence Ends
        //AnalyzeInput();
        //ExecuteInput();
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
            }
            else
            {
                for (int i = 0; i < activePossiblitlities.Count; i++)
                {
                    if (inputHistory.Count > sequenceIncrementor)
                    {
                        var input = inputHistory[sequenceIncrementor];
                        var step = activePossiblitlities[i].sequenceSteps[sequenceIncrementor];
                        if (input.action == step.action && input.deltaTime >= step.minChainTime && input.deltaTime <= step.maxChainTime)
                        {
                            sequenceIncrementor++;
                            movesActive.Add(step.action);
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
            string attackSequence = "";
            foreach (var item in movesActive)
            {
                attackSequence += item.ToString();
            }
            if (attackSequence != "" && attackSequence != lastAttackSequence)
            {
                Debug.Log(attackSequence);
                lastAttackSequence = attackSequence;
                animator.SetTrigger(attackSequence);
            }
            if (movesActive.Count == 3)
            {
                ResetSequence();
            }
        }
        else
        {
            lastAttackSequence = "";
            animator.SetBool("Attacking", false);
        }
    }
    public void ResetSequence()
    {
        sequenceIncrementor = 0;
        sequenceStarted = false;
        lastSequenceNumber = -1;
        lastAttackSequence = "";
        movesActive.Clear();
        inputHistory.Clear();
        activePossiblitlities.Clear();

    }
    void AnalyzeInput()
    {

        bool addedAnAttack = false;
        for (int i = 0; i < availableSqeuences.Length; i++)
        {
            if (addedAnAttack)
                break;
            for (int j = 0; j < inputHistory.Count; j++)
            {
                InputAction action = inputHistory[j].action;
                float deltaTime = 0;
                if (j != 0)
                    deltaTime = inputHistory[j].timeStamp - inputHistory[j - 1].timeStamp;
                else
                    deltaTime = Time.time - inputHistory[j].timeStamp;
                Debug.Log(deltaTime);
                ComboStep currentSequnceStep = availableSqeuences[i].sequenceSteps[j];
                if (action == currentSequnceStep.action && deltaTime <= currentSequnceStep.maxChainTime && deltaTime >= currentSequnceStep.minChainTime)
                {
                    var step = new ComboRec(currentSequnceStep, inputHistory[j].timeStamp);
                    if (!tempSequenceWithTimeStamp.Contains(step))
                    {
                        tempSequenceWithTimeStamp.Add(step);
                        addedAnAttack = true;
                        break;
                    }
                }
                else//Sequence Doesn't Match
                {
                    break;
                }
            }
        }

        while (tempSequenceWithTimeStamp.Count > MaxActions)
        {
            tempSequenceWithTimeStamp.RemoveAt(0);
        }

        if (tempSequenceWithTimeStamp.Count > 0)
        {
            var lastStepInSequence = tempSequenceWithTimeStamp[tempSequenceWithTimeStamp.Count - 1];

            if (Time.time - lastStepInSequence.timeStamp > lastStepInSequence.comboStep.maxChainTime)
            {
                tempSequenceWithTimeStamp.Clear();
            }
        }
    }
    string lastAttackSequence = "";
    void ExecuteInput()
    {
        if (tempSequenceWithTimeStamp.Count > 0)
        {
            animator.SetBool("Attacking", true);
            string attackSequence = "";
            foreach (var item in tempSequenceWithTimeStamp)
            {
                attackSequence += item.comboStep.action.ToString();
            }
            if (attackSequence != "" && attackSequence != lastAttackSequence)
            {
                Debug.Log(attackSequence);
                lastAttackSequence = attackSequence;
                animator.SetTrigger(attackSequence);
            }
        }
        else
        {
            lastAttackSequence = "";
            animator.SetBool("Attacking", false);
        }
    }
}
