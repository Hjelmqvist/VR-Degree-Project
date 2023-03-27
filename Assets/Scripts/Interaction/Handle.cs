using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class Handle : Interactable
{
    [Header("Handle specific")]
    [SerializeField] Vector3 handOffset;
    [SerializeField] Rigidbody bodyToMove;
    [SerializeField] float minDistanceToMove = 0.1f;
    [SerializeField] float dragMultiplier = 2f;

    HandController controller;
    Rigidbody handRigidbody;
    Transform handTransform;
    Transform handInput;

    const float BreakDistance = 0.8f;
    const float BreakDotRotationThreshold = 0f;

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);

        controller = hand.Controller;
        controller.IsControlling = false;
        handRigidbody = controller.RB;
        handTransform = hand.transform;
        handInput = controller.Input;
    }

    public override void Drop(Hand hand)
    {
        base.Drop(hand);

        rb.useGravity = false;
        controller.IsControlling = true;
        controller = null;
        handRigidbody = null;
        handTransform = null;
        handInput = null;
    }

    public override void HeldFixedUpdate(float step)
    {
        SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);

        Vector3 offset = handOffset;
        if (holdingHand.HandType == SteamVR_Input_Sources.LeftHand)
        {
            offset.x = -offset.x;
        }

        handTransform.position = transform.TransformPoint(-snapshot.position + offset);
        //controller.SetVelocity(transform.TransformPoint(-snapshot.position + offset));
        handRigidbody.angularVelocity = Vector3.zero;
        handTransform.rotation = transform.rotation * Quaternion.Inverse(snapshot.rotation);

        float handDistance = Vector3.Distance(handTransform.position, handInput.position);

        if (bodyToMove && handDistance > minDistanceToMove)
        {
            bodyToMove.AddForceAtPosition((handInput.position - handTransform.position) * dragMultiplier, transform.position, ForceMode.Acceleration);
            Debug.Log(bodyToMove.velocity);
        }

        if (handDistance > BreakDistance ||
            Vector3.Dot(handTransform.forward, handInput.forward) < BreakDotRotationThreshold)
        {
            holdingHand.DropInteractable();
            return;
        }
    }
}