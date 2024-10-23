using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitScript : MonoBehaviour {
    public int unitHealth = 50;
    public int unitAP = 2;
    public float unitSpeed = 3f;
    public bool isActive = true;

    private battleManager bm;
    //GameObject target;
    //^^^ to be used later for battle
    
    // Start is called before the first frame update
    void Start() {
        bm = FindObjectOfType<battleManager>();
        Debug.Log($"Unit has {unitAP} AP.");
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

    public void actionUse() {
        if (unitAP > 0) {
            Debug.Log("Action Used!");
            unitAP--;
        }

        if (unitAP <= 0) {
            if (bm != null) {
                bm.decreaseUnits();
                Debug.Log("No more AP available.");
            }
        }
    }
}
