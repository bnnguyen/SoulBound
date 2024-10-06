using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PropelBackward : MonoBehaviour
{
    //Unity
    /// <summary>1-left, 2-right, 3-up, 4-down</summary>
    public int U_RightDirection = 2;
    public Vector3 U_Change = new Vector3(0.001f, 0);
    //Object
    //Thread
    Thread t_Propel;
    Vector3 t_Position;
    bool t_isComplete;


    //System Functions


    private void Awake()
    {
        t_isComplete = false;
    }

    private void Start()
    {
        if (U_RightDirection == 3) transform.Rotate(0, 0, 90);
        else if (U_RightDirection == 4) transform.Rotate(0, 0, -90);
        t_Position = transform.position;
        t_Propel = new Thread(new ThreadStart(PropelControl));
        t_Propel.Start();
    }

    void Update()
    {
        lock (this)
        {
            PropelAftermath();
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Propel.IsAlive)
                t_Propel.Abort();
        }
        catch
        {
            Debug.Log("Propel Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Propel.IsAlive)
                t_Propel.Abort();
        }
        catch
        {
            Debug.Log("Propel Thread can not abort on quit");
        }
        t_isComplete = true;
    }


    //Custom Functions

    /// <summary>Run when the PropelControl Thread pass parameters</summary>
    void PropelAftermath()
    {
        transform.position = t_Position;
    }


    //Thread

    void PropelControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (U_RightDirection == 2 || U_RightDirection == 4) t_Position -= U_Change;
                else if (U_RightDirection == 1 || U_RightDirection == 3) t_Position += U_Change;
            }
            Thread.Sleep(1);
        }
    }
}
