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
    private GameObject selectedUnit = null;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
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

    public int getUnitAP() {
        if (selectedUnit == null) {
            Debug.LogWarning("No unit selected.");
            return 0;
        }
        
        unitScript us = selectedUnit.GetComponent<unitScript>();
        if (us != null) {
            return us.unitAP;    
        }
        else {
            Debug.LogError("Unit script not found.");
            return 0;
        }
    }

    public RaycastHit2D? GetFocusedOnTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2d, Vector2.zero);
        
        
        if (hit != null) {
            return hit;
        }

        return null;
    }
    
    void CheckSpriteClick(Vector3 mousePos) {
        RaycastHit2D? hitResult = GetFocusedOnTile();

        // check for tile hit
        if (hitResult.HasValue) {
            RaycastHit2D hit = hitResult.Value;
            tile = hit.collider.GetComponent<tileScript>();
            tileScript clickedTile = hit.collider.GetComponent<tileScript>();

            // if clicked tile
            if (clickedTile != null) {
                // first click for selection
                if (selectedUnit == null) {
                    if (clickedTile.hasUnit) {
                        selectedUnit = clickedTile.unit;
                        Debug.Log("Unit selected");
                    }
                }
                // second click to move to another tile
                else {
                    if (!clickedTile.hasUnit) {
                        MoveUnitToTile(selectedUnit, tile, clickedTile);
                        selectedUnit = null;
                    }
                    else {
                        Debug.Log("Cannot move to a tile that already has a unit.");
                    }

                    selectedUnit = null;
                    tile = null;
                }
            }
            
            if (spr != null) {
                //spr.sortingLayerName = "Units";
                Color orig = spr.color;
                orig.a = 1;
                spr.color = orig;
            }
            spr = hit.collider.GetComponent<SpriteRenderer>();
            
            Debug.Log($"Sprite clicked: {tile.name} at {hit.transform.position}");
        }

        else {
            Debug.Log("Nothing here?");
            selectedUnit = null;
        }
    }

    void MoveUnitToTile(GameObject unit, tileScript prevTile, tileScript targetTile) {
        unitScript us = unit.GetComponent<unitScript>();

        if (us.unitAP > 0) {
            unit.transform.position = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y + 0.5f, -1);
            prevTile.ClearUnit();
            targetTile.AssignUnit(unit);
            
            us.actionUse();
        }

        else {
            Debug.Log("Unit has no AP.");
        }
        
        
        
        Debug.Log("Unit moved to new tile.");
    }
}
