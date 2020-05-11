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
        if(thisCard.isSpell && ((SpellCard)thisCard).spellTarget != SpellCard.TargetType.NO_TARGET)
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
        var spellCard = (SpellCard)thisCard;

        switch(spellCard.spell)
        {
            case SpellCard.SpellType.ADD_PROVOCATION: //добавление провокиции
                if(!target.thisCard.isProvocation)
                {
                    target.thisCard.abilities.Add(Card.AbilityType.PROVOCATION);
                    target.ability.provocation.SetActive(true);
                }
            break;


            case SpellCard.SpellType.DAMAGE_CARD: //урон одной карте
                GiveDamageTo(target, spellCard.spellValue);
            break;


            case SpellCard.SpellType.DAMAGE_CARDS: //урон всем картам соперника
                var enemyCards = isPlayerCard ?
                                 new List<CardControllerScript>(gameManager.enemyFieldCards):
                                 new List<CardControllerScript>(gameManager.playerFieldCards);
                
                foreach(var card in enemyCards)
                    GiveDamageTo(card, spellCard.spellValue);
            break;


            case SpellCard.SpellType.DAMAGE_HERO: //урон герою
                if(isPlayerCard)
                    gameManager.enemyHP -= spellCard.spellValue;
                else
                    gameManager.playerHP -= spellCard.spellValue;
                
                gameManager.ShowHP();
                gameManager.CheckForResult();
            break;


            case SpellCard.SpellType.DESTROY_CARD: //уничтожение карты
                GiveDamageTo(target, target.thisCard.helth);
            break;


            case SpellCard.SpellType.HEAL_CARD: //хилл одной карты
                target.thisCard.helth += spellCard.spellValue;
            break;


            case SpellCard.SpellType.HEAL_CARDS: //хилл всех своих карт
                var allyCards = isPlayerCard ? 
                                gameManager.playerFieldCards : 
                                gameManager.enemyFieldCards;

                foreach(var card in allyCards)
                {
                    card.thisCard.helth += spellCard.spellValue;
                    card.info.RefreshData();
                } 
            break;


            case SpellCard.SpellType.HEAL_HERO: //хилл героя
                if(isPlayerCard)
                    gameManager.playerHP += spellCard.spellValue;
                else
                    gameManager.enemyHP += spellCard.spellValue;
                
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
