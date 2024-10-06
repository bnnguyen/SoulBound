using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Flash : MonoBehaviour
{
    //Unity
    public bool U_RandomDirection = false;
    public Vector3 U_SizeChange, U_MaxSize;
    //Object
    bool m_Expanding = true;
    //Thread
    Thread t_Expansion;
    Vector3 t_Scale;
    bool t_isComplete;


    //System Functions


    private void Awake()
    {
        t_isComplete = false;
    }

    private void Start()
    {
        if (U_RandomDirection) transform.Rotate(0, 0, Random.Range(0f, 180f));
        t_Scale = transform.localScale;
        t_Expansion = new Thread(new ThreadStart(ExpansionControl));
        t_Expansion.Start();
    }

    void Update()
    {
        lock (this)
        {
            ExpansionAftermath();
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
            Debug.Log("Expansion Thread can not abort on destroy");
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
            Debug.Log("Expansion Thread can not abort on quit");
        }
        t_isComplete = true;
    }


    //Custom Functions

    /// <summary>Run when the PropelControl Thread pass parameters</summary>
    void ExpansionAftermath()
    {
        transform.localScale = t_Scale;
        if (t_Scale.x > 0 || t_Scale.y > 0) { }
        else Destroy(gameObject);
    }


    //Thread

    void ExpansionControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (m_Expanding)
                {
                    if (t_Scale.x < U_MaxSize.x || t_Scale.y < U_MaxSize.y) t_Scale += U_SizeChange;
                    else if (t_Scale.x < U_MaxSize.x) t_Scale += new Vector3(U_SizeChange.x, 0);
                    else if (t_Scale.y < U_MaxSize.y) t_Scale += new Vector3(0, U_SizeChange.y);
                    else m_Expanding = false;
                } else
                {
                    if (t_Scale.x > 0 || t_Scale.y > 0) t_Scale -= U_SizeChange;
                    else if (t_Scale.x > 0) t_Scale -= new Vector3(U_SizeChange.x, 0);
                    else if (t_Scale.y > 0) t_Scale -= new Vector3(0, U_SizeChange.y);
                }
            }
            Thread.Sleep(1);
        }
    }
}
