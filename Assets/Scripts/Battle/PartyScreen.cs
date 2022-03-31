using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour {

    [SerializeField] private Text messageText;
    
    private PartyMemberUI[] memberSlots;

    public void Init() {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> pokemons) {
        for (var i = 0; i < memberSlots.Length; i += 1) {
            if (i < pokemons.Count) {
                memberSlots[i].SetData(pokemons[i]);
            } else {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose a pokemon";
    }
}
