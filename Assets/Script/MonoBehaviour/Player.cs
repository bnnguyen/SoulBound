using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    //Const
    private const int MaxAbilities = 4;

    //Prefabs
    [SerializeField] private GameObject pf_WhiteDash, pf_GreenDash, pf_DashBG, pf_DashLine, pf_Slash, pf_WhirlWind, pf_SkyCurrentDash, pf_SkyCurrentLine, pf_SkyCurrentBG;
    [Space]
    [SerializeField] private GameObject pf_CoreSoul, pf_SoulFlash;

    //Unity Interface
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, 10)][SerializeField] private float m_MovementSpeed = 5f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, .3f)][SerializeField] private float m_StopDampening = .05f;
    [SerializeField] private LayerMask m_Ground;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Animator m_Anim;

    //Custom Parameters - Constant
    const float k_GroundRad = .5f;
    Vector3 k_SkyCurrentHitBoxSize = new Vector3(6f,6f,0), k_SkyCurrentHitBoxPos = new Vector3(0,0,0);
    Vector3 k_SlashHitBoxSize = new Vector3(1.7f, 3f), k_SlashHitBoxPos = new Vector3(1.3f, -.2f);
    Vector3 k_WhirlwindHitBoxSize = new Vector3(6f, 9f), k_WhirlwindHitBoxPos = new Vector3(0, 3.5f);
    Vector3 k_FuriousRushHitBoxSize = new Vector3(6f,6f,0), k_FuriousRushHitBoxPos = new Vector3(0,0,0);

    //Custom Parameters - Public
    public bool Moveable = true;

    //Custom Parameters
    private bool m_Grounded, m_LaggingJump = false;
    private Rigidbody2D m_MyBody;
    private bool m_RightDirection = true;
    private Vector3 m_Velocity = Vector3.zero, m_Position;
    private int m_Counter = 0;
    private List<GameObject> m_VectorManipulationList = new List<GameObject>();
    [Space]

    //Thread Parameters
    Thread t_Movement, t_Attack;
    DateTime t_time;
    bool t_isComplete = false, t_Furious = false, t_Slash = false, t_Whirlwind = false, t_Grounded = false;
    bool t_SkyCurrent = false;
    float t_VelocityX, t_ForceY, t_ForceX;
    const float k_ThreadSpeed = 1;
    Vector3 t_HitBoxPos, t_HitBoxSize, t_Velocity;
    Vector3 t_VectorManipulationPos, t_VectorManipulationSize;
    Vector2 t_VectorManipulationForce;
    string t_AnimAction = "";
    GameObject t_DashBG;

    //Buttons Configuration
    public KeyCode kc_Left = KeyCode.A, kc_Right = KeyCode.D, kc_Up = KeyCode.W, kc_Down = KeyCode.S;
    bool kc_LeftPressed = false, kc_RightPressed = false, kc_UpPressed = false, kc_DownPressed = false;

    //Abilities Confiruation
    int[] Priority = new int[MaxAbilities] {4,3,2,0};
    //0-Left, 1-Right, 2-Up, 3-Down
    //Slash : 0
    bool[] ac_Slash = new bool[] { false, false, false, true };
    //Dash : 1
    bool[] ac_Dash = new bool[] { false, false, false, true };
    //Furios Rush : 2
    bool[] ac_FuriosRush = new bool[] { false, false, true, true };
    //Whirlwind : 3
    bool[] ac_Whirlwind = new bool[] { true, false, false, true };
    //Whirlwind : 4
    bool[] ac_SkyCurrent = new bool[] { false, true, false, true };

    //Health Configuration
    Soul[] hp_SoulPoint = new Soul[25];
    int hp_Count = 0;

    //Event
    public UnityEvent OnHit, OnUseSkill, OnCleanUpSoul;

    //System Functions

    private void Awake()
    {
        t_HitBoxPos = new Vector3(0, 0, 0);
        t_HitBoxSize = new Vector3(0, 0, 0);
        m_Ground = LayerMask.GetMask("Ground");
        m_MyBody = GetComponent<Rigidbody2D>();
        t_VelocityX = 0; t_ForceY = 0;
        t_Velocity = new Vector3(0, 0, 0);

        //Skill
        OnHit = new UnityEvent();
        OnHit.AddListener(WhenHit);

        OnUseSkill = new UnityEvent();
        OnUseSkill.AddListener(UseSkill);

        OnCleanUpSoul = new UnityEvent();
        OnCleanUpSoul.AddListener(CleanSoulUp);
    }

    void Start()
    {
        
        SetMovement();
        SetAttack();
        t_Movement = new Thread(new ThreadStart(MovementControl));
        t_Attack = new Thread(new ThreadStart(AttackControl));
        t_Movement.Start();
        t_Attack.Start();
    }

    private void Update()
    {
        lock (this)
        {
            m_Counter++;
            if (m_Counter > 10000) m_Counter = 0;
            SetMovement();
            SetAttack();
        }
        lock (this)
        {
            MovementAftermath();
            AttackAftermath();
        }
        if (!t_Grounded && !m_LaggingJump) StartCoroutine(JumpLag());
    }

    private void FixedUpdate()
    {
        m_Grounded = CheckGround();
        lock (this)
        {
            AttackAftermath2();
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
            Debug.Log("Movement Thread can not abort on destroy");
        }
        try
        {
            if (t_Attack.IsAlive)
                t_Attack.Abort();
        }
        catch
        {
            Debug.Log("Attack Thread can not abort on destroy");
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
            Debug.Log("Movement Thread can not abort on quit");
        }
        try
        {
            if (t_Attack.IsAlive)
                t_Attack.Abort();
        }
        catch
        {
            Debug.Log("Attack Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    private void OnDrawGizmos()
    {
        if (t_HitBoxSize.x != 0) Gizmos.DrawWireCube(transform.position + t_HitBoxPos,t_HitBoxSize);
        if (t_VectorManipulationSize.x != 0) Gizmos.DrawWireCube(transform.position + t_VectorManipulationPos, t_VectorManipulationSize);
        //Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundRad);
    }


    // Custom Functions

    /// <summary>Manage the MovementControl Thread after the Thread Sented values</summary>
    private void MovementAftermath()
    {
        if (t_VelocityX != 0)
        {
            if (Moveable)
            {
                Vector3 Vel = new Vector2(t_VelocityX, m_MyBody.velocity.y);
                m_MyBody.velocity = Vector3.SmoothDamp(m_MyBody.velocity, Vel, ref m_Velocity, m_MovementSmoothing);
                t_VelocityX = 0;
                if (kc_LeftPressed && kc_RightPressed) { }
                else if (kc_LeftPressed && m_RightDirection) { Flip(); }
                else if (kc_RightPressed && !m_RightDirection) { Flip(); }
            }
        }
        if (t_ForceY != 0)
        {
            if (Moveable)
            {
                m_MyBody.velocity = new Vector2(m_MyBody.velocity.x, 0);
                m_MyBody.AddForce(new Vector2(0f, t_ForceY));
                t_ForceY = 0;
            }
        }
        if (!kc_LeftPressed && !kc_RightPressed)
        {
            Vector2 Vel = new Vector2(0, m_MyBody.velocity.y);
            m_MyBody.velocity = Vector3.SmoothDamp(m_MyBody.velocity, Vel, ref m_Velocity, m_StopDampening);
        }
    }

    /// <summary>Set the ready state for the MovementControl Thread</summary>
    private void SetMovement()
    {
        if (Input.GetKey(kc_Left)) kc_LeftPressed = true;
        else kc_LeftPressed = false;
        if (Input.GetKey(kc_Right)) kc_RightPressed = true;
        else kc_RightPressed = false;
        if (Input.GetKey(kc_Up)) kc_UpPressed = true;
        else kc_UpPressed = false;
        t_time = DateTime.Now;
        m_Position = transform.position;
        
    }

    /// <summary>Set the ready state for the AttackControl Thread</summary>
    private void SetAttack()
    {
        if (Input.GetKey(kc_Left)) kc_LeftPressed = true;
        else kc_LeftPressed = false;
        if (Input.GetKey(kc_Right)) kc_RightPressed = true;
        else kc_RightPressed = false;
        if (Input.GetKey(kc_Up)) kc_UpPressed = true;
        else kc_UpPressed = false;
        if (Input.GetKey(kc_Down)) kc_DownPressed = true;
        else kc_DownPressed = false;
        t_Velocity = m_MyBody.velocity;
    }

    /// <summary>Manage the AttackControl Thread after the Thread Sented values</summary>
    private void AttackAftermath()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hp_Count == 0) StartCoroutine(SpawnSouls());
            else
            {
                for (int i = 0; i < hp_Count; i++)
                {
                    hp_SoulPoint[i].u_Show = !hp_SoulPoint[i].u_Show;
                }
            }
        }
        if (t_AnimAction != AnimationName.Null)
        {
            if (t_AnimAction == AnimationName.swd_SkyCurrent)
            {
                StartCoroutine(EnlargeSwordForSec(0.5f));
            }
            m_Anim.Play(t_AnimAction);
            t_AnimAction = AnimationName.Null;
        }
        if (t_ForceX != 0)
        {
            m_MyBody.AddForce(new Vector2(t_ForceX, 0f));
            t_ForceX = 0;
            StartCoroutine(SpawnDash((t_Furious)?pf_GreenDash:pf_WhiteDash, 0.02f));
        }
        if (t_Furious)
        {
            if (UnityEngine.Random.Range(1, 5) == 1)
            {
                GameObject obj = Instantiate(pf_DashLine);
                obj.transform.position = transform.position + new Vector3((m_RightDirection) ? -1 : 1, UnityEngine.Random.Range(-1f,1f));
                obj.GetComponent<PropelBackward>().U_RightDirection = (m_RightDirection)? 2 : 1;
                obj.transform.localScale = new Vector3(UnityEngine.Random.Range(0.1f, 0.3f), 0.5f);
            }
            if (m_Counter % 2 == 0)
            {
                GameObject obj = Instantiate(pf_Slash);
                obj.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
                float xy = UnityEngine.Random.Range(0.1f, 0.3f);
                obj.transform.localScale = new Vector3(xy, xy);
                obj.GetComponent<Expane>().U_MaxSize = new Vector3(xy + 0.3f, xy + 0.3f);
                obj.transform.Rotate(0,0,UnityEngine.Random.Range(0f, 180f));
            }
        }
        if (t_SkyCurrent)
        {
            if (m_Anim.GetComponent<SpriteRenderer>().enabled)
            {
                OnUseSkill.Invoke();
                m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (UnityEngine.Random.Range(1, 5) == 1)
            {
                GameObject obj = Instantiate(pf_SkyCurrentLine);
                obj.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f),-1f);;
                obj.transform.localScale = new Vector3(UnityEngine.Random.Range(0.1f, 0.3f), 0.5f);
            }
            if (m_Counter % 10 == 0)
            {
                GameObject obj = Instantiate(pf_SkyCurrentDash);
                obj.transform.position = transform.position + new Vector3(0, -.5f);
                obj.transform.localScale = new Vector3(.3f, .3f);
            }
        }
        if (t_Whirlwind)
        {
            if (m_Anim.GetComponent<SpriteRenderer>().enabled)
            {
                OnUseSkill.Invoke();
                m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (m_Counter % 9 == 0)
            {
                GameObject obj = Instantiate(pf_WhirlWind);
                obj.transform.position = transform.position + new Vector3(0,-.5f);
                obj.transform.localScale = new Vector3(.3f, .3f);
            }
        }
        if (!t_Whirlwind && !m_Anim.GetComponent<SpriteRenderer>().enabled && !t_Furious && !t_SkyCurrent) m_Anim.GetComponent<SpriteRenderer>().enabled = true;
        if (t_Furious && !t_DashBG)
        {
            OnUseSkill.Invoke();
            m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            t_DashBG = Instantiate(pf_DashBG);
            t_DashBG.transform.SetParent(transform);
            t_DashBG.transform.localPosition = new Vector3(0, 0, 0);
            t_DashBG.transform.localScale = new Vector3(0.6f, 0.6f);
        }
        if (!t_Furious && t_DashBG)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = true;
            Destroy(t_DashBG);
        }
    }

    /// <summary>Manage the AttackControl Thread after the Thread sented values in FixedUpdate</summary>
    private void AttackAftermath2()
    {
        if (t_VectorManipulationSize.x != 0)
        {
            Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position + t_VectorManipulationPos, t_VectorManipulationSize, 0, LayerMask.GetMask("Player"));
            foreach (Collider2D Enemy in EnemyHit)
            {
                if (Enemy.gameObject == gameObject) continue;
                if (m_VectorManipulationList.Contains(Enemy.gameObject)) continue;
                Enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                Enemy.GetComponent<Rigidbody2D>().AddForce(t_VectorManipulationForce);
                m_VectorManipulationList.Add(Enemy.gameObject);
                Enemy.GetComponent<Player>().Moveable = false;
                Debug.Log(Enemy.gameObject + " Up");
            }
        }
        else if (t_VectorManipulationSize == new Vector3(0, 0, 0))
        {
            if (m_VectorManipulationList.Count != 0)
            {
                foreach (GameObject Enemy in m_VectorManipulationList) Enemy.GetComponent<Player>().Moveable = true;
                m_VectorManipulationList.Clear();
            }
        }
    }

    /// <summary>Check if the object touched the ground</summary>
    public bool CheckGround()
    {
        return(Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundRad, m_Ground));
    }


    //Managing Thread

    /// <summary>Simulate movement control of an object</summary>
    public void MovementControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                float move = 0;
                if (kc_LeftPressed) move -= m_MovementSpeed;
                if (kc_RightPressed) move += m_MovementSpeed;
                //Apply Velocity
                t_VelocityX = move / k_ThreadSpeed;
                //Jumping
                if (t_Grounded && m_Grounded && kc_UpPressed && !kc_DownPressed)
                {
                    m_Grounded = false;
                    t_Grounded = false;
                    t_ForceY = m_JumpForce / k_ThreadSpeed;
                }
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>Simulate attack control of an object</summary>
    public void AttackControl()
    {
        while (!t_isComplete)
        {
            //Attack
            for (int i = 0; i < MaxAbilities; i++)
            {
                if (Priority[i] == 0)
                    if (kc_LeftPressed == ac_Slash[0] && kc_RightPressed == ac_Slash[1]
                    && kc_UpPressed == ac_Slash[2] && kc_DownPressed == ac_Slash[3])
                        Slash();
                if (Priority[i] == 1)
                    if (kc_LeftPressed == ac_Dash[0] && kc_RightPressed == ac_Dash[1]
                    && kc_UpPressed == ac_Dash[2] && kc_DownPressed == ac_Dash[3])
                        Dash();
                if (Priority[i] == 2 && hp_Count > 0)
                    if (kc_LeftPressed == ac_FuriosRush[0] && kc_RightPressed == ac_FuriosRush[1]
                    && kc_UpPressed == ac_FuriosRush[2] && kc_DownPressed == ac_FuriosRush[3])
                        FuriousRush();
                if (Priority[i] == 3 && hp_Count > 0)
                    if (kc_LeftPressed == ac_Whirlwind[0] && kc_RightPressed == ac_Whirlwind[1]
                    && kc_UpPressed == ac_Whirlwind[2] && kc_DownPressed == ac_Whirlwind[3])
                        Whirlwind();
                if (Priority[i] == 4 && hp_Count > 0)
                    if (kc_LeftPressed == ac_SkyCurrent[0] && kc_RightPressed == ac_SkyCurrent[1]
                    && kc_UpPressed == ac_SkyCurrent[2] && kc_DownPressed == ac_SkyCurrent[3])
                        SkyCurrent();
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>Dash Ability</summary>
    private void Dash(bool FollowUp = false)
    {
        lock (this)
        {
            t_ForceX = 0;
            if (m_RightDirection) t_ForceX += m_MovementSpeed * 300;
            else t_ForceX -= m_MovementSpeed * 300;
        }
        if (!FollowUp)
        {
            Thread.Sleep(500);
        }
    }

    /// <summary>Furious Rush Skill</summary>
    private void FuriousRush()
    {
        t_Furious = true;
        float Force = 0;
        lock (this)
        {
            Force = t_Velocity.x;
            if (Force > -m_MovementSpeed * 300 && Force < m_MovementSpeed * 300)
            {
                Dash(true);
                Force = t_ForceX;
            }
        }
        lock (this)
        {
            t_HitBoxPos = k_FuriousRushHitBoxPos;
            t_HitBoxSize = k_FuriousRushHitBoxSize;
        }
        for (int i = 1; i <= 5; i++)
        {
            lock (this)
            {
                t_ForceX = Force;
            }
            Thread.Sleep(100);
        }
        t_Furious = false;
        lock (this)
        {
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
        }
        Thread.Sleep(250);
    }

    /// <summary>Whirlwind Skill</summary>
    private void Whirlwind()
    {
        t_Whirlwind = true;
        m_MovementSpeed /= 2;
        m_JumpForce /= 2;
        lock (this)
        {
            t_HitBoxPos = k_WhirlwindHitBoxPos;
            t_HitBoxSize = k_WhirlwindHitBoxSize;
        }
        for (int i = 1; i <= 5; i++)
        {
            t_VectorManipulationSize = k_WhirlwindHitBoxSize;
            t_VectorManipulationPos = k_WhirlwindHitBoxPos;
            t_VectorManipulationForce = new Vector2(0, m_JumpForce/2.5f);
            Thread.Sleep(50);
            t_VectorManipulationSize = Vector3.zero;
            t_VectorManipulationPos = Vector3.zero;
            t_VectorManipulationForce = Vector2.zero;
            Thread.Sleep(50);
        }
        t_Whirlwind = false;
        lock (this)
        {
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
        }
        m_MovementSpeed *= 2;
        m_JumpForce *= 2;
        Thread.Sleep(250);
    }

    /// <summary>SkyCurrent Skill</summary>
    private void SkyCurrent()
    {
        t_SkyCurrent = true;
        float Force = m_JumpForce*1.5f;
        for (int i = 1; i <= 3; i++)
        {
            lock (this)
            {
                t_VectorManipulationSize = k_SkyCurrentHitBoxSize;
                t_VectorManipulationPos = k_SkyCurrentHitBoxPos;
                t_VectorManipulationForce = new Vector2(0, Force);
                t_ForceY = Force;
            }
            Thread.Sleep(50);
            lock (this)
            {
                t_VectorManipulationSize = Vector3.zero;
                t_VectorManipulationPos = Vector3.zero;
                t_VectorManipulationForce = Vector2.zero;
            }
            Thread.Sleep(100);
        }
        t_SkyCurrent = false;
        lock (this)
        {
            t_AnimAction = AnimationName.swd_SkyCurrent;
            t_HitBoxPos = k_SkyCurrentHitBoxPos;
            t_HitBoxSize = k_SkyCurrentHitBoxSize;
        }
        Thread.Sleep(250);
        lock (this)
        {
            t_AnimAction = AnimationName.swd_Recover2;
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
        }
        Thread.Sleep(500);
        t_AnimAction = AnimationName.swd_Idle;
    }

    /// <summary>Slash Ability</summary>
    private void Slash()
    {
        t_Slash = true;
        Thread.Sleep(100);
        lock (this)
        {
            t_HitBoxPos = k_SlashHitBoxPos * new Vector2((m_RightDirection ? 1 : -1), 1);
            t_HitBoxSize = k_SlashHitBoxSize;
            t_AnimAction = AnimationName.swd_Swing;
        }
        Thread.Sleep(250);
        lock (this)
        {
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
            t_AnimAction = AnimationName.swd_Recover;
        }
        t_Slash = false;
        Thread.Sleep(250);
    }

    /// <summary>Flip the object</summary>
    private void Flip()
    {
        if (!t_Slash && !t_Furious && !t_Whirlwind)
        {
            m_RightDirection = !m_RightDirection;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void UseSkill()
    {
        if (hp_Count <= 0) return;
        ShowSouls();
        StartCoroutine(WaitAndDestroySoul(0.5f, hp_Count-1));
    }

    private void WhenHit()
    {
        if (hp_Count <= 0) return;
        ShowSouls();
        StartCoroutine(WaitAndDestroySoul(0.5f, 0));
    }

    public void ShowSouls()
    {
        lock (this)
        {
            for (int i = 0; i < hp_Count; i++)
            {
                hp_SoulPoint[i].u_Show = true;
            }
        }
    }

    public void HideSouls()
    {
        return;
        lock (this)
        {
            for (int i = 0; i < hp_Count; i++)
            {
                hp_SoulPoint[i].u_Show = false;
            }
        }
    }

    public void CleanSoulUp()
    {
        StartCoroutine(DelayedSort(0f));
    }

    private void Swap(ref Soul a, ref Soul b)
    {
        Soul c = a; a = b; b = c;
    }

    private void Sort(int l, int r)
    {
        int i = l, j = r;
        int x = ConvertSoulNameToPriority(hp_SoulPoint[l + UnityEngine.Random.Range(0,r - l + 1)].u_SoulType);
        while (i < j)
        {
            while (ConvertSoulNameToPriority(hp_SoulPoint[i].u_SoulType) < x) { i++; }
            while (ConvertSoulNameToPriority(hp_SoulPoint[j].u_SoulType) > x) { j--; }
            if (i <= j)
            {
                Swap(ref hp_SoulPoint[i], ref hp_SoulPoint[j]);
                Swap(ref hp_SoulPoint[i], ref hp_SoulPoint[j]);
                i++; j--;
            }
        }
        if (i < r) { Sort(i, r); }
        if (l < j) { Sort(l, j); }
    }

    private int ConvertSoulNameToPriority(string Name)
    {
        if (Name == SoulName.Null) return 10;
        if (Name == SoulName.Core) return 1;
        if (Name == SoulName.Sustain) return 2;
        if (Name == SoulName.Fight) return 3;
        if (Name == SoulName.Burst) return 4;
        return 10;
    }

    //Corountine

    private IEnumerator SpawnDash(GameObject obj, float waitSec)
    {
        yield return new WaitForSeconds(waitSec);
        GameObject spawnedObj = Instantiate(obj);
        spawnedObj.transform.position = transform.position;
        spawnedObj.transform.localScale = new Vector3(.35f,.35f);
        spawnedObj.transform.localScale = new Vector3((m_RightDirection? -1 : 1)*spawnedObj.transform.localScale.x, spawnedObj.transform.localScale.y);
        spawnedObj.GetComponent<PropelBackward>().U_RightDirection = (m_RightDirection) ? 2 : 1;
    }

    /// <summary>Spawning the souls of an object</summary>
    private IEnumerator SpawnSouls()
    {
        float SoulsDistancy = 0.25f;
        for (int i = -2; i <= 2; i++)
        {
            GameObject obj = Instantiate(pf_SoulFlash);
            obj.transform.SetParent(transform.GetChild(0));
            obj.transform.localPosition = new Vector3(SoulsDistancy * i, 0);
            yield return new WaitForSeconds(0.1f);
            obj = Instantiate(pf_CoreSoul);
            obj.transform.SetParent(transform.GetChild(0));
            obj.transform.localPosition = new Vector3(SoulsDistancy * i, 0);
            obj.transform.localScale = new Vector3(0.05f,0.05f);
            obj.GetComponent<Soul>().u_Player = this;
            obj.GetComponent<Soul>().u_SoulType = SoulName.Core;
            hp_SoulPoint[hp_Count] = obj.GetComponent<Soul>();
            hp_Count++;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        HideSouls();
    }

    /// <summary>Time between jumps </summary>
    private IEnumerator JumpLag()
    {
        m_LaggingJump = true;
        yield return new WaitForSeconds(0.2f);
        m_LaggingJump = false;
        t_Grounded = true;
    }

    private IEnumerator WaitAndDestroySoul(float sec, int id)
    {
        yield return new WaitForSeconds(sec);
        hp_SoulPoint[id].OnDestruction.Invoke();
    }

    private IEnumerator DelayedSort(float sec)
    {
        yield return new WaitForSeconds(sec);
        lock (this)
        {
            for (int i = 0; i < hp_Count; i++)
            {
                if (hp_SoulPoint[i] == null)
                {
                    Swap(ref hp_SoulPoint[i], ref hp_SoulPoint[hp_Count - 1]);
                    hp_Count--;
                    break;
                }
            }
            Sort(0, hp_Count - 1);
        }
        yield return new WaitForSeconds(0.75f);
        HideSouls();
    }

    private IEnumerator EnlargeSwordForSec(float sec)
    {
        Vector3 position = m_Anim.transform.localPosition;
        m_Anim.transform.localPosition = new Vector3(0,0,0);
        m_Anim.transform.localScale *= 1.5f;
        yield return new WaitForSeconds(sec);
        m_Anim.transform.localPosition = position;
        m_Anim.transform.localScale /= 1.5f;
    }
}

public class AnimationName
{
    public static string Null = "", swd_Swing = "SwordSwing", swd_Idle = "SwordIdle", swd_Recover = "SwordRecover", swd_SkyCurrent = "SkyCurrent", swd_Recover2 = "SwordRecover2";
}