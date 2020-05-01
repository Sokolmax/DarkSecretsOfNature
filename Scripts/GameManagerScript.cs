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

    public int PlayerEnergy, EnemyEnergy;
    public Text SelfEnergyTxt, EnemyEnergyTxt;

    public int PlayerHP, EnemyHP;
    public Text PlayerHPTxt, EnemyHPTxt;

    public GameObject EndBg, VinGO, LoseGO;

    public AttackedHeroScript EnemyHero, PlayerHero;

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
        PlayerEnergy = EnemyEnergy = 20;
        PlayerHP = EnemyHP = 20000;

        ShowEnergy();
        ShowHP();

        EndBg.SetActive(false);
        VinGO.SetActive(false);
        LoseGO.SetActive(false);
        
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

    IEnumerator TurnFunk() // карутина хода
    {
        TurnTime = 25;
        TurnTimeText.text = TurnTime.ToString();

        foreach(var card in PlayerFieldCards)
            card.DeHighlightCard();

        CheckCardsForAvailability();

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
            ChangeTurn();
        }
        else//ход противника
        {
            foreach(var card in EnemyFieldCards)
                card.SelfCard.ChangeAttackState(true);


            StartCoroutine(EnemyTurn(EnemyHandCards));
        }
        
    }

    IEnumerator EnemyTurn(List<CardInfoScript> cards)
    {
        yield return new WaitForSeconds(1);

        int count = Random.Range(0, cards.Count + 1);

        for(int i = 0; i < count; i++) //выставление карт соперником для проверки
        {
            if (EnemyFieldCards.Count > 5/*баг с выкладыванием бльшего кол-ва карт*/ || EnemyEnergy == 0
                || EnemyHandCards.Count == 0)
                break;

            List<CardInfoScript> cardsList = cards.FindAll(x => EnemyEnergy >= x.SelfCard.Cost); //карты с подходящей ценой в руке

            if(cardsList.Count == 0)
                break;

            cardsList[0].GetComponent<CardMovementScript>().MoveToField(EnemyField);

            yield  return new WaitForSeconds(0.51f);

            ReduceEnergy(false, cardsList[0].SelfCard.Cost);

            cardsList[0].ShowCardInfo(cardsList[0].SelfCard, false);
            cardsList[0].transform.SetParent(EnemyField);

            EnemyFieldCards.Add(cardsList[0]);
            EnemyHandCards.Remove(cardsList[0]);
        }

        yield  return new WaitForSeconds(1);

        foreach(var activeCard in EnemyFieldCards.FindAll(x => x.SelfCard.CanAttack)) // атака соперником для проверки
        {
            if(Random.Range(0, 2) == 0 && PlayerFieldCards.Count > 0)
            {
                var enemy = PlayerFieldCards[Random.Range(0, PlayerFieldCards.Count)];

                Debug.Log(activeCard.SelfCard.Name);

                activeCard.SelfCard.ChangeAttackState(false);

                activeCard.GetComponent<CardMovementScript>().MoveToTurget(enemy.transform);
                yield return new WaitForSeconds(.75f);

                CardsFight(enemy, activeCard);
            }
            else
            {
                activeCard.SelfCard.ChangeAttackState(false);

                activeCard.GetComponent<CardMovementScript>().MoveToTurget(PlayerHero.transform);
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
        Turn++;
        EndTurnBtn.interactable = IsPlayerTurn;

        if(IsPlayerTurn)
        {
            PlayerEnergy += Turn/2*10;

            if(PlayerHandCards.Count < 5/*макс. кол-во карт в руке*/)
                GiveCardToHand(CurrentGame.PlayerDeck, PlayerHand);
            
        }
        else
        {
            EnemyEnergy += Turn/2*10;

            if(EnemyHandCards.Count < 5/*макс. кол-во карт в руке*/)
            GiveCardToHand(CurrentGame.EnemyDeck, EnemyHand);
        }
        ShowEnergy();

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

    void ShowEnergy()
    {
        SelfEnergyTxt.text = PlayerEnergy.ToString();
        EnemyEnergyTxt.text = EnemyEnergy.ToString();
    }

    void ShowHP()
    {
        PlayerHPTxt.text = PlayerHP.ToString();
        EnemyHPTxt.text = EnemyHP.ToString();
    }

    public void ReduceEnergy(bool playerEnergy, int cost)
    {
        if(playerEnergy)
            PlayerEnergy = Mathf.Clamp(PlayerEnergy - cost, 0, int.MaxValue);
        else
            EnemyEnergy = Mathf.Clamp(EnemyEnergy - cost, 0, int.MaxValue);

        ShowEnergy();
    }

    public void DamageHero(CardInfoScript card, bool isEnemyAttacked)
    {
        if(isEnemyAttacked)
            EnemyHP = Mathf.Clamp(EnemyHP - card.SelfCard.Attack, 0, int.MaxValue);
        else
            PlayerHP = Mathf.Clamp(PlayerHP - card.SelfCard.Attack, 0, int.MaxValue);
        
        ShowHP();
        card.DeHighlightCard();
        CheckForResult();
    }

    void CheckForResult()
    {
        if(EnemyHP <= 0)
        {
            VinGO.SetActive(true);
            EndBg.SetActive(true);
            
            StopAllCoroutines();
        }
        else if(PlayerHP <= 0)
        {
            EndBg.SetActive(true);
            LoseGO.SetActive(true);
            StopAllCoroutines();
        }
    }

    public void CheckCardsForAvailability() //прозрачность карт
    {
        foreach(var card in PlayerHandCards)
            card.CheckForAvailability(PlayerEnergy);
    }

    public void HighlightTargets(bool highlight)// подсветка карт для атаки
    {
        foreach(var card in EnemyFieldCards)
            card.HighlightAsTarget(highlight);

        EnemyHero.HighlightHero(highlight);
        
    }
}
