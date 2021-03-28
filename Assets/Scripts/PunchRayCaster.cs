using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchRayCaster : MonoBehaviour
{
    public RaycastHit PunchCast(float range)
    {
        Debug.DrawRay(transform.position, transform.position + transform.up * range, Color.red, 5);
        Physics.Raycast(transform.position, transform.up, out RaycastHit hit, range);
        return hit;
    }
}
