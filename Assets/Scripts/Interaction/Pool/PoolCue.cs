using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class PoolCue : Interactable
{
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

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        rb.freezeRotation = false;
    }

    public override void HeldFixedUpdate(float step)
    {
        base.HeldFixedUpdate(step);
        rb.freezeRotation = IsInteracting || secondaryHandle.IsInteracting;
    }

    protected override void Move(SteamVR_Skeleton_PoseSnapshot snapshot, float step)
    {
        Vector3 targetPosition = handInput.TransformPoint(snapshot.position);

        if (IsInteracting)
        {
            Vector3 direction = handInput.position - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            targetPosition = transform.position + transform.forward * localDirection.z;
            Debug.Log($"Current Position: {transform.position}, Target: {targetPosition}, Direction: {direction}");
        }

        rb.SetVelocity(targetPosition, step);
    }
}