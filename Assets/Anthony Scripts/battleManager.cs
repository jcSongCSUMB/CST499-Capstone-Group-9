using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class battleManager : MonoBehaviour {
    public int activeUnits = 0;
    private bool turnComplete = false;
    public int morale = 100;
    public TextMeshProUGUI  unitCounter;
    public tileMap tmap;

    public List<unitScript> enemyUnits = new List<unitScript>();
    
    
    // Start is called before the first frame update
    void Start() {
        unitCounter.text = $"Units: {activeUnits} - {tmap.getUnitAP()}";

        foreach (var tile in FindObjectsOfType<tileScript>()) {
            if (tile.hasUnit && !tile.unit.friendly) {
                enemyUnits.Add(tile.unit);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        SetText();
        if (turnComplete == true) {
            // switch to enemy
            // otherwise, can still use up time on their turn even with inactive units
        }
    }

    void SetText() {
        //
        if (tmap.selectedUnit != null) {
            unitScript unit = tmap.getUnitAP();
            if (unit) {
                unitCounter.text = $"AP: {unit.unitAP} HEALTH: {unit.unitHealth} DMG: {unit.unitDMG}";
            }
            else {
                unitCounter.text = $"Units: {activeUnits} - Selected AP: NONE";
            }
        }
        else {
            unitCounter.text = $"ACTIVE UNITS [{activeUnits}]";
        }
    }

    public void decreaseUnits() {
        activeUnits--;
    }

    public void decreaseMorale(int amt) {
        morale -= amt;
    }

    void endTurn() {
        // to be used on button in UI
        // will stop turn regardless of active units
        // possibily add UI prompt of un-used units
        turnComplete = true;
    }
}
