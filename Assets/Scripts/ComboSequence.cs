using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
[System.Serializable]
public class ComboStep
{
    public InputAction action;
    public float minChainTime;
    public float maxChainTime;
}
[CreateAssetMenu(fileName ="Combo",menuName ="SO/ComboSO")]
public class ComboSequence:ScriptableObject
{
    public ComboStep[] sequenceSteps;
}

