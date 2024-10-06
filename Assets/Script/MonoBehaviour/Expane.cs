using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Expane : MonoBehaviour
{
    //Unity
    public Vector3 U_ChangeSize = new Vector3(0.001f, 0.001f), U_MaxSize = new Vector3(0.5f,0.5f);
    //Object
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
    }


    //Thread

    void ExpansionControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (t_Scale.x < U_MaxSize.x)
                    if (t_Scale.x > 0)
                        t_Scale += new Vector3(U_ChangeSize.x,U_ChangeSize.y);
                    else t_Scale -= new Vector3(U_ChangeSize.x, -U_ChangeSize.y);
            }
            Thread.Sleep(1);
        }
    }
}
