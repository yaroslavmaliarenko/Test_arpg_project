using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuickBar : MonoBehaviour {

    List<GameItem> listGameItem;
    public bool useBlock;
    
    // Use this for initialization
    void Start () {
        listGameItem = new List<GameItem>();
        useBlock = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            QuickBarCell qbCell = transform.GetChild(i).GetChild(0).GetComponent<QuickBarCell>();
            if(qbCell != null)
            {
                qbCell.SetIndex(i);
                GameItem _item = null;
                listGameItem.Add(_item);

            }
        }

        Debug.Log("list item count = " + listGameItem.Count);

    }
	
	// Update is called once per frame
	void Update () {

        if (Managers.player.IsDead()) return;
        if (useBlock) return;

        int KeyDownIndex = GetKeyDownIndex();

        if(KeyDownIndex != -1)
        {
            if (!Managers.player.moveBlock)
            {
                if(listGameItem[KeyDownIndex]!=null) Managers.inventory.UseItemFromInventory(listGameItem[KeyDownIndex]);

            }
            
        }

             

        for (int i = 0; i < transform.childCount; i++)
        {

            QuickBarCell qBcell = transform.GetChild(i).GetChild(0).GetComponent<QuickBarCell>();
            Image _cellImage = transform.GetChild(i).GetChild(0).GetComponent<Image>();

            GameItem _item = listGameItem[i];
            if (_item != null)
            {
                _cellImage.sprite = _item._itemSprite;
                _cellImage.color = new Color(255, 255, 255, 255);

                if (_item._count > 0) qBcell.transform.GetChild(0).GetComponent<Text>().text = _item._count.ToString();
                else qBcell.transform.GetChild(0).GetComponent<Text>().text = "";




            }
            else
            {
                _cellImage.sprite = qBcell.emptyButtonSprite;
                _cellImage.color = qBcell.emptyButtonColor;
                qBcell.transform.GetChild(0).GetComponent<Text>().text = "";
            }


        }

    }


    public bool AddGameItem(GameItem _item,int index)
    {
        if(listGameItem.Count - 1 >= index)
        {
            int oldIndex = listGameItem.IndexOf(_item);//Проверяем есть ли такой объект у нас на панели
            if(oldIndex != -1) listGameItem[oldIndex] = null;            
            listGameItem[index] = _item;
            return true;
        }

        return false;        
    }

    public void ReplaceGameItem(int index1,int index2)
    {

        GameItem _item1 = listGameItem[index1];
        GameItem _item2 = listGameItem[index2];
        listGameItem[index1] = _item2;
        listGameItem[index2] = _item1;

    }

    int GetKeyDownIndex()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) return 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) return 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) return 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) return 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6)) return 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7)) return 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8)) return 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9)) return 8;
        else if (Input.GetKeyDown(KeyCode.Alpha0)) return 9;

        return -1;
    }
}
