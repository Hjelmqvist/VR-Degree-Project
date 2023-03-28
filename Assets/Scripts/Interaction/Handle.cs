using Hjelmqvist.VR;
using UnityEngine;
using Valve.VR;

public class Handle : Interactable
{
    [Header("Handle specific")]
    [SerializeField] Vector3 handOffset;
    [SerializeField] Rigidbody bodyToMove;
    [SerializeField] Collider[] collidersToIgnore;

    HandController controller;
    Rigidbody handRigidbody;
    Transform handTransform;
    Transform handInput;

    const float MinDistanceToMove = 0.01f;
    const float DragMultiplier = 4f;
    const float BreakDistance = 0.8f;
    const float BreakDotRotationThreshold = 0f;

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);

        for (int i = 0; i < collidersToIgnore.Length; i++)
        {
            Physics.IgnoreCollision(collidersToIgnore[i], hand.Collider);
        }

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

        for (int i = 0; i < collidersToIgnore.Length; i++)
        {
            Physics.IgnoreCollision(collidersToIgnore[i], hand.Collider, false);
        }

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

        handRigidbody.velocity = Vector3.zero;
        handRigidbody.angularVelocity = Vector3.zero;
        handTransform.position = transform.TransformPoint(-snapshot.position + offset);
        handTransform.rotation = transform.rotation * Quaternion.Inverse(snapshot.rotation);

        float handDistance = Vector3.Distance(handTransform.position, handInput.position);

        if (bodyToMove && handDistance > MinDistanceToMove)
        {
            bodyToMove.AddForceAtPosition((handInput.position - handTransform.position) * DragMultiplier, transform.position, ForceMode.Acceleration);
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