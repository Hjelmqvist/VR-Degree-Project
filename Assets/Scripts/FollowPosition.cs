using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] Transform followTransform;
    [SerializeField] CharacterController controller;
    [SerializeField] Transform middleBody;

    Vector3 lastPos;

    void Update()
    {
        Vector3 pos = followTransform.localPosition;
        pos.y = 0;
        Vector3 change = pos - lastPos;

        controller.Move(controller.transform.TransformDirection(change));
        middleBody.localPosition -= change;

        lastPos = pos;
    }
}