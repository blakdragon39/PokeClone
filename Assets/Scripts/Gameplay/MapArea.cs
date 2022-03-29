using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour {

    [SerializeField] private List<Pokemon> wildPokemons;

    public Pokemon GetRandomWildPokemon() {
        var pokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        pokemon.Init();
        return pokemon;
    }
}
