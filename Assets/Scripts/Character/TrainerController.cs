using System.Collections;
using UnityEngine;

public class TrainerController : MonoBehaviour {

    [SerializeField] private Dialog dialog;
    [SerializeField] private GameObject exclamation;
    [SerializeField] private GameObject fov;

    private Character character;

    private void Awake() {
        character = GetComponent<Character>();
    }

    private void Start() {
        SetFOVRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player) {
        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));
        yield return character.Move(moveVec);
        
        // Show dialog
        yield return DialogManager.Instance.ShowDialog(dialog, () => {
            Debug.Log("Starting trainer battle");
            // todo start trainer battle
        });
    }

    public void SetFOVRotation(FacingDirection dir) {
        float angle = 0f;
        if (dir == FacingDirection.Right) {
            angle = 90f;
        } else if (dir == FacingDirection.Up) {
            angle = 180f;
        } else if (dir == FacingDirection.Left) {
            angle = 270f;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}