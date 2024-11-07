using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class actionManager : MonoBehaviour {
    public GameObject actionPanel;
    public Button moveButton;
    public Button attackButton;
    private unitScript selectedUnit;

    public tileMap tmap;

    void Start() {
        actionPanel.SetActive(false);
        
    }
    
    public void OpenActionPanel(unitScript unit) {
        selectedUnit = unit;
        actionPanel.SetActive(true);
    }
    
    public void CloseActionPanel() {
        //selectedUnit = null;
        actionPanel.SetActive(false);
    }

    public void OnMoveButtonClicked() {
        if (selectedUnit != null && selectedUnit.unitAP > 0) {
            Debug.Log($"Move being pressed! isMoving: {tmap.isMovingUnit}");
            tmap.isMovingUnit = true;
            //selectedUnit.actionUse();
            CloseActionPanel();
        }
        else {
            Debug.Log("Not enough AP!!!");
            CloseActionPanel();
        }
    }

    public void OnAttackButtonClicked() {
        if (selectedUnit != null && selectedUnit.unitAP > 0) {
            tmap.isAttacking = true;
            //selectedUnit.actionUse();
            CloseActionPanel();
        }

        else {
            Debug.Log("Not enough AP!!!");
            CloseActionPanel();
        }
    }
}
