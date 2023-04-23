using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class PoolCue : Interactable
{
    public override void HeldFixedUpdate(float step)
    {
        rb.freezeRotation = IsInteracting || secondaryHandle.IsInteracting;

        SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
        Vector3 targetPosition = handInput.TransformPoint(snapshot.position);

        if (IsInteracting)
        {
            Vector3 direction = targetPosition - transform.position;
            Vector3 localDirection = transform.InverseTransformDirection(direction);
            targetPosition = transform.position + transform.forward * localDirection.z;
        }

        rb.SetVelocity(targetPosition, step);
        Rotate(snapshot, step);
    }
}