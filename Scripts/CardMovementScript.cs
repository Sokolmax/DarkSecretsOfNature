using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO;
    public GameManagerScript GameManager; // для проверки хода
    public bool IsDraggable; //проверка на возможность перетягивания карты по другим полям

    void Awake() // or start
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
        GameManager = FindObjectOfType<GameManagerScript>();
    }

    public void OnBeginDrag(PointerEventData eventData) // начало движения карты
    {
        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = transform.parent;

        IsDraggable = GameManager.IsPlayerTurn && 
                      (
                      (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.SELF_HAND 
                      && GameManager.PlayerEnergy >= GetComponent<CardInfoScript>().SelfCard.Cost)
                      || (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.SELF_FIELD
                      && GetComponent<CardInfoScript>().SelfCard.CanAttack)
                      );
        
                    /*(DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.SELF_HAND ||
                      DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.SELF_FIELD)
                      && GameManager.IsPlayerTurn;*/

        if(!IsDraggable)
            return;

        if(GetComponent<CardInfoScript>().SelfCard.CanAttack)
            GameManager.HighlightTargets(true);
        
        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) // движение
    {
        if(!IsDraggable)
            return;

        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if(TempCardGO.transform.parent != DefaultTempCardParent)
            TempCardGO.transform.SetParent(DefaultTempCardParent);
            
        if(DefaultParent.GetComponent<DropPlaceScript>().Type != FieldType.SELF_FIELD) //запрет перемещения карт по полю    
            CheckPosition(); 
    }

    public void OnEndDrag(PointerEventData eventData) // отпускание карты
    {
        if(!IsDraggable)
            return;

         GameManager.HighlightTargets(false);

        transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // замена позиции карты на позицию прототипа
        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex()); 
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(800, 0);
    }


    void CheckPosition() // для замены и запоминание позиции карты через прототип
    {
        int newIndex = DefaultTempCardParent.childCount;

        for(int i = 0; i < DefaultTempCardParent.childCount; i++)
        {
            if(transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if(TempCardGO.transform.GetSiblingIndex() < newIndex)
                {
                    newIndex--;
                }
                break;
            }
        }

        TempCardGO.transform.SetSiblingIndex(newIndex);
    }

    public void MoveToField(Transform field)//плавный розыгрыш карты
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(field.position, .5f);
    }

    public void MoveToTurget(Transform target)// плавное перемещение карты
    {
        StartCoroutine(MoveToTurgetCor(target));
    }

    IEnumerator MoveToTurgetCor(Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();

        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        transform.SetParent(GameObject.Find("Canvas").transform);

        transform.DOMove(target.position, .25f);

        yield return new WaitForSeconds(.25f);

        transform.DOMove(pos, .25f);

        yield return new WaitForSeconds(.25f);

        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
