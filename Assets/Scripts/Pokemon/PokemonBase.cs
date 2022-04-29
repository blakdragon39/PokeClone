using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject {
    
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    // Base Stats
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defence;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefence;
    [SerializeField] private int speed;

    [SerializeField] private int expYield;
    [SerializeField] private GrowthRate growthRate;

    [SerializeField] private int catchRate = 255;

    [SerializeField] private List<LearnableMove> learnableMoves;

    public string Name => name;
    public string Description => description;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

    public int MaxHp => maxHp;
    public int Attack => attack;
    public int Defence => defence;
    public int SpAttack => spAttack;
    public int SpDefence => spDefence;
    public int Speed => speed;

    public int ExpYield => expYield;
    public GrowthRate GrowthRate => growthRate;
    
    public int CatchRate => catchRate;

    public List<LearnableMove> LearnableMoves => learnableMoves;

    public int GetExpForLevel(int level) {
        if (growthRate == GrowthRate.Fast) {
            return 4 * (level * level * level) / 5;
        } else if (growthRate == GrowthRate.MediumFast) {
            return (level * level * level);
        }

        return -1; // error state, growth rate doesn't exist
    }
}

[System.Serializable]
public class LearnableMove {
    
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int level;

    public MoveBase Base => moveBase;

    public int Level => level;
}

public enum PokemonType {
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon
}

public enum GrowthRate {
    Fast,
    MediumFast
}

public enum Stat {
    Attack,
    Defence,
    SpAttack,
    SpDefence,
    Speed,
    
    Accuracy,
    Evasion
}