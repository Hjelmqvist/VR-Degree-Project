using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] ConfigurableJoint joint;
    [SerializeField] Vector3 pushedLocalPosition;
    [SerializeField] Vector3 releasedLocalPosition;
    [SerializeField] float positionPadding;
    [SerializeField] char value;
    [SerializeField] UnityEvent<char> OnClicked;
    [SerializeField] UnityEvent OnRelease;

    Transform pushObject;
    Vector3 startPosition;
    bool wasClicked = false;

    void Start()
    {
        pushObject = joint.transform;
        startPosition = pushObject.localPosition - joint.targetPosition;
        startPosition.y += joint.linearLimit.limit;
    }

    void Update()
    {
        if (!wasClicked)
        {
            if (Vector3.Distance(pushObject.localPosition, pushedLocalPosition) < positionPadding)
            {
                wasClicked = true;
                OnClicked.Invoke(value);
            }
        }
        else if (Vector3.Distance(pushObject.localPosition, releasedLocalPosition) < positionPadding)
        {
            wasClicked = false;
            OnRelease.Invoke();
        }
    }
}
