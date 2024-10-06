using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    //Unity
    public Vector3 U_Rotate = new Vector3(0,0,0.1f);
    //Object
    //Thread
    Thread t_Rotate;
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
        t_Rotate = new Thread(new ThreadStart(RotateControl));
        t_Rotate.Start();
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
            if (t_Rotate.IsAlive)
                t_Rotate.Abort();
        }
        catch
        {
            Debug.Log("Rotate Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Rotate.IsAlive)
                t_Rotate.Abort();
        }
        catch
        {
            Debug.Log("Rotate Thread can not abort on quit");
        }
        t_isComplete = true;
    }


    //Custom Functions

    /// <summary>Run when the PropelControl Thread pass parameters</summary>
    void ExpansionAftermath()
    {
        transform.localEulerAngles = t_Scale;
    }


    //Thread

    void RotateControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                t_Scale += U_Rotate;
            }
            Thread.Sleep(1);
        }
    }
}
