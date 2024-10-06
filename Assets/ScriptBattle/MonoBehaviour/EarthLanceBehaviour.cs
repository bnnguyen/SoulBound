using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class EarthLanceBehaviour : MonoBehaviour
{
    //Unity
    public Vector3 k_HitBoxSizeOn, k_HitBoxOffsetOn, U_Change, U_Accel;
    public float u_Limit = 500f, u_RotateChange = .002f;
    //Object
    public GameObject caster;
    //Thread
    Thread t_Movement;
    Vector3 t_Position, t_Rotation;
    private List<GameObject> m_EnemiesHitList = new List<GameObject>();
    bool t_isComplete, t_Up = true;
    int m_Random;


    //System Functions


    private void Awake()
    {
        t_isComplete = false;
    }

    private void Start()
    {
        t_Position = transform.position;
        t_Movement = new Thread(new ThreadStart(MovementControl));
        t_Movement.Start();
    }

    void Update()
    {
        m_Random = Random.Range(1, 4);
        lock (this)
        {
            m_Random = Random.Range(1, 4);
            MovementAftermath();
            if (CheckGround() && u_Limit > 2 && u_Limit <= 498) {
                if (!t_Up) u_Limit = 1;
                else u_Limit = 1.5f;
                MovementAftermath2();
            } else if (!t_isComplete)
            {
                if (u_Limit <= 497) MovementAftermath2();
            }
            if (t_isComplete)
            {
                GetComponent<Fade>().U_Fading = true;
            }
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (t_Movement.IsAlive)
                t_Movement.Abort();
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
            if (t_Movement.IsAlive)
                t_Movement.Abort();
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
    void MovementAftermath()
    {
        transform.position = t_Position;
        transform.eulerAngles = t_Rotation;
        m_Random = Random.Range(1, 4);
    }

    /// <summary>Run when the ExpansionControl Thread pass parameters in FixedUpdate</summary>
    void MovementAftermath2()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"));
        foreach (Collider2D Enemy in EnemyHit)
        {
            if (Enemy.gameObject == caster) continue;
            if (m_EnemiesHitList.Contains(Enemy.gameObject)) continue;
            if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().OnHit.Invoke();
            if (!caster.GetComponent<Player>().m_OutsideSoulGain)
            {
                if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
                if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().m_OutsideSoulGain = true;
            }
            if (!Enemy.GetComponent<Player>().m_Immunity) m_EnemiesHitList.Add(Enemy.gameObject);
        }
    }

    bool CheckGround()
    {
        return Physics2D.OverlapBox(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Ground"));
    }

    //Thread

    void MovementControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (t_Up)
                {
                    if (U_Accel.y <= 0)
                    {
                        t_Rotation += new Vector3(10, 0, 0);
                        if (t_Rotation.x == 90)
                        {
                            int x = m_Random;
                            if (x == 1)
                            {
                                t_Rotation += new Vector3(0, 0, -10);
                                U_Accel += new Vector3(-u_RotateChange, 0);
                            }
                            else if (x == 2)
                            {
                                t_Rotation += new Vector3(0, 0, 10);
                                U_Accel += new Vector3(u_RotateChange, 0);
                            }
                        }
                        if (t_Rotation.x >= 180)
                        {
                            t_Up = false;
                            U_Change *= 2;
                        }
                    }
                    else
                    {
                        t_Position += U_Accel;
                        U_Accel -= U_Change;
                        u_Limit -= U_Accel.y;
                        if (u_Limit <= 0)
                        {
                            t_isComplete = true;
                        }
                    }
                } else
                {
                    t_Position -= U_Accel;
                    U_Accel += U_Change;
                    u_Limit -= U_Accel.y;
                    if (u_Limit <= 0)
                    {
                        t_isComplete = true;
                    }
                }
            }
            Thread.Sleep(1);
        }
    }
}
