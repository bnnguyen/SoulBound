using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class WindGimmick : MonoBehaviour
{
    //Unity
    public Sprite sp_Activate, sp_Unactivate;
    public float U_WindForce = 50f;
    public int U_WindCD = 5000, U_WindExist = 10000;
    //Object
    public Vector3 k_HitBoxSizeOff, k_HitBoxSizeOn, k_HitBoxOffsetOff, k_HitBoxOffsetOn; 
    //Thread
    Thread t_Wind;
    bool t_isComplete, t_Open = false, t_OnCD = false;

    private void Start()
    {
        t_Wind = new Thread(new ThreadStart(PropelControl));
        t_Wind.Start();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(k_HitBoxOffsetOff + transform.position, k_HitBoxSizeOff);
        //Gizmos.DrawWireCube(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn);
    }

    private void FixedUpdate()
    {
        if (!t_Open)
        {
            GetComponent<SpriteRenderer>().sprite = sp_Unactivate;
            if (!t_OnCD)
            if (Physics2D.OverlapBox(k_HitBoxOffsetOff + transform.position, k_HitBoxSizeOff, 0, LayerMask.GetMask("Player"))) 
            {
                t_Open = true;
                GetComponent<SpriteRenderer>().sprite = sp_Activate;
            }
        } else
        {
            Collider2D[] Players = Physics2D.OverlapBoxAll(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"));
            foreach (Collider2D Player in Players)
            {
                Player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,U_WindForce));
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
            Debug.Log("Wind Thread can not abort on destroy");
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
            Debug.Log("Wind Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    //Thread

    void PropelControl()
    {
        while (!t_isComplete)
        {
            if (t_Open)
            {
                Thread.Sleep(U_WindExist);
                t_Open = false;
                t_OnCD = true;
                Thread.Sleep(U_WindCD);
                t_OnCD = false;
            }
            Thread.Sleep(100);
        }
    }
}
