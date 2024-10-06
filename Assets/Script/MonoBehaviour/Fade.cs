using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Fade : MonoBehaviour
{
    //Unity
    public float U_TransparentDecreaseRate = 0.01f;
    //Object
    SpriteRenderer m_MySprite;
    Color m_Color;
    float m_Trans;
    //Thread
    Thread t_Fade;
    float t_Trans;
    bool t_isComplete;


    //System Functions


    private void Awake()
    {
        m_MySprite = GetComponent<SpriteRenderer>();
        m_Color = m_MySprite.color;
        m_Trans = t_Trans = m_Color.a;
        t_isComplete = false;
    }

    private void Start()
    {
        t_Fade = new Thread(new ThreadStart(ColorControl));
        t_Fade.Start();
    }

    void Update()
    {
        lock (this)
        {
            ColorAftermath();
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Fade.IsAlive)
                t_Fade.Abort();
        }
        catch
        {
            Debug.Log("Color Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Fade.IsAlive)
                t_Fade.Abort();
        }
        catch
        {
            Debug.Log("Color Thread can not abort on quit");
        }
        t_isComplete = true;
    }


    //Custom Functions

    /// <summary>Run when the ColorControl Thread pass parameters</summary>
    void ColorAftermath()
    {
        if (t_Trans != m_Trans)
        {
            m_Color = m_MySprite.color;
            m_Color = new Color(m_Color.r, m_Color.g, m_Color.b, t_Trans);
            m_MySprite.color = m_Color;
            m_Trans = t_Trans;
            if (m_Trans <= 0) Destroy(gameObject);
        }
    }


    //Thread

    void ColorControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                t_Trans -= U_TransparentDecreaseRate;
            }
            Thread.Sleep(5);
        }
    }
}
