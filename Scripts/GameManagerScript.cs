using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Game
{
    public List<Card> EnemyDeck, PlayerDeck;

    public Game()
    {
        EnemyDeck = GiveDeckCard();
        PlayerDeck = GiveDeckCard();

        //EnemyHand = new List<Card>();
        //PlayerHand = new List<Card>();

        //EnemyField = new List<Card>();
        //PlayerField = new List<Card>();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();
        for(int i = 0; i < 10/*карты в колоде*/; i++)
            list.Add(CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)]);
        
        return list;
    }
}

public class GameManagerScript : MonoBehaviour
{
    public Game CurrentGame;
    public Transform EnemyHand, PlayerHand, EnemyField, PlayerField;
    public GameObject CardPref;
    int Turn, TurnTime = 25;
    public Text TurnTimeText;
    public Button EndTurnBtn;

    public List<CardInfoScript> PlayerHandCards = new List<CardInfoScript>(),
                                PlayerFieldCards = new List<CardInfoScript>(),
                                EnemyHandCards = new List<CardInfoScript>(),
                                EnemyFieldCards = new List<CardInfoScript>();

    public bool IsPlayerTurn
    {
        get{return Turn % 2 == 0;}
    }

    void Start()
    {
        Turn = 0;

        CurrentGame = new Game();

        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);

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
        
        Card card = deck[0];

        GameObject cardGO = Instantiate(CardPref, hand, false);

        if(hand == EnemyHand)
        { 
            cardGO.GetComponent<CardInfoScript>().HideCardInfo(card);//скрытие руки соперника
            EnemyHandCards.Add(cardGO.GetComponent<CardInfoScript>());
        }
        else
        {
            cardGO.GetComponent<CardInfoScript>().ShowCardInfo(card, true);
            PlayerHandCards.Add(cardGO.GetComponent<CardInfoScript>());
            cardGO.GetComponent<AttackedCardScript>().enabled = false;
        }

        deck.RemoveAt(0);
    }

    IEnumerator TurnFunk() // таймер хода
    {
        TurnTime = 25;
        TurnTimeText.text = TurnTime.ToString();

        foreach(var card in PlayerFieldCards)
            card.DeHighlightCard();


        if(IsPlayerTurn)//ход игрока
        {
            foreach(var card in PlayerFieldCards)
            {
                card.SelfCard.ChangeAttackState(true); //разрешить картам атаку
                card.HighlightCard();
            }

            while(TurnTime-- > 0)
            {
                TurnTimeText.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }
        }
        else//ход противника
        {
            foreach(var card in EnemyFieldCards)
                card.SelfCard.ChangeAttackState(true);

            while(TurnTime-- > 20)
            {
                TurnTimeText.text = TurnTime.ToString();
                yield return new WaitForSeconds(1);
            }

            if(EnemyHandCards.Count > 0)
                EnemyTurn(EnemyHandCards);
        }
        ChangeTurn();
    }

    void EnemyTurn(List<CardInfoScript> cards)
    {
        if (EnemyFieldCards.Count > 5/*баг с выкладыванием бльшего кол-ва карт*/)
            return;

        int count = Random.Range(0, cards.Count + 1);

        for(int i = 0; i < count; i++) //выставление карт соперником для проверки
        {
            cards[0].ShowCardInfo(cards[0].SelfCard, false);
            cards[0].transform.SetParent(EnemyField);

            EnemyFieldCards.Add(cards[0]);
            EnemyHandCards.Remove(cards[0]);
        }

        foreach(var activeCard in EnemyFieldCards.FindAll(x => x.SelfCard.CanAttack)) // атака соперником для проверки
        {
            if(PlayerFieldCards.Count == 0)
                return;
            
            var enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];

            Debug.Log(activeCard.SelfCard.Name);

            activeCard.SelfCard.ChangeAttackState(false);
            CardsFight(enemy, activeCard);
        }
    }

    public void ChangeTurn() //смена хода
    {
        StopAllCoroutines();
        Turn++;
        EndTurnBtn.interactable = IsPlayerTurn;

        if(IsPlayerTurn && PlayerHandCards.Count < 5/*макс. кол-во карт в руке*/)
            GiveCardToHand(CurrentGame.PlayerDeck, PlayerHand);
        else if (EnemyHandCards.Count < 5/*макс. кол-во карт в руке*/)
            GiveCardToHand(CurrentGame.EnemyDeck, EnemyHand);
        
        StartCoroutine(TurnFunk());
    }

    /*void GiveNewCards()
    {
        
        
    }*/

    public void CardsFight(CardInfoScript playerCard, CardInfoScript enemyCard)
    {
        playerCard.SelfCard.GetDamage(enemyCard.SelfCard.Attack);
        enemyCard.SelfCard.GetDamage(playerCard.SelfCard.Attack);

        if(playerCard.SelfCard.IsAlive)
            playerCard.RefreshData();
        else
            DestroyCard(playerCard);

        if(enemyCard.SelfCard.IsAlive)
            enemyCard.RefreshData();
        else
            DestroyCard(enemyCard);
    }

    void DestroyCard(CardInfoScript card)
    {
        card.GetComponent<CardMovementScript>().OnEndDrag(null); // для снятия прототипа карты с поля

        if (PlayerFieldCards.Exists(x => x == card)) //карта на поле игрока
            PlayerFieldCards.Remove(card);
        else /*if (EnemyFieldCards.Exists(x => x == card))*/ // поле соперника
            EnemyFieldCards.Remove(card);

        Destroy(card.gameObject);
    }
}
