using UnityEngine;

public class CarDestroyer : MonoBehaviour
{
    private Transform exitPoint;

    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
    }

    private void Update()
    {
        if (exitPoint != null && Vector3.Distance(transform.position, exitPoint.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}