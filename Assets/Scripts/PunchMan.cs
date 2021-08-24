using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PunchMan : MonoBehaviour
{
    [SerializeField] GameObject hitParticlesPrefab;
    CinemachineVirtualCamera playerCam;
    CinemachineImpulseSource impulseSource;
    ComboHandler comboHandler;
    Animator animator;
    [SerializeField] PunchRayCaster rightHandRayCaster;
    [SerializeField] PunchRayCaster leftHandRayCaster;
    [SerializeField] PunchRayCaster rightLegRayCaster;
    [SerializeField] PunchRayCaster leftLegRayCaster;
    bool checkForHit;
    PunchRayCaster myTempCaster = null;
    ComboStep myTempStep = null;
    List<Collider> alreadyHit = new List<Collider>();
    List<Rigidbody> dueForAHit = new List<Rigidbody>();
    private void Start()
    {
        playerCam = FindObjectOfType<CinemachineVirtualCamera>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        comboHandler = GetComponentInParent<ComboHandler>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (checkForHit)
        {
            var hit = myTempCaster.PunchCast(myTempStep.attackRange);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (alreadyHit.Contains(hit[i])) continue;
                    dueForAHit.Add(hit[i].attachedRigidbody);
                    alreadyHit.Add(hit[i]);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if (dueForAHit.Count > 0)
        {
            for (int i = 0; i < dueForAHit.Count; i++)
            {
                Hit(dueForAHit[i]);
            }
            dueForAHit.Clear();
        }
    }

    public void Hit(Rigidbody attachedRigidbody)
    {
        Instantiate(hitParticlesPrefab, myTempCaster.transform.position - transform.forward * 0.05f, Quaternion.identity);
        attachedRigidbody.AddForce(myTempCaster.transform.forward * myTempStep.attackPower, ForceMode.Impulse);
        impulseSource.GenerateImpulse();
    }
    public void PlayStartPunchEffects()
    {
    }
    public void PlayPunchLandedEffects()
    {
        myTempStep = comboHandler.comboStepActive;
        switch (myTempStep.limb)
        {
            case AttackLimb.RightHand:
                myTempCaster = rightHandRayCaster;
                break;
            case AttackLimb.LeftHand:
                myTempCaster = leftHandRayCaster;
                break;
            case AttackLimb.RightLeg:
                myTempCaster = rightLegRayCaster;
                break;
            case AttackLimb.LeftLeg:
                myTempCaster = leftLegRayCaster;
                break;
        }
        checkForHit = true;
    }
    public void PlayPunchEndedEffects()
    {
        checkForHit = false;
        alreadyHit.Clear();
    }

}
