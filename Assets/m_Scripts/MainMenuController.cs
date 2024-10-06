using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Camera menuCam, shopCam, dailyCam, achieveCam, characterCam;
    [SerializeField] private CanvasGroup shopGroup, menuGroup, dailyGroup, achieveGroup, characterGroup;
    [SerializeField] private Image BG, Logo;
    private static int Enter = 1;

    private void Start()
    {
        toMenu();
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        if (Enter == 1)
        {
            Enter--;
            for (int i = 1; i <= 100; i++)
            {
                Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, Logo.color.a + 0.01f);
                yield return new WaitForSeconds(0.005f);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 1; i <= 100; i++)
            {
                Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, Logo.color.a - 0.01f);
                BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, BG.color.a - 0.01f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        Destroy(BG.gameObject);
    }

    public void toBattle()
    {
        SceneManager.LoadScene("Demo");
    }

    public void toShop()
    {
        menuCam.enabled = false;
        menuCam.depth = -1;
        menuGroup.interactable = false;
        shopCam.depth = 0;
        shopCam.enabled = true;
        shopGroup.interactable = true;
    }

    public void toDaily()
    {
        menuCam.enabled = false;
        menuCam.depth = -1;
        menuGroup.interactable = false;
        dailyCam.enabled = true;
        dailyCam.depth = 0;
        dailyGroup.interactable = true;
    }

    public void toAchieve()
    {
        menuCam.enabled = false;
        menuCam.depth = -1;
        menuGroup.interactable = false;
        achieveCam.enabled = true;
        achieveCam.depth = 0;
        achieveGroup.interactable = true;
    }

    public void toMenu()
    {
        shopCam.enabled = false;
        shopGroup.interactable = false;
        shopCam.depth = -1;
        //
        achieveCam.enabled = false;
        achieveGroup.interactable = false;
        achieveCam.depth = -1;
        //
        dailyCam.enabled = false;
        dailyCam.depth = -1;
        dailyGroup.interactable = false;
        //
        characterCam.depth = -1;
        characterCam.enabled = false;
        characterGroup.interactable = false;
        //
        menuCam.enabled = true;
        menuGroup.interactable = true;
        menuCam.depth = 0;
    }

    public void quitApp()
    {
        Application.Quit();
    }
}
