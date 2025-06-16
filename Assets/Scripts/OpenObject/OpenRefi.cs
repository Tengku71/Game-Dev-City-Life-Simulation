using UnityEngine;

public class OpenRefi : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        targetObject.SetActive(true);
   
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targetObject.SetActive(false);
    }
}
