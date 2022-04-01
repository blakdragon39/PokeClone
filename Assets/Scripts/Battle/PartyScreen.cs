using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour {

    [SerializeField] private Text messageText;
    
    private PartyMemberUI[] memberSlots;
    private List<Pokemon> _pokemons;

    public void Init() {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons) {
        _pokemons = pokemons;
        
        for (var i = 0; i < memberSlots.Length; i += 1) {
            if (i < pokemons.Count) {
                memberSlots[i].SetData(pokemons[i]);
            } else {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose a pokemon";
    }

    public void UpdateMemberSelection(int selectedMember) {
        for (var i = 0; i < _pokemons.Count; i += 1) {
            memberSlots[i].SetSelected(i == selectedMember);
        }
    }

    public void SetMessageText(string message) {
        messageText.text = message;
    }
}
