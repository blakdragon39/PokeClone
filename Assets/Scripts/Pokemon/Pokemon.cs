using System.Collections.Generic;
using UnityEngine;

public class Pokemon {
    
    public PokemonBase Base { get; set; }
    public int Level { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Pokemon(PokemonBase pBase, int pLevel) {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) {
            if (move.Level <= Level) {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4) break;
            }
        }
    }

    public int MaxHp => Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10;

    public int Attack => Mathf.FloorToInt(Base.Attack * Level / 100f) + 5;

    public int Defence => Mathf.FloorToInt(Base.Defence * Level / 100f) + 5;

    public int SpAttack => Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5;

    public int SpDefence => Mathf.FloorToInt(Base.SpDefence * Level / 100f) + 5;

    public int Speed => Mathf.FloorToInt(Base.Speed * Level / 100f) + 5;

    public bool TakeDamage(Move move, Pokemon attacker) {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attacker.Attack / Defence) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;

        if (HP <= 0) {
            HP = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove() {
        return Moves[Random.Range(0, Moves.Count)];
    }
}