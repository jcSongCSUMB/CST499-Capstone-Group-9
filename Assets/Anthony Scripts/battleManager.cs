using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class battleManager : MonoBehaviour {
    public int activeUnits = 4;
    private bool turnComplete = false;
    public TextMeshProUGUI  unitCounter;
    public tileMap tmap;
    
    
    // Start is called before the first frame update
    void Start() {
        unitCounter.text = $"Units: {activeUnits} - {tmap.getUnitAP()}";
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
        if (tmap.getUnitAP() != null) {
            unitScript unit = tmap.getUnitAP();
            if (unit.unitAP > 0) {
                unitCounter.text = $"Units: {activeUnits} - AP: {unit.unitAP} H: {unit.unitHealth} S: {unit.unitSpeed}";
            }
            else {
                unitCounter.text = $"Units: {activeUnits} - Selected AP: NONE";
            }
        }
    }

    public void decreaseUnits() {
        activeUnits--;
    }

    void endTurn() {
        // to be used on button in UI
        // will stop turn regardless of active units
        // possibily add UI prompt of un-used units
        turnComplete = true;
    }
}
