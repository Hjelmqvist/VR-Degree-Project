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
        [SerializeField] Vector3 leftHandOffset;
        [SerializeField] Vector3 rightHandOffset;
        [SerializeField] Vector3 rotationEulerOffset;
        [SerializeField] protected Material hoverMaterial;
        [SerializeField] protected Handle secondaryHandle;
        [Tooltip("Colliders on parent objects that should be ignored.")]
        [SerializeField] Collider[] collidersToIgnore;
        protected List<Hand> hoveringHands = new List<Hand>();
        protected Hand holdingHand;

        protected Collider[] colliders;
        protected Rigidbody rb;
        protected SteamVR_Skeleton_Poser skeletonPoser;
        MeshRenderer[] meshRenderers;
        bool isHighlighting = false;
        bool isInteracting = false;

        protected HandController handController;
        protected Rigidbody handRigidbody;
        protected Transform handTransform;
        protected Transform handInput;

        public UnityEvent OnPickup;
        public UnityEvent OnDrop;
        public UnityEvent OnStartInteract;
        public UnityEvent OnStopInteract;

        const float PoseBlendTime = 0.1f;
        const float SkeletonBlendTime = 0f;

        public Collider[] Colliders => colliders;
        
        public virtual bool CanBeGrabbed(bool ranged) => holdingHand == null && (canBeRangedGrabbed || !ranged);
        public bool IsGrabbed => holdingHand;
        public bool IsInteracting => isInteracting;
        protected virtual bool IsUsingTwoHands => secondaryHandle && secondaryHandle.IsGrabbed;

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
            IgnoreCollision(colliders, hand.Collider, true);
            IgnoreCollision(collidersToIgnore, hand.Collider, true);
            rb.useGravity = false;

            holdingHand = hand;
            handController = hand.Controller;
            handController.IsControlling = false;
            handRigidbody = handController.RB;
            handTransform = hand.transform;
            handInput = handController.Input;

            OnPickup.Invoke();
        }

        public virtual void Drop(Hand hand)
        {
            hand.Skeleton.BlendToSkeleton(SkeletonBlendTime);
            IgnoreCollision(colliders, hand.Collider, false);
            IgnoreCollision(collidersToIgnore, hand.Collider, false);
            rb.useGravity = true;

            holdingHand = null;
            handController.IsControlling = true;

            if (isInteracting)
            {
                StopInteract();
            }

            OnDrop.Invoke();
        }

        public void IgnoreCollision(Collider[] colliders, Collider collider, bool ignore)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Physics.IgnoreCollision(colliders[i], collider, ignore);
            }
        }

        public virtual void HeldUpdate(float step)
        {
            SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
            MoveHand(snapshot);
        }

        public virtual void HeldFixedUpdate(float step)
        {
            SteamVR_Skeleton_PoseSnapshot snapshot = skeletonPoser.GetBlendedPose(holdingHand.Skeleton);
            Move(snapshot, step);
            Rotate(snapshot, step);  
        }

        protected virtual void Move(SteamVR_Skeleton_PoseSnapshot snapshot, float step)
        {
            Vector3 targetPosition = handInput.TransformPoint(snapshot.position);
            rb.SetVelocity(targetPosition, step);
        }

        protected virtual void Rotate(SteamVR_Skeleton_PoseSnapshot snapshot, float step)
        {
            Quaternion targetRotation;

            if (IsUsingTwoHands)
            {
                targetRotation = Quaternion.LookRotation(secondaryHandle.HandInput.position - handInput.position) * Quaternion.Euler(rotationEulerOffset);
            }
            else
            {
                targetRotation = handInput.rotation * snapshot.rotation;
            }

            rb.SetAngularVelocity(targetRotation, step);
        }

        protected void MoveHand(SteamVR_Skeleton_PoseSnapshot snapshot)
        {
            Vector3 offset = holdingHand.HandType == SteamVR_Input_Sources.LeftHand ? leftHandOffset : rightHandOffset;
            handRigidbody.velocity = Vector3.zero;
            handRigidbody.angularVelocity = Vector3.zero;
            handTransform.position = transform.TransformPoint(offset);
            handTransform.rotation = transform.rotation * Quaternion.Inverse(snapshot.rotation);
        }

        public virtual void StartInteract()
        {
            isInteracting = true;
            OnStartInteract.Invoke();
        }

        public virtual void StopInteract()
        {
            isInteracting = false;
            OnStopInteract.Invoke();
        }
    }
}