using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour {

    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private Color highlightColor;

    private int currentSelection = 0;
    
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove) {
        for (int i = 0; i < currentMoves.Count; i += 1) {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void HandleMoveSelection(Action<int> onSelected) {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            currentSelection += 1;
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            currentSelection -= 1; 
        }

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumMoves);
        UpdateMoveSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Z)) {
            onSelected(currentSelection);
        }
    }
    
    public void UpdateMoveSelection(int selection) {
        for (int i = 0; i < PokemonBase.MaxNumMoves + 1; i += 1) {
            moveTexts[i].color = (i == selection) ? highlightColor : Color.black;
        }
    }
}