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
    public tileScript tile, lastTile;
    private SpriteRenderer spr = null;
    private unitScript selectedUnit, lastUnit;
    public bool isMovingUnit = false;
    public bool isAttacking = false;
    public actionManager acMan;
    public battleManager bm;
    
    // Start is called before the first frame update
    void Start() {
        acMan = FindObjectOfType<actionManager>();
    }

    // Update is called once per frame
    void Update() {
        // for selection, we want to check for valid tile for selection.
        // this gives us a selected unit if a battle unit is present on the tile
        
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckSpriteClick(mousePos);
        }
        

        /*
        if (Input.GetMouseButtonDown(0)) {
            if (tile != null) {
                lastTile = tile;
                BattleLogic();
            }
            tile = GetTile().collider.GetComponent<tileScript>();
            spr = GetTile().collider.GetComponent<SpriteRenderer>();

            if (selectedUnit != null) {
                lastUnit = selectedUnit;
            }
            CheckSprite(tile);
        }
        */

        if (tile != null) {
            Color newColor = spr.color;
            newColor.a = 0.5f + Mathf.PingPong(Time.time * 0.5f, 0.5f);
            spr.color = newColor;
        }
    }

    void SprColorReset() {
        Color orig = spr.color;
        orig.a = 1;
        spr.color = orig;
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
    
    /*
    public RaycastHit2D GetTile() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mPos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mPos2D, Vector2.zero);

        return hit;
    }

    void BattleLogic() {
        // battle logic
        if (isMovingUnit) {
            MoveUnitToTile(selectedUnit, lastTile, tile);
            isMovingUnit = false;
        } else if (isAttacking) {
            AttackUnitOnTile(lastUnit, selectedUnit);
            isAttacking = false;
        } else {

        }
    }


    void CheckSprite(tileScript tile) {
        // selected unit
        if (tile != null) {

            // checking for unit on tile
            if (tile.hasUnit) {
                Debug.Log("Unit selected!");
                acMan.OpenActionPanel(tile.unit);

            } else {
                Debug.Log("Unit de-selected.");
                acMan.CloseActionPanel();
            }

        // nothing selected
        } else {
            Debug.Log("No tile selected!");
            SprColorReset();
        }
    }
    */
    
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
                        MoveUnitToTile(selectedUnit, lastTile, clickedTile);
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

    void AttackUnitOnTile(unitScript unit, unitScript target) {
        if (unit.unitAP > 0) {
            target.unitHealth -= unit.unitDMG;
            unit.actionUse();
            isAttacking = false;
            selectedUnit = null;
        }

        else {
            Debug.Log("Unit has no AP.");
        }
    }

    void MoveUnitToTile(unitScript unit, tileScript prevTile, tileScript targetTile) {
        //unitScript us = unit.GetComponent<unitScript>();

        if (unit.unitAP > 0) {
            unit.transform.position = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y + 0.5f, -1);
            prevTile.ClearUnit();
            targetTile.AssignUnit(unit);
            unit.actionUse();
        }

        else {
            Debug.Log("Unit has no AP.");
        }
        
        Debug.Log("Unit moved to new tile.");
    }
    
    public List<tileScript> GetSurroundingTiles(tileScript centerTile) {
        List<tileScript> surroundingTiles = new List<tileScript>();
        Vector3Int[] neighborOffsets = new Vector3Int[] {
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(1, 0, 0),  // Right
            new Vector3Int(0, -1, 0), // Down
            new Vector3Int(0, 1, 0),  // Up
            new Vector3Int(-1, -1, 0), // Bottom-left
            new Vector3Int(-1, 1, 0),  // Top-left
            new Vector3Int(1, -1, 0),  // Bottom-right
            new Vector3Int(1, 1, 0)   // Top-right
        };

        foreach (var tile in neighborOffsets) {
            Vector3Int neighborPosition = tilemap.WorldToCell(centerTile.transform.position) + tile;
            TileBase neighborTileBase = tilemap.GetTile(neighborPosition);
        
            if (neighborTileBase != null) {
                tileScript neighborTile = tilemap.GetInstantiatedObject(neighborPosition).GetComponent<tileScript>();
                if (neighborTile != null) {
                    surroundingTiles.Add(neighborTile);
                }
            }
        }

        return surroundingTiles;
    }
    
    public int getSurroundingUnits(tileScript centerTile) {
        List<tileScript> surroundingTiles = GetSurroundingTiles(centerTile);
        int unitCount = 0;

        foreach (var tile in surroundingTiles) {
            if (tile.hasUnit) {
                unitCount++;
            }
        }

        return unitCount;
    }
    
    void moraleCheck(tileScript selectedTile) {
        int surroundingUnits = getSurroundingUnits(selectedTile);
        if (surroundingUnits > 8) {
            bm.decreaseMorale(10);
            Debug.Log("Morale decreased by 10 due to surrounding units.");
        }
    }
}
