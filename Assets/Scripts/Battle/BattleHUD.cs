using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour {
    
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text statusText;
    [SerializeField] private HPBar hpBar;

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
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);

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

    public IEnumerator UpdateHP() {
        if (_pokemon.HPChanged) {
            yield return hpBar.SetHPSmooth((float) _pokemon.HP / _pokemon.MaxHp);
            _pokemon.HPChanged = false;
        }
    }
}