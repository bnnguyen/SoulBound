using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AirSlashMovement : MonoBehaviour
{
    //Unity
    public Vector3 U_Change = new Vector3(0.01f, 0);
    public GameObject pf_Ghost, caster;
    public float u_limit = 10f;
    //Object
    int m_Count = 0;
    float m_deltaTime = 0;
    private List<GameObject> m_EnemiesHitList = new List<GameObject>();
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
    }

    private void FixedUpdate()
    {
        lock (this)
        {
            MovementAftermath2();
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
        Gizmos.DrawWireCube(transform.position, new Vector2(4.5f, 6));
    }

    //Custom Functions

    /// <summary>Run when the MovementControl Thread pass parameters</summary>
    void MovementAftermath()
    {
        if (transform.position != t_Position)
        {
            m_Count++;
            transform.position = t_Position;
            if (m_Count % 5 == 0)
            {
                GameObject obj = Instantiate(pf_Ghost);
                obj.transform.position = transform.position;
                obj.transform.localScale = transform.localScale;
                obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                obj.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
                obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
            if (0 > u_limit) Destroy(gameObject);
        }
    }

    /// <summary>Run when the MovementControl Thread pass parameters in FixedUpdate</summary>
    void MovementAftermath2()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position, new Vector2(4.5f,6), 0, LayerMask.GetMask("Player"));
        foreach (Collider2D Enemy in EnemyHit)
        {
            if (Enemy.gameObject == caster) continue;
            if (m_EnemiesHitList.Contains(Enemy.gameObject)) continue;
            if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().OnHit.Invoke();
            if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
            if (!Enemy.GetComponent<Player>().m_Immunity) m_EnemiesHitList.Add(Enemy.gameObject);
        }
    }

    /// <summary>Run before the MovementControl Thread can run</summary>
    void SetMovement()
    {
        m_deltaTime = Time.deltaTime;
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
                    t_Position += U_Change * m_deltaTime * 1000;
                    u_limit -= Mathf.Abs(U_Change.x) * m_deltaTime * 1000;
                    m_deltaTime = 0;
                }
            }
            Thread.Sleep(1);
        }
    }
}
