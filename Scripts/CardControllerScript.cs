using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControllerScript : MonoBehaviour
{
    public Card thisCard;

    public bool isPlayerCard;

    public CardInfoScript info;
    public CardMovementScript movement;
    public CardAbility ability;

    GameManagerScript gameManager;

    public void Init(Card card, bool isPlayerCard)
    {
        thisCard = card;
        gameManager = GameManagerScript.instance;
        this.isPlayerCard = isPlayerCard;

        if(isPlayerCard)
        {
            info.ShowCardInfo();
            GetComponent<AttackedCardScript>().enabled = false;
        }
        else
        {
            info.HideCardInfo();
        }
    }

    public void OnCast()//каст
    {
        if(thisCard.isSpell && thisCard.spellTarget != Card.TargetType.NO_TARGET)
            return;

        if(isPlayerCard)
        {
            gameManager.playerHandCards.Remove(this);
            gameManager.playerFieldCards.Add(this);
            gameManager.ReduceEnergy(true, thisCard.cost);
            gameManager.CheckCardsForManaAvailability();
        }
        else
        {
            gameManager.enemyHandCards.Remove(this);
            gameManager.enemyFieldCards.Add(this);
            gameManager.ReduceEnergy(false, thisCard.cost);
            info.ShowCardInfo();
        }

        thisCard.isPlaced = true;

        if(thisCard.hasAbility)
            ability.OnCast();
        
        if(thisCard.isSpell)
            UseSpell(null);
    }

    public void OnTakeDamage()//получение урона
    {
        if(thisCard.hasAbility)
            ability.OnDamageTake();
        CheckForAlive();
    }

    public void OnDamageDeal()//нанесение урона
    {
        thisCard.timesDealeDamage++;
        thisCard.canAttack = false;
        info.HighlightCard(false);

        if(thisCard.hasAbility)
            ability.OnDamageDeal();
    }

    public void UseSpell(CardControllerScript target)
    {
        switch(thisCard.spell)
        {
            case Card.SpellType.ADD_PROVOCATION: //добавление провокиции
                if(!target.thisCard.isProvocation)
                {
                    target.thisCard.abilities.Add(Card.AbilityType.PROVOCATION);
                    target.ability.provocation.SetActive(true);
                }
            break;


            case Card.SpellType.DAMAGE_CARD: //урон одной карте
                GiveDamageTo(target, thisCard.spellValue);
            break;


            case Card.SpellType.DAMAGE_CARDS: //урон всем картам соперника
                var enemyCards = isPlayerCard ?
                                 new List<CardControllerScript>(gameManager.enemyFieldCards):
                                 new List<CardControllerScript>(gameManager.playerFieldCards);
                
                foreach(var card in enemyCards)
                    GiveDamageTo(card, thisCard.spellValue);
            break;


            case Card.SpellType.DAMAGE_HERO: //урон герою
                if(isPlayerCard)
                    gameManager.enemyHP -= thisCard.spellValue;
                else
                    gameManager.playerHP -= thisCard.spellValue;
                
                gameManager.ShowHP();
                gameManager.CheckForResult();
            break;


            case Card.SpellType.DESTROY_CARD: //уничтожение карты
                GiveDamageTo(target, target.thisCard.helth);
            break;


            case Card.SpellType.HEAL_CARD: //хилл одной карты
                target.thisCard.helth += thisCard.spellValue;
            break;


            case Card.SpellType.HEAL_CARDS: //хилл всех своих карт
                var allyCards = isPlayerCard ? 
                                gameManager.playerFieldCards : 
                                gameManager.enemyFieldCards;

                foreach(var card in allyCards)
                {
                    card.thisCard.helth += thisCard.spellValue;
                    card.info.RefreshData();
                } 
            break;


            case Card.SpellType.HEAL_HERO: //хилл героя
                if(isPlayerCard)
                    gameManager.playerHP += thisCard.spellValue;
                else
                    gameManager.enemyHP += thisCard.spellValue;
                
                gameManager.ShowHP();
            break;
        }

        if(target != null)
        {
            target.ability.OnCast();
            target.CheckForAlive();
        }
        DestroyCard();
    }

    void GiveDamageTo(CardControllerScript card, int damage)
    {
        card.thisCard.GetDamage(damage);
        card.CheckForAlive();
        card.OnTakeDamage();
    }

    public void CheckForAlive()
    {
        if(thisCard.isAlive)
            info.RefreshData();
        else
            DestroyCard();
    }

    public void DestroyCard()
    {
        movement.OnEndDrag(null);

        RemoveCardFromList(gameManager.enemyFieldCards);
        RemoveCardFromList(gameManager.enemyHandCards);
        RemoveCardFromList(gameManager.playerFieldCards);
        RemoveCardFromList(gameManager.playerHandCards);

        Destroy(gameObject);
    }

    void RemoveCardFromList(List<CardControllerScript> list)//удаление карты из списка
    {
        if(list.Exists(x => x == this))
            list.Remove(this);
    }
}
