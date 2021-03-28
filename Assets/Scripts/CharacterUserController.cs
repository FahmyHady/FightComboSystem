// ClickToMove.cs
using UnityEngine;
using UnityEngine.AI;

public class CharacterUserController : MonoBehaviour
{
    Transform camera;
    Rigidbody rigidbody;
    int comboCounter;
    [SerializeField] bool hasControl = true;
    [SerializeField] float maxTimeBetweenAttacks = 0.5f;
    [SerializeField] int maxComboCounter;
    BzKovSoft.ObjectSlicerSamples.BzKnife bzKnife;
    float elapsedTime;
    float lastYRot;
    float lastXRot;
    float h = 0;
    float v = 0;
    CharacterLocomotion locomotion;

    void DisableControl() => hasControl = false;
    void EnableControl() => hasControl = true;

    void Start()
    {
        camera = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        locomotion = GetComponent<CharacterLocomotion>();
        bzKnife = GetComponentInChildren<BzKovSoft.ObjectSlicerSamples.BzKnife>();
    }
    void Update()
    {
        if (hasControl)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            elapsedTime += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                elapsedTime = 0;
                comboCounter++;
            }
            if (elapsedTime >= maxTimeBetweenAttacks ||comboCounter > maxComboCounter)
            {
                comboCounter = 0;
            }
            if (comboCounter>0)
            {
                bzKnife.enabled = true;
            }
            else
            {
                bzKnife.enabled = false;
            }
        }
        locomotion.UpdateAnimatorState(comboCounter);
        Vector3 cameraForward = Vector3.Scale(camera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * cameraForward + h * camera.right;
        locomotion.rotationVector = Vector3.zero;
        locomotion.upperBodyRotationEulers = Vector3.zero;
        locomotion.Move(move);

    }



}