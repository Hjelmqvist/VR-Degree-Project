using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    [SerializeField] ConfigurableJoint joint;
    [SerializeField] float clickDistance;
    [SerializeField] float releaseDistance;
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
            if (Vector3.Distance(startPosition, pushObject.localPosition) > clickDistance)
            {
                wasClicked = true;
                OnClicked.Invoke(value);
            }
        }
        else if (Vector3.Distance(startPosition, pushObject.localPosition) < releaseDistance)
        {
            wasClicked = false;
            OnRelease.Invoke();
        }
    }
}
