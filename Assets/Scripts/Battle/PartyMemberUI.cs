using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour {
    
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HPBar hpBar;

    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon) {
        _pokemon = pokemon;
        
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp);
    }


}
