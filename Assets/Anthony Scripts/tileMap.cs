using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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

// TODO:
// fix issue with start of moving characters or attacking. (fixed after first move?)
// incorporate area detection 

public class tileMap : MonoBehaviour {
    public Tilemap tilemap;
    public tileScript tile, lastTile;
    public SpriteRenderer spr = null;
    public unitScript selectedUnit, lastUnit;
    public bool isMovingUnit = false;
    public bool isAttacking = false;
    
    public actionManager acMan;
    public battleManager bm;
    public unitList unitOptions;
    
    public Tile groundTile;
    private List<tileScript> validMoveTiles = new List<tileScript>();
    
    // Start is called before the first frame update
    void Start() {
        acMan = FindObjectOfType<actionManager>();
        //CreateGrid(tilemap);
    }

    // Update is called once per frame
    void Update() {
        // for selection, we want to check for valid tile for selection.
        // this gives us a selected unit if a battle unit is present on the tile
        
        if (Input.GetMouseButtonDown(0)) {
            if (tile != null) {
                lastTile = tile;
                tile = GetTileHit().collider.GetComponent<tileScript>();
                //spr = GetTileHit().collider.GetComponent<SpriteRenderer>();
                BattleLogic();
            }
            else {
                tile = GetTileHit().collider.GetComponent<tileScript>();
                spr = GetTileHit().collider.GetComponent<SpriteRenderer>();
            }
            
            

            if (selectedUnit != null) {
                lastUnit = selectedUnit;
            }
            CheckSprite(tile);
        }
        

        if (tile != null) {
            Color newColor = spr.color;
            newColor.a = 0.5f + Mathf.PingPong(Time.time * 0.5f, 0.5f);
            spr.color = newColor;
        }
    }
    
    public void SpawnEnemies() {
        Vector3[] EnemySpawns = new Vector3[] {
            new Vector3(3.15f, -0.75f, 0),
            new Vector3(4.05f, -0.25f, 0),
            new Vector3(4.95f, 0.25f, 0)
        };

        foreach (var pos in EnemySpawns) {
            Vector2 nPosition = new Vector2(pos.x, pos.y);
            Vector2 cam = Camera.main.ScreenToWorldPoint(nPosition);
            Vector2 direction = (nPosition - cam).normalized;
            RaycastHit2D h = Physics2D.Raycast(nPosition, direction);

            if (h.collider != null) {
                tileScript tile = h.collider.GetComponent<tileScript>();

                if (tile != null && !tile.hasUnit) {
                    GameObject unit = Instantiate(unitOptions.warrior);
                    unit.transform.position = pos + new Vector3(0, 0.3f, 0);
                    tile.hasUnit = true;
                    tile.unit = unit.GetComponent<unitScript>();
                }
            }
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
    
    public RaycastHit2D GetTileHit() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mPos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mPos2D, Vector2.zero);
        
        if (hit == null) {
            acMan.CloseActionPanel();
            Debug.LogWarning("Clicked non-tile.");
        }
        
        return hit;
    }

    void BattleLogic() {
        // battle logic
        if (isMovingUnit) {
            MoveUnitToTile(selectedUnit, lastTile, tile);
            isMovingUnit = false;
        } else if (isAttacking) {
            AttackUnitOnTile(selectedUnit, tile);
            isAttacking = false;
        } else {
            //Debug.Log("BL else reached.");
        }
    }

    
    void UnitStatCalculations(unitScript unit, tileScript target) {
        
    }

    void ResetValidMoveTiles() {
        foreach (var tile in validMoveTiles) {
            SpriteRenderer sprRen = tile.GetComponent<SpriteRenderer>();
            if (sprRen != null) {
                sprRen.color = Color.white;
            }
        }
        validMoveTiles.Clear();
    }

    void CheckSprite(tileScript tile) {
        // selected tile
        if (tile != null) {

            // checking for unit on tile
            if (tile.hasUnit) {
                Debug.Log("Unit selected!");
                selectedUnit = tile.unit;
                acMan.OpenActionPanel();

                validMoveTiles = GetSurroundingTiles(tile.gameObject);

                foreach (var valid in validMoveTiles) {
                    SpriteRenderer sprRen = valid.GetComponent<SpriteRenderer>();
                    if (sprRen != null) {
                        sprRen.color = Color.yellow;
                    }
                }

            } else {
                Debug.Log("Unit de-selected.");
                lastUnit = selectedUnit;
                selectedUnit = null;
                acMan.CloseActionPanel();
                ResetValidMoveTiles();
            }

        // nothing selected
        } else {
            Debug.Log("No tile selected!");
            SprColorReset();
        }
    }

    // to avoid null object, use tileScript as target and check for unit or apply to unit
    // on the tile. 
    void AttackUnitOnTile(unitScript unit, tileScript target) {
        if (!validMoveTiles.Contains(target)) {
            Debug.LogWarning("Invalid attack!");
            return;
        }
        if (unit.unitAP > 0) {
            target.unit.unitHealth -= unit.unitDMG;
            unit.actionUse();
            isAttacking = false;
            selectedUnit = null;
            
            ResetValidMoveTiles();
        }

        else {
            //Debug.Log("Unit has no AP.");
        }
    }

    void MoveUnitToTile(unitScript unit, tileScript prevTile, tileScript targetTile) {
        //unitScript us = unit.GetComponent<unitScript>();
        //Debug.Log($"Unit: {unit}, prevTile: {prevTile.transform.position}, target: {targetTile.transform.position}");
        if (!validMoveTiles.Contains(targetTile)) {
            Debug.LogWarning("Invalid move!");
            return;
        }
        
        if (unit.unitAP > 0) {
            unit.transform.position = new Vector3(targetTile.transform.position.x, targetTile.transform.position.y + 0.3f, 0);
            prevTile.ClearUnit();
            targetTile.AssignUnit(unit);
            moraleCheck(targetTile.gameObject); // morale check for target tile to move to.
            unit.actionUse();
            
            ResetValidMoveTiles();
        }

        else {
            Debug.Log("Unit has no AP.");
        }
        
        //Debug.Log("Unit moved to new tile.");
    }

