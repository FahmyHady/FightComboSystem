using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttackAssistanceComponent : MonoBehaviour
{
    Rigidbody rb;
    EnemyDetector enemyDetector;
    [SerializeField] float stoppingDistanceFromTarget = 2;
    [SerializeField] float lookAtSmoothingDuration = 0.25f;
    [SerializeField] [Range(0, 1)] float helpPercentage = 1;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyDetector = GetComponentInChildren<EnemyDetector>();
    }
    private void OnEnable()
    {
        EventManager.StartListening("Attack Will Land In", (UnityAction<float>)AttackTriggered);
    }

    private void AttackTriggered(float timeDue)
    {
        if ( enemyDetector.enemiesDetected.Length > 0)
        {
            Transform target = enemyDetector.enemiesDetected[0].transform;
            transform.DODynamicLookAt(target.position, lookAtSmoothingDuration);
            Vector3 lookRotationVector = Quaternion.LookRotation(target.position) * Vector3.forward;
            Vector3 targetPos = target.position - (lookRotationVector * stoppingDistanceFromTarget * helpPercentage);
            targetPos.y = rb.position.y;
            rb.DOMove(targetPos, timeDue);
        }
    }
    //private void Update()
    //{
    //    if ( enemyDetector.enemiesDetected.Length > 0)
    //    {
    //        Transform target = enemyDetector.enemiesDetected[0].transform;
    //        transform.DOLookAt(target.position, lookAtSmoothingDuration);
    //        Vector3 lookRotationVector = Quaternion.LookRotation(target.position) * Vector3.one;
    //        Vector3 targetPos = target.position - (lookRotationVector * stoppingDistanceFromTarget * helpPercentage);
    //        targetPos.y = rb.position.y;
    //        rb.DOMove(targetPos, 2);
    //    }
    //}

}
