using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DoorGimmick : MonoBehaviour
{
    //Unity
    /// <summary>1-left, 2-right, 3-up, 4-down</summary>
    public int U_RightDirection = 2;
    public Vector3 U_Change = new Vector3(10f, 0);
    public Vector2 U_MilisecondRandomizerLimit = new Vector2(5000, 6000);
    //Object
    //Thread
    Thread t_Door;
    Vector3 t_Position;
    bool t_isComplete, t_Open = false;
    int t_Random = 0;


    //System Functions


    private void Awake()
    {
        t_isComplete = false;
    }

    private void Start()
    {
        t_Random = (int)Random.Range(U_MilisecondRandomizerLimit.x, U_MilisecondRandomizerLimit.y);
        t_Position = transform.position;
        t_Door = new Thread(new ThreadStart(PropelControl));
        t_Door.Start();
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
            if (t_Door.IsAlive)
                t_Door.Abort();
        }
        catch
        {
            Debug.Log("Door Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Door.IsAlive)
                t_Door.Abort();
        }
        catch
        {
            Debug.Log("Door Thread can not abort on quit");
        }
        t_isComplete = true;
    }


    //Custom Functions

    /// <summary>Run when the PropelControl Thread pass parameters</summary>
    void PropelAftermath()
    {
        transform.position = t_Position;
        if (t_Random == 0) t_Random = (int)Random.Range(U_MilisecondRandomizerLimit.x, U_MilisecondRandomizerLimit.y);
    }


    //Thread

    void PropelControl()
    {
        while (!t_isComplete)
        {
            Thread.Sleep(t_Random);
            t_Random = 0;
            int x = 1;
            if (t_Open == true) x = -1;
            for (int i = 1; i <= 100; i++)
            {
                lock (this)
                {
                    if (U_RightDirection == 2 || U_RightDirection == 4) t_Position -= U_Change/100f*x;
                    else if (U_RightDirection == 1 || U_RightDirection == 3) t_Position += U_Change/100f*x;
                }
                Thread.Sleep(10);
            }
            t_Open = !t_Open;
        }
    }
}
