using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Color = UnityEngine.Color;

// Notes
// We want to get a raycast click onto the tile we're selecting
// to select the tile, but after a tile is selected, temporarily 
// disable the raycast selection (through if statement) to allow for
// click on menu to then allow the movement to occur. Have two functions
// that allow one to select the tile, then the other to move the unit 
// selected.

public class tileMap : MonoBehaviour {
    public Tilemap tilemap;
    public tileScript tile;
    private SpriteRenderer spr;
    private GameObject selectedUnit = null;
    public bool isMovingUnit = false;
    public bool isAttacking = false;
    public actionManager acMan;
    // Start is called before the first frame update
    void Start() {
        acMan = FindObjectOfType<actionManager>();
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

    public unitScript getUnitAP() {
        if (selectedUnit == null) {
            Debug.LogWarning("No unit selected.");
            return null;
        }
        
        unitScript us = selectedUnit.GetComponent<unitScript>();
        if (us != null) {
            return us;    
        }
        else {
            Debug.LogError("Unit script not found.");
            return null;
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

    void CheckTileClick(Vector3 mousePos) {
        
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
                if (clickedTile.hasUnit && selectedUnit == null) {
                    selectedUnit = clickedTile.unit;
                    Debug.Log("Unit selected");
                    acMan.OpenActionPanel(selectedUnit.GetComponent<unitScript>());
                }
                // second click to move to another tile
                else if (isMovingUnit && selectedUnit != null) {
                    // logic for unit already on tile
                    if (!clickedTile.hasUnit) {
                        MoveUnitToTile(selectedUnit, tile, clickedTile);
                        isMovingUnit = false;
                        selectedUnit = null;
                    }
                    else {
                        Debug.Log("Cannot move to a tile that already has a unit.");
                    }
                }
                
                else if (isAttacking && selectedUnit != null) {
                    if (clickedTile.hasUnit) {
                        unitScript us = clickedTile.unit.GetComponent<unitScript>();
                        us.unitHealth -= 10;
                        us.actionUse();
                        isAttacking = false;
                        selectedUnit = null;
                    }
                }
                
                else {
                    acMan.CloseActionPanel();
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
            
            //Debug.Log($"Sprite clicked: {tile.name} at {hit.transform.position}");
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
