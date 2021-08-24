using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [SerializeField] float detectionDistance;
    [SerializeField] LayerMask enemyMask;
    public Collider[] enemiesDetected;
    Transform camera;
    float h;
    float v;
    Vector3 directionOfCast = Vector3.forward;
    private void Start()
    {
        camera = Camera.main.transform;
    }
    private void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        Vector3 cameraForward = Vector3.Scale(camera.forward, new Vector3(1, 0, 1)).normalized;
        directionOfCast = v * cameraForward + h * camera.right;
        enemiesDetected = Physics.OverlapCapsule(transform.position, transform.position + directionOfCast * detectionDistance, 0.2f, enemyMask);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, directionOfCast * detectionDistance);
        Gizmos.DrawSphere(transform.position + directionOfCast * detectionDistance, 0.2f);
        if (enemiesDetected.Length > 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(enemiesDetected[0].transform.position, 0.2f);
        }
    }
}
