using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string name;
    public Sprite logo;
    public int attack, helth, cost; 
    public bool canAttack;
    public bool isPlaced; //фикс бага с повторным отнятием энергии

    public bool isAlive
    {
        get
        {
            return helth > 0;
        }
    }

    public Card(string name, string logoPath, int attack, int helth, int cost)
    {
        this.name = name;
        logo = Resources.Load<Sprite>(logoPath);
        this.attack = attack;
        this.helth = helth;
        this.cost = cost;
        canAttack = false;
        isPlaced = false;
    }

    public void GetDamage(int dmg)
    {
        helth -= dmg;
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
        CardManager.allCards.Add(new Card("Bird Archer", "Sprite/Cards/IcePack/BirdArcherCard", 5000, 1550, 20));
        CardManager.allCards.Add(new Card("Dead Viking", "Sprite/Cards/IcePack/DeadVikingCard", 2000, 500, 10));
        CardManager.allCards.Add(new Card("Giant", "Sprite/Cards/IcePack/GiantCard", 3000, 9000, 40));
        CardManager.allCards.Add(new Card("Gnom Card", "Sprite/Cards/IcePack/GnomCard", 3000, 3000, 20));
        CardManager.allCards.Add(new Card("Ice Demon", "Sprite/Cards/IcePack/IceDemonCard", 7000, 7000, 60));
        CardManager.allCards.Add(new Card("Ice Golem", "Sprite/Cards/IcePack/IceGolemCard", 6000, 8000, 60));
        CardManager.allCards.Add(new Card("Ice Knight", "Sprite/Cards/IcePack/IceKnightCard", 10000, 10000, 99));
        CardManager.allCards.Add(new Card("Ice Ork", "Sprite/Cards/IcePack/IceOrkCard", 7000, 5000, 70));
        CardManager.allCards.Add(new Card("Rogatiy", "Sprite/Cards/IcePack/RogatiiCard", 9000, 4000, 70));
        CardManager.allCards.Add(new Card("Lord Giant", "Sprite/Cards/IcePack/SecondGiantCard", 4000, 10000, 60));
    }

}
