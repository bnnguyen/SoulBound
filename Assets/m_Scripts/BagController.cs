using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagController : MonoBehaviour
{
    public Sprite m_Skill, m_Character, m_Special;
    public Button btn_Skill, btn_Char, btn_Special;
    public TMP_Text PageShow;
    public GameObject ItemFrame;
    private GameObject[] Items = new GameObject[5];
    private Vector3 IncX = new(1.5f, 0);
    private const string k_Skill = "Skill", k_Char = "Character", k_Spec = "Special";
    private string Page = k_Skill;
    private int MaxPage, CurrPage;

    private void Start()
    {
        btn_Skill.onClick.Invoke();
    }

    public void m_SkillsPressed()
    {
        Page = k_Skill;
        GetComponent<Image>().sprite = m_Skill;
        for (int i = 0; i < 5; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage = 1;
        for (int i = 0; i < ItemList.Skills; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage-1)*5 && cnt < CurrPage*5)
            {
                Items[cnt % 5] = Instantiate(ItemFrame);
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 5].GetComponent<ItemInfo>().m_ShowInfoSell);
                Items[cnt % 5].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 5].transform.SetParent(transform);
                Items[cnt % 5].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Items[cnt % 5].transform.position = transform.position + IncX*(cnt%5);
                Items[cnt % 5].GetComponent<Image>().sprite = ItemList.items[i].frame;
                Items[cnt % 5].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[i].icon;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 5 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_CharPressed()
    {
        Page = k_Char;
        GetComponent<Image>().sprite = m_Character;
        for (int i = 0; i < 5; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage = 1;
        for (int i = 250; i < 250+ItemList.Chars; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 5 && cnt < CurrPage * 5)
            {
                Items[cnt % 5] = Instantiate(ItemFrame);
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 5].GetComponent<ItemInfo>().m_ShowInfoSell);
                Items[cnt % 5].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 5].transform.SetParent(transform);
                Items[cnt % 5].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Items[cnt % 5].transform.position = transform.position + IncX * (cnt % 5);
                Items[cnt % 5].GetComponent<Image>().sprite = ItemList.items[i].frame;
                Items[cnt % 5].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[i].icon;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 5 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_SpecialPressed()
    {
        Page = k_Spec;
        GetComponent<Image>().sprite = m_Special;
        for (int i = 0; i < 5; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage = 1;
        for (int i = 500; i < 500+ItemList.Specs; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 5 && cnt < CurrPage * 5)
            {
                Items[cnt % 5] = Instantiate(ItemFrame);
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 5].GetComponent<ItemInfo>().m_ShowInfoSell);
                Items[cnt % 5].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 5].transform.SetParent(transform);
                Items[cnt % 5].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Items[cnt % 5].transform.position = transform.position + IncX * (cnt % 5);
                Items[cnt % 5].GetComponent<Image>().sprite = ItemList.items[i].frame;
                Items[cnt % 5].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[i].icon;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 5 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_RightArrowPressed()
    {
        if (CurrPage >= MaxPage) return;
        int x = 0;
        if (Page == k_Char) x = 250;
        if (Page == k_Spec) x = 500;
        for (int i = 0; i < 5; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage++;
        for (int i = x; i < ItemList.Skills+x; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 5 && cnt < CurrPage * 5)
            {
                Items[cnt % 5] = Instantiate(ItemFrame);
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 5].GetComponent<ItemInfo>().m_ShowInfoSell);
                Items[cnt % 5].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 5].transform.SetParent(transform);
                Items[cnt % 5].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Items[cnt % 5].transform.position = transform.position + IncX * (cnt % 5);
                Items[cnt % 5].GetComponent<Image>().sprite = ItemList.items[i].frame;
                Items[cnt % 5].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[i].icon;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 5 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }

    public void m_LeftArrowPressed()
    {
        if (CurrPage <= 1) return;
        int x = 0;
        if (Page == k_Char) x = 250;
        if (Page == k_Spec) x = 500;
        for (int i = 0; i < 5; i++) if (Items[i]) Destroy(Items[i]);
        int cnt = 0;
        CurrPage--;
        for (int i = x; i < ItemList.Skills + x; i++)
        {
            if (ItemList.items[i].Own && cnt >= (CurrPage - 1) * 5 && cnt < CurrPage * 5)
            {
                Items[cnt % 5] = Instantiate(ItemFrame);
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[cnt % 5].GetComponentInChildren<Button>().onClick.AddListener(Items[cnt % 5].GetComponent<ItemInfo>().m_ShowInfoSell);
                Items[cnt % 5].GetComponent<ItemInfo>().item = ItemList.items[i];
                Items[cnt % 5].transform.SetParent(transform);
                Items[cnt % 5].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                Items[cnt % 5].transform.position = transform.position + IncX * (cnt % 5);
                Items[cnt % 5].GetComponent<Image>().sprite = ItemList.items[i].frame;
                Items[cnt % 5].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[i].icon;
            }
            if (ItemList.items[i].Own)
            {
                cnt++;
            }
        }
        MaxPage = cnt / 5 + 1;
        PageShow.text = CurrPage + "/" + MaxPage;
    }
}
