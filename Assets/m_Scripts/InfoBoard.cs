using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class InfoBoard : MonoBehaviour
{
    public TMP_Text m_NameInfoBoard, m_Des, m_Buy, m_Sell;
    public Image frame, icon;
    public Button Action;
    public GameObject notice, confirm;
    private int id;
    private Item item;
    private string m_Type;

    public void SetInfo(string Type, Item item)
    {
        if (item == null) return;
        this.item = item;
        id = item.id;
        m_Type = Type;
        if (Type == InfoBoardType.Inf_Buy)
        {
            Action.GetComponent<Image>().color = Color.green;
            Action.transform.GetChild(0).GetComponent<TMP_Text>().text = "Buy";
        }
        if (Type == InfoBoardType.Inf_Sell)
        {
            Action.GetComponent<Image>().color = Color.red;
            Action.transform.GetChild(0).GetComponent<TMP_Text>().text = "Sell";
        }
        if (Type == InfoBoardType.Inf_Show) Destroy(Action.gameObject);
        m_NameInfoBoard.text = "Name : " + item.Name;
        m_Des.text = "Description : " + item.Des;
        m_Buy.text = "Buy : " + item.cost;
        m_Sell.text = "Sell : " + (item.cost/4*3);
        frame.sprite = item.frame;
        icon.sprite = item.icon;
    }

    public void m_Cancel()
    {
        Destroy(gameObject);
    }

    public void m_ActionButton()
    {
        if (m_Type == InfoBoardType.Inf_Sell)
        {
            ConfirmBoard obj = Instantiate(confirm).GetComponent<ConfirmBoard>();
            obj.SetInfo("Proceed?","Do you want to sell " + item.Name + " for " + (item.cost / 4 * 3) + " gems? You have " + ItemList.Gems + " gems?");
            obj.Confirm.AddListener(m_SellItem);
            obj.transform.SetParent(transform.root.transform);
            obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            obj.transform.position = transform.root.position;
        }
        if (m_Type == InfoBoardType.Inf_Buy)
        {
            if (ItemList.Gems >= item.cost) {
                Debug.Log(item + " " + item.cost);
                ConfirmBoard obj = Instantiate(confirm).GetComponent<ConfirmBoard>();
                obj.SetInfo("Proceed?", "Do you want to buy " + item.Name + " for " + item.cost + " gems? You have " + ItemList.Gems + " gems?");
                obj.Confirm.AddListener(m_BuyItem);
                obj.transform.SetParent(transform.root.transform);
                obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                obj.transform.position = transform.root.position;
            } else
            {
                NoticeBoard obj = Instantiate(notice).GetComponent<NoticeBoard>();
                obj.SetInfo("Not enough gems!", "You don't have enough gems to purchase " + item.Name + "!");
                obj.transform.SetParent(transform.root.transform);
                obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                obj.transform.position = transform.root.position;
            }
        }
    }

    public void m_SellItem()
    {
        NoticeBoard obj = Instantiate(notice).GetComponent<NoticeBoard>();
        obj.SetInfo("Sold!", "You have sold " + item.Name + " for " + (item.cost / 4 * 3) + " gems!");
        obj.transform.SetParent(transform.root.transform);
        obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        obj.transform.position = transform.root.position;
        //
        ItemList.Gems += ItemList.items[id].cost / 4 * 3;
        ItemList.items[id].Own = false;
        GameObject.Find("BagFrame").GetComponent<BagController>().btn_Skill.onClick.Invoke();
        Destroy(gameObject);
    }

    public void m_BuyItem()
    {
        NoticeBoard obj = Instantiate(notice).GetComponent<NoticeBoard>();
        obj.SetInfo("Sold!", "You have buy " + item.Name + " for " + item.cost + " gems!");
        obj.transform.SetParent(transform.root.transform);
        obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        obj.transform.position = transform.root.position;
        //
        ItemList.Gems -= item.cost;
        ItemList.items[id].Own = true;
        GameObject.Find("ShopFrame").GetComponent<ShopController>().btn_Skill.onClick.Invoke();
        int xxx = 2;
        if (DailyAchieveList.dailyTasks[xxx].progressIndex < DailyAchieveList.dailyTasks[xxx].progressMax)
        {
            DailyAchieveList.dailyTasks[xxx].progressIndex += item.cost;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex > DailyAchieveList.dailyTasks[xxx].progressMax)
                DailyAchieveList.dailyTasks[xxx].progressIndex = DailyAchieveList.dailyTasks[xxx].progressMax;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex == DailyAchieveList.dailyTasks[xxx].progressMax && !DailyAchieveList.dailyTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.dailyTasks[xxx].reward;
            }
        }
        xxx = 3;
        if (DailyAchieveList.dailyTasks[xxx].progressIndex < DailyAchieveList.dailyTasks[xxx].progressMax)
        {
            DailyAchieveList.dailyTasks[xxx].progressIndex++;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex > DailyAchieveList.dailyTasks[xxx].progressMax)
                DailyAchieveList.dailyTasks[xxx].progressIndex = DailyAchieveList.dailyTasks[xxx].progressMax;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex == DailyAchieveList.dailyTasks[xxx].progressMax && !DailyAchieveList.dailyTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.dailyTasks[xxx].reward;
            }
        }
        xxx = 0;
        if (DailyAchieveList.achieveTasks[xxx].progressIndex < DailyAchieveList.achieveTasks[xxx].progressMax)
        {
            DailyAchieveList.achieveTasks[xxx].progressIndex += item.cost;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex > DailyAchieveList.achieveTasks[xxx].progressMax)
                DailyAchieveList.achieveTasks[xxx].progressIndex = DailyAchieveList.achieveTasks[xxx].progressMax;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex == DailyAchieveList.achieveTasks[xxx].progressMax && !DailyAchieveList.achieveTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.achieveTasks[xxx].reward;
            }
        }
        xxx = 1;
        if (DailyAchieveList.achieveTasks[xxx].progressIndex < DailyAchieveList.achieveTasks[xxx].progressMax)
        {
            DailyAchieveList.achieveTasks[xxx].progressIndex += item.cost;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex > DailyAchieveList.achieveTasks[xxx].progressMax)
                DailyAchieveList.achieveTasks[xxx].progressIndex = DailyAchieveList.achieveTasks[xxx].progressMax;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex == DailyAchieveList.achieveTasks[xxx].progressMax && !DailyAchieveList.achieveTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.achieveTasks[xxx].reward;
            }
        }
        xxx = 2;
        if (DailyAchieveList.achieveTasks[xxx].progressIndex < DailyAchieveList.achieveTasks[xxx].progressMax)
        {
            DailyAchieveList.achieveTasks[xxx].progressIndex += item.cost;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex > DailyAchieveList.achieveTasks[xxx].progressMax)
                DailyAchieveList.achieveTasks[xxx].progressIndex = DailyAchieveList.achieveTasks[xxx].progressMax;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex == DailyAchieveList.achieveTasks[xxx].progressMax && !DailyAchieveList.achieveTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.achieveTasks[xxx].reward;
            }
        }
        xxx = 3;
        if (DailyAchieveList.achieveTasks[xxx].progressIndex < DailyAchieveList.achieveTasks[xxx].progressMax)
        {
            DailyAchieveList.achieveTasks[xxx].progressIndex++;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex > DailyAchieveList.achieveTasks[xxx].progressMax)
                DailyAchieveList.achieveTasks[xxx].progressIndex = DailyAchieveList.achieveTasks[xxx].progressMax;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex == DailyAchieveList.achieveTasks[xxx].progressMax && !DailyAchieveList.achieveTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.achieveTasks[xxx].reward;
            }
        }
        xxx = 4;
        if (DailyAchieveList.achieveTasks[xxx].progressIndex < DailyAchieveList.achieveTasks[xxx].progressMax)
        {
            DailyAchieveList.achieveTasks[xxx].progressIndex++;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex > DailyAchieveList.achieveTasks[xxx].progressMax)
                DailyAchieveList.achieveTasks[xxx].progressIndex = DailyAchieveList.achieveTasks[xxx].progressMax;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex == DailyAchieveList.achieveTasks[xxx].progressMax && !DailyAchieveList.achieveTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.achieveTasks[xxx].reward;
            }
        }
        xxx = 5;
        if (DailyAchieveList.achieveTasks[xxx].progressIndex < DailyAchieveList.achieveTasks[xxx].progressMax)
        {
            DailyAchieveList.achieveTasks[xxx].progressIndex++;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex > DailyAchieveList.achieveTasks[xxx].progressMax)
                DailyAchieveList.achieveTasks[xxx].progressIndex = DailyAchieveList.achieveTasks[xxx].progressMax;
            if (DailyAchieveList.achieveTasks[xxx].progressIndex == DailyAchieveList.achieveTasks[xxx].progressMax && !DailyAchieveList.achieveTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.achieveTasks[xxx].reward;
            }
        }
        Destroy(gameObject);
    }
}

public class InfoBoardType
{
    public static string Inf_Buy = "Buy", Inf_Sell = "Sell", Inf_Show = "Show";
}
