using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialog : MonoBehaviour {
    
    [SerializeField] private int lettersPerSecond;
    [SerializeField] private Color highlightedColor;

    [SerializeField] private Text dialogText;
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;
    [SerializeField] private GameObject choiceBox;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private List<Text> moveTexts;

    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text yesText;
    [SerializeField] private Text noText;
    
    public void SetDialog(string dialog) {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog) {
        dialogText.text = "";

        foreach (var c in dialog.ToCharArray()) {
            dialogText.text += c;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        
        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled) {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled) {
        actionSelector.SetActive(enabled);
    }
    
    public void EnableChoiceBox(bool enabled) {
        choiceBox.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled) {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction) {
        for (int i = 0; i < actionTexts.Count; i++) {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void SetMoveNames(List<Move> moves) {
        for (int i = 0; i < moveTexts.Count; i++) {
            if (i < moves.Count) {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else {
                moveTexts[i].text = "-";
            }
        }
    }

    public void UpdateChoiceBox(bool yesSelected) {
        yesText.color = yesSelected ? highlightedColor : Color.black;
        noText.color = yesSelected ? Color.black : highlightedColor;
    }
    
    public void UpdateMoveSelection(int selectedMove, Move move) {
        for (int i = 0; i < moveTexts.Count; i++) {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"{move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
        
        ppText.color = move.PP == 0 ? Color.red : Color.black;
    }
}