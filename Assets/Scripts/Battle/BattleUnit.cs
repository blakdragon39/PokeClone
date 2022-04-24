using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour {

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit => isPlayerUnit;
    public BattleHUD HUD => hud;
    
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHUD hud;

    private Image image;
    private Vector3 originalPos;
    private Color originalColor;

    private void Awake() {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup(Pokemon pokemon) {
        Pokemon = pokemon;

        image.sprite = isPlayerUnit ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite;
        image.color = originalColor;
        
        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);

        transform.localScale = new Vector3(1, 1, 1);
        
        PlayEnterAnimation();
    }

    public void Clear() {
        hud.gameObject.SetActive(false);
    }

    private void PlayEnterAnimation() {
        var startX = isPlayerUnit ? -500f : 500f;
        image.transform.localPosition = new Vector3(startX, originalPos.y);
        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayLeaveAnimation() {
        var endX = isPlayerUnit ? -500f : 500f;
        image.transform.DOLocalMoveX(endX, 1f);
    }

    public void PlayAttackAnimation() {
        var startX = isPlayerUnit ? originalPos.x + 50f : originalPos.x - 50f;
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveX(startX, 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation() {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation() {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public IEnumerator PlayCaptureAnimation() {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
    
    public IEnumerator PlayBreakOutAnimation() {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }
}