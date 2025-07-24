using UnityEngine;

public class StoreLayout : MonoBehaviour
{
    [Header("Item Interaction Areas")]
    public Vector2 milkPosition = new Vector2(-2f, -2f);
    public Vector2 breadPosition = new Vector2(0f, -2f);
    public Vector2 applePosition = new Vector2(2f, -2f);
    public Vector2 interactionAreaSize = new Vector2(1f, 1f); // Size of the invisible trigger area

    private void Start()
    {
        CreateItemInteractionAreas();
    }

    private void CreateItemInteractionAreas()
    {
        // Create invisible trigger areas for each item
        CreateItemTrigger(milkPosition, 1, "Milk Interaction Area");
        CreateItemTrigger(breadPosition, 2, "Bread Interaction Area");
        CreateItemTrigger(applePosition, 3, "Apple Interaction Area");
    }

    private void CreateItemTrigger(Vector2 position, int itemID, string areaName)
    {
        // Create an empty GameObject for the trigger
        GameObject triggerArea = new GameObject(areaName);
        triggerArea.transform.parent = transform;
        triggerArea.transform.position = new Vector3(position.x, position.y, 0);

        // Add BoxCollider2D as trigger
        BoxCollider2D collider = triggerArea.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = interactionAreaSize;

        // Add ItemComponent
        ItemComponent itemComponent = triggerArea.AddComponent<ItemComponent>();
        itemComponent.SetItemID(itemID);
        
        // Instead of using a tag, we'll use a layer
        // The default layer (0) is fine, no need to set it explicitly
    }
} 