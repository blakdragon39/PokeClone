using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB {

    public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
        new Dictionary<ConditionID, Condition> {
            {
                ConditionID.Poison, 
                new Condition {
                    Name = "Poison",
                    StartMessage = "has been poisoned.",
                    OnAfterTurn = pokemon => {
                        pokemon.UpdateHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} took poison damage.");
                    } 
                }
                
            },
            {
                ConditionID.Burn, 
                new Condition {
                    Name = "Burn",
                    StartMessage = "has been burned.",
                    OnAfterTurn = pokemon => {
                        pokemon.UpdateHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to its burn.");
                    } 
                }
            },
            {
                ConditionID.Paralyzed,
                new Condition {
                    Name = "Paralyzed",
                    StartMessage = "has been paralyzed.",
                    OnBeforeMove = pokemon => {
                        if (Random.Range(1, 5) == 1) {
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed and can't move.");
                            return false;
                        }

                        return true;
                    }
                }
            },
            {
                ConditionID.Freeze,
                new Condition {
                    Name = "Freeze",
                    StartMessage = "has been frozen.",
                    OnBeforeMove = pokemon => {
                        if (Random.Range(1, 5) == 1) {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} thaws out.");
                            return true;
                        }

                        return false;
                    }
                }
            },
            {
                ConditionID.Sleep,
                new Condition {
                    Name = "Sleep",
                    StartMessage = "has fallen asleep.",
                    OnStart = pokemon => {
                        // sleep for 1 - 3 turns
                        pokemon.StatusTime = Random.Range(1, 4);
                        Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                    },
                    OnBeforeMove = pokemon => {
                        if (pokemon.StatusTime <= 0) {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                            return true;
                        }
                        
                        pokemon.StatusTime -= 1;
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping.");
                        return false;
                    }
                }
            }
        };
}

public enum ConditionID {
    None, Poison, Burn, Sleep, Paralyzed, Freeze
}
