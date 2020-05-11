using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public enum AbilityType
    {
        NO_ABILITY,
        INSTANT_ACTIVE,
        DOUBLE_ATTACK,
        PROVOCATION,
        REGENERATION,
        STRENGTH_GAIN
    }
    public enum SpellType
    {
        NO_SPELL,
        HEAL_CARDS,
        DAMAGE_CARDS,
        HEAL_CARD,
        DAMAGE_CARD,
        DAMAGE_HERO,
        HEAL_HERO,
        ADD_PROVOCATION,
        DESTROY_CARD
    }

    public enum TargetType
    {
        NO_TARGET,
        ALLY_CARD_TARGET,
        ENEMY_CARD_TARGET
    }

    public string name;
    public Sprite logo;
    public int attack, helth, cost; 
    public bool canAttack;
    public bool isPlaced; //фикс бага с повторным отнятием энергии

    public List<AbilityType> abilities;
    public SpellType spell;
    public TargetType spellTarget;
    public int spellValue;

    public bool isAlive
    {
        get
        {
            return helth > 0;
        }
    }
    public bool hasAbility
    {
        get
        {
            return abilities.Count > 0;
        }
    }
    public bool isProvocation
    {
        get
        {
            return abilities.Exists(x => x == AbilityType.PROVOCATION);
        }
    }
    public bool isSpell
    {
        get
        {
            return spell != SpellType.NO_SPELL;
        }
    }

    //public int timesTookDamage;
    public int timesDealeDamage;

    public Card(string name, string logoPath, int attack, int helth, int cost, AbilityType abilityType = 0, 
               SpellType spellType = 0, int spellVal = 0, TargetType targetType = 0)
    {
        this.name = name;
        logo = Resources.Load<Sprite>(logoPath);
        this.attack = attack;
        this.helth = helth;
        this.cost = cost;
        canAttack = false;
        isPlaced = false;

        abilities = new List<AbilityType>();

        if(abilityType != 0)
            abilities.Add(abilityType);

        spell = spellType;
        spellValue = spellVal;
        spellTarget = targetType;

        timesDealeDamage = 0;// для дабл атаки
    }

    public void GetDamage(int dmg)
    {
        helth -= dmg;
    }

    public Card GetCopy()
    {
        Card card = this;
        card.abilities = new List<AbilityType>(abilities);
        return card;
    }
}

public static class CardManager //хранение всех карт
{
    public static List<Card> allCards = new List<Card>();
}

public class CardManagerScript : MonoBehaviour
{
    public void Awake()
    {
        CardManager.allCards.Add(new Card("Bird Archer", "Sprite/Cards/IcePack/BirdArcherCard", 2000, 1000, 20, Card.AbilityType.INSTANT_ACTIVE));
        CardManager.allCards.Add(new Card("Dead Viking", "Sprite/Cards/IcePack/DeadVikingCard", 1500, 1000, 10, Card.AbilityType.NO_ABILITY));
        CardManager.allCards.Add(new Card("Giant", "Sprite/Cards/IcePack/GiantCard", 2000, 9000, 40, Card.AbilityType.PROVOCATION));
        CardManager.allCards.Add(new Card("Gnom Card", "Sprite/Cards/IcePack/GnomCard", 1500, 3000, 20, Card.AbilityType.DOUBLE_ATTACK));
        CardManager.allCards.Add(new Card("Ice Demon", "Sprite/Cards/IcePack/IceDemonCard", 9000, 5000, 60, Card.AbilityType.DOUBLE_ATTACK));
        CardManager.allCards.Add(new Card("Ice Golem", "Sprite/Cards/IcePack/IceGolemCard", 6000, 8000, 50, Card.AbilityType.PROVOCATION));
        CardManager.allCards.Add(new Card("Ice Knight", "Sprite/Cards/IcePack/IceKnightCard", 11000, 14000, 95, Card.AbilityType.NO_ABILITY));
        CardManager.allCards.Add(new Card("Ice Ork", "Sprite/Cards/IcePack/IceOrkCard", 4000, 12000, 70, Card.AbilityType.REGENERATION));
        CardManager.allCards.Add(new Card("Rogatiy", "Sprite/Cards/IcePack/RogatiiCard", 9000, 4000, 70, Card.AbilityType.STRENGTH_GAIN));
        CardManager.allCards.Add(new Card("Lord Giant", "Sprite/Cards/IcePack/SecondGiantCard", 4000, 12000, 60, Card.AbilityType.PROVOCATION));
        
        CardManager.allCards.Add(new Card("Avalance", "Sprite/Cards/IceSpell/AvalanceSpell", 0, 0, 40, 0, Card.SpellType.DAMAGE_CARDS, 4000, Card.TargetType.NO_TARGET));
        CardManager.allCards.Add(new Card("Cave", "Sprite/Cards/IceSpell/CaveSpell", 0, 0, 35, 0, Card.SpellType.DAMAGE_HERO, 3000, Card.TargetType.NO_TARGET));
        CardManager.allCards.Add(new Card("Frozen", "Sprite/Cards/IceSpell/FrozenSpell", 0, 0, 15, 0, Card.SpellType.ADD_PROVOCATION, 0, Card.TargetType.ALLY_CARD_TARGET));
        CardManager.allCards.Add(new Card("Snow Attack", "Sprite/Cards/IceSpell/SnowAttack", 0, 0, 35, 0, Card.SpellType.DESTROY_CARD, 0, Card.TargetType.ENEMY_CARD_TARGET));
        CardManager.allCards.Add(new Card("Frozen Wave", "Sprite/Cards/IceSpell/FrozenWaveSpell", 0, 0, 25, 0, Card.SpellType.DAMAGE_CARD, 10000, Card.TargetType.ENEMY_CARD_TARGET));
    }

}
