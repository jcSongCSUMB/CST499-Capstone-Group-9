using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

public class tileMap : MonoBehaviour {
    public Tilemap tilemap;
    public tileScript tile;
    private SpriteRenderer spr;
    public GameObject currentUnit;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        // This line will show the mouse position in world coordinates for debugging
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log($"Mouse Position: {mousePos}");
        
        /*
        if (Input.GetMouseButtonDown(0)) { // Check for left mouse button click
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mousePos);
            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if (clickedTile != null) {
                //Debug.Log($"Tile clicked at {cellPosition}: {clickedTile}");
            } else {
                // Check if a sprite renderer GameObject was clicked
                CheckSpriteClick(mousePos);
            }
        }*/

        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckSpriteClick(mousePos);
        }

        if (tile != null) {
            Color newColor = spr.color;
            newColor.a = 0.5f + Mathf.PingPong(Time.time * 0.5f, 0.5f);
            spr.color = newColor;
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

    public RaycastHit2D? GetFocusedOnTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);

        if (hits.Length > 0) {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }

        return null;
    }

    void PositionUnit(tileScript _tile) {
        if (_tile.hasUnit) {
            unitScript us = currentUnit.GetComponent<unitScript>();

            if (us.isActive) {
                us.actionUse();
                currentUnit.transform.position = _tile.transform.position + new Vector3(0, 0.5f, 0);
                Debug.Log($"current unitAP: {us.unitAP}");
            }
            
        }
    }
    
    void CheckSpriteClick(Vector3 mousePos) {
        // Cast a ray from the mouse position
        //Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);
        //RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero); // Use Vector2.zero for 2D
        RaycastHit2D? hitResult = GetFocusedOnTile();

        if (hitResult.HasValue) {
            RaycastHit2D hit = hitResult.Value;
            if (tile != null) {
                Color orig = spr.color;
                orig.a = 1;
                spr.color = orig;
            }
            
            // If something was hit, log the name of the GameObject
            tile = hit.collider.GetComponent<tileScript>();
            spr = hit.collider.GetComponent<SpriteRenderer>();
            PositionUnit(tile);
            Debug.Log($"Sprite clicked: {tile.name} at {hit.transform.position}");
        }

        else {
            Debug.Log("Nothing here?");
        }
    }
}
