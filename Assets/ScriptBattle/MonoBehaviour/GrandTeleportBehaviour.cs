using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GrandTeleportBehaviour : MonoBehaviour
{
    //Unity
    public GameObject caster, TeleportPoint;
    public Sprite sp_Activate, sp_Unactivate;
    public int U_TeleportCoolDown = 5000, U_Exist = 60000;
    //Object
    public Vector3 k_HitBoxSizeOn, k_HitBoxOffsetOn;
    //Thread
    Thread t_Teleport, t_Terminate;
    bool t_isComplete, t_Open = false;
    public bool t_OnCD = true;

    private void Start()
    {
        t_Teleport = new Thread(new ThreadStart(CooldownControl));
        t_Teleport.Start();
        t_Terminate = new Thread(new ThreadStart(TerminateControl));
        t_Terminate.Start();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn);
    }

    private void FixedUpdate()
    {
        if (t_isComplete)
        {
            Destroy(gameObject);
        }
        if (!t_Open)
        {
            GetComponent<Rotate>().U_Rotate = new Vector3(0, 0, 0.025f);
            GetComponent<SpriteRenderer>().sprite = sp_Unactivate;
            if (TeleportPoint) t_Open = true;
        }   
        else
        {
            GetComponent<Rotate>().U_Rotate = new Vector3(0, 0, 0.05f);
            if (!t_OnCD) GetComponent<SpriteRenderer>().sprite = sp_Activate;
            else GetComponent<SpriteRenderer>().sprite = sp_Unactivate;
            if (!TeleportPoint) t_Open = false;
            if (!t_OnCD)
            {
                lock (this)
                {
                    Collider2D[] Players = Physics2D.OverlapBoxAll(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"));
                    foreach (Collider2D Player in Players)
                    {
                        if (Player.gameObject == caster)
                        {
                            t_OnCD = true;
                            TeleportPoint.GetComponent<GrandTeleportBehaviour>().t_OnCD = true;
                            Player.transform.position = TeleportPoint.transform.position;
                            break;
                        }
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Teleport.IsAlive)
                t_Teleport.Abort();
        }
        catch
        {
            Debug.Log("TeleportCooldown Thread can not abort on destroy");
        }
        try
        {
            if (t_Terminate.IsAlive)
                t_Terminate.Abort();
        }
        catch
        {
            Debug.Log("Terminate Thread can not abort on destroy");
        }
        t_isComplete = true;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (t_Teleport.IsAlive)
                t_Teleport.Abort();
        }
        catch
        {
            Debug.Log("TeleportCooldown Thread can not abort on quit");
        }
        try
        {
            if (t_Terminate.IsAlive)
                t_Terminate.Abort();
        }
        catch
        {
            Debug.Log("Terminate Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    //Thread

    void CooldownControl()
    {
        while (!t_isComplete)
        {
            if (t_OnCD)
            {
                Thread.Sleep(U_TeleportCoolDown);
                t_OnCD = false;
            }
            Thread.Sleep(100);
        }
    }

    void TerminateControl()
    {
        Thread.Sleep(U_Exist);
        t_isComplete = true;
    }
}
