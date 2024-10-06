using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyControl : MonoBehaviour
{
    public Button btn_Page1, btn_Page2;
    public GameObject DailyFrame;
    private GameObject[] DailyTasks = new GameObject[3];
    private static int[] DailyIds;
    //public TMP_Text Count;
    private Vector3 IncY = new(0, -1.75f);
    
    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (DailyAchieveList.DailyMission >= 1)
        {
            DailyIds = new int[5] { GenerateRandomUnownedItemId(0, DailyAchieveList.Daily), GenerateRandomUnownedItemId(0, DailyAchieveList.Daily), GenerateRandomUnownedItemId(0, DailyAchieveList.Daily), GenerateRandomUnownedItemId(0, DailyAchieveList.Daily), GenerateRandomUnownedItemId(0, DailyAchieveList.Daily) };
            while (DailyIds[1] == DailyIds[0]) DailyIds[1] = GenerateRandomUnownedItemId(0, DailyAchieveList.Daily, DailyIds[0]);
            while (DailyIds[2] == DailyIds[0] || DailyIds[2] == DailyIds[1])
                DailyIds[2] = GenerateRandomUnownedItemId(0, DailyAchieveList.Daily, DailyIds[0], DailyIds[1]);
            while (DailyIds[2] == DailyIds[0] || DailyIds[2] == DailyIds[1])
                DailyIds[2] = GenerateRandomUnownedItemId(0, DailyAchieveList.Daily, DailyIds[0], DailyIds[1]);
            while (DailyIds[3] == DailyIds[0] || DailyIds[3] == DailyIds[1] || DailyIds[3] == DailyIds[2])
                DailyIds[3] = GenerateRandomUnownedItemId(0, DailyAchieveList.Daily, DailyIds[0], DailyIds[1], DailyIds[2]);
            while (DailyIds[4] == DailyIds[0] || DailyIds[4] == DailyIds[1] || DailyIds[4] == DailyIds[2] || DailyIds[4] == DailyIds[3])
                DailyIds[4] = GenerateRandomUnownedItemId(0, DailyAchieveList.Daily, DailyIds[0], DailyIds[1], DailyIds[2], DailyIds[3]);
            DailyAchieveList.DailyMission--;
        }
        btn_Page1.onClick.Invoke();
    }

    int GenerateRandomUnownedItemId(int minId, int maxId, int ban1 = -2, int ban2 = -2, int ban3 = -2, int ban4 = -2)
    {
        int id = -1;
        while (id == ban1 || id == ban2 || id == ban3 || id == ban4 || id == -1) id = Random.Range(minId, maxId);
        return id;
    }

    public void m_Page1()
    {
        for (int i = 0; i < 3; i++) if (DailyTasks[i]) Destroy(DailyTasks[i]);
        for (int i = 0; i < 3; i++)
        {
            DailyTasks[i] = Instantiate(DailyFrame);
            DailyTasks[i].GetComponent<DailyInfo>().SetInfo(DailyAchieveList.dailyTasks[DailyIds[i]]);
            DailyTasks[i].transform.SetParent(transform);
            DailyTasks[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
            DailyTasks[i].transform.position = transform.position + IncY * i;
        }
    }

    public void m_Page2()
    {
        for (int i = 0; i < 3; i++) if (DailyTasks[i]) Destroy(DailyTasks[i]);
        for (int i = 3; i < 5; i++)
        {
            DailyTasks[i-3] = Instantiate(DailyFrame);
            DailyTasks[i-3].GetComponent<DailyInfo>().SetInfo(DailyAchieveList.dailyTasks[DailyIds[i]]);
            DailyTasks[i-3].transform.SetParent(transform);
            DailyTasks[i-3].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
            DailyTasks[i-3].transform.position = transform.position + IncY * (i-3);
        }
        DailyTasks[2] = Instantiate(DailyFrame);
        DailyTasks[2].GetComponent<DailyInfo>().SetInfo(DailyAchieveList.dailyTasks[99]);
        DailyTasks[2].transform.SetParent(transform);
        DailyTasks[2].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
        DailyTasks[2].transform.position = transform.position + IncY * 2;
    }
}
