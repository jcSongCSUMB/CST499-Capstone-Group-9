using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Tilemap tilemap;
    public GameObject unitPrefab;
    public RectTransform troopInventoryBar;  // Reference to the inventory panel RectTransform
    private Transform originalParent;

    void Start()
    {
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsPointerOverInventory())
        {
            // Return to original position if over inventory
            transform.SetParent(originalParent);
        }
        else if (IsPointerOverTilemapArea())
        {
            // Spawn unit if released over Tilemap area
            SpawnUnitOnTilemap();
            gameObject.SetActive(false);
        }
        else
        {
            // Return to original position if not over inventory or tilemap
            transform.SetParent(originalParent);
        }
    }

    private bool IsPointerOverInventory()
    {
        // Use RectTransformUtility to check if mouse is over the inventory area
        return RectTransformUtility.RectangleContainsScreenPoint(
            troopInventoryBar, Input.mousePosition, Camera.main);
    }

    private bool IsPointerOverTilemapArea()
    {
        // Convert mouse position to world position
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;

        // Check if the worldPoint is within the bounds of the tilemap collider
        Bounds tilemapBounds = tilemap.GetComponent<TilemapCollider2D>().bounds;
        return tilemapBounds.Contains(worldPoint);
    }

    private void SpawnUnitOnTilemap()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);
        Vector3 spawnPosition = tilemap.GetCellCenterWorld(cellPosition);

        Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
    }
}

