using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace Hjelmqvist.VR
{
    [RequireComponent(typeof(Rigidbody), typeof(SteamVR_Skeleton_Poser))]
    public class Interactable : MonoBehaviour
    {
        [Header("Highlight")]
        [SerializeField] Material hoverMaterial;  

        [Header("Posing"), Space(10)]
        [SerializeField] bool usePoser = true;
        [SerializeField] float poseBlendTime = 0.1f;
        [SerializeField] float skeletonBlendTime = 0f;

        MeshRenderer[] meshRenderers;
        bool isHighlighting = false;

        List<Hand> hoveringHands = new List<Hand>();
        Hand holdingHand;

        Rigidbody rb;
        SteamVR_Skeleton_Poser skeletonPoser;

        const float MaxVelocityChange = 20f;
        const float MaxAngularVelocityChange = 10f;
        const float AngularVelocitySpeed = 50f;

        public bool CanBeGrabbed => holdingHand == null;

        protected virtual void Awake()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            rb = GetComponent<Rigidbody>();
            skeletonPoser = GetComponent<SteamVR_Skeleton_Poser>();
        }

        private void UpdateHighlight()
        {
            if (hoveringHands.Count == 0) // Remove highlight
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material[] currentMaterials = meshRenderers[i].materials;
                    Material[] materials = new Material[currentMaterials.Length - 1];
                    for (int j = 0; j < materials.Length; j++)
                    {
                        materials[j] = currentMaterials[j];
                    }
                    meshRenderers[i].materials = materials;
                }
                isHighlighting = false;
            }
            else if (!isHighlighting) // Add highlight
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    Material[] currentMaterials = meshRenderers[i].materials;
                    Material[] materials = new Material[currentMaterials.Length + 1];
                    for (int j = 0; j < currentMaterials.Length; j++)
                    {
                        materials[j] = currentMaterials[j];
                    }
                    materials[materials.Length - 1] = hoverMaterial;
                    meshRenderers[i].materials = materials;
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
            holdingHand = hand;
        }

        public virtual void Drop(Hand hand)
        {
            if (usePoser)
            {
                hand.Skeleton.BlendToSkeleton(skeletonBlendTime);
            }

            rb.useGravity = true;
            holdingHand = null;
        }

        public virtual void HeldFixedUpdate(float speed)
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

            SetVelocity(targetPosition, speed);
            SetAngularVelocity(targetRotation, speed);
        }

        private void SetVelocity(Vector3 targetPosition, float speed)
        {
            Vector3 distance = targetPosition - transform.position;
            Vector3 targetVelocity = distance / Time.fixedDeltaTime;
            Vector3 velocity = targetVelocity * speed;
            rb.velocity = Vector3.MoveTowards(rb.velocity, velocity, MaxVelocityChange);
        }

        private void SetAngularVelocity(Quaternion targetRotation, float speed)
        {
            Quaternion rotationDifference = targetRotation * Quaternion.Inverse(transform.rotation);
            rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

            if (angle > 180)
                angle -= 360;

            if (angle != 0 && !float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
            {
                Vector3 angularTarget = angle * axis * speed * AngularVelocitySpeed * Time.fixedDeltaTime;
                rb.angularVelocity = Vector3.MoveTowards(rb.angularVelocity, angularTarget, MaxAngularVelocityChange);
            }    
        }

        public virtual void Interact()
        {

        }
    }
}