using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Text dialogText;
    [SerializeField] private int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    
    public static DialogManager Instance { get; private set; }

    private int currentLine;
    private Dialog dialog;
    private bool isTyping;
    
    public bool IsShowing { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void HandleUpdate() {
        if (!isTyping && Input.GetKeyDown(KeyCode.Z)) {
            currentLine += 1;
            if (currentLine < dialog.Lines.Count) {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            } else {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialog(Dialog dialog) {
        yield return new WaitForEndOfFrame();
        
        this.dialog = dialog;
        currentLine = 0;

        IsShowing = true;
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
    }
    
    private IEnumerator TypeDialog(string line) {
        isTyping = true;
        dialogText.text = "";

        foreach (var c in line.ToCharArray()) {
            dialogText.text += c;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        isTyping = false;
    }
}