using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

[System.Serializable]
public class ComboStep
{
    public InputAction action;
    public float minChainTime; //Minimum relative time before input is accepted for next attack
    public float maxChainTime; //Maximum relative time after which input is not accepted to chain next attack
}
[CreateAssetMenu(fileName = "Combo", menuName = "SO/ComboSO")]
public class ComboSequence : ScriptableObject
{
    public ComboStep[] sequenceSteps;


}



