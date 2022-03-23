using System;
using System.Collections.Generic;

public enum TypeEffectiveness {
    None,
    Half,
    Normal,
    Double
}

public class TypeChart {
    public static float GetEffectiveness(PokemonType attacker, PokemonType defender) {
        if (attacker == PokemonType.None || defender == PokemonType.None) {
            return 1f;
        }
        
        switch (chart[attacker][defender]) {
            case TypeEffectiveness.None:
                return 0f;
            case TypeEffectiveness.Half:
                return 0.5f;
            case TypeEffectiveness.Normal:
                return 1f;
            case TypeEffectiveness.Double:
                return 2f;
        }

        throw new Exception($"Type effectiveness not found for {attacker} attacking {defender}");
    }
    
    private static Dictionary<PokemonType, Dictionary<PokemonType, TypeEffectiveness>> chart =
        new Dictionary<PokemonType, Dictionary<PokemonType, TypeEffectiveness>> {
            { 
                PokemonType.Normal, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Normal },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Half },
                    { PokemonType.Ghost, TypeEffectiveness.None },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Fire, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Half },
                    { PokemonType.Water, TypeEffectiveness.Half },
                    { PokemonType.Grass, TypeEffectiveness.Double },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Double },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Double },
                    { PokemonType.Rock, TypeEffectiveness.Half },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Half }
                }
            },
            { 
                PokemonType.Water, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Double },
                    { PokemonType.Water, TypeEffectiveness.Half },
                    { PokemonType.Grass, TypeEffectiveness.Half },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Double },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Double },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Half }
                }
            },
            { 
                PokemonType.Grass, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Half },
                    { PokemonType.Water, TypeEffectiveness.Double },
                    { PokemonType.Grass, TypeEffectiveness.Half },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Half },
                    { PokemonType.Ground, TypeEffectiveness.Double },
                    { PokemonType.Flying, TypeEffectiveness.Half },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Half },
                    { PokemonType.Rock, TypeEffectiveness.Double },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Half }
                }
            },
            { 
                PokemonType.Electric, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Double },
                    { PokemonType.Grass, TypeEffectiveness.Half },
                    { PokemonType.Electric, TypeEffectiveness.Half },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.None },
                    { PokemonType.Flying, TypeEffectiveness.Double },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Half }
                }
            },
            { 
                PokemonType.Ice, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Half },
                    { PokemonType.Water, TypeEffectiveness.Half },
                    { PokemonType.Grass, TypeEffectiveness.Double },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Half },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Double },
                    { PokemonType.Flying, TypeEffectiveness.Double },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Double }
                }
            },
            { 
                PokemonType.Fighting, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Double },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Normal },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Double },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Half },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Half },
                    { PokemonType.Psychic, TypeEffectiveness.Half },
                    { PokemonType.Bug, TypeEffectiveness.Half },
                    { PokemonType.Rock, TypeEffectiveness.Double },
                    { PokemonType.Ghost, TypeEffectiveness.None },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Poison, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Double },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Half },
                    { PokemonType.Ground, TypeEffectiveness.Half },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Half },
                    { PokemonType.Ghost, TypeEffectiveness.Half },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Ground, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Double },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Half },
                    { PokemonType.Electric, TypeEffectiveness.Double },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Double },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.None },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Half },
                    { PokemonType.Rock, TypeEffectiveness.Double },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Flying, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Double },
                    { PokemonType.Electric, TypeEffectiveness.Half },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Double },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Double },
                    { PokemonType.Rock, TypeEffectiveness.Half },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Psychic, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Normal },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Double },
                    { PokemonType.Poison, TypeEffectiveness.Double },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Half },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Bug, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Half },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Double },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Half },
                    { PokemonType.Poison, TypeEffectiveness.Half },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Half },
                    { PokemonType.Psychic, TypeEffectiveness.Double },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Half },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Rock, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Double },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Normal },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Double },
                    { PokemonType.Fighting, TypeEffectiveness.Half },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Half },
                    { PokemonType.Flying, TypeEffectiveness.Double },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Double },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Ghost, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.None },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Normal },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Double },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Double },
                    { PokemonType.Dragon, TypeEffectiveness.Normal }
                }
            },
            { 
                PokemonType.Dragon, new Dictionary<PokemonType, TypeEffectiveness> {
                    { PokemonType.Normal, TypeEffectiveness.Normal },
                    { PokemonType.Fire, TypeEffectiveness.Normal },
                    { PokemonType.Water, TypeEffectiveness.Normal },
                    { PokemonType.Grass, TypeEffectiveness.Normal },
                    { PokemonType.Electric, TypeEffectiveness.Normal },
                    { PokemonType.Ice, TypeEffectiveness.Normal },
                    { PokemonType.Fighting, TypeEffectiveness.Normal },
                    { PokemonType.Poison, TypeEffectiveness.Normal },
                    { PokemonType.Ground, TypeEffectiveness.Normal },
                    { PokemonType.Flying, TypeEffectiveness.Normal },
                    { PokemonType.Psychic, TypeEffectiveness.Normal },
                    { PokemonType.Bug, TypeEffectiveness.Normal },
                    { PokemonType.Rock, TypeEffectiveness.Normal },
                    { PokemonType.Ghost, TypeEffectiveness.Normal },
                    { PokemonType.Dragon, TypeEffectiveness.Double }
                }
            }
        };
}