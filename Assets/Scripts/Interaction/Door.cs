using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HingeJoint))]
public class Door : MonoBehaviour
{
    [SerializeField] float unlockedMinLimit;
    [SerializeField] float unlockedMaxLimit;
    [SerializeField] UnityEvent OnLocked;
    [SerializeField] UnityEvent OnUnlocked;

    HingeJoint joint;
    JointLimits lockedLimits;
    JointLimits unlockedLimits;

    private void Awake()
    {
        joint = GetComponent<HingeJoint>();
        lockedLimits = joint.limits;
        unlockedLimits = lockedLimits;
        unlockedLimits.min = unlockedMinLimit;
        unlockedLimits.max = unlockedMaxLimit;
    }

    public void Lock()
    {
        joint.limits = lockedLimits;
        OnLocked.Invoke();
    }

    public void Unlock()
    {
        joint.limits = unlockedLimits;
        OnUnlocked.Invoke();
    }
}