using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour
{
    public Item item;
    public InfoBoard board, spawnedBoard;
    public SelectBoard selectBoard, spawnedSelectBoard;
    public ConfirmBoard confirmBoard, spawnedConfirmBoard;
    public UnityEvent unityEvent;
    public CharacterControl character;

    public void m_ShowInfoSell()
    {
        lock (this)
        {
            if (spawnedBoard) return;
            spawnedBoard = Instantiate(board);
            spawnedBoard.SetInfo(InfoBoardType.Inf_Sell,item);
            spawnedBoard.transform.SetParent(transform.root.transform);
            spawnedBoard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            spawnedBoard.transform.position = transform.root.position;
        }
    }

    public void m_ShowInfoBuy()
    {
        lock (this)
        {
            if (spawnedBoard) return;
            spawnedBoard = Instantiate(board);
            spawnedBoard.SetInfo(InfoBoardType.Inf_Buy, item);
            spawnedBoard.transform.SetParent(transform.root.transform);
            spawnedBoard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            spawnedBoard.transform.position = transform.root.position;
        }
    }

    public void m_ShowInfo()
    {
        lock (this)
        {
            if (spawnedBoard) return;
            spawnedBoard = Instantiate(board);
            spawnedBoard.SetInfo(InfoBoardType.Inf_Show, item);
            spawnedBoard.transform.SetParent(transform.root.transform);
            spawnedBoard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            spawnedBoard.transform.position = transform.root.position;
        }
    }

    public void m_ShowSelection()
    {
        lock (this)
        {
            if (spawnedSelectBoard) return;
            spawnedSelectBoard = Instantiate(selectBoard);
            spawnedSelectBoard.SetInfo(item,character);
            spawnedSelectBoard.transform.SetParent(transform.root.transform);
            spawnedSelectBoard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            spawnedSelectBoard.transform.position = transform.root.position;
        }
    }

    public void m_Equip()
    {
        lock (this)
        {
            if (spawnedConfirmBoard) return;
            spawnedConfirmBoard = Instantiate(confirmBoard);
            spawnedConfirmBoard.SetInfo("Proceed?","Do you want to change character to " + item.Name + "?");
            spawnedConfirmBoard.transform.SetParent(transform.root.transform);
            spawnedConfirmBoard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            spawnedConfirmBoard.transform.position = transform.root.position;
            spawnedConfirmBoard.Confirm.AddListener(m_ChangeCharacter);
        }
    }

    public void m_ChangeCharacter()
    {

    }

    private void FixedUpdate()
    {
        if (item == null) { }
        else
        {
            GetComponent<Image>().sprite = item.frame;
            transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
        }
    }
}
