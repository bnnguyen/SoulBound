using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OceanBeamBehaviour : MonoBehaviour
{
    //Unity
    public Vector3 U_ChangeSize = new Vector3(0, 0.2f), U_MaxSize = new Vector3(50f, 10f);
    public Vector3 k_HitBoxSizeOn, k_HitBoxOffsetOn;
    //Object
    public GameObject caster, signal;
    //Thread
    Thread t_Expansion;
    Vector3 t_Scale;
    bool t_isComplete, t_Growing = false, t_Shrinking = false, t_Strike = false, t_Activating = false;


    //System Functions


    private void Awake()
    {
        t_isComplete = false;
    }

    private void Start()
    {
        t_Scale = transform.localScale;
        t_Expansion = new Thread(new ThreadStart(ExpansionControl));
        t_Expansion.Start();
    }

    void Update()
    {
        lock (this)
        {
            if (!t_Activating)
                if (signal)
                {
                    t_Activating = true;
                }
            if (t_Activating)
            {
                ExpansionAftermath();
                if (t_Strike)
                {
                    ExpansionAftermath2();
                    t_Strike = false;
                    t_Shrinking = true;
                    t_Growing = false;
                }
                else if (!signal && !t_Shrinking && !t_Strike) t_Growing = true;
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Expansion.IsAlive)
                t_Expansion.Abort();
        }
        catch
        {
            Debug.Log("Expansion Ocean Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Expansion.IsAlive)
                t_Expansion.Abort();
        }
        catch
        {
            Debug.Log("Expansion Ocean Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn);
    }

    //Custom Functions

    /// <summary>Run when the ExpansionControl Thread pass parameters</summary>
    void ExpansionAftermath()
    {
        transform.localScale = t_Scale;
        if (transform.localScale.y < 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Run when the ExpansionControl Thread pass parameters in FixedUpdate</summary>
    void ExpansionAftermath2()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"));
        foreach (Collider2D Enemy in EnemyHit)
        {
            if (Enemy.gameObject == caster) continue;
            if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().OnHit.Invoke();
            if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
            //if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().StartCoroutine(Enemy.GetComponent<Player>().WhenHitAfterTime(0.25f));
            //if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
        }
    }


    //Thread

    void ExpansionControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (t_Activating)
                {
                    if (t_Growing)
                    {
                        if (t_Scale.y < U_MaxSize.y)
                            t_Scale += new Vector3(U_ChangeSize.x, U_ChangeSize.y);
                        else
                        {
                            t_Strike = true;
                            t_Growing = false;
                        }
                    }
                    if (t_Shrinking)
                    {
                        t_Scale -= new Vector3(U_ChangeSize.x, U_ChangeSize.y) / 10;
                    }
                }
            }
            Thread.Sleep(1);
        }
    }
}
