using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class PoolCue : Interactable
{
    [SerializeField] PoolLine poolLine;
    [SerializeField] PoolCueTip cueTip;
    Vector3 startPosition;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);
        secondaryHandle.gameObject.SetActive(true);
        poolLine.gameObject.SetActive(true);
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        secondaryHandle.gameObject.SetActive(false);
        poolLine.gameObject.SetActive(false);
        rb.freezeRotation = false;
    }

    public override void HeldFixedUpdate(float step)
    {
        base.HeldFixedUpdate(step);
        rb.freezeRotation = IsInteracting || secondaryHandle.IsInteracting;
    }

    protected override void Move(SteamVR_Skeleton_PoseSnapshot snapshot, float step)
    {
        Vector3 targetPosition;

        if (IsInteracting)
        {
            Vector3 direction = handInput.TransformPoint(snapshot.position) - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            targetPosition = transform.position + transform.forward * localDirection.z;
            Debug.Log($"Current Position: {transform.position}, Target: {targetPosition}, Direction: {direction}");
        }
        else
        {
            targetPosition = handInput.TransformPoint(snapshot.position);
        }
        rb.SetVelocity(targetPosition, step);
    }

    public override void StartInteract()
    {
        base.StartInteract();
        cueTip.gameObject.SetActive(true);
    }

    public override void StopInteract()
    {
        base.StopInteract();
        cueTip.gameObject.SetActive(false);
    }
}