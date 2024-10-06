using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyInfo : MonoBehaviour
{
    public Sprite DailyMission, Achievement;
    public TMP_Text m_Des, m_ProgressText, m_RewardText;
    public Slider m_ProgressBar;
    public Image m_Icon,m_HeaderIcon;
    public Daily m_Daily;

    public void SetInfo(Daily daily)
    {
        m_Daily = daily;
        if (daily.DailyTask) m_HeaderIcon.sprite = DailyMission;
        else m_HeaderIcon.sprite = Achievement;
        m_Des.text = "Description : " + daily.Des;
        m_ProgressText.text = daily.progressIndex + "/" + daily.progressMax;
        if (daily.progressIndex < daily.progressMax)
            m_RewardText.text = "x " + daily.reward;
        else m_RewardText.text = "  Done";
        m_ProgressBar.maxValue = daily.progressMax;
        m_ProgressBar.minValue = 0;
        m_ProgressBar.value = daily.progressIndex;
        m_Icon.sprite = daily.Premium;
    }

    public void FixedUpdate()
    {
        if (m_Daily == null) { }
        else
        {
            m_ProgressText.text = m_Daily.progressIndex + "/" + m_Daily.progressMax;
            m_ProgressBar.maxValue = m_Daily.progressMax;
            m_ProgressBar.minValue = 0;
            m_ProgressBar.value = m_Daily.progressIndex;
            if (m_Daily.progressMax == m_Daily.progressIndex) m_Daily.Finish = true;
            if (!m_Daily.Finish)
                m_RewardText.text = " x " + m_Daily.reward;
            else m_RewardText.text = "  Done";
        }
        lock (this)
        {
            DailyAchieveList.DailyComplete = 0;
            for (int i = 0; i < DailyAchieveList.Daily; i++) if (DailyAchieveList.dailyTasks[i].Finish) DailyAchieveList.DailyComplete++;
            DailyAchieveList.dailyTasks[99].progressIndex = DailyAchieveList.DailyComplete;
            if (DailyAchieveList.dailyTasks[99].progressMax == DailyAchieveList.dailyTasks[99].progressIndex && !DailyAchieveList.dailyTasks[99].Finish) { DailyAchieveList.dailyTasks[99].Finish = true; ItemList.SpecGems += DailyAchieveList.dailyTasks[99].reward; }
        }
    }
}
