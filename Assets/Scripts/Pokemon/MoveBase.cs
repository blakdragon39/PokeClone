using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject {
    
    [SerializeField] private string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;
    [SerializeField] private bool isSpecial;

    public string Name => name;

    public string Description => description;

    public PokemonType Type => type;

    public int Power => power;

    public int Accuracy => accuracy;

    public int PP => pp;

    public bool IsSpecial => isSpecial;
}