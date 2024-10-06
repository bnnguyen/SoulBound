using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Soul : MonoBehaviour
{
    //Unity
    public string u_SoulType = SoulName.Null;
    [Range(0,60)] public float u_LifeSpan = -1;
    public Player u_Player;
    public bool u_Show = true;

    //Custom Parameter
    SpriteRenderer m_MySprite;
    Color m_Color;
    float m_Trans;
    TimeSpan m_TimeRemaining;
    DateTime m_Time;
    Animator m_Anim;

    //Constants
    float c_TransChange = 0.01f;

    //Thread
    DateTime t_Now;
    Thread t_Soul;
    float t_Trans;
    bool t_isComplete, t_Show;

    //Event
    public UnityEvent OnDestruction;

    void Start()
    {
        m_MySprite = GetComponent<SpriteRenderer>();
        m_Color = m_MySprite.color;
        m_Trans = t_Trans = m_Color.a;
        t_Show = u_Show;
        m_Time = DateTime.Now;

        //Event
        OnDestruction = new UnityEvent();
        m_Anim = GetComponent<Animator>();
        OnDestruction.AddListener(Destruction);

        //Thread
        t_Soul = new Thread(new ThreadStart(SoulControl));
        t_Soul.Start();
    }


    void Update()
    {
        if (m_TimeRemaining.TotalSeconds > u_LifeSpan && u_LifeSpan != -1) OnDestruction.Invoke();
        lock (this)
        {
            SoulAftermath();
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Soul.IsAlive)
                t_Soul.Abort();
        }
        catch
        {
            Debug.Log("Soul Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Soul.IsAlive)
                t_Soul.Abort();
        }
        catch
        {
            Debug.Log("Soul Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    //Custom Function

    //Thread

    void SoulControl()
    {
        while (!t_isComplete)
        {
            t_Now = DateTime.Now;
            if (t_Show != u_Show)
            {
                if (u_Show)
                {
                    while (t_Trans <= 0.5f)
                    {
                        lock (this) {
                            t_Trans += c_TransChange;
                        }
                        Thread.Sleep(1);
                    }
                    t_Show = u_Show;
                } else
                {
                    while (t_Trans >= 0f)
                    {
                        lock (this)
                        {
                            t_Trans -= c_TransChange;
                        }
                        Thread.Sleep(1);
                    }
                    t_Show = u_Show;
                }
            }
            Thread.Sleep(1);
        }
    }

    void SoulAftermath()
    {
        m_TimeRemaining = m_Time - t_Now;
        if (t_Trans != m_Trans)
        {
            m_Color = m_MySprite.color;
            m_Color = new Color(m_Color.r, m_Color.g, m_Color.b, t_Trans);
            m_MySprite.color = m_Color;
            m_Trans = t_Trans;
        }
    }

    //Event

    void Destruction()
    {
        StartCoroutine(DestroySoul());
    }

    private IEnumerator DestroySoul()
    {
        u_Player.ShowSouls();
        m_Anim.Play("Explode");
        yield return new WaitForSeconds(0.2f);
        u_Player.OnCleanUpSoul.Invoke();
        Destroy(gameObject);
    }
}

public class SoulName
{
    public static string Core = "Core Soul", Fight = "Fight Soul", Sustain = "Sustain Soul", Burst = "Burst Soul", Null = "";
}
