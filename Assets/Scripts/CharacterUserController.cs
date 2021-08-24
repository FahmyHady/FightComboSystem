// ClickToMove.cs
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CharacterUserController : MonoBehaviour
{
    Transform camera;
    Rigidbody rigidbody;
    [SerializeField] bool hasControl = true;
    float lastYRot;
    float lastXRot;
    float h = 0;
    float v = 0;
    CharacterLocomotion locomotion;
    private void OnEnable()
    {
        EventManager.StartListening("Attacking", (UnityAction<bool>)Attacking);

    }

    void Attacking(bool flag) => hasControl = !flag;

    void Start()
    {
        camera = Camera.main.transform;
        rigidbody = GetComponent<Rigidbody>();
        locomotion = GetComponent<CharacterLocomotion>();

    }
    void Update()
    {
        if (hasControl)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }
        Vector3 cameraForward = Vector3.Scale(camera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = v * cameraForward + h * camera.right;
        locomotion.rotationVector = Vector3.zero;
        locomotion.upperBodyRotationEulers = Vector3.zero;
        locomotion.Move(move);
    }



}