using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoticeBoard : MonoBehaviour
{
    public TMP_Text m_NameNoticeBoard, m_Des;

    public void SetInfo(string Name, string Des)
    {
        m_NameNoticeBoard.text = Name;
        m_Des.text = Des;
    }

    public void m_Cancel()
    {
        Destroy(gameObject);
    }
}
