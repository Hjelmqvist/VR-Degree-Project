using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Lock : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] float delay = 0f;
    [SerializeField] UnityEvent OnLocked;
    [SerializeField] UnityEvent OnUnlocked;
    Key placedKey;

    private void OnTriggerEnter(Collider other)
    {
        if (placedKey == null && other.TryGetComponent(out Key key))
        {
            Debug.Log("Enter");
            placedKey = key;
            bool correctKey = key.Equals(this.key);
            key.PlaceInLock(this, correctKey);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (placedKey != null && other.TryGetComponent(out Key key) && key.Equals(placedKey))
        {
            Debug.Log("Exit");
            placedKey = null;
            ToggleLock(true);
        }
    }

    public void ToggleLock(bool locked)
    {
        StartCoroutine(LockCoroutine());

        IEnumerator LockCoroutine()
        {
            yield return new WaitForSeconds(delay);
            if (locked)
            {
                OnLocked.Invoke();
            }
            else
            {
                OnUnlocked.Invoke();
            }
        }
    }
}