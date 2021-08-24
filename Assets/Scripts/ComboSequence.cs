using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;
public enum AttackLimb { RightHand, LeftHand, RightLeg, LeftLeg }
[System.Serializable]
public class ComboStep
{
    [Header("Attack Properties")]
    public AttackLimb limb;
    public float attackPower;
    public float attackRange;
    [Header("Attack Configurations")]
    public InputAction action;
    public float minChainTime; //Minimum relative time before input is accepted for next attack
    public float maxChainTime; //Maximum relative time after which input is not accepted to chain next attack
    public AnimationClip animationClip;
    public bool autoConfigureTime;
    [Range(0, 1)] public float percentageOfMinChainTime;
    [Range(0, 1)] public float percentageOfMaxChainTime;
}
[CreateAssetMenu(fileName = "Combo", menuName = "SO/ComboSO")]
public class ComboSequence : ScriptableObject
{
    public ComboStep[] sequenceSteps;
    private void OnValidate()
    {
        for (int i = 0; i < sequenceSteps.Length; i++)
        {
            var step = sequenceSteps[i];
            if (step.autoConfigureTime)
            {
                step.minChainTime = (step.animationClip.length) - ((1 - step.percentageOfMinChainTime) * step.animationClip.length);
                step.maxChainTime = step.percentageOfMaxChainTime * step.animationClip.length;
            }
        }
    }

}



