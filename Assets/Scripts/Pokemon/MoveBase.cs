using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject {
    
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private MoveTarget target;

    public string Name => name;
    public string Description => description;
    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;
    public MoveCategory Category => category;
    public MoveEffects Effects => effects;
    public MoveTarget Target => target;
}

[System.Serializable]
public class MoveEffects {
    
    public List<StatBoost> Boosts => boosts;
    public ConditionID Status => status;
    
    [SerializeField] private List<StatBoost> boosts;
    [SerializeField] private ConditionID status;
}

[System.Serializable]
public class StatBoost {
    public Stat stat;
    public int boost;
}

public enum MoveCategory {
    Physical, Special, Status
}

public enum MoveTarget {
    Foe, Self
}