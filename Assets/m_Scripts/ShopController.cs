using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public Sprite m_Skill, m_Character, m_Special;
    public Button btn_Skill, btn_Char, btn_Special;
    public GameObject ItemFrame;
    private GameObject[] Items = new GameObject[3];
    private int[] SkillIds, CharIds, SpecialIds;
    public TMP_Text Count;
    private Vector3 IncX = new(2.5f, 0);

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        SkillIds = new int[3] { GenerateRandomUnownedItemId(0, 250), GenerateRandomUnownedItemId(0, 250), GenerateRandomUnownedItemId(0, 250) };
        while (SkillIds[1] == SkillIds[0]) SkillIds[1] = GenerateRandomUnownedItemId(0, 250, SkillIds[0]);
        while (SkillIds[2] == SkillIds[0] || SkillIds[2] == SkillIds[1]) 
            SkillIds[2] = GenerateRandomUnownedItemId(0, 250, SkillIds[0], SkillIds[1]);
        //
        CharIds = new int[3] { GenerateRandomUnownedItemId(250, 500), GenerateRandomUnownedItemId(250, 500), GenerateRandomUnownedItemId(250, 500) };
        while (CharIds[1] == CharIds[0]) CharIds[1] = GenerateRandomUnownedItemId(250, 500, CharIds[0]);
        while (CharIds[2] == CharIds[0] || CharIds[2] == CharIds[1])
            CharIds[2] = GenerateRandomUnownedItemId(250, 500, CharIds[0], CharIds[1]);
        //
        SpecialIds = new int[3] { GenerateRandomUnownedItemId(500, 750), GenerateRandomUnownedItemId(500, 750), GenerateRandomUnownedItemId(500, 750) };
        while (SpecialIds[1] == SpecialIds[0]) SpecialIds[1] = GenerateRandomUnownedItemId(500, 750, SpecialIds[0]);
        while (SpecialIds[2] == SpecialIds[0] || SpecialIds[2] == SpecialIds[1])
            SpecialIds[2] = GenerateRandomUnownedItemId(500, 750, SpecialIds[0], SpecialIds[1]);
        //
        ItemList.RefreshCount--;
        Count.text = ItemList.RefreshCount + "/3";
        btn_Skill.onClick.Invoke();
    }

    int GenerateRandomUnownedItemId(int minId, int maxId, int ban1 = -2, int ban2 = -2)
    {
        int id = -1, i = Random.Range(250, 500), cnt = 0, cnt2 = 10000;
        while (i > 0)
        {
            if (!ItemList.items[cnt].Own && ItemList.items[cnt].id == cnt && cnt != ban1 && cnt != ban2)
            {
                id = cnt;
                i--;
            }
            cnt++;
            if (cnt >= maxId) cnt = minId;
            cnt2--;
            if (cnt2 <= 0) break;
        }
        return id;
    }

    public void m_SkillsPressed()
    {
        GetComponent<Image>().sprite = m_Skill;
        for (int i = 0; i < 3; i++) if (Items[i]) Destroy(Items[i]);
        for (int i = 0; i < 3; i++)
        {
            if (!ItemList.items[SkillIds[i]].Own)
            {
                Items[i] = Instantiate(ItemFrame);
                Items[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[i].GetComponentInChildren<Button>().onClick.AddListener(Items[i].GetComponent<ItemInfo>().m_ShowInfoBuy);
                Items[i].GetComponent<ItemInfo>().item = ItemList.items[SkillIds[i]];
                Items[i].transform.SetParent(transform);
                Items[i].GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 2);
                Items[i].transform.position = transform.position + IncX * (i % 5);
                Items[i].GetComponent<Image>().sprite = ItemList.items[SkillIds[i]].frame;
                Items[i].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[SkillIds[i]].icon;
            }
        }
    }

    public void m_CharPressed()
    {
        GetComponent<Image>().sprite = m_Character;
        for (int i = 0; i < 3; i++) if (Items[i]) Destroy(Items[i]);
        for (int i = 0; i < 3; i++)
        {
            if (!ItemList.items[CharIds[i]].Own)
            {
                Items[i] = Instantiate(ItemFrame);
                Items[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[i].GetComponentInChildren<Button>().onClick.AddListener(Items[i].GetComponent<ItemInfo>().m_ShowInfoBuy);
                Items[i].GetComponent<ItemInfo>().item = ItemList.items[CharIds[i]];
                Items[i].transform.SetParent(transform);
                Items[i].GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 2);
                Items[i].transform.position = transform.position + IncX * (i % 5);
                Items[i].GetComponent<Image>().sprite = ItemList.items[CharIds[i]].frame;
                Items[i].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[CharIds[i]].icon;
            }
        }
    }

    public void m_SpecialPressed()
    {
        GetComponent<Image>().sprite = m_Special;
        for (int i = 0; i < 3; i++) if (Items[i]) Destroy(Items[i]);
        for (int i = 0; i < 3; i++)
        {
            if (!ItemList.items[SpecialIds[i]].Own)
            {
                Items[i] = Instantiate(ItemFrame);
                Items[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                Items[i].GetComponentInChildren<Button>().onClick.AddListener(Items[i].GetComponent<ItemInfo>().m_ShowInfoBuy);
                Items[i].GetComponent<ItemInfo>().item = ItemList.items[SpecialIds[i]];
                Items[i].transform.SetParent(transform);
                Items[i].GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 2);
                Items[i].transform.position = transform.position + IncX * (i % 5);
                Items[i].GetComponent<Image>().sprite = ItemList.items[SpecialIds[i]].frame;
                Items[i].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.items[SpecialIds[i]].icon;
            }
        }
    }
}
