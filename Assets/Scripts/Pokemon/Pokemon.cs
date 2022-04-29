using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Pokemon {

    public PokemonBase Base => _base;
    public int Level => level;
    
    public int Exp { get; set; }
    
    public int HP { get; set; }
    public int MaxHp { get; private set; }

    public int Attack => GetStat(Stat.Attack);
    public int Defence => GetStat(Stat.Defence);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefence => GetStat(Stat.SpDefence);
    public int Speed => GetStat(Stat.Speed);
    
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; set; }
    public Condition VolatileStatus { get; set; }
    public int StatusTime { get; set; }
    public int VolatileStatusTime { get; set; }

    public Queue<string> StatusChanges { get; private set; }
    public bool HPChanged { get; set; }
    public event Action OnStatusChanged;

    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;

    public Pokemon(PokemonBase pBase, int pLevel) {
        _base = pBase;
        level = pLevel;

        Init();
    }
    
    public void Init() {
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) {
            if (move.Level <= Level) {
                Moves.Add(new Move(move.Base));

                if (Moves.Count >= 4) break;
            }
        }
        
        Exp = Base.GetExpForLevel(Level);
        
        CalculateStats();
        HP = MaxHp;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
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

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage) {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HPChanged = true;
    }

    public void SetStatus(ConditionID conditionID) {
        if (Status != null) return;
        
        Status = ConditionsDB.Conditions[conditionID];
        Status.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{_base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus() {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    
    public void SetVolatileStatus(ConditionID conditionID) {
        if (VolatileStatus != null) return;
        
        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{_base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus() {
        VolatileStatus = null;
    }

    public Move GetRandomMove() {
        var movesWithPP = Moves.Where(move => move.PP > 0).ToList();
        return movesWithPP[Random.Range(0, movesWithPP.Count)];
    }

    public void ApplyBoosts(List<StatBoost> statBoosts) {
        foreach (var statBoost in statBoosts) {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0) {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            } else {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            }
            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public bool CheckForLevelUp() {
        if (Exp >= Base.GetExpForLevel(level + 1)) {
            level += 1;
            return true;
        }

        return false;
    }

    public bool OnBeforeMove() {
        bool canPerformMove = true;
        
        if (Status?.OnBeforeMove != null) {
            if (!Status.OnBeforeMove(this)) canPerformMove = false;
        }
        
        if (VolatileStatus?.OnBeforeMove != null) {
            if (!VolatileStatus.OnBeforeMove(this)) canPerformMove = false;
        }
        
        return canPerformMove;
    }
    
    public void OnAfterTurn() {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver() {
        VolatileStatus = null;
        ResetStatBoost();
    }

    private void CalculateStats() {
        Stats = new Dictionary<Stat, int> {
            { Stat.Attack, Mathf.FloorToInt(Base.Attack * Level / 100f) + 5 },
            { Stat.Defence, Mathf.FloorToInt(Base.Defence * Level / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5 },
            { Stat.SpDefence, Mathf.FloorToInt(Base.SpDefence * Level / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt(Base.Speed * Level / 100f) + 5 }
        };

        MaxHp = Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10 + Level;
    }

    private void ResetStatBoost() {
        StatBoosts = new Dictionary<Stat, int> {
            {Stat.Attack, 0},
            {Stat.Defence, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefence, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0}
        };
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