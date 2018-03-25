using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class QuickBarCell : MonoBehaviour,IDropHandler,IBeginDragHandler,IDragHandler,IEndDragHandler {

    QuickBar quickBar;    
    int index;//Порядковый номер ячейки начиная с 0
    public Sprite emptyButtonSprite;
    public Color emptyButtonColor;

    Transform beginDragT;
    Vector3 beginDragPos;
    [SerializeField] Transform canvasT;

    // Use this for initialization
    void Start () {
        
        quickBar = transform.parent.parent.GetComponent<QuickBar>();
        canvasT = GameObject.Find("Canvas").transform;
    }	
	

    public void SetIndex(int _index)
    {
        index = _index;
    }

    public int GetIndex()
    {
        return index;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        Debug.Log("OnDrop name =" + eventData.pointerDrag.name);

        //Добавляем предмет в QuickBar из инвентаря
        InventButton invButton = eventData.pointerDrag.GetComponent<InventButton>();
        if(invButton!= null)
        {
            if(invButton._gameItemRef != null)
            {
                
                quickBar.AddGameItem(invButton._gameItemRef,index);
            }        

        }

        //Перемещение из ячейки квик бара
        QuickBarCell qBcell = eventData.pointerDrag.GetComponent<QuickBarCell>();
        if (qBcell != null)
        {            
            quickBar.ReplaceGameItem(index, qBcell.GetIndex());
        }


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginDragT = transform.parent;
        beginDragPos = transform.position;
        transform.SetParent(canvasT);
        GetComponent<CanvasGroup>().blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnDragEnd name =" + eventData.pointerDrag.name);
        transform.SetParent(beginDragT);
        transform.position = beginDragPos;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }
}
