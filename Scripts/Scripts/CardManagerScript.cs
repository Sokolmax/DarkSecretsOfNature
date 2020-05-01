using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string Name;
    public Sprite Logo;
    public int Attack, Helth, Cost; 
    public bool CanAttack;
    public bool IsPlaced; //фикс бага с повторным отнятием энергии

    public bool IsAlive
    {
        get
        {
            return Helth > 0;
        }
    }

    public Card(string name, string logoPath, int attack, int helth, int cost)
    {
        Name = name;
        Logo = Resources.Load<Sprite>(logoPath);
        Attack = attack;
        Helth = helth;
        Cost = cost;
        CanAttack = false;
        IsPlaced = false;
    }

    public void ChangeAttackState(bool can)
    {
        CanAttack = can;
    }

    public void GetDamage(int dmg)
    {
        Helth -= dmg;
    }
}

public static class CardManager //хранение всех карт
{
    public static List<Card> AllCards = new List<Card>();
}

public class CardManagerScript : MonoBehaviour
{
    public void Awake()
    {
        CardManager.AllCards.Add(new Card("Bird Archer", "Sprite/Cards/IcePack/BirdArcherCard", 5000, 1550, 20));
        CardManager.AllCards.Add(new Card("Dead Viking", "Sprite/Cards/IcePack/DeadVikingCard", 2000, 500, 10));
        CardManager.AllCards.Add(new Card("Giant", "Sprite/Cards/IcePack/GiantCard", 3000, 9000, 40));
        CardManager.AllCards.Add(new Card("Gnom Card", "Sprite/Cards/IcePack/GnomCard", 3000, 3000, 20));
        CardManager.AllCards.Add(new Card("Ice Demon", "Sprite/Cards/IcePack/IceDemonCard", 7000, 7000, 60));
        CardManager.AllCards.Add(new Card("Ice Golem", "Sprite/Cards/IcePack/IceGolemCard", 6000, 8000, 60));
        CardManager.AllCards.Add(new Card("Ice Knight", "Sprite/Cards/IcePack/IceKnightCard", 10000, 10000, 99));
        CardManager.AllCards.Add(new Card("Ice Ork", "Sprite/Cards/IcePack/IceOrkCard", 7000, 5000, 70));
        CardManager.AllCards.Add(new Card("Rogatii", "Sprite/Cards/IcePack/RogatiiCard", 9000, 4000, 70));
        CardManager.AllCards.Add(new Card("Second Giant", "Sprite/Cards/IcePack/SecondGiantCard", 4000, 10000, 60));
    }

}
