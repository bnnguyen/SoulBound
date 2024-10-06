using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ConfirmBoard : MonoBehaviour
{
    public TMP_Text m_NameConfirmBoard, m_Des;
    public UnityEvent Confirm;

    public void SetInfo(string Name, string Des)
    {
        m_NameConfirmBoard.text = Name;
        m_Des.text = Des;
    }

    public void m_Cancel()
    {
        Destroy(gameObject);
    }

    public void m_Confirm()
    {
        Confirm.Invoke();
        Destroy(gameObject);
    }
}
