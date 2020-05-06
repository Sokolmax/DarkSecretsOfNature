using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Game
{
    public List<Card> enemyDeck, playerDeck;

    public Game()
    {
        enemyDeck = GiveDeckCard();
        playerDeck = GiveDeckCard();

        //EnemyHand = new List<Card>();
        //PlayerHand = new List<Card>();

        //EnemyField = new List<Card>();
        //PlayerField = new List<Card>();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for(int i = 0; i < 15/*карты в колоде*/; i++)
            list.Add(CardManager.allCards[Random.Range(0, CardManager.allCards.Count)]);
        
        return list;
    }
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript instance;

    public Game currentGame;
    public Transform enemyHand, playerHand, enemyField, playerField;
    public GameObject cardPref;
    int turn, turnTime = 25;
    public Text turnTimeText;
    public Button endTurnBtn;

    public int playerEnergy, enemyEnergy;
    public Text selfEnergyTxt, enemyEnergyTxt;

    public int playerHP, enemyHP;
    public Text playerHPTxt, enemyHPTxt;

    public GameObject endBg, vinGO, loseGO;

    public AttackedHeroScript enemyHero, playerHero;

    public List<CardControllerScript> playerHandCards = new List<CardControllerScript>(),
                                playerFieldCards = new List<CardControllerScript>(),
                                enemyHandCards = new List<CardControllerScript>(),
                                enemyFieldCards = new List<CardControllerScript>();

    public bool isPlayerTurn
    {
        get{return turn % 2 == 0;}
    }

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        turn = 0;

        currentGame = new Game();

        GiveHandCards(currentGame.enemyDeck, enemyHand);
        GiveHandCards(currentGame.playerDeck, playerHand);
        playerEnergy = enemyEnergy = 20;
        playerHP = enemyHP = 20000;

        ShowEnergy();
        ShowHP();

        endBg.SetActive(false);
        vinGO.SetActive(false);
        loseGO.SetActive(false);
        
        StartCoroutine(TurnFunk());
    }

    void GiveHandCards(List<Card> deck, Transform hand) //стартовая рука
    {
        int i = 0;
        while(i++ < 3)
            GiveCardToHand(deck, hand);
    }

    void GiveCardToHand(List<Card> deck, Transform hand) // взять новую карту
    {
        if(deck.Count == 0)
            return;
        
        CreateCardPref(deck[0], hand);

        deck.RemoveAt(0);
    }


    void CreateCardPref(Card card, Transform hand)//создание префаба карты
    {
        GameObject cardGO = Instantiate(cardPref, hand, false);
        CardControllerScript cardC = cardGO.GetComponent<CardControllerScript>();

        cardC.Init(card, hand == playerHand);

        if(cardC.isPlayerCard)
            playerHandCards.Add(cardC);
        else
            enemyHandCards.Add(cardC);
    }

    IEnumerator TurnFunk() // карутина хода
    {
        turnTime = 25;
        turnTimeText.text = turnTime.ToString();

        foreach(var card in playerFieldCards)
            card.info.HighlightCard(false);

        CheckCardsForManaAvailability();
        
        if(isPlayerTurn)//ход игрока
        {
            foreach(var card in playerFieldCards)
            {
                card.thisCard.canAttack = true; //разрешить картам атаку
                card.info.HighlightCard(true);
                card.ability.OnNewTurn();
            }

            while(turnTime-- > 0)
            {
                turnTimeText.text = turnTime.ToString();
                yield return new WaitForSeconds(1);
            }
            ChangeTurn();
        }
        else//ход противника
        {
            foreach(var card in enemyFieldCards)
            {
                card.thisCard.canAttack = true;
                card.ability.OnNewTurn();
            }

            StartCoroutine(EnemyTurn(enemyHandCards));
        }
        
    }

    IEnumerator EnemyTurn(List<CardControllerScript> cards)
    {
        yield return new WaitForSeconds(1);

        int count = Random.Range(0, cards.Count + 1);

        for(int i = 0; i < count; i++) //выставление карт соперником для проверки
        {
            if (enemyFieldCards.Count > 5/*баг с выкладыванием бльшего кол-ва карт*/ || enemyEnergy == 0
                || enemyHandCards.Count == 0)
                break;

            List<CardControllerScript> cardsList = cards.FindAll(x => enemyEnergy >= x.thisCard.cost); //карты с подходящей ценой в руке

            if(cardsList.Count == 0)
                break;

            cardsList[0].GetComponent<CardMovementScript>().MoveToField(enemyField);

            yield  return new WaitForSeconds(0.51f);

            cardsList[0].transform.SetParent(enemyField);

            cardsList[0].OnCast();
        }

        yield  return new WaitForSeconds(1);

        while(enemyFieldCards.Exists(x => x.thisCard.canAttack)) // атака соперником для проверки
        {
            var activeCard = enemyFieldCards.FindAll(x => x.thisCard.canAttack)[0];
            bool hasProvocation  = playerFieldCards.Exists(x => x.thisCard.isProvocation);

            if(hasProvocation || Random.Range(0, 2) == 0 && playerFieldCards.Count > 0)
            {
                CardControllerScript enemy;

                if(hasProvocation)
                    enemy = playerFieldCards.Find(x => x.thisCard.isProvocation);
                else
                    enemy = playerFieldCards[Random.Range(0, playerFieldCards.Count)];

                //Debug.Log(activeCard.SelfCard.Name);

                activeCard.thisCard.canAttack = false;

                activeCard.movement.MoveToTurget(enemy.transform);
                yield return new WaitForSeconds(.75f);

                CardsFight(enemy, activeCard);
            }
            else
            {
                activeCard.thisCard.canAttack = false;

                activeCard.GetComponent<CardMovementScript>().MoveToTurget(playerHero.transform);
                yield return new WaitForSeconds(.75f);

                DamageHero(activeCard, false);
            }
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds(1);
        ChangeTurn();
    }

    public void ChangeTurn() //смена хода
    {
        StopAllCoroutines();
        turn++;
        endTurnBtn.interactable = isPlayerTurn;

        if(isPlayerTurn)
        {
            playerEnergy += turn/2*10;

            if(playerHandCards.Count < 5/*макс. кол-во карт в руке*/)
                GiveCardToHand(currentGame.playerDeck, playerHand);
            
        }
        else
        {
            enemyEnergy += turn/2*10;

            if(enemyHandCards.Count < 5/*макс. кол-во карт в руке*/)
            GiveCardToHand(currentGame.enemyDeck, enemyHand);
        }
        ShowEnergy();

        StartCoroutine(TurnFunk());
    }

    /*void GiveNewCards()
    {
        
        
    }*/

    public void CardsFight(CardControllerScript attacker, CardControllerScript defender)
    {
        defender.thisCard.GetDamage(attacker.thisCard.attack);

        attacker.OnDamageDeal();
        defender.OnTakeDamage(attacker);
        attacker.OnTakeDamage();

        attacker.thisCard.GetDamage(defender.thisCard.attack);

        defender.CheckForAlive();
        attacker.CheckForAlive();
    }


    void ShowEnergy()
    {
        selfEnergyTxt.text = playerEnergy.ToString();
        enemyEnergyTxt.text = enemyEnergy.ToString();
    }

    void ShowHP()
    {
        playerHPTxt.text = playerHP.ToString();
        enemyHPTxt.text = enemyHP.ToString();
    }

    public void ReduceEnergy(bool playerEnergy, int cost)
    {
        if(playerEnergy)
            this.playerEnergy = Mathf.Clamp(this.playerEnergy - cost, 0, int.MaxValue);
        else
            enemyEnergy = Mathf.Clamp(enemyEnergy - cost, 0, int.MaxValue);

        ShowEnergy();
    }

    public void DamageHero(CardControllerScript card, bool isEnemyAttacked)
    {
        if(isEnemyAttacked)
            enemyHP = Mathf.Clamp(enemyHP - card.thisCard.attack, 0, int.MaxValue);
        else
            playerHP = Mathf.Clamp(playerHP - card.thisCard.attack, 0, int.MaxValue);
        
        ShowHP();
        card.OnDamageDeal();
        CheckForResult();
    }

    void CheckForResult()
    {
        if(enemyHP <= 0)
        {
            vinGO.SetActive(true);
            endBg.SetActive(true);
            
            StopAllCoroutines();
        }
        else if(playerHP <= 0)
        {
            endBg.SetActive(true);
            loseGO.SetActive(true);
            StopAllCoroutines();
        }
    }

    public void CheckCardsForManaAvailability() //прозрачность карт
    {
        foreach(var card in playerHandCards)
            card.info.HighlightManaAvailability(playerEnergy);
    }

    public void HighlightTargets(bool highlight)// подсветка карт для атаки
    {
        List<CardControllerScript> targets = new List<CardControllerScript>();

        if(enemyFieldCards.Exists(x => x.thisCard.isProvocation))
            targets = enemyFieldCards.FindAll(x => x.thisCard.isProvocation);
        else
        {
            targets = enemyFieldCards;
            enemyHero.HighlightHero(highlight);
        }

        foreach(var card in targets)
            card.info.HighlightAsTarget(highlight);
        
    }
}
