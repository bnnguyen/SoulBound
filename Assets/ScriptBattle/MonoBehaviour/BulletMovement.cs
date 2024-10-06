using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    //Unity
    public Vector3 U_Change = new Vector3(0.01f, 0);
    public GameObject caster;
    public Sprite sp_trail;
    public float u_limit = 10f;
    //Object
    float m_deltaTime = 0;
    public float m_Direction = 0f;
    public bool m_Special = false;
    //Thread
    Thread t_Movement;
    Vector3 t_Position;
    bool t_isComplete;


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
        lock (this)
        {
            SetMovement();
        }
        lock (this)
        {
            MovementAftermath();
        }
        lock (this)
        {
            MovementAftermath2();
        }
    }

    private void FixedUpdate()
    {
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
            Debug.Log("AirSlashMovement Thread can not abort on destroy");
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
            Debug.Log("AirSlashMovement Thread can not abort on quit");
        }
        t_isComplete = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(0.5f, 0.2f));
    }

    //Custom Functions

    /// <summary>Run when the MovementControl Thread pass parameters</summary>
    void MovementAftermath()
    {
        if (transform.position != t_Position)
        {
            transform.position = t_Position;
            if (0 > u_limit) Destroy(gameObject);
        }
    }

    /// <summary>Run when the MovementControl Thread pass parameters in FixedUpdate</summary>
    void MovementAftermath2()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position, new Vector2(0.5f, 0.3f), 0, LayerMask.GetMask("Player") + LayerMask.GetMask("Ground") + LayerMask.GetMask("Door"));
        foreach (Collider2D Enemy in EnemyHit)
        {
            if (Enemy.gameObject == caster) continue;
            if (Enemy.gameObject.layer == 7)
            {
                if (Enemy.GetComponent<Player>().m_Immunity) return;
                Enemy.GetComponent<Player>().OnHit.Invoke();
                caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
            }
            Destroy(gameObject);
        }
    }

    /// <summary>Run before the MovementControl Thread can run</summary>
    void SetMovement()
    {
        m_deltaTime = Time.deltaTime;
    }

    /// <summary>Change the object's position by Step scaling from its rotation in 2d</summary>
    public void Move(float Step)
    {
        t_Position += new Vector3(Mathf.Cos(m_Direction * Mathf.Deg2Rad), Mathf.Sin(m_Direction * Mathf.Deg2Rad)) * Step * m_deltaTime;
        u_limit -= Mathf.Sqrt(Mathf.Cos(m_Direction * Mathf.Deg2Rad)* Mathf.Cos(m_Direction * Mathf.Deg2Rad) + Mathf.Sin(m_Direction * Mathf.Deg2Rad)* Mathf.Sin(m_Direction * Mathf.Deg2Rad)) * Step * m_deltaTime;
    }

    /// <summary>Set the object 2d direction to Value</summary>
    public void SetDirection(float Value)
    {
        transform.SetPositionAndRotation(transform.position, new Quaternion(0, 0, 0, 0));
        transform.Rotate(0, 0, Value);
        m_Direction = Value;
    }

    /// <summary>Point the object toward the targetted object</summary>
    public void PointTo(GameObject Target)
    {
        float x = Target.transform.position.x - transform.position.x, y = Target.transform.position.y - transform.position.y;
        float z = Mathf.Sqrt(x * x + y * y);
        if (x < 0)
            if (y < 0)
                SetDirection(-180 - Mathf.Asin(y / z) * Mathf.Rad2Deg);
            else
                SetDirection(180 - Mathf.Asin(y / z) * Mathf.Rad2Deg);
        else
            SetDirection(Mathf.Asin(y / z) * Mathf.Rad2Deg);
    }

    /// <summary>Point the object toward a position</summary>
    public void PointTo(Vector3 Pos)
    {
        float x = Pos.x - transform.position.x, y = Pos.y - transform.position.y;
        float z = Mathf.Sqrt(x * x + y * y);
        if (x < 0)
            if (y < 0)
                SetDirection(-180 - Mathf.Asin(y / z) * Mathf.Rad2Deg);
            else
                SetDirection(180 - Mathf.Asin(y / z) * Mathf.Rad2Deg);
        else
            SetDirection(Mathf.Asin(y / z) * Mathf.Rad2Deg);
    }

    //Thread

    void MovementControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (m_deltaTime != 0)
                {
                    if (m_Special)
                    {
                        Move(100);
                    } else
                    { 
                        t_Position += U_Change * m_deltaTime * 1000;
                        u_limit -= Mathf.Abs(U_Change.x) * m_deltaTime * 1000;
                    }
                    m_deltaTime = 0;
                }
            }
            Thread.Sleep(2);
        }
    }
}
