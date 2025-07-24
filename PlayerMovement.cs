using UnityEngine;
using System.Collections;
using Unity.Jobs;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionDelay = 2f;
    private Vector3 targetPosition;
    public float zPosition = -4f;
    [SerializeField] private float interactionRange = 1f;
    private Coroutine movementCoroutine;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetRandomTargetPosition();
        Debug.Log("Movement started");
    }

    private void SetRandomTargetPosition()
    {
        // Set initial target position (counter)
        targetPosition = new Vector3(0f, 0f, zPosition);
        movementCoroutine = StartCoroutine(MoveToTarget(targetPosition));
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        Debug.Log("Moving to target: " + transform.position);
        while ((Vector3)transform.position != target)
        {
            if (GameManager.Instance.IsGameOver)
            {
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            
            // Update animation direction
            if (animator != null)
            {
                Vector3 direction = (target - transform.position).normalized;
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
            }

            yield return null;
        }

        Debug.Log("Reached target position: " + transform.position);
        
        // Pause at counter
        if (animator != null) animator.enabled = false;
        yield return new WaitForSeconds(interactionDelay);
        if (animator != null) animator.enabled = true;

        // Move to exit
        Vector3 exitPosition = new Vector3(10f, 0f, zPosition);
        yield return StartCoroutine(MoveToTarget(exitPosition));

        // Destroy at exit
        Destroy(gameObject);
    }

    public void StopMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        StopMovement();
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();

        // Ensure the sprite is always visible
        if (spriteRenderer.enabled == false)
        {
            spriteRenderer.enabled = true;
            Debug.LogWarning("SpriteRenderer was disabled, re-enabling it.");
        }

        // Maintain constant z position
        if (transform.position.z != zPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, zPosition);
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Check for nearby items
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);
            foreach (Collider2D collider in colliders)
            {
                // Check if it has an ItemComponent instead of checking tag
                ItemComponent itemComponent = collider.GetComponent<ItemComponent>();
                if (itemComponent != null)
                {
                    // Get item ID directly from the component
                    int itemID = itemComponent.ItemID;
                    
                    // Report low stock through GameManager
                    if (GameManager.Instance.GetItemStock(itemID) < 10)
                    {
                        GameManager.Instance.ReportLowStock(itemID);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}