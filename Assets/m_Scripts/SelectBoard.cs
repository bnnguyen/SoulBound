using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectBoard : MonoBehaviour
{
    public TMP_Text m_NameSelectBoard;
    public CharacterControl controller;
    public Button ActionUp, ActionDown, ActionLeft, ActionRight;
    public GameObject notice;
    private Item item;

    public void SetInfo(Item item, CharacterControl control)
    {
        this.item = item;
        controller = control;
    }

    public void m_Cancel()
    {
        Destroy(gameObject);
    }

    public void m_ActionButtonLeft()
    {
        controller.btn_Left.item = item;
    }

    public void m_ActionButtonRight()
    {
        controller.btn_Right.item = item;
    }

    public void m_ActionButtonUp()
    {
        controller.btn_Up.item = item;
    }

    public void m_ActionButtonDown()
    {
        controller.btn_Down.item = item;
    }
}
