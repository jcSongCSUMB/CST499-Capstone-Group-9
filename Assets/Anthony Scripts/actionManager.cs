using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class actionManager : MonoBehaviour {
    public GameObject actionPanel;
    public Button moveButton;
    public Button attackButton;
    private unitScript selectedUnit;

    private tileMap tmap;

    void Start() {
        actionPanel.SetActive(false);
        
        moveButton.onClick.AddListener(OnMoveButtonClicked);
        attackButton.onClick.AddListener(OnAttackButtonClicked);
    }
    
    public void OpenActionPanel(unitScript unit) {
        selectedUnit = unit;
        actionPanel.SetActive(true);
    }
    
    public void CloseActionPanel() {
        actionPanel.SetActive(false);
        selectedUnit = null;
    }

    private void OnMoveButtonClicked() {
        if (selectedUnit != null && tmap != null) {
            tmap.isMovingUnit = true;
            selectedUnit.actionUse();
            CloseActionPanel();
        }
    }

    private void OnAttackButtonClicked() {
        if (selectedUnit != null) {
            selectedUnit.actionUse();
            CloseActionPanel();
        }
    }
}
