using UnityEngine;
using System.Collections;

public class CustomerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float purchaseDelay = 2f;
    [SerializeField] private float zPosition = -1f; // Set this to be in front of 

    private Vector3 startPosition;
    private Vector3 counterPosition;
    private Vector3 endPosition;
    private Coroutine movementCoroutine;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(Vector3 start, Vector3 counter, Vector3 end)
    {
        startPosition = new Vector3(start.x, start.y, zPosition);
        counterPosition = new Vector3(counter.x, counter.y, zPosition);
        endPosition = new Vector3(end.x, end.y, zPosition);
        
        transform.position = startPosition;
        movementCoroutine = StartCoroutine(MovementSequence());
    }

    private IEnumerator MovementSequence()
    {
        yield return StartCoroutine(MoveToPosition(counterPosition));
        
        if (animator != null) animator.enabled = false;
        // notifying the gamemager to play the dollar animation
        GameManager.Instance.PlayDollarAnimation(counterPosition);
        yield return new WaitForSeconds(purchaseDelay);
        if (animator != null) animator.enabled = true;

        yield return StartCoroutine(MoveToPosition(endPosition));

        Destroy(gameObject);
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (GameManager.Instance.IsGameOver)
            {
                yield break;
            }

            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            newPosition.z = zPosition;
            transform.position = newPosition;
            
            if (animator != null)
            {
                float direction = Mathf.Sign(target.x - transform.position.x);
            }

            yield return null;
        }
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
        
        if (!GameManager.Instance.IsGameOver)
        {
            try
            {
                int itemID = Random.Range(1, 6); // Milk, Bread, Apple, Eggs, Cheese
                int quantity = Random.Range(1, 4);
                int satisfactionChange = 5;

                var item = DatabaseManager.Instance.GetItemByID(itemID);
                if (item == null)
                {
                    Debug.LogWarning($"Item {itemID} not found for purchase");
                    return;
                }

                if (DatabaseManager.Instance.GetItemStock(itemID) < quantity)
                {
                    Debug.LogWarning($"Insufficient stock for item {itemID}: {item.Name}");
                    return;
                }

                int customerID = DatabaseManager.Instance.AddNewCustomer();
                if (customerID == 0)
                {
                    Debug.LogError("Failed to create new customer for transaction");
                    return;
                }

                float totalCost = quantity * item.UnitPrice;
                DatabaseManager.Instance.RecordTransaction(customerID, itemID, quantity, totalCost, satisfactionChange);
                DatabaseManager.Instance.UpdateItemStock(itemID, quantity);
                GameManager.Instance.UpdateStats(-quantity, satisfactionChange, Mathf.RoundToInt(totalCost), 0);

                UIManager uiManager = FindFirstObjectByType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.UpdateUI();
                }

                Debug.Log($"Customer {customerID} purchased {quantity} of {item.Name} (ID: {itemID}) for {totalCost}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to process purchase: {e.Message}\nStackTrace: {e.StackTrace}");
            }
        }
    }
}