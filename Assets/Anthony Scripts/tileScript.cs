using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileScript : MonoBehaviour {
    public GameObject unit;
    public bool hasUnit {
        get { return unit != null; }
    }
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void AssignUnit(GameObject newUnit) {
        unit = newUnit;
    }

    public void ClearUnit() {
        unit = null;
    }
}
