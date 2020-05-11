using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public enum AbilityType
    {
        NO_ABILITY,
        INSTANT_ACTIVE,
        DOUBLE_ATTACK,
        PROVOCATION,
        REGENERATION,
        STRENGTH_GAIN,
        SHIELD
    }
    public string name;
    public Sprite logo;
    public int attack, helth, cost; 
    public bool canAttack;
    public bool isPlaced; //фикс бага с повторным отнятием энергии

    public List<AbilityType> abilities;
    public bool isSpell;



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


    //public int timesTookDamage;
    public int timesDealeDamage;

    public Card(string name, string logoPath, int attack, int helth, int cost, AbilityType abilityType = 0)
    {
        this.name = name;
        logo = Resources.Load<Sprite>(logoPath);
        this.attack = attack;
        this.helth = helth;
        this.cost = cost;
        canAttack = false;
        isPlaced = false;
        isSpell = false;

        abilities = new List<AbilityType>();

        if(abilityType != 0)
            abilities.Add(abilityType);

        timesDealeDamage = 0;// для дабл атаки
    }

    public Card(Card card)
    {
        name = card.name;
        logo = card.logo;
        attack = card.attack;
        helth = card.helth;
        cost = card.cost;
        canAttack = false;
        isPlaced = false;
        isSpell = false;

        abilities = new List<AbilityType>(card.abilities);

        timesDealeDamage = 0;// для дабл атаки
    }

    public void GetDamage(int dmg)
    {
        if(abilities.Exists(x => x == AbilityType.SHIELD))
            abilities.Remove(AbilityType.SHIELD);
        else
            helth -= dmg;   
    }

    public Card GetCopy()
    {
        return new Card(this);
    }
}

public class SpellCard : Card
{
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

    public SpellType spell;
    public TargetType spellTarget;
    public int spellValue;

    public SpellCard(string name, string logoPath, int cost, 
                SpellType spellType = 0, int spellVal = 0, TargetType targetType = 0) :
                base(name, logoPath, 0, 0, cost)
    {
        isSpell = true;
        spell = spellType;
        spellTarget = targetType;
        spellValue = spellVal;
    }

    public SpellCard(SpellCard card) : base(card)
    {
        isSpell = true;

        spell = card.spell;
        spellTarget = card.spellTarget;
        spellValue = card.spellValue;
    }

    public new SpellCard GetCopy()
    {
        return new SpellCard(this);
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
        CardManager.allCards.Add(new Card("Ice Demon", "Sprite/Cards/IcePack/IceDemonCard", 9000, 5000, 60, Card.AbilityType.SHIELD));
        CardManager.allCards.Add(new Card("Ice Golem", "Sprite/Cards/IcePack/IceGolemCard", 6000, 8000, 50, Card.AbilityType.PROVOCATION));
        CardManager.allCards.Add(new Card("Ice Knight", "Sprite/Cards/IcePack/IceKnightCard", 11000, 14000, 95, Card.AbilityType.NO_ABILITY));
        CardManager.allCards.Add(new Card("Ice Ork", "Sprite/Cards/IcePack/IceOrkCard", 4000, 12000, 70, Card.AbilityType.REGENERATION));
        CardManager.allCards.Add(new Card("Rogatiy", "Sprite/Cards/IcePack/RogatiiCard", 9000, 4000, 70, Card.AbilityType.STRENGTH_GAIN));
        CardManager.allCards.Add(new Card("Lord Giant", "Sprite/Cards/IcePack/SecondGiantCard", 4000, 12000, 60, Card.AbilityType.PROVOCATION));
        
        CardManager.allCards.Add(new SpellCard("Avalance", "Sprite/Cards/IceSpell/AvalanceSpell", 40, SpellCard.SpellType.DAMAGE_CARDS, 4000, SpellCard.TargetType.NO_TARGET));
        CardManager.allCards.Add(new SpellCard("Cave", "Sprite/Cards/IceSpell/CaveSpell", 35, SpellCard.SpellType.DAMAGE_HERO, 3000, SpellCard.TargetType.NO_TARGET));
        CardManager.allCards.Add(new SpellCard("Frozen", "Sprite/Cards/IceSpell/FrozenSpell", 15, SpellCard.SpellType.ADD_PROVOCATION, 0, SpellCard.TargetType.ALLY_CARD_TARGET));
        CardManager.allCards.Add(new SpellCard("Snow Attack", "Sprite/Cards/IceSpell/SnowAttack", 35, SpellCard.SpellType.DESTROY_CARD, 0, SpellCard.TargetType.ENEMY_CARD_TARGET));
        CardManager.allCards.Add(new SpellCard("Frozen Wave", "Sprite/Cards/IceSpell/FrozenWaveSpell", 25, SpellCard.SpellType.DAMAGE_CARD, 10000, SpellCard.TargetType.ENEMY_CARD_TARGET));
    }

}
