using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class RevivalAlterGimmick : MonoBehaviour
{
    //Unity
    public Sprite sp_ActivateStage1, sp_ActivateStage2, sp_ActivateStage3, sp_ActivateStage4, sp_ActivateStage5, sp_Unactivate;
    public int U_HealCD = 60000, U_StandingRequirement = 10000;
    //Object
    public Vector3 k_HitBoxSizeOn, k_HitBoxOffsetOn;
    //Thread
    Thread t_Wind;
    bool t_isComplete, t_Channeling = false, t_OnCD = false;
    public float m_Timer = 0;

    private void Start()
    {
        t_Wind = new Thread(new ThreadStart(RevivalThread));
        t_Wind.Start();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn);
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!t_Channeling)
        {
            GetComponent<SpriteRenderer>().sprite = sp_Unactivate;
            if (!t_OnCD)
                if (Physics2D.OverlapBox(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player")))
                {
                    t_Channeling = true;
                    m_Timer = 0;
                }
        }
        else
        {
            if (m_Timer <= U_StandingRequirement/4) GetComponent<SpriteRenderer>().sprite = sp_ActivateStage1;
            else if (m_Timer <= U_StandingRequirement/2) GetComponent<SpriteRenderer>().sprite = sp_ActivateStage2;
            else if (m_Timer <= U_StandingRequirement*3/4) GetComponent<SpriteRenderer>().sprite = sp_ActivateStage3;
            else if (m_Timer <= U_StandingRequirement) GetComponent<SpriteRenderer>().sprite = sp_ActivateStage4;
            else GetComponent<SpriteRenderer>().sprite = sp_ActivateStage5;
            if (m_Timer >= U_StandingRequirement*1.25f) t_Channeling = false;
            if (!Physics2D.OverlapBox(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"))) t_Channeling = false;
            if (m_Timer >= U_StandingRequirement && !t_OnCD)
            {
                Collider2D[] Healeds = Physics2D.OverlapBoxAll(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"));
                foreach (Collider2D healed in Healeds)
                {
                    healed.GetComponent<Player>().StartCoroutine(healed.GetComponent<Player>().CreateSouls(0, 1, SoulName.Core));
                }
                t_OnCD = true;
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Wind.IsAlive)
                t_Wind.Abort();
        }
        catch
        {
            Debug.Log("Revival Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Wind.IsAlive)
                t_Wind.Abort();
        }
        catch
        {
            Debug.Log("Revival Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    //Thread

    void RevivalThread()
    {
        while (!t_isComplete)
        {
            if (t_OnCD)
            {
                Thread.Sleep(U_HealCD);
                t_OnCD = false;
            }
            Thread.Sleep(100);
        }
    }
}
