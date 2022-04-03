using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon {

    public PokemonBase Base => _base;
    public int Level => level;
    
    public int HP { get; set; }
    public int MaxHp { get; private set; }

    public int Attack => GetStat(Stat.Attack);
    public int Defence => GetStat(Stat.Defence);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefence => GetStat(Stat.SpDefence);
    public int Speed => GetStat(Stat.Speed);
    
    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;

    public void Init() {
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) {
            if (move.Level <= Level) {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4) break;
            }
        }
        
        CalculateStats();
        HP = MaxHp;
        
        StatBoosts = new Dictionary<Stat, int> {
            { Stat.Attack, 0 },
            { Stat.Defence, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefence, 0 },
            { Stat.Speed, 0 },
        };
    }

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

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defence = (move.Base.Category == MoveCategory.Special) ? SpDefence : Defence;
        
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

    public void ApplyBoosts(List<StatBoost> statBoosts) {
        foreach (var statBoost in statBoosts) {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    private void CalculateStats() {
        Stats = new Dictionary<Stat, int> {
            { Stat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5 },
            { Stat.Defence, Mathf.FloorToInt(Base.Defence * Level / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5 },
            { Stat.SpDefence, Mathf.FloorToInt(Base.SpDefence * Level / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5 }
        };

        MaxHp = Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10;
    }

    private int GetStat(Stat stat) {
        int statValue = Stats[stat];
        int boost = StatBoosts[stat];
        var boostValues = new [] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        return Mathf.FloorToInt(boost >= 0 ? statValue * boostValues[boost] : statValue / boostValues[-boost]);
    }
}

public class DamageDetails {
    public bool Fainted { get; set; }
    public bool Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}