using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 10f; // Force applied to move the car
    [SerializeField] private float stopDuration = 2f; // Time to stop at the counter
    [SerializeField] private float maxSpeed = 5f; // Maximum speed of the car

    private Vector3 counterPoint;
    private Vector3 exitPoint;

    private Rigidbody2D rb;
    private bool isStopped = false;

    public void Initialize(Vector3 spawn, Vector3 counter, Vector3 exit)
    {
        transform.position = spawn; // Set the initial position to the spawn point
        counterPoint = counter;
        exitPoint = exit;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing on the car prefab!");
            return;
        }

        StartCoroutine(MovementSequence());
    }

    private IEnumerator MovementSequence()
    {
        // Move to the counter point
        yield return StartCoroutine(MoveToX(counterPoint.x));

        // Stop at the counter for a specified duration
        isStopped = true;
        rb.linearVelocity = Vector2.zero; // Stop the car
        yield return new WaitForSeconds(stopDuration);
        isStopped = false;

        // Move to the exit point
        yield return StartCoroutine(MoveToX(exitPoint.x));

        // Destroy the car and its wheels when it reaches the exit point
        DestroyCarAndWheels();
    }

    private IEnumerator MoveToX(float targetX)
    {
        while (Mathf.Abs(transform.position.x - targetX) > 0.1f)
        {
            if (!isStopped)
            {
                // Apply force to move the car smoothly
                float direction = Mathf.Sign(targetX - transform.position.x); // Determine direction (+1 or -1)
                if (Mathf.Abs(rb.linearVelocity.x) < maxSpeed) // Limit the speed
                {
                    rb.AddForce(new Vector2(direction * moveForce, 0f), ForceMode2D.Force);
                }
            }
            yield return null;
        }

        // Stop the car when it reaches the target
        rb.linearVelocity = Vector2.zero;
    }

    private void DestroyCarAndWheels()
    {
        // Check if the car is at the exit x-coordinate
        if (Mathf.Abs(transform.position.x - -9.2f) < 0.1f)
        {
            // Destroy the car and its wheels
            foreach (Transform child in transform.parent) // Assuming wheels are siblings of carsss_0
            {
                Destroy(child.gameObject);
            }
            Destroy(gameObject); // Destroy the car itself
        }
    }
}