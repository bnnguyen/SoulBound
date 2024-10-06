using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour
{
    public int Character = 1;
    private bool PlayerMode = true;
    [Space]
    public Sprite m_Skill, m_Character, m_Special;
    [Space]
    public Button btn_Skill, btn_Char, btn_Special;
    [Space]
    public ItemInfo btn_Left, btn_Right, btn_Up, btn_Down;
    public TMP_Text PageShow, Player;
    public GameObject ItemFrame;
    private GameObject[] Items = new GameObject[6];
    private Vector3 IncX = new(1.25f, 0), IncY = new(0,-.75f);
    private const string k_Skill = "Skill", k_Char = "Character", k_Spec = "Special";
    private string Page = k_Skill;
    private int MaxPage, CurrPage;

    private void Start()
    {
        btn_Skill.onClick.Invoke();
    }

    private void FixedUpdate()
    {
        if (Player)
        if (PlayerMode) Player.text = "Player";
        else Player.text = "Bot";
    }

    public void m_PlayerClick()
    {
        PlayerMode = true;
    }

    public void m_BotClick()
    {
        PlayerMode = false;
    }

    public void m_SkillsPressed()
    {
        Page = k_Skill;
        GetComponent<Image>().sprite = m_Skill;
        for (int i = 0; i < 6; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage = 1;
        for (int i = 0; i < ItemList.Skills; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 6 && cnt < CurrPage * 6)
            {
                Items[cnt % 6] = Instantiate(ItemFrame);
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 6].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 6].GetComponent<ItemInfo>().character = this;
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 6].GetComponent<ItemInfo>().m_ShowSelection);
                Items[cnt % 6].transform.SetParent(transform);
                Items[cnt % 6].GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
                if (cnt%6 <= 2) 
                    Items[cnt % 6].transform.position = transform.position + IncX * (cnt % 6);
                else 
                    Items[cnt % 6].transform.position = transform.position + IncX * ((cnt % 6) - 3) + IncY;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 6 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_CharPressed()
    {
        Page = k_Char;
        GetComponent<Image>().sprite = m_Character;
        for (int i = 0; i < 6; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage = 1;
        for (int i = 250; i < 250 + ItemList.Chars; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 6 && cnt < CurrPage * 6)
            {
                Items[cnt % 6] = Instantiate(ItemFrame);
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 6].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 6].GetComponent<ItemInfo>().character = this;
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 6].GetComponent<ItemInfo>().m_Equip);
                Items[cnt % 6].transform.SetParent(transform);
                Items[cnt % 6].GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
                if (cnt % 6 <= 2)
                    Items[cnt % 6].transform.position = transform.position + IncX * (cnt % 6);
                else
                    Items[cnt % 6].transform.position = transform.position + IncX * ((cnt % 6) - 3) + IncY;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 6 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_SpecialPressed()
    {
        Page = k_Spec;
        GetComponent<Image>().sprite = m_Special;
        for (int i = 0; i < 6; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage = 1;
        for (int i = 500; i < 500 + ItemList.Specs; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 6 && cnt < CurrPage * 6)
            {
                Items[cnt % 6] = Instantiate(ItemFrame);
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 6].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 6].GetComponent<ItemInfo>().character = this;
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 6].GetComponent<ItemInfo>().m_ShowSelection);
                Items[cnt % 6].transform.SetParent(transform);
                Items[cnt % 6].GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
                if (cnt % 6 <= 2)
                    Items[cnt % 6].transform.position = transform.position + IncX * (cnt % 6);
                else
                    Items[cnt % 6].transform.position = transform.position + IncX * ((cnt % 6) - 3) + IncY;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 6 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_RightArrowPressed()
    {
        if (CurrPage >= MaxPage) return;
        int x = 0;
        if (Page == k_Char) x = 250;
        if (Page == k_Spec) x = 500;
        for (int i = 0; i < 6; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage++;
        for (int i = x; i < ItemList.Skills + x; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 6 && cnt < CurrPage * 6)
            {
                Items[cnt % 6] = Instantiate(ItemFrame);
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 6].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 6].GetComponent<ItemInfo>().character = this;
                if (Page == k_Skill)
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 6].GetComponent<ItemInfo>().m_ShowSelection);
                if (Page == k_Char)
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 6].GetComponent<ItemInfo>().m_Equip);
                Items[cnt % 6].transform.SetParent(transform);
                Items[cnt % 6].GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
                if (cnt % 6 <= 2)
                    Items[cnt % 6].transform.position = transform.position + IncX * (cnt % 6);
                else
                    Items[cnt % 6].transform.position = transform.position + IncX * ((cnt % 6) - 3) + IncY;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 6 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_LeftArrowPressed()
    {
        if (CurrPage <= 1) return;
        int x = 0;
        if (Page == k_Char) x = 250;
        if (Page == k_Spec) x = 500;
        for (int i = 0; i < 6; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage--;
        for (int i = x; i < ItemList.Skills + x; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 6 && cnt < CurrPage * 6)
            {
                Items[cnt % 6] = Instantiate(ItemFrame);
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 6].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 6].GetComponent<ItemInfo>().character = this;
                Items[cnt % 6].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 6].GetComponent<ItemInfo>().m_ShowSelection);
                Items[cnt % 6].transform.SetParent(transform);
                Items[cnt % 6].GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
                if (cnt % 6 <= 2)
                    Items[cnt % 6].transform.position = transform.position + IncX * (cnt % 6);
                else
                    Items[cnt % 6].transform.position = transform.position + IncX * ((cnt % 6) - 3) + IncY;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 6 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }
}
