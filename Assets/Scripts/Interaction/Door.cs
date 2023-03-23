using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class Door : MonoBehaviour
{
    [SerializeField] float unlockedMinLimit;
    [SerializeField] float unlockedMaxLimit;

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
    }

    public void Unlock()
    {
        joint.limits = unlockedLimits;
    }
}