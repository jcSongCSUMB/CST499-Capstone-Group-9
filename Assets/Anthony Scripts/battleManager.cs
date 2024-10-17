using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class battleManager : MonoBehaviour {
    private int activeUnits = 4;
    private bool turnComplete = false;
    public TextMeshPro unitCounter;
    
    
    // Start is called before the first frame update
    void Start() {
        unitCounter.text = $"Units: {activeUnits}";
    }

    // Update is called once per frame
    void Update() {
        if (turnComplete == true) {
            // switch to enemy
            // otherwise, can still use up time on their turn even with inactive units
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
