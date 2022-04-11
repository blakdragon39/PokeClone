using System.Collections.Generic;

public class ConditionsDB {

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
        new Dictionary<ConditionID, Condition> {
            {ConditionID.Poison, new Condition {
                Name = "Poison",
                StartMessage = "has been poisoned",
                OnAfterTurn = pokemon => {
                    pokemon.UpdateHP(pokemon.MaxHp / 8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took poison damage.");
                } 
            }},
            {ConditionID.Burn, new Condition {
                Name = "Burn",
                StartMessage = "has been burned",
                OnAfterTurn = pokemon => {
                    pokemon.UpdateHP(pokemon.MaxHp / 16);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to its burn.");
                } 
            }}
        };
}

public enum ConditionID {
    None, Poison, Burn, Sleep, Paralyzed, Freeze
}
