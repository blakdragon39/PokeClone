using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour {
    
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statusText;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private GameObject expBar;

    [SerializeField] private Color poisonColor;
    [SerializeField] private Color burnColor;
    [SerializeField] private Color sleepColor;
    [SerializeField] private Color paralyzeColor;
    [SerializeField] private Color freezeColor;

    private Pokemon _pokemon;
    private Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon) {
        _pokemon = pokemon;
        
        nameText.text = pokemon.Base.Name;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
        SetExp();
        SetLevel();

        statusColors = new Dictionary<ConditionID, Color> {
            {ConditionID.Poison, poisonColor},
            {ConditionID.Burn, burnColor},
            {ConditionID.Sleep, sleepColor},
            {ConditionID.Paralyzed, paralyzeColor},
            {ConditionID.Freeze, freezeColor}
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
    }

    private void SetStatusText() {
        if (_pokemon.Status == null) {
            statusText.text = "";
        } else {
            statusText.text = _pokemon.Status.SmallName;
            statusText.color = statusColors[_pokemon.Status.ID];
        }
    }

    public void SetLevel() {
        levelText.text = "Lvl " + _pokemon.Level;
    }

    public void SetExp() {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }
    
    public IEnumerator SetExpSmooth(bool reset = false) {
        if (expBar == null) yield break;

        if (reset) {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }
    
    public IEnumerator UpdateHP() {
        if (_pokemon.HPChanged) {
            yield return hpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.MaxHp);
            _pokemon.HPChanged = false;
        }
    }

    private float GetNormalizedExp() {
        int currLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);
        
        float normalizedExp = (float)(_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
}