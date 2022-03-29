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

    public DamageDetails TakeDamage(Move move, Pokemon attacker) {
        float critical = 1f;
        if (Random.value * 100f <= 6.24)
            critical = 2f;
        
        float type = TypeChart.GetEffectiveness(move.Base.Type, Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, Base.Type2);

        var damageDetails = new DamageDetails {
            TypeEffectiveness = type,
            Critical = critical > 1f,
            Fainted = false
        };

        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
        float defence = (move.Base.IsSpecial) ? SpDefence : Defence;
        
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * (attack / defence) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;

        if (HP <= 0) {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove() {
        return Moves[Random.Range(0, Moves.Count)];
    }
}

public class DamageDetails {
    public bool Fainted { get; set; }
    public bool Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}