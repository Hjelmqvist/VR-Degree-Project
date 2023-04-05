using UnityEngine;
using UnityEngine.Events;

public class BuzzWirePuzzle : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] UnityEvent OnOpened;
    [SerializeField] UnityEvent OnClosed;

    public void SetState(bool open)
    {
        animator.SetBool("IsOpen", open);
        if (open)
        {
            OnOpened.Invoke();
        }
        else
        {
            OnClosed.Invoke();
        }
    }
}