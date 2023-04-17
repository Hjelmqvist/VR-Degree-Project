using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace Hjelmqvist.VR
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(SteamVR_Skeleton_Poser))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] protected bool canBeRangedGrabbed = true;

        [Header("Highlight")]
        [SerializeField] protected Material hoverMaterial;
        protected List<Hand> hoveringHands = new List<Hand>();
        protected Hand holdingHand;

        protected Collider[] colliders;
        protected Rigidbody rb;
        protected SteamVR_Skeleton_Poser skeletonPoser;
        MeshRenderer[] meshRenderers;
        bool isHighlighting = false;

        public UnityEvent OnPickup;
        public UnityEvent OnDrop;

        const float PoseBlendTime = 0.1f;
        const float SkeletonBlendTime = 0f;

        public Collider[] Colliders => colliders;

        public virtual bool CanBeGrabbed(bool ranged) => holdingHand == null && (canBeRangedGrabbed || !ranged);

        protected virtual void Awake()
        {
            colliders = GetComponents<Collider>();
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
            hand.Skeleton.BlendToPoser(skeletonPoser, PoseBlendTime);
            IgnoreHandCollision(hand, true);
            rb.useGravity = false;
            holdingHand = hand;
            OnPickup.Invoke();
        }

        public virtual void Drop(Hand hand)
        {
            hand.Skeleton.BlendToSkeleton(SkeletonBlendTime);
            IgnoreHandCollision(hand, false);
            rb.useGravity = true;
            holdingHand = null;
            OnDrop.Invoke();
        }

        private void IgnoreHandCollision(Hand hand, bool ignore)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Physics.IgnoreCollision(colliders[i], hand.Collider, ignore);
            }
        }

        public virtual void HeldFixedUpdate(float step)
        {
            SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
            Vector3 targetPosition = holdingHand.transform.TransformPoint(snapshot.position);
            Quaternion targetRotation = holdingHand.transform.rotation * snapshot.rotation;

            rb.SetVelocity(targetPosition, step);
            rb.SetAngularVelocity(targetRotation, step);
        }

        public virtual void Interact()
        {
            // Override to add functionality for things like firing guns.
        }
    }
}