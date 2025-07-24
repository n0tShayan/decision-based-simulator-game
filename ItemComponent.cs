using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    [SerializeField] private int itemID;
    public int ItemID => itemID;

    public void SetItemID(int id)
    {
        itemID = id;
    }

    private void Start()
    {
        // Ensure the item has a collider
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        
        // No need to set tags anymore
    }
} 