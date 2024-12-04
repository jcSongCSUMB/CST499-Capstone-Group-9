using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileScript : MonoBehaviour {
    public unitScript unit;
    public bool hasUnit;
    public string spawnType = "None";
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void AssignUnit(unitScript newUnit) {
        unit = newUnit;
        hasUnit = true;
    }

    public void ClearUnit() {
        unit = null;
        hasUnit = false;
    }
}
