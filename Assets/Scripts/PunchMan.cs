using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PunchMan : MonoBehaviour
{
    [SerializeField] float punchRange;
    [SerializeField] GameObject hitParticlesPrefab;
    CinemachineVirtualCamera playerCam;
    CinemachineImpulseSource impulseSource;
    Animator animator;
    PunchRayCaster rayCaster;
    private void Start()
    {
        rayCaster = GetComponentInChildren<PunchRayCaster>();
        playerCam = FindObjectOfType<CinemachineVirtualCamera>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        animator = GetComponent<Animator>();
    }

    public void PlayStartPunchEffects()
    {

    }
    public void PlayPunchLandedEffects()
    {
        RaycastHit hit = rayCaster.PunchCast(punchRange);
        if (hit.transform != null)
        {
            Instantiate(hitParticlesPrefab, hit.point-transform.forward*0.1f, Quaternion.identity);
        }
        impulseSource.GenerateImpulse();
    }
    public void PlayPunchEndedEffects()
    {
    }
 
}
