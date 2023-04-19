using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Class for interactables where two hands can be used.
/// This acts as the main hand and a Handle is used for the secondary hand.
/// </summary>
public class MainHandle : Handle
{
    [SerializeField] Handle secondaryHandle;

    private bool TwoHandedRotation => secondaryHandle && secondaryHandle.IsGrabbed;

    public override void Drop(Hand hand)
    {
        base.Drop(hand);
        rb.useGravity = true;
    }

    public override void HeldFixedUpdate(float step)
    {
        MoveHandToHandle();

        SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
        Vector3 targetPosition = holdingHand.Controller.Input.transform.TransformPoint(snapshot.position);
        //Quaternion targetRotation = holdingHand.transform.rotation * snapshot.rotation;

        rb.SetVelocity(targetPosition, step);
        //rb.SetAngularVelocity(targetRotation, step);

        //SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
        //Vector3 targetPosition = holdingHand.transform.TransformPoint(snapshot.position);
        //rb.SetVelocity(targetPosition, step);

        Quaternion targetRotation;

        if (TwoHandedRotation)
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
