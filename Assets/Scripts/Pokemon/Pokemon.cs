using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    
    private PokemonBase _base;
    private int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        HP = pBase.MaxHp;
        
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= level)
            {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4) break;
            }
        }
    }

    public int MaxHp => Mathf.FloorToInt(_base.MaxHp * level / 100f) + 10;

    public int Attack => Mathf.FloorToInt(_base.Attack * level / 100f) + 5;
    
    public int Defence => Mathf.FloorToInt(_base.Defence * level / 100f) + 5;

    public int SpAttack => Mathf.FloorToInt(_base.SpAttack * level / 100f) + 5;
    
    public int SpDefence => Mathf.FloorToInt(_base.SpDefence * level / 100f) + 5;
    
    public int Speed => Mathf.FloorToInt(_base.Speed * level / 100f) + 5;
}
