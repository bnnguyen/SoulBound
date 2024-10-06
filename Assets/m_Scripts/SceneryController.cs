using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneryController : MonoBehaviour
{
    public Image ui_ShopShadow;
    public MainMenuController m_MainMenuController;

    private void Start()
    {
        MouseOut();
    }

    public void MouseHover()
    {
        ui_ShopShadow.color = new Color(ui_ShopShadow.color.r, ui_ShopShadow.color.g, ui_ShopShadow.color.b, 0.4f);
    }

    public void MouseOut()
    {
        ui_ShopShadow.color = new Color(ui_ShopShadow.color.r, ui_ShopShadow.color.g, ui_ShopShadow.color.b, 0);
    }

    public void toShop()
    {
        m_MainMenuController.toShop();
    }
}
