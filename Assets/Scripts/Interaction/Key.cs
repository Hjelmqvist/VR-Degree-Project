using Hjelmqvist.VR;
using System.Collections;
using UnityEngine;

public class Key : Interactable
{
    [Header("Key specific")]
    [SerializeField] KeyMoveType moveType = KeyMoveType.Snap;
    [SerializeField] float lerpTime = 0.2f;
    [SerializeField] bool setParentOnSuccess = true;
    [SerializeField] bool canBeTakenFromCorrectLock = false;

    bool isInLock = false;
    bool inCorrectLock = false;

    public enum KeyMoveType
    {
        None,
        Snap,
        Lerp
    }

    public override bool CanBeGrabbed(bool ranged) => base.CanBeGrabbed(ranged) && (!isInLock  || (canBeTakenFromCorrectLock || !inCorrectLock));

    public void PlaceInLock(Lock lockObject, bool correctKey)
    {
        isInLock = true;
        inCorrectLock = correctKey;

        if (correctKey && setParentOnSuccess)
        {
            transform.SetParent(lockObject.transform);
        }

        if (holdingHand && moveType != KeyMoveType.None)
        {
            holdingHand.DropInteractable();
        }

        MoveToLock(lockObject, correctKey);
    }

    public override void Pickup(Hand hand)
    {
        base.Pickup(hand);
        transform.SetParent(null);
        isInLock = false;
        rb.isKinematic = false;
        StopAllCoroutines();
    }

    private void MoveToLock(Lock lockObject, bool correctKey)
    {
        rb.isKinematic = moveType != KeyMoveType.None;

        switch (moveType)
        {
            case KeyMoveType.None:
                if (correctKey)
                {
                    lockObject.ToggleLock(false);
                }
                break;

            case KeyMoveType.Snap:
                transform.position = lockObject.transform.position;
                transform.rotation = lockObject.transform.rotation;
                if (correctKey)
                {
                    lockObject.ToggleLock(false);
                }
                break;

            case KeyMoveType.Lerp:
                StartCoroutine(LerpToLock(lockObject, correctKey));
                break;
        }
    }

    IEnumerator LerpToLock(Lock lockObject, bool correctKey)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = lockObject.transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = lockObject.transform.rotation;

        for (float time = 0; time < lerpTime; time += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time / lerpTime);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, time / lerpTime);
            yield return null;
        }

        transform.position = endPosition;
        transform.rotation = endRotation;

        if (correctKey)
        {
            lockObject.ToggleLock(false);
        }
    }
}
