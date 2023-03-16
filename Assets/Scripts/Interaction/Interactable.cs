using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace Hjelmqvist.VR
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {
        [Header("Highlight")]
        [SerializeField] Material hoverMaterial;
        List<Hand> hoveringHands = new List<Hand>();
        protected Hand holdingHand;

        [Header("Posing")]
        [SerializeField] bool usePoser = true;
        [SerializeField] float poseBlendTime = 0.1f;
        [SerializeField] float skeletonBlendTime = 0f;

        protected Rigidbody rb;
        SteamVR_Skeleton_Poser skeletonPoser;
        MeshRenderer[] meshRenderers;
        bool isHighlighting = false;

        public UnityEvent OnPickup;
        public UnityEvent OnDrop;

        const float MaxVelocityChange = 20f;
        const float MaxAngularVelocityChange = 10f;
        const float AngularVelocitySpeed = 50f;

        public virtual bool CanBeGrabbed => holdingHand == null;

        protected virtual void Awake()
        {
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
            if (usePoser)
            {
                hand.Skeleton.BlendToPoser(skeletonPoser, poseBlendTime);
            }
            rb.useGravity = false;
            rb.isKinematic = false;
            holdingHand = hand;
            OnPickup.Invoke();
        }

        public virtual void Drop(Hand hand)
        {
            if (usePoser)
            {
                hand.Skeleton.BlendToSkeleton(skeletonBlendTime);
            }
            rb.useGravity = true;
            holdingHand = null;
            OnDrop.Invoke();
        }

        public virtual void HeldFixedUpdate(float step)
        {
            SteamVR_Behaviour_Skeleton skeleton = holdingHand.Skeleton;
            Vector3 targetPosition;
            Quaternion targetRotation;

            if (usePoser)
            {
                targetPosition = holdingHand.transform.TransformPoint(skeletonPoser.GetBlendedPose(skeleton).position);
                targetRotation = holdingHand.transform.rotation * skeletonPoser.GetBlendedPose(skeleton).rotation;
            }
            else
            {
                targetPosition = holdingHand.transform.position;
                targetRotation = holdingHand.transform.rotation;
            }

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