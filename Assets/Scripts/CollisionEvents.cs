using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    [SerializeField] UnityEvent OnEnter;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        OnEnter.Invoke();
    }
}