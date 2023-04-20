using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Class for interactables where two hands can be used.
/// This acts as the main hand and a Handle is used for the secondary hand.
/// </summary>
public class MainHandle : Handle
{
    [SerializeField] protected Handle secondaryHandle;

    protected virtual bool IsUsingTwoHands => secondaryHandle && secondaryHandle.IsGrabbed;

    protected virtual void Update()
    {
        if (IsGrabbed)
        {
            MoveHandToHandle();
        }
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
    }

    public override void HeldFixedUpdate(float step)
    {
        SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
        Vector3 targetPosition = handInput.TransformPoint(snapshot.position);
        rb.SetVelocity(targetPosition, step);
        Rotate(snapshot, step);
    }

    protected void Rotate(SteamVR_Skeleton_PoseSnapshot snapshot, float step)
    {
        Quaternion targetRotation;

        if (IsUsingTwoHands)
        {
            targetRotation = Quaternion.LookRotation(secondaryHandle.HandInput.position - handInput.position);
        }
        else
        {
            targetRotation = handInput.rotation * snapshot.rotation;
        }

        rb.SetAngularVelocity(targetRotation, step);
    }
}