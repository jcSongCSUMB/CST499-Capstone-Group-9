using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitScript : MonoBehaviour {
    public int unitHealth = 50;
    public int unitAP = 2;
    public float unitSpeed = 3f;

    private battleManager bm;
    //GameObject target;
    //^^^ to be used later for battle
    
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    void CheckSpriteClick(Vector3 mousePos) {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        
        if (hit.collider != null) {
            // If something was hit, log the name of the GameObject
            Debug.Log($"Unit selected!!");
        }
    }

    void calcMove() {
        
    }

    void actionUse() {
        if (unitAP > 1) {
            Debug.Log("Action Used!");
            unitAP--;
        }

        else {
            bm.decreaseUnits();
        }
    }
}
