using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform head;

    [SerializeField] SteamVR_Action_Vector2 movementAction = SteamVR_Input.GetVector2Action("Movement");
    [SerializeField] float movementSpeed = 3;

    [Space(10)]
    [SerializeField] SteamVR_Action_Vector2 rotationAction = SteamVR_Input.GetVector2Action("Rotation");
    [SerializeField] float rotationSpeed = 180;

    CharacterController controller;

    Vector3 gravity = Physics.gravity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Move();
        ApplyGravity();
        Rotate();
    }

    private void Move()
    {
        Vector2 input = movementAction.axis;
        Vector3 direction = head.TransformDirection(new Vector3(input.x, 0, input.y));
        Vector3 movement = direction * movementSpeed * Time.deltaTime;
        controller.Move(movement);
    }

    private void ApplyGravity()
    {
        controller.Move(gravity * Time.deltaTime);
    }

    private void Rotate()
    {
        Vector2 input = rotationAction.axis;
        Vector3 rotation = new Vector3(0, input.x) * rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation);
    }
}