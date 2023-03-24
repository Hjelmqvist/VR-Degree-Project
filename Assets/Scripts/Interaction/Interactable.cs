using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace Hjelmqvist.VR
{
    [RequireComponent(typeof(Rigidbody), typeof(SteamVR_Skeleton_Poser))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] protected bool canBeRangedGrabbed = true;
        [SerializeField] protected int itemLayer = 7;
        [SerializeField] protected int playerLayer = 6;

        [Header("Highlight")]
        [SerializeField] protected Material hoverMaterial;
        protected List<Hand> hoveringHands = new List<Hand>();
        protected Hand holdingHand;

        [Header("Posing")]
        [SerializeField] protected float poseBlendTime = 0.1f;
        [SerializeField] protected float skeletonBlendTime = 0f;

        protected Collider collider;
        protected Rigidbody rb;
        protected SteamVR_Skeleton_Poser skeletonPoser;
        MeshRenderer[] meshRenderers;
        bool isHighlighting = false;

        public UnityEvent OnPickup;
        public UnityEvent OnDrop;

        const float MaxVelocityChange = 20f;
        const float MaxAngularVelocityChange = 10f;
        const float AngularVelocitySpeed = 50f;

        public virtual bool CanBeGrabbed(bool ranged) => holdingHand == null && (canBeRangedGrabbed || !ranged);

        protected virtual void Awake()
        {
            collider = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            skeletonPoser = GetComponent<SteamVR_Skeleton_Poser>();
        }

        private void UpdateHighlight()
        {
            if (hoveringHands.Count == 0) // Remove highlight
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material[] current = meshRenderers[i].materials;
                    Material[] updated = new Material[current.Length - 1];
                    for (int j = 0; j < updated.Length; j++)
                    {
                        updated[j] = current[j];
                    }
                    meshRenderers[i].materials = updated;
                }
                isHighlighting = false;
            }
            else if (!isHighlighting) // Add highlight
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material[] current = meshRenderers[i].materials;
                    Material[] updated = new Material[current.Length + 1];
                    for (int j = 0; j < current.Length; j++)
                    {
                        updated[j] = current[j];
                    }
                    updated[updated.Length - 1] = hoverMaterial;
                    meshRenderers[i].materials = updated;
                }
                isHighlighting = true;
            }
        }

        public virtual void StartHover(Hand hand)
        {
            hoveringHands.Add(hand);
            UpdateHighlight();
        }

        public virtual void StopHover(Hand hand)
        {
            hoveringHands.Remove(hand);
            UpdateHighlight();
        }

        public virtual void Pickup(Hand hand)
        {
            hand.Skeleton.BlendToPoser(skeletonPoser, poseBlendTime);
            rb.useGravity = false;
            holdingHand = hand;
            Physics.IgnoreCollision(collider, hand.Collider);
            OnPickup.Invoke();
        }

        public virtual void Drop(Hand hand)
        {
            hand.Skeleton.BlendToSkeleton(skeletonBlendTime);
            Physics.IgnoreCollision(collider, hand.Collider, false);
            rb.useGravity = true;
            holdingHand = null;
            OnDrop.Invoke();
        }

        public virtual void HeldFixedUpdate(float step)
        {
            SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
            Vector3 targetPosition = holdingHand.transform.TransformPoint(snapshot.position);
            Quaternion targetRotation = holdingHand.transform.rotation * snapshot.rotation;

            SetVelocity(targetPosition, step);
            SetAngularVelocity(targetRotation, step);
        }

        private void SetVelocity(Vector3 targetPosition, float step)
        {
            Vector3 distance = targetPosition - transform.position;
            Vector3 targetVelocity = distance / Time.fixedDeltaTime;
            Vector3 velocity = targetVelocity * step;
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocity, MaxVelocityChange);
        }

        private void SetAngularVelocity(Quaternion targetRotation, float step)
        {
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);
            rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180)
                angle -= 360;

            if (angle != 0 && !float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
            {
                Vector3 angularTarget = angle * axis * step * AngularVelocitySpeed * Time.fixedDeltaTime;
                rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, MaxAngularVelocityChange);
            }    
        }

        public virtual void Interact()
        {

        }
    }
}