using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour {

    public List<Pokemon> Pokemons => pokemons;
    
    [SerializeField] private List<Pokemon> pokemons;

    private void Start() {
        pokemons.ForEach(p => p.Init());
    }

    public Pokemon GetHealthyPokemon() {
        return pokemons.FirstOrDefault(pokemon => pokemon.HP > 0);
    }
}
