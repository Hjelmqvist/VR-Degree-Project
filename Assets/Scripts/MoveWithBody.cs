using UnityEngine;

/// <summary>
/// Moves the player object and body child object so that colliders follows the IRL body
/// </summary>
public class MoveWithBody : MonoBehaviour
{
    [SerializeField] Transform realBody;
    [SerializeField] CharacterController player;
    [SerializeField] Transform body;

    Vector3 lastPos;

    void Update()
    {
        Vector3 pos = realBody.localPosition;
        pos.y = 0;
        Vector3 change = pos - lastPos;

        player.Move(player.transform.TransformDirection(change));
        body.localPosition -= change;

        lastPos = pos;
    }
}