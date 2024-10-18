using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Tilemap tilemap;  // Tilemap reference
    public GameObject unitPrefab;  // Prefab for unit to be spawned
    public RectTransform troopInventoryBar;  // Reference to the inventory panel RectTransform
    private Transform originalParent;  // Store the original parent

    void Start()
    {
        originalParent = transform.parent;  // Save original parent
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Temporarily re-parent to avoid Grid Layout influence
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();  // Bring to front during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow mouse position during drag
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsPointerOverInventory())
        {
            // If dropped over inventory, return to original position
            transform.SetParent(originalParent);
        }
        else if (IsPointerOverTilemapArea())
        {
            // If dropped over Tilemap, spawn a unit
            SpawnUnitOnTilemap();
            gameObject.SetActive(false);  // Hide the dragged object
        }
        else
        {
            // Otherwise, return to original position
            transform.SetParent(originalParent);
        }
    }

    private bool IsPointerOverInventory()
    {
        // Check if the pointer is over the inventory panel using RectTransformUtility
        return RectTransformUtility.RectangleContainsScreenPoint(
            troopInventoryBar, Input.mousePosition, Camera.main);
    }

    private bool IsPointerOverTilemapArea()
    {
        // Convert the mouse position to world position
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;  // Ensure the Z position is correct for 2D

        // Get the TilemapCollider2D bounds
        TilemapCollider2D tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            Debug.LogError("TilemapCollider2D not found on the Tilemap!");
            return false;
        }

        Bounds tilemapBounds = tilemapCollider.bounds;
        Debug.Log("World Point: " + worldPoint + " | Tilemap Bounds: " + tilemapBounds);

        // Check if the worldPoint is within the tilemap bounds
        bool isInBounds = tilemapBounds.Contains(worldPoint);
        Debug.Log("Is Pointer Over Tilemap Area: " + isInBounds);

        return isInBounds;
    }

    private void SpawnUnitOnTilemap()
    {
        // Convert mouse position to world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;

        // Convert world position to Tilemap cell position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);
        Debug.Log("World Position: " + worldPos + " | Cell Position: " + cellPosition);

        // Get the center of the tile for correct placement
        Vector3 spawnPosition = tilemap.GetCellCenterWorld(cellPosition);
        Debug.Log("Spawn Position: " + spawnPosition);

        // Instantiate the unit prefab at the calculated spawn position
        Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Spawned Unit at: " + spawnPosition);
    }
}
