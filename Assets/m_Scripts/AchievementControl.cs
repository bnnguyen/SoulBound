using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementControl : MonoBehaviour
{
    public Button btn_Up, btn_Down;
    public GameObject DailyFrame;
    public TMP_Text PageText;
    private GameObject[] AchievementTasks = new GameObject[3];
    private int Page = 1, MaxPage;
    private Vector3 IncY = new(0, -1.75f);

    private void Start()
    {
        MaxPage = (DailyAchieveList.Achievement/3);
        btn_Up.onClick.Invoke();
    }

    public void m_PageUp()
    {
        Page--;
        if (Page <= 0) Page = 1;
        for (int i = 0; i < 3; i++) if (AchievementTasks[i]) Destroy(AchievementTasks[i]);
        for (int i = 0; i < 3; i++)
        {
            if (DailyAchieveList.achieveTasks[i + (Page - 1) * 3].Des == "None") { }
            else
            {
                AchievementTasks[i] = Instantiate(DailyFrame);
                AchievementTasks[i].GetComponent<DailyInfo>().SetInfo(DailyAchieveList.achieveTasks[i + (Page - 1) * 3]);
                AchievementTasks[i].transform.SetParent(transform);
                AchievementTasks[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
                AchievementTasks[i].transform.position = transform.position + IncY * i;
            }
        }
        PageText.text = Page + "/" + MaxPage;
    }

    public void m_PageDown()
    {
        Page++;
        if (Page > MaxPage) Page = MaxPage;
        for (int i = 0; i < 3; i++) if (AchievementTasks[i]) Destroy(AchievementTasks[i]);
        for (int i = 0; i < 3; i++)
        {
            if (DailyAchieveList.achieveTasks[i + (Page - 1) * 3].Des == "None") { }
            else
            {
                AchievementTasks[i] = Instantiate(DailyFrame);
                AchievementTasks[i].GetComponent<DailyInfo>().SetInfo(DailyAchieveList.achieveTasks[i + (Page - 1) * 3]);
                AchievementTasks[i].transform.SetParent(transform);
                AchievementTasks[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1);
                AchievementTasks[i].transform.position = transform.position + IncY * i;
            }
        }
        PageText.text = Page + "/" + MaxPage;
    }
}
