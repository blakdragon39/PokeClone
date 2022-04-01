using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour {
    
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HPBar hpBar;

    [SerializeField] private Color highlightedColour;

    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon) {
        _pokemon = pokemon;
        
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
    }

    public void SetSelected(bool selected) {
        nameText.color = selected ? highlightedColour : Color.black;
    }
}
