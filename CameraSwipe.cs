using UnityEngine;

public class CameraSwipe : MonoBehaviour
{
    public float minX = -1.66f; // Minimum x position
    public float maxX = 3.15f;  // Maximum x position
    public float swipeSpeed = 0.1f; // Speed of the swipe movement

    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isDragging = false;

    void Update()
    {
        HandleTouch();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        currentTouchPosition = touch.position;
                        Vector2 delta = currentTouchPosition - startTouchPosition;
                        float moveAmount = delta.x * swipeSpeed * Time.deltaTime;

                        // Move the camera only in the x direction within the limits
                        Vector3 newPosition = transform.position + new Vector3(moveAmount, 0, 0);
                        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                        transform.position = newPosition;

                        startTouchPosition = currentTouchPosition; // Update the start position for the next frame
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
    }
}