    public void CreateGrid(Tilemap tmap) {
        for (int i = 10; i < 15; i++) {
            for (int j = 2; j > -3; j--) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                tmap.SetTile(pos, groundTile);
            }
        }
    }

    unitScript FindNearestPlayer(unitScript enemy) {
        unitScript nearestPlayer = null;
        float shortestDistance = 100f;

        foreach (var unit in FindObjectsOfType<unitScript>()) {
            if (unit.friendly) {
                float distance = Vector3.Distance(enemy.transform.position, unit.transform.position);

                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    nearestPlayer = unit;
                }
            }
        }

        return nearestPlayer;
    }

    tileScript enemyMoveUnit(unitScript enemyUnit, unitScript targetUnit) {
        List<tileScript> surroundingTiles = GetSurroundingTiles(enemyUnit.gameObject);
        tileScript bestMove = null;
        float shortestDistance = 100f;

        foreach (var tile in surroundingTiles) {
            if (!tile.hasUnit) {
                float distance = Vector3.Distance(tile.transform.position, targetUnit.transform.position);
                if (distance < shortestDistance) {
                    shortestDistance = distance;
                    bestMove = tile;
                }
            }
        }

        return bestMove;
    }
    
    public void enemyTurn() {
        foreach (var enemy in bm.enemyUnits) {
            if (enemy.unitAP > 0 && enemy.isActive) {
                unitScript target = FindNearestPlayer(enemy);

                if (target != null) {
                    tileScript targetTile = enemyMoveUnit(enemy, target);
                    if (targetTile != null && targetTile.hasUnit && targetTile.unit.friendly) {
                        AttackUnitOnTile(enemy, targetTile);
                    }
                    else {
                        MoveUnitToTile(enemy, enemy.GetComponentInParent<tileScript>(), targetTile);
                    }
                }
            }
        }
    }
    
    public List<tileScript> GetSurroundingTiles(GameObject centerTile) {
        // Ensure the Tilemap and Grid are valid
        if (centerTile == null || tilemap == null) {
            Debug.LogWarning("Center tile or tilemap is null.");
            return new List<tileScript>();
        }

        Grid grid = tilemap.GetComponentInParent<Grid>();
        Vector3Int gridPos = grid.WorldToCell(centerTile.transform.position);
        //Debug.Log("WORLD TO CELL: " + gridPos);
        //Debug.Log($"OBJ: {tilemap.GetInstantiatedObject(gridPos).name}");

        List<tileScript> surroundingTiles = new List<tileScript>();
        Vector3Int[] neighborOffsets = new Vector3Int[] {
            new Vector3Int(0, 1, 0), // Left
            new Vector3Int(0, -1, 0),  // Right
            new Vector3Int(-1, 0, 0), // Down
            new Vector3Int(1, 0, 0),  // Up
            new Vector3Int(-1, 1, 0), // Bottom-left
            new Vector3Int(1, 1, 0),  // Top-left
            new Vector3Int(-1, -1, 0),  // Bottom-right
            new Vector3Int(1, -1, 0)   // Top-right
        };
        
        foreach (var offset in neighborOffsets) {
            Vector3Int sum = gridPos + offset;
            Vector3 neighborPosition = tilemap.CellToLocal(sum);
            Vector2 nPosition = new Vector2(neighborPosition.x, neighborPosition.y);
            Vector2 cam = Camera.main.ScreenToWorldPoint(nPosition);
            Vector2 direction = (nPosition - cam).normalized;
            
            
            RaycastHit2D h = Physics2D.Raycast(nPosition, direction);
            Debug.Log($"h: {h.IsUnityNull()}");

            if (h.collider != null) {
                GameObject tileObject = h.collider.gameObject;
                Debug.Log($"\n[1] Being reached? Offset: {offset} gridPos: {gridPos} Sum: {sum} WPOS: {neighborPosition}");
        
                if (tileObject != null) {
                    tileScript neighborTile = tileObject.GetComponent<tileScript>();
                    Debug.Log($"\n[2] Being reached? neighborTile: {neighborTile}");
                    if (neighborTile != null) {
                        surroundingTiles.Add(neighborTile);
                        Debug.Log("\n[3] Is add being reached?!??!?!?!?!");
                    }
                }
            }
        }

        return surroundingTiles;
    }
    
    public int getSurroundingUnits(GameObject centerTile) {
        List<tileScript> surroundingTiles = GetSurroundingTiles(centerTile);
        //Debug.Log("Reaches function");
        int unitCount = 0;

        foreach (var tile in surroundingTiles) {
            Debug.Log($"Tile Position: {tile.transform.position} | HasUnit: {tile.hasUnit} | Tiles: {surroundingTiles.Count}");
            if (tile.hasUnit) {
                unitCount++;
            }
        }

        return unitCount;
    }
    
    void moraleCheck(GameObject selectedTile) {
        int surroundingUnits = getSurroundingUnits(selectedTile);
        if (surroundingUnits > 1) {
            bm.decreaseMorale(10);
            Debug.Log($"Morale decreased by 10 due to surrounding units.\nMorale: {bm.morale}");
        }

        else {
            Debug.Log($"Moral: {bm.morale} | Units: {surroundingUnits}");
        }
    }
}
