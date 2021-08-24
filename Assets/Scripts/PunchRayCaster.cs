using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchRayCaster : MonoBehaviour
{
    float rangeTempForGizmos;
    [SerializeField] LayerMask targetMask;
    public Collider[] PunchCast(float range)
    {
        rangeTempForGizmos = range;
        return Physics.OverlapCapsule(transform.position, transform.position + transform.up * range, 0.1f, targetMask);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        Gizmos.DrawWireSphere(transform.position + transform.up * rangeTempForGizmos, 0.1f);
    }
}
