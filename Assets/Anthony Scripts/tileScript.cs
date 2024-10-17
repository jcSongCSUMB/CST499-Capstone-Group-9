using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class tileScript : MonoBehaviour {
    public Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        // This line will show the mouse position in world coordinates for debugging
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log($"Mouse Position: {mousePos}");
        
        if (Input.GetMouseButtonDown(0)) { // Check for left mouse button click
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mousePos);
            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if (clickedTile != null) {
                Debug.Log($"Tile clicked at {cellPosition}: {clickedTile}");
            } else {
                // Check if a sprite renderer GameObject was clicked
                CheckSpriteClick(mousePos);
            }
        }
    }
    
    /*
    void OnMouseDown() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = tilemap.WorldToCell(mousePos);
        TileBase clickedTile = tilemap.GetTile(cellPosition);

        if (clickedTile != null) {
            Debug.Log($"Tile clicked at {cellPosition}: {clickedTile}");
        } else {
            Debug.Log($"No tile found at {cellPosition}");
        }
    }
    */
    
    void CheckSpriteClick(Vector3 mousePos) {
        // Cast a ray from the mouse position
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); // Use Vector2.zero for 2D

        if (hit.collider != null) {
            // If something was hit, log the name of the GameObject
            Debug.Log($"Sprite clicked: {hit.collider.gameObject.name} at {hit.collider.gameObject.transform.position}");
        }

        else {
            Debug.Log("Nothing here?");
        }
    }
}
