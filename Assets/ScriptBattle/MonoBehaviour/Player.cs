using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
//using UnityEngine.UI;
//using JetBrains.Annotations;
using UnityEngine.Events;
//using static UnityEngine.GraphicsBuffer;
//using static UnityEngine.EventSystems.EventTrigger;
//using UnityEditor.Experimental.GraphView;
using System.Linq;

public class Player : MonoBehaviour
{
    //Const
    private const int MaxAbilities = 4;
    public Color c_sustain;

    //Prefabs
    [Space]
    public bool m_PlayerMode = true, m_Enable = true;
    public Sprite Swordman, Gunner, Assasin, Mage;
    [Space]
    [SerializeField] private GameObject pf_WhiteDash, pf_GreenDash, pf_DashBG, pf_DashLine, pf_Slash,
        pf_WhirlWind, pf_SkyCurrentDash, pf_SkyCurrentLine, pf_SkyCurrentBG, pf_AirSlash, pf_Bullet, pf_Gateway, pf_GatewayEffect,
        pf_FlareBurstSignal, pf_BulletTornado, pf_SnipeIcon, pf_ShadowGhost, pf_ShadowCloak, pf_SoulSurgeEffect, pf_DeathCurse,
        pf_ManaSupply, pf_GrandTeleport, pf_PrepareBeam, pf_OceanBeam, pf_EarthenLance, pf_OverloadGhost, pf_HolyRadiance;
    [Space]
    [SerializeField] private Sprite sp_OverloadGhost; 
    public GameObject pf_DarkSpike;
    [Space]
    [SerializeField] private GameObject pf_CoreSoul, pf_SoulFlash;

    //Unity Interface
    [SerializeField] public float m_JumpForce = 400f;
    [Range(0, 10)][SerializeField] public float m_MovementSpeed = 5f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [Range(0, .3f)][SerializeField] private float m_StopDampening = .05f;
    [SerializeField] private LayerMask m_Ground;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Animator m_Anim;

    //Custom Parameters - Constant
    const float k_GroundRad = .5f, k_BottomLimit = -20f, k_SustainSpawnRate = 10, k_SustainLifeTime = 50f, k_FightLifeTime = 20f;
    Vector3 k_SkyCurrentHitBoxSize = new Vector3(6f,6f,0), k_SkyCurrentHitBoxPos = new Vector3(0.5f,0,0);
    Vector3 k_SlashHitBoxSize = new Vector3(1.7f, 3f), k_SlashHitBoxPos = new Vector3(1.3f, -.2f);
    Vector3 k_WhirlwindHitBoxSize = new Vector3(4f, 4f), k_WhirlwindHitBoxPos = new Vector3(0, 1.5f);
    Vector3 k_FuriousRushHitBoxSize = new Vector3(6f,6f,0), k_FuriousRushHitBoxPos = new Vector3(0,0,0);
    Vector3 k_BulletTornadoHitBoxSize = new Vector3(9f,9f,0), k_BulletTornadoHitBoxPos = new Vector3(0,0,0);
    Vector3 k_EscapeBlastHitBoxSize = new Vector3(5f,5f,0), k_EscapeBlastHitBoxPos = new Vector3(5.5f,0,0);
    Vector3 k_BackstabHitBoxSize = new Vector3(1.5f, 0.3f), k_BackstabHitBoxPos = new Vector3(2f, 0.05f);
    Vector3 k_SuicidalLungeHitBoxSize = new Vector3(14f, 5f), k_SuicidalLungeHitBoxPos = new Vector3(-1f, 0.05f);

    //Custom Parameters - Public
    public bool m_Moveable = true, m_Immobilized = true, m_ButtonBind = false, m_Dead = false, m_OutsideSoulGain = false, m_Turnable = true, m_Immunity = false, t_DoubleDamage = false;
    [HideInInspector] public int m_UnmoveableStack = 0, m_ImmobilizeStack = 1;
    public GameObject m_P2, m_P3, m_P4;

    //Custom Parameters
    private bool m_Grounded, m_LaggingJump = false;
    private Rigidbody2D m_MyBody;
    private bool m_RightDirection = true;
    private Vector3 m_Velocity = Vector3.zero, m_Position;
    private int m_Counter = 0;
    private List<GameObject> m_VectorManipulationList = new List<GameObject>();
    private List<GameObject> m_EnemiesHitList = new List<GameObject>();
    private float m_SoulsDistancy = 0.2f;
    [Space]

    //Thread Parameters
    Thread t_Movement, t_Attack, t_Sustain, t_Bot;
    DateTime t_time;
    bool t_isComplete = false, t_Furious = false, t_Slash = false, t_Whirlwind = false, t_Grounded = false;
    bool t_SkyCurrent = false, t_AirSlash = false, t_GroundSlam = false, t_Shoot = false, t_Gateway = false, t_FlareBurst = false,
        t_BulletTornado = false, t_EscapeBlast = false, t_SnipePoint = false, t_Backstab = false, t_ShadowCloak = false, t_DarkSpike = false,
        t_SoulSurge = false, t_SoulSurgeActivate = false, t_DeathCurse = false, t_SuicidalLunge = false, t_SuicidalLungeInitiate = false,
        t_Chant = false, t_GrandTeleportation = false, t_OceanBeam = false, t_EarthenLance = false, t_Overload = false, t_OverloadBacklash = false,
        t_OverloadCost = false, t_HolyRadiance = false;
    int t_ShadowCount = 0, t_GrandTeleportNewest = 0;
    float t_VelocityX, t_ForceY, t_ForceX;
    const float k_ThreadSpeed = 1;
    Vector3 t_HitBoxPos, t_HitBoxSize, t_Velocity, t_FixedDestination = Vector3.zero;
    Vector3 t_VectorManipulationPos, t_VectorManipulationSize;
    Vector2 t_VectorManipulationForce;
    bool t_VectorManipulationKnockback = false, t_HitBoxPlayerFlip = false;
    string t_AnimAction = "";
    bool t_GenerateSustainSoul = false, t_LauchAirSlash = false, t_AirSlashLeftSide, t_DeathCurseLeftSide;
    GameObject t_DashBG, t_SkyCurrentBG, t_GroundSlamBG, t_GatewayIcon, t_FlareSignal1, t_FlareSignal2, t_FlareSignal3, t_SnipeTarget,
        t_GrandTeleport1, t_GrandTeleport2;

    //Buttons Configuration
    public KeyCode kc_Left = KeyCode.A, kc_Right = KeyCode.D, kc_Up = KeyCode.W, kc_Down = KeyCode.S;
    bool kc_LeftPressed = false, kc_RightPressed = false, kc_UpPressed = false, kc_DownPressed = false, kc_InRange = false;
    int kc_RandomInteger = 0;

    //Abilities Confiruation
    public int Job = 1;
    //1-Swordman, 2-Gunner, 3-Assasin, 4-Mage
    public int[] Priority = new int[MaxAbilities] {4,3,2,0};
    public string[] PriorityName = new string[5] {"0-Slash","1-Dash","2-Furious Rush","3-Whirlwind","4-Sky Current"};
    //0-Left, 1-Right, 2-Up, 3-Down
    //Slash : 0
    public bool[] ac_Slash = new bool[] { false, false, false, true };
    //Dash : 1
    public bool[] ac_Dash = new bool[] { false, false, false, true };
    //Furios Rush : 2
    public bool[] ac_FuriosRush = new bool[] { false, false, true, true };
    //Whirlwind : 3
    public bool[] ac_Whirlwind = new bool[] { true, false, false, true };
    //Whirlwind : 4
    public bool[] ac_SkyCurrent = new bool[] { false, true, false, true };
    //Air Slash : 5
    public bool[] ac_AirSlash = new bool[] { false, false, true, true };
    //Ground Slam : 6
    public bool[] ac_GroundSlam = new bool[] { true, false, false, true };

    [Space]
    [Space]
    [Space]

    //Shoot : 0
    public bool[] ac_Shoot = new bool[] { false, false, false, true };
    //Gateway : 2
    public bool[] ac_Gateway = new bool[] { true, false, false, true };
    //Flare Burst : 3
    public bool[] ac_FlareBurst = new bool[] { false, true, false, true };
    //Flare Burst : 4
    public bool[] ac_BulletTornado = new bool[] { false, false, true, true };
    //Escape Blast : 5
    public bool[] ac_EscapeBlast = new bool[] { false, false, true, true };
    //Snipe Point : 6
    public bool[] ac_SnipePoint = new bool[] { false, true, false, true };
    

    [Space]
    [Space]
    [Space]

    //Backstab : 1
    public bool[] ac_Backstab = new bool[] { false, false, false, true };
    //Shadow Cloak : 2
    public bool[] ac_ShadowCloak = new bool[] { true, false, false, true };
    //Dark Spike : 3
    public bool[] ac_DarkSpike = new bool[] { false, true, false, true };
    //Soul Surge : 4
    public bool[] ac_SoulSurge = new bool[] { false, false, true, true };
    //Death Curse : 6
    public bool[] ac_DeathCurse = new bool[] { false, false, true, true };
    //SucidalLunge : 7
    public bool[] ac_SuicidalLunge = new bool[] { false, true, false, true };

    [Space]
    [Space]
    [Space]

    //Chant : 1
    public bool[] ac_Chant = new bool[] { false, false, false, true };
    //Grand Teleportation : 2
    public bool[] ac_GrandTeleportation = new bool[] { false, true, false, true };
    //Ocean Beam : 3
    public bool[] ac_OceanBeam = new bool[] { false, false, true, true };
    //Earthen Lances : 4
    public bool[] ac_EarthenLances = new bool[] { true, false, false, true };
    //Overload : 5
    public bool[] ac_Overload = new bool[] { false, true, false, true };
    //Holy Radiance : 6
    public bool[] ac_HolyRadiance = new bool[] { true, false, false, true };


    public string[] SkillElementMeaning = new string[4] { "Left", "Right", "Up", "Down" };

    //Health Configuration
    Soul[] hp_SoulPoint = new Soul[25];
    int hp_Count = 0, hp_CoreCount = 0, hp_FightCount = 0, hp_SustainCount = 0;
    ManaSupply[] mp_ManaPoint = new ManaSupply[3];
    int mp_Count = 0;

    //Event
    [HideInInspector] public UnityEvent OnHit, OnUseSkill, OnCleanUpSoul;

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
        if (SaveFileDirectory.Tutorial)
        {
            Priority = new int[4] { 0, 0, 0, 0 };
            ac_Slash = new bool[4] { false, false, false, true };
        }
        StartCoroutine(SpawnSouls());
        SetMovement();
        SetAttack();
        t_Movement = new Thread(new ThreadStart(MovementControl));
        t_Attack = new Thread(new ThreadStart(AttackControl));
        t_Sustain = new Thread(new ThreadStart(SustainSoulControl));
        if (!m_PlayerMode)
        {
            t_Bot = new Thread(new ThreadStart(BotMovementControl));
            t_Bot.Start();
        }
        t_Movement.Start();
        t_Attack.Start();
        t_Sustain.Start();
        if (Job == 1)
        {
            m_Anim.Play(AnimationName.swd_Idle);
            m_Anim.transform.localPosition = new Vector3(m_Anim.transform.localPosition.x, -0.33f);
            GetComponent<SpriteRenderer>().sprite = Swordman;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Job == 2)
        {
            m_Anim.Play(AnimationName.gun_Idle);
            m_Anim.transform.localPosition = new Vector3(m_Anim.transform.localPosition.x, 0.1f);
            GetComponent<SpriteRenderer>().sprite = Gunner;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Job == 3)
        {
            m_Anim.Play(AnimationName.knife_Idle);
            m_Anim.transform.localPosition = new Vector3(m_Anim.transform.localPosition.x, 0.04f);
            GetComponent<SpriteRenderer>().sprite = Assasin;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Job == 4)
        {
            m_Anim.Play(AnimationName.staff_Idle);
            if (Priority.Contains(0)) m_MovementSpeed *= 0.75f;
            GetComponent<SpriteRenderer>().sprite = Mage;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public void ActivatePlayerMode()
    {
        t_Bot = new Thread(new ThreadStart(BotMovementControl));
        t_Bot.Start();
    }

    private void Update()
    {
        lock (this)
        {
            if (m_UnmoveableStack > 0)
            {
                m_Moveable = false;
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            else
            {
                m_Moveable = true;
            }
            if (m_ImmobilizeStack > 0)
            {
                m_Immobilized = true;
                if (hp_Count > 0) GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
            else
            {
                m_Immobilized = false;
            }
            if (hp_Count > 0 && m_Enable)
            {
                kc_RandomInteger = UnityEngine.Random.Range(0, 10000);
                m_Counter++;
                if (m_Counter > 10000) m_Counter = 0;
                SetMovement();
                SetAttack();
            }
        }
        lock (this)
        {
            if (hp_Count > 0 && m_Enable)
            {
                MovementAftermath();
                AttackAftermath();
                SustainAftermath();
            }
            if ((hp_Count <= 0 && !m_Immobilized) || !m_Enable)
            {
                int xxx = 1;
                if (DailyAchieveList.dailyTasks[xxx].progressIndex < DailyAchieveList.dailyTasks[xxx].progressMax)
                {
                    DailyAchieveList.dailyTasks[xxx].progressIndex++;
                    if (DailyAchieveList.dailyTasks[xxx].progressIndex == DailyAchieveList.dailyTasks[xxx].progressMax) ItemList.Gems += DailyAchieveList.dailyTasks[xxx].reward;
                }
                if (t_GatewayIcon) Destroy(t_GatewayIcon);
                if (t_FlareSignal1) Destroy(t_FlareSignal1);
                if (t_FlareSignal2) Destroy(t_FlareSignal2);
                if (t_FlareSignal3) Destroy(t_FlareSignal3);
                if (t_SnipeTarget) Destroy(t_SnipeTarget);
                if (t_GrandTeleport1) Destroy(t_GrandTeleport1);
                if (t_GrandTeleport2) Destroy(t_GrandTeleport2);
                for (int i = 0; i < mp_Count; i++) Destroy(mp_ManaPoint[i]); 
                gameObject.layer = 5;
                GetComponent<BoxCollider2D>().enabled = false;
                m_MyBody.bodyType = RigidbodyType2D.Static;
                m_Anim.gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
                m_ImmobilizeStack=100;
                m_Dead = true;
                t_isComplete = true;
                transform.position = new Vector3(100, 100);
            }
        }
        if (!t_Grounded && !m_LaggingJump) StartCoroutine(JumpLag());
    }

    private void FixedUpdate()
    {
        if (hp_Count > 0 && m_Enable)
        {
            m_Grounded = CheckGround();
            lock (this)
            {
                AttackAftermath2();
            }
            lock (this)
            {
                SetBot();
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
        try
        {
            if (t_Sustain.IsAlive)
                t_Sustain.Abort();
        }
        catch
        {
            Debug.Log("Sustain Soul Thread can not abort on destroy");
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
        try
        {
            if (t_Sustain.IsAlive)
                t_Sustain.Abort();
        }
        catch
        {
            Debug.Log("Sustain Soul Thread can not abort on quit");
        }
        t_isComplete = true;
    }

    private void OnDrawGizmos()
    {
        if (t_HitBoxSize.x != 0)
        {
            Vector3 multiplicator = new Vector2(1, 1);
            if (t_HitBoxPlayerFlip)
            {
                if (!m_RightDirection) multiplicator.x = -1;
            }
            Gizmos.DrawWireCube(transform.position + new Vector3(t_HitBoxPos.x * multiplicator.x, t_HitBoxPos.y * multiplicator.y), t_HitBoxSize);
        }
        if (t_VectorManipulationSize.x != 0)
        {
            Vector3 multiplicator = new Vector2(1, 1);
            if (t_HitBoxPlayerFlip)
            {
                if (!m_RightDirection) multiplicator.x = -1;
            }
            Gizmos.DrawWireCube(transform.position + new Vector3(t_VectorManipulationPos.x * multiplicator.x, t_VectorManipulationPos.y * multiplicator.y), t_VectorManipulationSize);
        }
        if (!m_PlayerMode)
        {
            if (Job == 1)
            {
                Gizmos.DrawWireCube(transform.position +
                    k_FuriousRushHitBoxPos,
                    k_FuriousRushHitBoxSize);
            }
            if (Job == 2)
            {
                Gizmos.DrawWireCube(transform.position +
                    k_BulletTornadoHitBoxPos,
                    k_BulletTornadoHitBoxSize);
                Gizmos.DrawWireCube(transform.position,
                    new Vector2(100, 0.5f));
            }
        }
        //Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundRad);
    }


    // Custom Functions

    /// <summary>Manage the MovementControl Thread after the Thread Sented values</summary>
    private void MovementAftermath()
    {
        if (t_VelocityX != 0 && !m_Immobilized)
        {
            if (m_Moveable && hp_Count > 0)
            {
                Vector3 Vel = new Vector2(t_VelocityX, m_MyBody.velocity.y);
                m_MyBody.velocity = Vector3.SmoothDamp(m_MyBody.velocity, Vel, ref m_Velocity, m_MovementSmoothing);
                t_VelocityX = 0;
                if (!t_AirSlash && m_Turnable)
                    if (kc_LeftPressed && kc_RightPressed) { }
                    else if (kc_LeftPressed && m_RightDirection) { Flip(); }
                    else if (kc_RightPressed && !m_RightDirection) { Flip(); }
            }
        }
        if (t_ForceY != 0 && !m_Immobilized && hp_Count > 0)
        {
            if (m_Moveable)
            {
                m_MyBody.velocity = new Vector2(m_MyBody.velocity.x, 0);
                m_MyBody.AddForce(new Vector2(0f, t_ForceY));
                t_ForceY = 0;
            }
        }
        if (((!kc_LeftPressed && !kc_RightPressed) || (kc_LeftPressed && kc_RightPressed)) && !m_Immobilized && hp_Count > 0)
        {
            Vector2 Vel = new Vector2(0, m_MyBody.velocity.y);
            m_MyBody.velocity = Vector3.SmoothDamp(m_MyBody.velocity, Vel, ref m_Velocity, m_StopDampening);
        }
        if (transform.position.y <= k_BottomLimit)
        {
            t_ForceY = -k_BottomLimit/6.5f * m_JumpForce;
            OnHit.Invoke();
        }
    }

    /// <summary>Set the ready state for the MovementControl Thread</summary>
    private void SetMovement()
    {
        /*if (Input.GetKey(kc_Left) && !m_Immobilized && !m_ButtonBind) kc_LeftPressed = true;
        else kc_LeftPressed = false;
        if (Input.GetKey(kc_Right) && !m_Immobilized && !m_ButtonBind) kc_RightPressed = true;
        else kc_RightPressed = false;
        if (Input.GetKey(kc_Up) && !m_Immobilized && !m_ButtonBind) kc_UpPressed = true;
        else kc_UpPressed = false;*/
        t_time = DateTime.Now;
        m_Position = transform.position;
        
    }

    /// <summary>Set the ready state for the AttackControl Thread</summary>
    private void SetAttack()
    {
        if (m_PlayerMode)
        {
            if (Input.GetKey(kc_Left) && !m_Immobilized && !m_ButtonBind) kc_LeftPressed = true;
            else kc_LeftPressed = false;
            if (Input.GetKey(kc_Right) && !m_Immobilized && !m_ButtonBind) kc_RightPressed = true;
            else kc_RightPressed = false;
            if (Input.GetKey(kc_Up) && !m_Immobilized && !m_ButtonBind) kc_UpPressed = true;
            else kc_UpPressed = false;
            if (Input.GetKey(kc_Down) && !m_Immobilized && !m_ButtonBind) kc_DownPressed = true;
            else kc_DownPressed = false;
        }
        t_Velocity = m_MyBody.velocity;
        //Debug.Log(gameObject.name + " Left : " + kc_LeftPressed + " " + Input.GetKey(kc_Left) + " Right : " + kc_RightPressed + " " + Input.GetKey(kc_Right) + " Up : " + kc_UpPressed + " " + Input.GetKey(kc_Up) + " Down : " + kc_DownPressed + " " + Input.GetKey(kc_Down));
    }

    /// <summary>Set the ready state for the BotMovementControl Thread</summary>
    private void SetBot()
    {
        if (Job == 1 || Job == 3)
        {
            if (Physics2D.OverlapBoxAll(transform.position +
                k_FuriousRushHitBoxPos,
                k_FuriousRushHitBoxSize, 0, LayerMask.GetMask("Player")).Length > 1) kc_InRange = true;
            else kc_InRange = false;
        }
        else if (Job == 2 || Job == 4)
        {
            if (Physics2D.OverlapBoxAll(transform.position +
                k_BulletTornadoHitBoxPos,
                k_BulletTornadoHitBoxSize, 0, LayerMask.GetMask("Player")).Length > 1) kc_InRange = true;
            else kc_InRange = false;
            /*if (!kc_InRange)
                if (Physics2D.OverlapBoxAll(transform.position,
                    new Vector2(100,0.5f), 0, LayerMask.GetMask("Player")).Length > 1) kc_InRange = true;
                else kc_InRange = false;*/
        }
    }

    /// <summary>Manage the AttackControl Thread after the Thread Sented values</summary>
    private void AttackAftermath()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(SpawnSouls());
        if (t_AnimAction != AnimationName.Null)
        {
            if (t_AnimAction == AnimationName.swd_SkyCurrent)
            {
                StartCoroutine(EnlargeSwordForSec(0.5f,1.9f));
                FindObjectOfType<AudioManager>().Play("Slash");
            }
            if (t_AnimAction == AnimationName.knife_Stab && t_SoulSurgeActivate)
            {
                t_DoubleDamage = true;
                t_SoulSurgeActivate = false;
                StartCoroutine(EnlargeSwordForSec(0.5f,3f));
            }
            if (t_AnimAction == AnimationName.knife_Stab)
            {
                FindObjectOfType<AudioManager>().Play("Backstab");
            }
            if (t_AnimAction == AnimationName.knife_SuicidalLunge)
            {
                if (t_SoulSurgeActivate)
                {
                    t_DoubleDamage = true;
                    t_SoulSurgeActivate = false;
                    StartCoroutine(EnlargeSwordForSec(0.75f, 11f));
                } else StartCoroutine(EnlargeSwordForSec(0.75f, 5.5f));
            }
            if (t_AnimAction == AnimationName.swd_Swing)
            {
                FindObjectOfType<AudioManager>().Play("Slash");
            }
            if (t_AnimAction == AnimationName.swd_Prepare)
            {
                FindObjectOfType<AudioManager>().Play("AirSlash");
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
        if (t_GroundSlam)
        {
            if (m_Anim.GetComponent<SpriteRenderer>().enabled)
            {
                OnUseSkill.Invoke();
                m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (UnityEngine.Random.Range(1, 5) == 1)
            {
                GameObject obj = Instantiate(pf_SkyCurrentLine);
                obj.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), -1f); ;
                obj.transform.localScale = new Vector3(UnityEngine.Random.Range(0.1f, 0.3f), 0.5f);
                obj.GetComponent<PropelBackward>().U_RightDirection = 3;
            }
            if (m_Counter % 10 == 0)
            {
                GameObject obj = Instantiate(pf_SkyCurrentDash);
                obj.transform.position = transform.position + new Vector3(0, -.5f);
                obj.transform.localScale = new Vector3(.3f, .3f);
                obj.GetComponent<PropelBackward>().U_RightDirection = 3;
            }
        }
        if (t_Whirlwind)
        {
            if (m_Anim.GetComponent<SpriteRenderer>().enabled)
            {
                OnUseSkill.Invoke();
                m_Anim.GetComponent<SpriteRenderer>().enabled = false;
                FindObjectOfType<AudioManager>().Play("Whirlwind");
            }
            if (m_Counter % 9 == 0)
            {
                GameObject obj = Instantiate(pf_WhirlWind);
                obj.transform.position = transform.position + new Vector3(0,-.5f);
                obj.transform.localScale = new Vector3(.3f, .3f);
            }
        }
        if (t_ShadowCloak)
        {
            if (m_Anim.GetComponent<SpriteRenderer>().enabled)
            {
                t_ShadowCount++;
                if (t_ShadowCount >= 2)
                {
                    t_ShadowCount -= 2;
                    OnUseSkill.Invoke();
                }
                m_Anim.GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0.4f);
                GameObject obj = Instantiate(pf_ShadowCloak);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale *= (t_SoulSurgeActivate ? 2f : 1f);
                t_SoulSurgeActivate = false;
            }
            if (m_Counter % 5 == 0)
            {
                GameObject obj = Instantiate(pf_ShadowGhost);
                obj.transform.position = transform.position;
                obj.transform.localScale = transform.localScale;
                obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                obj.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
                obj.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
                obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
        }
        if (t_BulletTornado)
        {
            if (m_Anim.GetComponent<SpriteRenderer>().enabled)
            {
                OnUseSkill.Invoke();
                FindObjectOfType<AudioManager>().Play("BulletTornado");
                m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            }
            if (m_Counter % 1 == 0)
            {
                GameObject obj = Instantiate(pf_BulletTornado);
                obj.transform.position = transform.position;
                obj.transform.localScale = new Vector3(1f, 1f);
                obj.transform.Rotate(0, 0, UnityEngine.Random.Range(0, 360));
                transform.Rotate(0, 0, 10);
            }
        }
        if (!t_Whirlwind && !m_Anim.GetComponent<SpriteRenderer>().enabled && !t_Furious && !t_SkyCurrent && !t_BulletTornado && !t_ShadowCloak)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1);
            transform.SetPositionAndRotation(transform.position, new Quaternion(0,0,0,0));
        }
        if (t_SkyCurrent && !t_SkyCurrentBG)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            t_SkyCurrentBG = Instantiate(pf_SkyCurrentBG);
            t_SkyCurrentBG.transform.SetParent(transform);
            t_SkyCurrentBG.transform.localPosition = new Vector3(0, .75f, 0);
            t_SkyCurrentBG.transform.localScale = new Vector3(0.3f, 0.4f);
        }
        if (t_GroundSlam && !t_GroundSlamBG)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            t_GroundSlamBG = Instantiate(pf_SkyCurrentBG);
            t_GroundSlamBG.transform.SetParent(transform);
            t_GroundSlamBG.transform.localPosition = new Vector3(0, 0, 0);
            t_GroundSlamBG.transform.localScale = new Vector3(0.3f, -0.3f);
        }
        if (t_Furious && !t_DashBG)
        {
            OnUseSkill.Invoke();
            FindObjectOfType<AudioManager>().Play("MultipleSlash");
            m_Anim.GetComponent<SpriteRenderer>().enabled = false;
            t_DashBG = Instantiate(pf_DashBG);
            t_DashBG.transform.SetParent(transform);
            t_DashBG.transform.localPosition = new Vector3(0, .6f, 0) + (m_RightDirection? new Vector3(-.3f,0) : new Vector3(.3f,0));
            t_DashBG.transform.localScale = new Vector3(.9f, .9f) ;
        }
        if (!t_Furious && t_DashBG)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = true;
            Destroy(t_DashBG);
        }
        if (!t_SkyCurrent && t_SkyCurrentBG)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = true;
            Destroy(t_SkyCurrentBG);
        }
        if (!t_GroundSlam && t_GroundSlamBG)
        {
            m_Anim.GetComponent<SpriteRenderer>().enabled = true;
            Destroy(t_GroundSlamBG);
        }
        if (t_LauchAirSlash && hp_Count > 0)
        {
            t_LauchAirSlash = false;
            OnUseSkill.Invoke();
            AirSlashMovement airSlash = Instantiate(pf_AirSlash).GetComponent<AirSlashMovement>();
            if (t_AirSlashLeftSide)
            {
                airSlash.U_Change *= -1;
                airSlash.transform.localScale *= new Vector2(-1, 1);
            }
            airSlash.transform.position = transform.position + airSlash.U_Change * 10;
            airSlash.caster = gameObject;
        }
        if (t_DeathCurse && hp_Count > 0)
        {
            t_DeathCurse = false;
            OnUseSkill.Invoke();
            DeathCurseBehaviour deathCurse = Instantiate(pf_DeathCurse).GetComponent<DeathCurseBehaviour>();
            if (t_DeathCurseLeftSide)
            {
                deathCurse.U_Change *= -1;
                deathCurse.transform.localScale *= new Vector2(-1, 1);
            }
            deathCurse.transform.position = transform.position + deathCurse.U_Change * 10;
            deathCurse.caster = gameObject;
            if (t_SoulSurgeActivate)
            {
                deathCurse.Special = true;
                deathCurse.transform.localScale *= 2;
                deathCurse.U_Change *= 2;
                t_SoulSurgeActivate = false;
            }
        }
        if (t_Shoot && hp_Count > 0)
        {
            t_Shoot = false;
            BulletMovement bullet = Instantiate(pf_Bullet).GetComponent<BulletMovement>();
            if (!m_RightDirection)
            {
                bullet.U_Change *= -1;
                bullet.transform.localScale *= new Vector2(-1, 1);
            }
            bullet.transform.position = m_Anim.transform.position + bullet.U_Change * 10;
            bullet.caster = gameObject;
            if (t_SnipeTarget)
            {
                FindObjectOfType<AudioManager>().Play("Snipe");
                OnUseSkill.Invoke();
                bullet.m_Special = true;
                bullet.GetComponent<SpriteRenderer>().color = Color.red;
                bullet.PointTo(t_SnipeTarget.transform.parent.gameObject);
                Destroy(t_SnipeTarget);
            } else FindObjectOfType<AudioManager>().Play("Shoot");
        }
        if (t_EscapeBlast)
        {
            t_EscapeBlast = false;
            OnUseSkill.Invoke();
            FindObjectOfType<AudioManager>().Play("LowSnipe");
        }
        if (t_DarkSpike)
        {
            t_DarkSpike = false;
            OnUseSkill.Invoke();
            m_OutsideSoulGain = false;
            GameObject obj = Instantiate(pf_DarkSpike);
            obj.transform.position = transform.position;
            obj.transform.localScale *= (t_SoulSurgeActivate? 2f : 1f);
            obj.transform.position += new Vector3(m_RightDirection ? 1f+ (t_SoulSurgeActivate ? 1f : 0f) : -1f-(t_SoulSurgeActivate ? 1.5f : 0f), -.5f);
            obj.GetComponent<DarkSpikeBehaviour>().m_RightDirection = m_RightDirection;
            obj.GetComponent<DarkSpikeBehaviour>().caster = gameObject;
            obj.GetComponent<DarkSpikeBehaviour>().multiplier = (t_SoulSurgeActivate ? 2f : 1f);
            t_SoulSurgeActivate = false;
        }
        if (t_SnipePoint)
        {
            t_SnipePoint = false;
            GameObject Target = gameObject;
            FindObjectOfType<AudioManager>().Play("Gateway");
            float Dis = 1000000; 
            if (m_P2)
                if (m_P2.GetComponent<Player>().hp_Count > 0)
                    if (Dis > GetDistance(gameObject, m_P2))
                    {
                        Dis = GetDistance(gameObject, m_P2);
                        Target = m_P2;
                    }
            if (m_P3)
                if (m_P3.GetComponent<Player>().hp_Count > 0)
                    if (Dis > GetDistance(gameObject,m_P3))
                    {
                        Dis = GetDistance(gameObject, m_P3);
                        Target = m_P3;
                    }
            if (m_P4)
                if (m_P4.GetComponent<Player>().hp_Count > 0)
                    if (Dis > GetDistance(gameObject, m_P4))
                    {
                        Target = m_P4;
                    }
            if (Target != gameObject)
                if (Target.GetComponent<Player>().hp_Count <= 0)
                {
                    if (t_SnipeTarget) Destroy(t_SnipeTarget);
                }
                else
                {
                    if (!t_SnipeTarget)
                    {
                        t_SnipeTarget = Instantiate(pf_SnipeIcon);
                    }
                    t_SnipeTarget.transform.SetParent(Target.transform);
                    t_SnipeTarget.transform.localPosition = new Vector3(0, 0.5f);
                    t_SnipeTarget.transform.localScale = new Vector3(1, 1);
                }
        }
        if (t_SnipeTarget)
        {
            t_SnipeTarget.transform.Rotate(0, 0, 0.5f);
            if (t_SnipeTarget.transform.GetComponentInParent<Player>().hp_Count <= 0) t_SnipePoint = true;
        }
        if (t_SoulSurge)
        {
            OnUseSkill.Invoke();
            FindObjectOfType<AudioManager>().Play("SoulSurge");
            t_SoulSurgeActivate = true;
            t_SoulSurge = false;
        }
        if (t_SoulSurgeActivate)
        {
            if (m_Counter % 20 == 0)
            {
                GameObject obj = Instantiate(pf_SoulSurgeEffect);
                obj.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-.6f, .6f), -.5f);
                obj.transform.SetParent(transform);
            }
        }
        if (t_Gateway && hp_Count > 0)
        {
            t_Gateway = false;
            if (!t_GatewayIcon)
            {
                t_GatewayIcon = Instantiate(pf_Gateway);
                FindObjectOfType<AudioManager>().Play("Gateway");
                t_GatewayIcon.transform.position = m_Anim.transform.position;
            } else
            {
                m_MyBody.velocity = new Vector2(0, 0);
                OnUseSkill.Invoke();
                FindObjectOfType<AudioManager>().Play("LowShoot");
                GameObject obj = Instantiate(pf_GatewayEffect);
                obj.transform.position = transform.position - new Vector3(0, 0.6f);
                obj = Instantiate(pf_GatewayEffect);
                obj.transform.position = t_GatewayIcon.transform.position - new Vector3(0,0.6f);
                transform.position = t_GatewayIcon.transform.position;
                Destroy(t_GatewayIcon);
            }
        }
        if (t_SuicidalLunge)
        {
            if (m_Counter % 3 == 0)
            {
                GameObject obj = Instantiate(pf_ShadowGhost);
                obj.transform.position = transform.position;
                obj.transform.localScale = transform.localScale;
                obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                obj.GetComponent<SpriteRenderer>().color = Color.black;
                obj.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
                obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
        }
        if (t_SuicidalLungeInitiate)
        {
            OnUseSkill.Invoke();
            FindObjectOfType<AudioManager>().Play("SuicidalLunge");
            t_SuicidalLungeInitiate = false;
            t_FixedDestination = transform.position;
            for (int i = 1; i <= 40; i++)
            {
                t_FixedDestination += new Vector3(0.25f*((t_SoulSurgeActivate||t_DoubleDamage)?2:1), 0) * (m_RightDirection? 1 : -1);
                if (CheckGround(t_FixedDestination)) break;
            } 
        }
        if (t_FlareBurst && hp_Count > 0)
        {
            t_FlareBurst = false;
            if (!t_FlareSignal1)
            {
                t_FlareSignal1 = Instantiate(pf_FlareBurstSignal);
                t_FlareSignal1.transform.position = transform.position;
                t_FlareSignal1.GetComponent<FlareBehaviour>().caster = gameObject;
            }
            else if (!t_FlareSignal2)
            {
                t_FlareSignal2 = Instantiate(pf_FlareBurstSignal);
                t_FlareSignal2.transform.position = transform.position;
                t_FlareSignal2.GetComponent<FlareBehaviour>().caster = gameObject;
            }
            else if (!t_FlareSignal3)
            {
                t_FlareSignal3 = Instantiate(pf_FlareBurstSignal);
                t_FlareSignal3.transform.position = transform.position;
                t_FlareSignal3.GetComponent<FlareBehaviour>().caster = gameObject;
            } else
            {
                OnUseSkill.Invoke();
                m_OutsideSoulGain = false;
                t_FlareSignal1.GetComponent<FlareBehaviour>().Trigger();
                t_FlareSignal2.GetComponent<FlareBehaviour>().Trigger();
                t_FlareSignal3.GetComponent<FlareBehaviour>().Trigger();
            }
        }
        if (t_Overload && hp_Count > 0)
        {
            if (t_OverloadCost)
            {
                if (mp_Count >= 2)
                {
                    for (int i = mp_Count - 2; i < mp_Count; i++) Destroy(mp_ManaPoint[i].gameObject);
                    mp_Count -= 2;
                }
                else
                {
                    OnUseSkill.Invoke();
                }
                t_OverloadCost = false;
                StartCoroutine(Overloading());
            }
            t_Chant = true;
            if (m_Counter % 10 == 0)
            {
                GameObject obj = Instantiate(pf_OverloadGhost);
                obj.GetComponent<Expane>().Reverse = true;
                obj.GetComponent<PropelBackward>().U_Change = new Vector3(UnityEngine.Random.Range(0.0035f, 0.0045f) * (!m_RightDirection ? 1 : -1), UnityEngine.Random.Range(0.0035f, 0.0045f));
                obj.transform.position = transform.position;
                obj.transform.localScale = new Vector3(.6f, .6f);
                obj.GetComponent<SpriteRenderer>().sprite = sp_OverloadGhost;
                obj.GetComponent<SpriteRenderer>().color = Color.red;
                obj.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
                obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
        }
        if (t_OverloadBacklash && hp_Count > 0)
        {
            if (m_Counter % 10 == 0)
            {
                GameObject obj = Instantiate(pf_OverloadGhost);
                obj.GetComponent<Expane>().Reverse = true;
                obj.GetComponent<PropelBackward>().U_Change = new Vector3(UnityEngine.Random.Range(0.0035f, 0.0045f)*(!m_RightDirection? 1 : -1), UnityEngine.Random.Range(0.0035f, 0.0045f));
                obj.transform.position = transform.position;
                obj.transform.localScale = new Vector3(.6f, .6f);
                obj.GetComponent<SpriteRenderer>().sprite = sp_OverloadGhost;
                obj.GetComponent<SpriteRenderer>().color = Color.white;
                obj.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
                obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
        }
        if (t_Chant && hp_Count > 0)
        {
            if (Priority.Contains(0))
            {
                t_Chant = false;
                if (mp_Count < 3)
                {
                    mp_ManaPoint[mp_Count] = Instantiate(pf_ManaSupply).GetComponent<ManaSupply>();
                    mp_ManaPoint[mp_Count].transform.SetParent(transform);
                    if (mp_Count == 0)
                    {
                        mp_ManaPoint[mp_Count].transform.position = transform.position + new Vector3(m_RightDirection ? -.25f : .25f, 1.1f - .4f);
                    }
                    if (mp_Count == 1)
                    {
                        mp_ManaPoint[mp_Count].transform.position = transform.position + new Vector3(!m_RightDirection ? -.25f : .25f, 1.1f - .4f);
                    }
                    if (mp_Count == 2)
                    {
                        mp_ManaPoint[mp_Count].transform.position = transform.position + new Vector3(m_RightDirection ? -.05f : .05f, 1.4f - .4f);
                    }
                    mp_Count++;
                }
            }
        }
        if (t_GrandTeleportation && hp_Count > 0)
        {
            if (mp_Count >= 3)
            {
                for (int i = 0; i <= 2; i++) Destroy(mp_ManaPoint[i].gameObject);
                mp_Count = 0;
            } else
            {
                OnUseSkill.Invoke();
                StartCoroutine(WhenUseAfterTime(0.35f));
            }
            t_GrandTeleportation = false;
            if (t_GrandTeleportNewest == 0)
            {
                t_GrandTeleport1 = Instantiate(pf_GrandTeleport);
                t_GrandTeleport1.transform.position = m_Anim.transform.position;
                t_GrandTeleport1.GetComponent<GrandTeleportBehaviour>().caster = gameObject;
                t_GrandTeleportNewest = 1;
            }
            else if (t_GrandTeleportNewest == 1)
            {
                if (t_GrandTeleport2) Destroy(t_GrandTeleport2);
                t_GrandTeleport2 = Instantiate(pf_GrandTeleport);
                t_GrandTeleport2.transform.position = m_Anim.transform.position;
                t_GrandTeleport2.GetComponent<GrandTeleportBehaviour>().caster = gameObject;
                if (t_GrandTeleport2 && t_GrandTeleport1)
                {
                    t_GrandTeleport2.GetComponent<GrandTeleportBehaviour>().TeleportPoint = t_GrandTeleport1;
                    t_GrandTeleport1.GetComponent<GrandTeleportBehaviour>().TeleportPoint = t_GrandTeleport2;
                }
                t_GrandTeleportNewest = 2;
            }
            else if (t_GrandTeleportNewest == 2)
            {
                if (t_GrandTeleport1) Destroy(t_GrandTeleport1);
                t_GrandTeleport1 = Instantiate(pf_GrandTeleport);
                t_GrandTeleport1.transform.position = m_Anim.transform.position;
                t_GrandTeleport1.GetComponent<GrandTeleportBehaviour>().caster = gameObject;
                if (t_GrandTeleport2 && t_GrandTeleport1)
                {
                    t_GrandTeleport2.GetComponent<GrandTeleportBehaviour>().TeleportPoint = t_GrandTeleport1;
                    t_GrandTeleport1.GetComponent<GrandTeleportBehaviour>().TeleportPoint = t_GrandTeleport2;
                }
                t_GrandTeleportNewest = 1;
            }
        }
        if (!t_GrandTeleport1 && !t_GrandTeleport2) t_GrandTeleportNewest = 0;
        else if (!t_GrandTeleport1 && t_GrandTeleport2) t_GrandTeleportNewest = 2;
        else if (t_GrandTeleport1 && !t_GrandTeleport2) t_GrandTeleportNewest = 1;
        if (t_OceanBeam && hp_Count > 0)
        {
            if (mp_Count >= 3)
            {
                for (int i = 0; i <= 2; i++) Destroy(mp_ManaPoint[i].gameObject);
                mp_Count = 0;
            }
            else
            {
                OnUseSkill.Invoke();
                StartCoroutine(WhenUseAfterTime(0.35f));
            }
            FindObjectOfType<AudioManager>().Play("OceanBeam");
            t_OceanBeam = false;
            GameObject obj = Instantiate(pf_PrepareBeam);
            obj.transform.position = transform.position;
            if (!m_RightDirection)
            {
                //obj.transform.localScale *= new Vector2(-1, 1);
                //obj.GetComponent<Rotate>().U_Rotate *= -1;
            }
            GameObject beam = Instantiate(pf_OceanBeam);
            beam.transform.position = transform.position;
            beam.GetComponent<OceanBeamBehaviour>().caster = gameObject;
            beam.GetComponent<OceanBeamBehaviour>().signal = obj;
            beam.transform.localScale = new Vector2(50, 0.1f);
            beam.GetComponent<OceanBeamBehaviour>().k_HitBoxOffsetOn *= new Vector2(50,5);
            beam.GetComponent<OceanBeamBehaviour>().k_HitBoxSizeOn *= new Vector2(50,5);
            if (!m_RightDirection)
            {
                beam.GetComponent<OceanBeamBehaviour>().k_HitBoxOffsetOn *= -1;
                beam.transform.localScale *= new Vector2(-1, 1);
            }
        }        
        if (t_EarthenLance && hp_Count > 0)
        {
            int charge = mp_Count;
            if (mp_Count > 0)
            {
                for (int i = 0; i < mp_Count; i++) Destroy(mp_ManaPoint[i].gameObject);
                mp_Count = 0;
            }
            else
            {
                OnUseSkill.Invoke();
                charge = 2;
            }
            t_EarthenLance = false;
            int NumberOfLance = charge == 1? 4 :(charge==2 ? 8 : 16);
            float PosX;
            m_OutsideSoulGain = false;
            for (int i = 1; i <= NumberOfLance; i++)
            {
                PosX = charge == 3 ? UnityEngine.Random.Range(-8f, 8f) : UnityEngine.Random.Range(-4f, 4f);
                StartCoroutine(LaunchEarthenLance(NumberOfLance, PosX, i * 0.1f));
            }
        }
        if (t_HolyRadiance && hp_Count > 0)
        {
            int charge = mp_Count;
            if (mp_Count > 0)
            {
                for (int i = 0; i < mp_Count; i++) Destroy(mp_ManaPoint[i].gameObject);
                mp_Count = 0;
            }
            else
            {
                OnUseSkill.Invoke();
                charge = 2;
            }
            t_HolyRadiance = false;
            int NumberOfExposion = charge == 1 ? 2 : (charge == 2 ? 4 : 8);
            float WaitTime = charge == 1 ? 3 : (charge == 2 ? 2 : .5f);
            m_OutsideSoulGain = false;
            HolyRadiance Holy = Instantiate(pf_HolyRadiance).GetComponent<HolyRadiance>();
            Holy.transform.position = transform.position;
            Holy.caster = gameObject;
            Holy.m_Repeat = NumberOfExposion;
            Holy.m_WaitTime = WaitTime;
        }
        if (t_FixedDestination != Vector3.zero && hp_Count > 0)
        {
            transform.position = t_FixedDestination;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }

    /// <summary>Manage the AttackControl Thread after the Thread sented values in FixedUpdate</summary>
    private void AttackAftermath2()
    {
        if (t_VectorManipulationSize.x != 0)
        {
            Vector3 multiplicator = new Vector2(1, 1);
            if (t_HitBoxPlayerFlip)
            {
                if (!m_RightDirection) multiplicator.x = -1;
            }
            Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position + new Vector3(t_VectorManipulationPos.x * multiplicator.x, t_VectorManipulationPos.y * multiplicator.y), t_VectorManipulationSize, 0, LayerMask.GetMask("Player"));
            foreach (Collider2D Enemy in EnemyHit)
            {
                if (Enemy.gameObject == gameObject) continue;
                if (m_VectorManipulationList.Contains(Enemy.gameObject)) continue;
                Enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                Vector2 multiplicator2 = new Vector2(1, 1);
                if (t_VectorManipulationKnockback)
                {
                    if (Enemy.transform.position.x - transform.position.x < 0) multiplicator2.x = -1;
                    if (Enemy.transform.position.y - transform.position.y < 0) multiplicator2.y = -1;
                }
                Enemy.GetComponent<Rigidbody2D>().AddForce(t_VectorManipulationForce*multiplicator2);
                m_VectorManipulationList.Add(Enemy.gameObject);
                Enemy.GetComponent<Player>().m_Moveable = false;
            }
        }
        else if (t_VectorManipulationSize == new Vector3(0, 0, 0))
        {
            if (m_VectorManipulationList.Count != 0)
            {
                foreach (GameObject Enemy in m_VectorManipulationList) Enemy.GetComponent<Player>().m_Moveable = true;
                m_VectorManipulationList.Clear();
            }
        }
        if (t_HitBoxSize.x != 0)
        {
            Vector3 multiplicator = new Vector2(1, 1);
            if (t_HitBoxPlayerFlip)
            {
                if (!m_RightDirection) multiplicator.x = -1;
            }
            Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position + new Vector3(t_HitBoxPos.x*multiplicator.x,t_HitBoxPos.y*multiplicator.y), t_HitBoxSize, 0, LayerMask.GetMask("Player"));
            foreach (Collider2D Enemy in EnemyHit)
            {
                if (Enemy.gameObject == gameObject) continue;
                if (m_EnemiesHitList.Contains(Enemy.gameObject)) continue;
                if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().OnHit.Invoke();
                if (t_DoubleDamage)
                {
                    if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().InflictStatusEffect(StatusEffect.Immobilized, 1.5f);
                    if (!Enemy.GetComponent<Player>().m_Immunity) StartCoroutine(CreateSouls(0.35f,1,SoulName.Fight));
                    if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().StartCoroutine(Enemy.GetComponent<Player>().WhenHitAfterTime(0.25f));
                }
                if (!Enemy.GetComponent<Player>().m_Immunity) StartCoroutine(CreateSouls(0.35f,1,SoulName.Fight));
                if (!Enemy.GetComponent<Player>().m_Immunity) if (t_Backstab && UnityEngine.Random.Range(0,100) < 15) Enemy.GetComponent<Player>().InflictStatusEffect(StatusEffect.Immobilized, 0.75f);
                m_EnemiesHitList.Add(Enemy.gameObject);
            }
            if (!m_EnemiesHitList.Contains(gameObject)) m_EnemiesHitList.Add(gameObject);
        }
        else if (t_HitBoxSize == new Vector3(0, 0, 0))
        {
            if (m_EnemiesHitList.Count != 0)
            {
                if (t_DoubleDamage) t_DoubleDamage = false;
                m_EnemiesHitList.Clear();
            }
        }
    }

    /// <summary>Manage the SustainSoulControl Thread after the Thread sented values in Update</summary>
    private void SustainAftermath()
    {
        if (t_GenerateSustainSoul)
        {
            StartCoroutine(CreateSouls(0, 1, SoulName.Sustain));
            t_GenerateSustainSoul = false;
        }
    }

    /// <summary>Check if the object touched the ground</summary>
    public bool CheckGround()
    {
        return(Physics2D.OverlapCircle(m_GroundCheck.position, k_GroundRad, m_Ground));
    }

    /// <summary>Check if the position touched the ground</summary>
    public bool CheckGround(Vector3 pos)
    {
        return (Physics2D.OverlapCircle(pos, k_GroundRad, m_Ground));
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
                    int xxx = 4;
                    if (DailyAchieveList.dailyTasks[xxx].progressIndex < DailyAchieveList.dailyTasks[xxx].progressMax)
                    {
                        DailyAchieveList.dailyTasks[xxx].progressIndex += ItemList.Gems;
                        if (DailyAchieveList.dailyTasks[xxx].progressIndex > DailyAchieveList.dailyTasks[xxx].progressMax)
                            DailyAchieveList.dailyTasks[xxx].progressIndex = DailyAchieveList.dailyTasks[xxx].progressMax;
                        if (DailyAchieveList.dailyTasks[xxx].progressIndex == DailyAchieveList.dailyTasks[xxx].progressMax && !DailyAchieveList.dailyTasks[xxx].Finish)
                        {
                            ItemList.Gems += DailyAchieveList.dailyTasks[xxx].reward;
                        }
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    /// <summary>Simulate movement control of a bot object</summary>
    public void BotMovementControl()
    {
        while (!t_isComplete)
        {
            lock (this)
            {
                if (!m_Immobilized)
                {
                    if (kc_RandomInteger % 4 == 0) { kc_LeftPressed = true; kc_RightPressed = false; }
                    else if (kc_RandomInteger % 4 == 1) { kc_RightPressed = true; kc_LeftPressed = false; }
                    else
                    {
                        kc_LeftPressed = false;
                        kc_RightPressed = false;
                    }
                    if (kc_RandomInteger % 2 == 0 && !kc_UpPressed) kc_UpPressed = true;
                    else kc_UpPressed = false;
                    if (kc_InRange && !kc_DownPressed) kc_DownPressed = true;
                    else kc_DownPressed = false;
                } else
                {
                    kc_DownPressed = false;
                    kc_LeftPressed = false;
                    kc_RightPressed = false;
                    kc_UpPressed = true;
                }
            }
            Thread.Sleep(500);
        }
    }

    /// <summary>Simulate attack control of an object</summary>
    public void AttackControl()
    {
        while (!t_isComplete)
        {
            //Attack
            if (m_Moveable)
                for (int i = 0; i < MaxAbilities; i++)
                {
                    if (Job == 1)
                    {
                        if (Priority[i] == 0 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Slash[0] && kc_RightPressed == ac_Slash[1]
                            && kc_UpPressed == ac_Slash[2] && kc_DownPressed == ac_Slash[3])
                                Slash();
                        if (Priority[i] == 1 && hp_Count > 0)
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
                        if (Priority[i] == 5 && hp_Count > 0)
                            if (kc_LeftPressed == ac_AirSlash[0] && kc_RightPressed == ac_AirSlash[1]
                            && kc_UpPressed == ac_AirSlash[2] && kc_DownPressed == ac_AirSlash[3])
                                AirSlash();
                        if (Priority[i] == 6 && hp_Count > 0)
                            if (kc_LeftPressed == ac_GroundSlam[0] && kc_RightPressed == ac_GroundSlam[1]
                            && kc_UpPressed == ac_GroundSlam[2] && kc_DownPressed == ac_GroundSlam[3])
                                GroundSlam();
                    }
                    if (Job == 2)
                    {
                        if (Priority[i] == 0 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Shoot[0] && kc_RightPressed == ac_Shoot[1]
                            && kc_UpPressed == ac_Shoot[2] && kc_DownPressed == ac_Shoot[3])
                                Shoot();
                        if (Priority[i] == 1 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Dash[0] && kc_RightPressed == ac_Dash[1]
                            && kc_UpPressed == ac_Dash[2] && kc_DownPressed == ac_Dash[3])
                                Dash();
                        if (Priority[i] == 2 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Gateway[0] && kc_RightPressed == ac_Gateway[1]
                            && kc_UpPressed == ac_Gateway[2] && kc_DownPressed == ac_Gateway[3])
                                Gateway();
                        if (Priority[i] == 3 && hp_Count > 0 && m_Grounded)
                            if (kc_LeftPressed == ac_FlareBurst[0] && kc_RightPressed == ac_FlareBurst[1]
                            && kc_UpPressed == ac_FlareBurst[2] && kc_DownPressed == ac_FlareBurst[3])
                                FlareBurst();
                        if (Priority[i] == 4 && hp_Count > 0)
                            if (kc_LeftPressed == ac_BulletTornado[0] && kc_RightPressed == ac_BulletTornado[1]
                            && kc_UpPressed == ac_BulletTornado[2] && kc_DownPressed == ac_BulletTornado[3])
                                BulletTornado();
                        if (Priority[i] == 5 && hp_Count > 0)
                            if (kc_LeftPressed == ac_EscapeBlast[0] && kc_RightPressed == ac_EscapeBlast[1]
                            && kc_UpPressed == ac_EscapeBlast[2] && kc_DownPressed == ac_EscapeBlast[3])
                                EscapeBlast();
                        if (Priority[i] == 6 && hp_Count > 0)
                            if (kc_LeftPressed == ac_SnipePoint[0] && kc_RightPressed == ac_SnipePoint[1]
                            && kc_UpPressed == ac_SnipePoint[2] && kc_DownPressed == ac_SnipePoint[3])
                                SnipePoint();
                    }
                    if (Job == 3)
                    {
                        if (Priority[i] == 0 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Backstab[0] && kc_RightPressed == ac_Backstab[1]
                            && kc_UpPressed == ac_Backstab[2] && kc_DownPressed == ac_Backstab[3])
                                Backstab();
                        if (Priority[i] == 1 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Dash[0] && kc_RightPressed == ac_Dash[1]
                            && kc_UpPressed == ac_Dash[2] && kc_DownPressed == ac_Dash[3])
                                Dash();
                        if (Priority[i] == 2 && hp_Count > 0)
                            if (kc_LeftPressed == ac_ShadowCloak[0] && kc_RightPressed == ac_ShadowCloak[1]
                            && kc_UpPressed == ac_ShadowCloak[2] && kc_DownPressed == ac_ShadowCloak[3])
                                ShadowCloak();
                        if (Priority[i] == 3 && hp_Count > 0 && m_Grounded)
                            if (kc_LeftPressed == ac_DarkSpike[0] && kc_RightPressed == ac_DarkSpike[1]
                            && kc_UpPressed == ac_DarkSpike[2] && kc_DownPressed == ac_DarkSpike[3])
                                DarkSpike();
                        if (Priority[i] == 4 && hp_Count > 0 && !t_SoulSurge && !t_SoulSurgeActivate)
                            if (kc_LeftPressed == ac_SoulSurge[0] && kc_RightPressed == ac_SoulSurge[1]
                            && kc_UpPressed == ac_SoulSurge[2] && kc_DownPressed == ac_SoulSurge[3])
                                SoulSurge();
                        if (Priority[i] == 5 && hp_Count > 0)
                            if (kc_LeftPressed == ac_DeathCurse[0] && kc_RightPressed == ac_DeathCurse[1]
                            && kc_UpPressed == ac_DeathCurse[2] && kc_DownPressed == ac_DeathCurse[3])
                                DeathCurse();
                        if (Priority[i] == 6 && hp_Count > 0)
                            if (kc_LeftPressed == ac_SuicidalLunge[0] && kc_RightPressed == ac_SuicidalLunge[1]
                            && kc_UpPressed == ac_SuicidalLunge[2] && kc_DownPressed == ac_SuicidalLunge[3])
                                SuicideLunge();
                    }
                    if (Job == 4)
                    {
                        if (Priority[i] == 0 && hp_Count > 0 && mp_Count < 3 && !t_OverloadBacklash)
                            if (kc_LeftPressed == ac_Chant[0] && kc_RightPressed == ac_Chant[1]
                            && kc_UpPressed == ac_Chant[2] && kc_DownPressed == ac_Chant[3])
                                Chant();
                        if (Priority[i] == 1 && hp_Count > 0)
                            if (kc_LeftPressed == ac_Dash[0] && kc_RightPressed == ac_Dash[1]
                            && kc_UpPressed == ac_Dash[2] && kc_DownPressed == ac_Dash[3])
                                Dash();
                        if (Priority[i] == 2 && hp_Count > 0)
                            if (kc_LeftPressed == ac_GrandTeleportation[0] && kc_RightPressed == ac_GrandTeleportation[1]
                            && kc_UpPressed == ac_GrandTeleportation[2] && kc_DownPressed == ac_GrandTeleportation[3])
                                GrandTeleportation();
                        if (Priority[i] == 3 && hp_Count > 0)
                            if (kc_LeftPressed == ac_OceanBeam[0] && kc_RightPressed == ac_OceanBeam[1]
                            && kc_UpPressed == ac_OceanBeam[2] && kc_DownPressed == ac_OceanBeam[3])
                                OceanBeam();
                        if (Priority[i] == 4 && hp_Count > 0)
                            if (kc_LeftPressed == ac_EarthenLances[0] && kc_RightPressed == ac_EarthenLances[1]
                            && kc_UpPressed == ac_EarthenLances[2] && kc_DownPressed == ac_EarthenLances[3])
                                EarthenLance();
                        if (Priority[i] == 5 && hp_Count > 0 && !t_Overload && !t_OverloadBacklash)
                            if (kc_LeftPressed == ac_Overload[0] && kc_RightPressed == ac_Overload[1]
                            && kc_UpPressed == ac_Overload[2] && kc_DownPressed == ac_Overload[3])
                                Overload();
                        if (Priority[i] == 6 && hp_Count > 0)
                            if (kc_LeftPressed == ac_HolyRadiance[0] && kc_RightPressed == ac_HolyRadiance[1]
                            && kc_UpPressed == ac_HolyRadiance[2] && kc_DownPressed == ac_HolyRadiance[3])
                                HolyRadiance();
                    }
                }
            Thread.Sleep(1);
        }
    }

    public void SustainSoulControl()
    {
        while (!t_isComplete)
        {
            Thread.Sleep((int)k_SustainSpawnRate*1000);
            t_GenerateSustainSoul = true;
        }
    }

    /// <summary>Dash Ability</summary>
    private void Dash(bool FollowUp = false)
    {
        lock (this)
        {
            t_ForceX = 0;
            if (m_RightDirection) t_ForceX += m_MovementSpeed * 250;
            else t_ForceX -= m_MovementSpeed * 250;
        }
        if (!FollowUp)
        {
            Thread.Sleep(350);
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
        lock (this)
        {
            t_Furious = false;
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
        }
        Thread.Sleep(250);
    }

    /// <summary>Whirlwind Skill</summary>
    private void Whirlwind()
    {
        lock (this)
        {
            t_Whirlwind = true;
            m_MovementSpeed /= 2;
            m_JumpForce /= 2;
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
        lock (this)
        {
            t_Whirlwind = false;
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
            m_MovementSpeed *= 2;
            m_JumpForce *= 2;
        }
        Thread.Sleep(650);
    }

    /// <summary>Sky Current Skill</summary>
    private void SkyCurrent()
    {
        t_SkyCurrent = true;
        float Force = m_JumpForce*1.6f;
        for (int i = 1; i <= 3; i++)
        {
            lock (this)
            {
                t_VectorManipulationSize = k_SkyCurrentHitBoxSize*2/3f;
                t_VectorManipulationPos = k_SkyCurrentHitBoxPos*2f/3f;
                t_VectorManipulationForce = new Vector2(0, Force);
                t_ForceY = Force;
            }
            Thread.Sleep(25);
            lock (this)
            {
                t_VectorManipulationSize = Vector3.zero;
                t_VectorManipulationPos = Vector3.zero;
                t_VectorManipulationForce = Vector2.zero;
            }
            Thread.Sleep(50);
        }
        lock (this)
        {
            t_SkyCurrent = false;
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

    /// <summary>Ground Slam Skill</summary>
    private void GroundSlam()
    {
        t_GroundSlam = true;
        float Force = m_JumpForce * 3f;
        for (int i = 1; i <= 3; i++)
        {
            lock (this)
            {
                t_VectorManipulationSize = k_SkyCurrentHitBoxSize * 2 / 3f;
                t_VectorManipulationPos = k_SkyCurrentHitBoxPos * 2f / 3f;
                t_VectorManipulationForce = new Vector2(0, -Force);
                t_ForceY = -Force;
            }
            Thread.Sleep(25);
            lock (this)
            {
                t_VectorManipulationSize = Vector3.zero;
                t_VectorManipulationPos = Vector3.zero;
                t_VectorManipulationForce = Vector2.zero;
            }
            Thread.Sleep(50);
        }
        lock (this)
        {
            t_GroundSlam = false;
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
            t_Slash = false;
        }
        Thread.Sleep(500);
    }

    /// <summary>Air Slash Ability</summary>
    private void AirSlash()
    {
        t_AirSlash = true;
        Thread.Sleep(100);
        lock (this)
        {
            t_AnimAction = AnimationName.swd_Prepare;
            t_AirSlashLeftSide = !m_RightDirection;
        }
        Thread.Sleep(1000);
        lock (this)
        {
            t_AnimAction = AnimationName.swd_Attack;
            t_LauchAirSlash = true;
        }
        Thread.Sleep(300);
        t_AirSlash = false;
    }

    /// <summary>Shoot Ability</summary>
    private void Shoot()
    {
        Thread.Sleep(100);
        lock (this)
        {
            t_AnimAction = AnimationName.gun_Shoot;
            t_Shoot = true;
        }
        Thread.Sleep(750);
    }

    /// <summary>Gateway Ability</summary>
    private void Gateway()
    {
        Thread.Sleep(100);
        lock (this)
        {
            t_AnimAction = AnimationName.gun_Replica;
            t_Gateway = true;
        }
        Thread.Sleep(250);
    }

    /// <summary>Flare Burst Ability</summary>
    private void FlareBurst()
    {
        Thread.Sleep(100);
        lock (this)
        {
            t_AnimAction = AnimationName.gun_Replica;
            t_FlareBurst = true;
        }
        Thread.Sleep(250);
    }

    /// <summary>Bullet Tornado Skill</summary>
    private void BulletTornado()
    {
        lock (this)
        {
            t_BulletTornado = true;
            m_MovementSpeed /= 2;
            m_JumpForce /= 2;
            t_HitBoxSize = k_BulletTornadoHitBoxSize;
            t_HitBoxPos = k_BulletTornadoHitBoxPos;
        }
        for (int i = 1; i <= 1; i++)
        {
            t_VectorManipulationKnockback = true;
            t_VectorManipulationSize = k_BulletTornadoHitBoxSize;
            t_VectorManipulationPos = k_BulletTornadoHitBoxPos;
            t_VectorManipulationForce = new Vector2(m_MovementSpeed*750, m_JumpForce * 3f);
            Thread.Sleep(150);
            t_VectorManipulationSize = Vector3.zero;
            t_VectorManipulationPos = Vector3.zero;
            t_VectorManipulationForce = Vector2.zero;
            t_VectorManipulationKnockback = false;
            Thread.Sleep(150);
        }
        t_BulletTornado = false;
        lock (this)
        {
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
            m_MovementSpeed *= 2;
            m_JumpForce *= 2;
        }
        Thread.Sleep(1500);
    }

    /// <summary>Escape Blast Skill</summary>
    private void EscapeBlast()
    {
        lock (this)
        {
            t_EscapeBlast = true;
            m_Turnable = false;
            t_AnimAction = AnimationName.gun_Escape;
            t_HitBoxPlayerFlip = true;
            t_HitBoxSize = k_EscapeBlastHitBoxSize;
            t_HitBoxPos = k_EscapeBlastHitBoxPos;
        }
        lock (this)
        {
            t_VectorManipulationKnockback = true;
            t_VectorManipulationSize = k_EscapeBlastHitBoxSize;
            t_VectorManipulationPos = k_EscapeBlastHitBoxPos;
            t_VectorManipulationForce = new Vector2(m_MovementSpeed * 850, 0);
        }
        Thread.Sleep(150);
        lock (this) {
            t_ForceX = 0;
            if (m_RightDirection) t_ForceX -= m_MovementSpeed * 450;
            else t_ForceX += m_MovementSpeed * 500;
            t_VectorManipulationSize = Vector3.zero;
            t_VectorManipulationPos = Vector3.zero;
            t_VectorManipulationForce = Vector2.zero;
            t_VectorManipulationKnockback = false;
        }
        lock (this)
        {
            t_HitBoxPlayerFlip = false;
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
            m_Turnable = true;  
        }
        Thread.Sleep(750);
    }

    /// <summary>Snipe Point Ability</summary>
    private void SnipePoint()
    {
        Thread.Sleep(100);
        lock (this)
        {
            t_SnipePoint = true;
        }
        Thread.Sleep(250);
    }

    /// <summary>Backstab Ability</summary>
    private void Backstab()
    {
        t_Backstab = true;
        Thread.Sleep(100);
        lock (this)
        {
            m_Turnable = false;
            t_HitBoxPlayerFlip = true;
            t_HitBoxPos = k_BackstabHitBoxPos * (t_SoulSurgeActivate? 1.5f : 1f);
            t_HitBoxSize = k_BackstabHitBoxSize * (t_SoulSurgeActivate ? 3f : 1f);
            t_AnimAction = AnimationName.knife_Stab;
        }
        Thread.Sleep(400);
        lock (this)
        {
            m_Turnable = true;
            t_HitBoxPlayerFlip = false;
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
            t_Backstab = false;
        }
        Thread.Sleep(250);
    }

    /// <summary>Shadow Cloak Ability</summary>
    private void ShadowCloak()
    {
        float multiplier = (t_SoulSurgeActivate ? 2f : 1f);
        lock (this)
        {
            t_ShadowCloak = true;
            m_Immunity = true;
            m_MovementSpeed *= 2.5f * multiplier;
        }
        Thread.Sleep(1500 * (t_SoulSurgeActivate ? 2 : 1));
        lock (this)
        {
            m_MovementSpeed /= 2.5f * multiplier;   
            m_Immunity = false;
            t_ShadowCloak = false;
        }
    }

    /// <summary>Dark Spike Ability</summary>
    private void DarkSpike()
    {
        lock (this)
        {
            t_DarkSpike = true;
            m_Turnable = false;
            m_UnmoveableStack++;
            m_ButtonBind = true;
            t_AnimAction = AnimationName.Knife_SummonSpike;
        }
        Thread.Sleep(500);
        lock (this)
        {
            m_UnmoveableStack--;
            m_Turnable = true;
            m_ButtonBind = false;
        }
        Thread.Sleep(500);
    }

    /// <summary>Dark Spike Ability</summary>
    private void SoulSurge()
    {
        lock (this)
        {
            t_SoulSurge = true;
        }
        Thread.Sleep(750);
    }

    /// <summary>Death Curse Ability</summary>
    private void DeathCurse()
    {
        Thread.Sleep(100);
        lock (this)
        {
            t_DeathCurseLeftSide = !m_RightDirection;
            t_AnimAction = AnimationName.knife_FireDeathCurse;
            t_DeathCurse = true;
        }
        Thread.Sleep(1500);
    }

    /// <summary>Suicidal Lunge Skill</summary>
    private void SuicideLunge()
    {
        int multiplier = (t_SoulSurgeActivate ? 2 : 1);
        Thread.Sleep(100);
        lock (this)
        {
            t_SuicidalLunge = true;
            m_MovementSpeed *= 2f * multiplier;
        }
        Thread.Sleep(1000 * multiplier);
        lock (this)
        {
            m_Turnable = false;
            m_ButtonBind = true;
            t_AnimAction = AnimationName.knife_SuicidalLunge;
            t_SuicidalLungeInitiate = true;
            t_HitBoxPos = k_SuicidalLungeHitBoxPos * (t_SoulSurgeActivate ? 1.5f : 1f);
            t_HitBoxSize = k_SuicidalLungeHitBoxSize * (t_SoulSurgeActivate ? 2f : 1f);
            t_SuicidalLunge = false;
            t_HitBoxPlayerFlip = true;
        }
        Thread.Sleep(500);
        lock (this)
        {
            t_HitBoxPos = Vector3.zero;
            t_HitBoxSize = Vector3.zero;
            m_MovementSpeed /= 2f * multiplier;
            t_HitBoxPlayerFlip = false;
        }
        Thread.Sleep(5000 * multiplier);
        lock (this)
        {
            t_FixedDestination = Vector2.zero;
            m_Turnable = true;
            m_ButtonBind = false;
        }
    }

    /// <summary>Chant Ability</summary>
    private void Chant()
    {
        t_AnimAction = AnimationName.staff_Chant;
        m_MovementSpeed /= 5;
        m_JumpForce *= 0.75f;
        Thread.Sleep(1000);
        lock (this)
        {
            t_Chant = true;
        }
        m_MovementSpeed *= 5;
        m_JumpForce /= 0.75f;
        Thread.Sleep(3000);
    }

    /// <summary>Grand Teleportation Ability</summary>
    private void GrandTeleportation()
    {
        if (Priority.Contains(0))
        {
            t_AnimAction = AnimationName.staff_Teleport;
            m_MovementSpeed /= 5;
            m_JumpForce *= 0.75f;
            Thread.Sleep(1000);
        } else
        {
            t_AnimAction = AnimationName.staff_Teleport;
            Thread.Sleep(300);
        }
        lock (this)
        {
            t_GrandTeleportation = true;
        }
        if (Priority.Contains(0))
        {
            m_MovementSpeed *= 5;
            m_JumpForce /= 0.75f;
        }
        //Thread.Sleep(3000);
        Thread.Sleep(250);
    }

    /// <summary>Ocean Beam Ability</summary>
    private void OceanBeam()
    {
        if (Priority.Contains(0))
        {
            t_AnimAction = AnimationName.staff_Ocean;
            m_MovementSpeed /= 5;
            m_JumpForce *= 0.75f;
            m_Turnable = false;
            Thread.Sleep(1000);
        }
        else
        {
            t_AnimAction = AnimationName.staff_Ocean;
            Thread.Sleep(300);
        }
        lock (this)
        {
            t_OceanBeam = true;
        }
        Thread.Sleep(100);
        if (Priority.Contains(0))
        {
            m_Turnable = true;
            m_MovementSpeed *= 5;
            m_JumpForce /= 0.75f;
        }
        //Thread.Sleep(2900);
        Thread.Sleep(250);
    }

    /// <summary>Earthen Lance Ability</summary>
    private void EarthenLance()
    {
        if (Priority.Contains(0))
        {
            t_AnimAction = AnimationName.staff_Lance;
            m_MovementSpeed /= 5;
            m_JumpForce *= 0.75f;
            Thread.Sleep(1000);
        }
        else
        {
            t_AnimAction = AnimationName.staff_Lance;
            Thread.Sleep(300);
        }
        lock (this)
        {
            t_EarthenLance = true;
        }
        Thread.Sleep(100);
        if (Priority.Contains(0))
        {
            m_MovementSpeed *= 5;
            m_JumpForce /= 0.75f;
        }
        //Thread.Sleep(2900);
        Thread.Sleep(250);
    }

    /// <summary>Overload Ability</summary>
    private void Overload()
    {
        if (Priority.Contains(0))
        {
            t_AnimAction = AnimationName.staff_Overload;
            m_MovementSpeed /= 5;
            m_JumpForce *= 0.75f;
            Thread.Sleep(1000);
        }
        else
        {
            t_AnimAction = AnimationName.staff_Overload;
            Thread.Sleep(300);
        }
        lock (this)
        {
            t_OverloadCost = true;
            t_Overload = true;
            if (Priority.Contains(0))
            {
                m_MovementSpeed *= 5;
                m_JumpForce /= 0.75f;
            }
        }
        
        //Thread.Sleep(2900);
        Thread.Sleep(250);
    }

    /// <summary>Holy Radiance Ability</summary>
    private void HolyRadiance()
    {
        if (Priority.Contains(0))
        {
            t_AnimAction = AnimationName.staff_HolyRadiance;
            m_MovementSpeed /= 5;
            m_JumpForce *= 0.75f;
            Thread.Sleep(1000);
        }
        else
        {
            t_AnimAction = AnimationName.staff_HolyRadiance;
            Thread.Sleep(300);
        }
        lock (this)
        {
            t_HolyRadiance = true;
            if (Priority.Contains(0))
            {
                m_MovementSpeed *= 5;
                m_JumpForce /= 0.75f;
            }
        }
        //Thread.Sleep(2900);
        Thread.Sleep(250);
    }

    /// <summary>Flip the object</summary>
    public void Flip()
    {
        if (!t_Slash && !t_Furious && !t_Whirlwind)
        {
            m_RightDirection = !m_RightDirection;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
            transform.GetChild(0).transform.localScale = new Vector3(transform.GetChild(0).transform.localScale.x * -1, transform.GetChild(0).transform.localScale.y);
        }
    }

    private void UseSkill()
    {
        if (hp_Count <= 0) return;
        int xxx = 5;
        if (DailyAchieveList.dailyTasks[xxx].progressIndex < DailyAchieveList.dailyTasks[xxx].progressMax)
        {
            DailyAchieveList.dailyTasks[xxx].progressIndex += ItemList.Gems;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex > DailyAchieveList.dailyTasks[xxx].progressMax)
                DailyAchieveList.dailyTasks[xxx].progressIndex = DailyAchieveList.dailyTasks[xxx].progressMax;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex == DailyAchieveList.dailyTasks[xxx].progressMax && !DailyAchieveList.dailyTasks[xxx].Finish)
            {
                ItemList.Gems += DailyAchieveList.dailyTasks[xxx].reward;
            }
        }
        ShowSouls();
        StartCoroutine(WaitAndDestroySoul(0.1f, hp_Count-1));
    }

    private void WhenHit()
    {
        lock (this)
        {
            if (t_ShadowCloak) return;
            if (hp_Count <= 0) return;
            ShowSouls();
            StartCoroutine(WaitAndDestroySoul(0, 0));
        }
    }

    public IEnumerator WhenHitAfterTime(float sec)
    {
        yield return new WaitForSeconds(sec);
        lock (this)
        {
            if (t_ShadowCloak || hp_Count <= 0) { }
            else
            {
                ShowSouls();
                StartCoroutine(WaitAndDestroySoul(0, 0));
            }
        }
    }

    public IEnumerator WhenUseAfterTime(float sec)
    {
        yield return new WaitForSeconds(sec);
        lock (this)
        {
            int xxx = 5;
            if (DailyAchieveList.dailyTasks[xxx].progressIndex < DailyAchieveList.dailyTasks[xxx].progressMax)
            {
                DailyAchieveList.dailyTasks[xxx].progressIndex += ItemList.Gems;
                if (DailyAchieveList.dailyTasks[xxx].progressIndex > DailyAchieveList.dailyTasks[xxx].progressMax)
                    DailyAchieveList.dailyTasks[xxx].progressIndex = DailyAchieveList.dailyTasks[xxx].progressMax;
                if (DailyAchieveList.dailyTasks[xxx].progressIndex == DailyAchieveList.dailyTasks[xxx].progressMax && !DailyAchieveList.dailyTasks[xxx].Finish)
                {
                    ItemList.Gems += DailyAchieveList.dailyTasks[xxx].reward;
                }
            }
            if (t_ShadowCloak || hp_Count <= 0) { }
            else
            {
                ShowSouls();
                StartCoroutine(WaitAndDestroySoul(0.1f, hp_Count - 1));
            }
        }
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
        /*return;
        lock (this)
        {
            for (int i = 0; i < hp_Count; i++)
            {
                hp_SoulPoint[i].u_Show = false;
            }
        }*/
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
        //if (Name == SoulName.Burst) return 4;
        return 10;
    }

    public int GetHp()
    {
        return hp_Count;
    }

    private float GetDistance(GameObject a, GameObject b)
    {
        return Mathf.Sqrt((a.transform.position.x - b.transform.position.x) * (a.transform.position.x - b.transform.position.x) + (a.transform.position.y - b.transform.position.y) * (a.transform.position.y - b.transform.position.y));
    }

    public void InflictStatusEffect(string Name, float time)
    {
        StartCoroutine(InflictAbnormalStatusEffect(Name, time));
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
        for (int i = -2; i <= 2; i++)
        {
            StartCoroutine(CreateSouls(0,1,SoulName.Core));
            yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(1f);
        HideSouls();
    }

    /// <summary>Create a Soul</summary>
    public IEnumerator CreateSouls(float DelayedTime = 0, int numbersOfSouls = 1, string SoulType = "")
    {
        yield return new WaitForSeconds(DelayedTime);
        if (SoulType == "") SoulType = SoulName.Core;
        if ((m_Immobilized && SoulType != SoulName.Core) || gameObject.layer == 5) { }
        else
        {
            if (SoulType == SoulName.Core && hp_CoreCount >= 5) { }
            else if (SoulType == SoulName.Fight && hp_FightCount >= 3) { }
            else if (SoulType == SoulName.Sustain && hp_SustainCount >= 5) { }
            else
                for (int j = 0; j < numbersOfSouls; j++)
                {
                    lock (this)
                    {
                        if (SoulType == SoulName.Fight && hp_FightCount >= 3) break;
                        GameObject obj = Instantiate(pf_SoulFlash);
                        if (SoulType == SoulName.Fight) obj.GetComponent<SpriteRenderer>().color = Color.magenta;
                        if (SoulType == SoulName.Sustain) obj.GetComponent<SpriteRenderer>().color = c_sustain;
                        obj.transform.SetParent(transform.GetChild(0));
                        obj.transform.localPosition = new Vector3(m_SoulsDistancy * ((hp_Count % 5) - 2), 0 - (hp_Count / 5) * m_SoulsDistancy);
                    }
                    yield return new WaitForSeconds(0.1f);
                    lock (this)
                    {
                        GameObject obj = Instantiate(pf_CoreSoul);
                        obj.transform.SetParent(transform.GetChild(0));
                        obj.transform.localPosition = new Vector3(m_SoulsDistancy * ((hp_Count % 5) - 2), 0 - (hp_Count / 5) * m_SoulsDistancy);
                        obj.transform.localScale = new Vector3(0.04f, 0.04f);
                        obj.GetComponent<Soul>().u_Player = this;
                        obj.GetComponent<Soul>().u_SoulType = SoulType;
                        hp_SoulPoint[hp_Count] = obj.GetComponent<Soul>();
                        if (SoulType == SoulName.Fight)
                        {
                            hp_SoulPoint[hp_Count].GetComponent<SpriteRenderer>().color = Color.magenta;
                            StartCoroutine(WaitAndDestroySoul(k_FightLifeTime, hp_SoulPoint[hp_Count]));
                            hp_FightCount++;
                            if (SoulType == SoulName.Fight && hp_FightCount >= 4) StartCoroutine(WaitAndDestroySoul(0, hp_SoulPoint[hp_Count]));
                        }
                        if (SoulType == SoulName.Sustain)
                        {
                            hp_SoulPoint[hp_Count].GetComponent<SpriteRenderer>().color = c_sustain;
                            StartCoroutine(WaitAndDestroySoul(k_SustainLifeTime, hp_SoulPoint[hp_Count]));
                            hp_SustainCount++;
                        }
                        if (SoulType == SoulName.Core) hp_CoreCount++;
                        Sort(0, hp_Count);
                        hp_Count++;
                    }
                }
        }
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
        if (hp_SoulPoint[id])
        {
            if (hp_SoulPoint[id].u_SoulType == SoulName.Core) hp_CoreCount--;
            if (hp_SoulPoint[id].u_SoulType == SoulName.Fight) hp_FightCount--;
            if (hp_SoulPoint[id].u_SoulType == SoulName.Sustain) hp_SustainCount--;
            hp_SoulPoint[id].OnDestruction.Invoke();
        }
    }

    private IEnumerator WaitAndDestroySoul(float sec, Soul soul)
    {
        yield return new WaitForSeconds(sec);
        foreach (Soul Target in hp_SoulPoint)
        {
            if (!Target) continue;
            if (Target == soul)
            {
                if (Target.u_SoulType == SoulName.Core) hp_CoreCount--;
                if (Target.u_SoulType == SoulName.Fight) hp_FightCount--;
                if (Target.u_SoulType == SoulName.Sustain) hp_SustainCount--;
                Target.OnDestruction.Invoke();
                break;
            }
        }
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
            for (int i = 0; i < hp_Count; i++)
            {
                if (hp_SoulPoint[i])
                    hp_SoulPoint[i].transform.localPosition = new Vector3(m_SoulsDistancy * ((i % 5) - 2), 0 - (i / 5) * m_SoulsDistancy);
            }
        }
        yield return new WaitForSeconds(0.75f);
        HideSouls();
    }

    private IEnumerator EnlargeSwordForSec(float sec, float Size)
    {
        Vector3 position = m_Anim.transform.localPosition;
        m_Anim.transform.localPosition = new Vector3(0,0,0);
        m_Anim.transform.localScale *= Size;
        yield return new WaitForSeconds(sec);
        m_Anim.transform.localPosition = position;
        m_Anim.transform.localScale /= Size;
    }

    private IEnumerator InflictAbnormalStatusEffect(string Name, float time)
    {
        if (Name == StatusEffect.Unmoveable) m_UnmoveableStack++;
        if (Name == StatusEffect.Immobilized) m_ImmobilizeStack++;
        yield return new WaitForSeconds(time);
        if (Name == StatusEffect.Unmoveable) m_UnmoveableStack--;
        if (Name == StatusEffect.Immobilized) m_ImmobilizeStack--;
    }

    private IEnumerator LaunchEarthenLance(int numberOfLances, float PosX, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameObject obj = Instantiate(pf_EarthenLance);
        obj.transform.position = transform.position + new Vector3(PosX, 2.5f);
        obj.GetComponent<EarthLanceBehaviour>().caster = gameObject;
    }

    private IEnumerator Overloading()
    {
        yield return new WaitForSeconds(15);
        lock (this)
        {
            t_Overload = false;
            t_OverloadBacklash = true;
        }
        yield return new WaitForSeconds(30);
        lock (this)
        {
            t_OverloadBacklash = false;
        }
    }
}

public class AnimationName
{
    public static string Null = "", swd_Swing = "SwordSwing", swd_Idle = "SwordIdle", swd_Recover = "SwordRecover", swd_SkyCurrent = "SkyCurrent", swd_Recover2 = "SwordRecover2";
    public static string swd_Prepare = "AirSlashPrepare", swd_Attack = "AirSlashAttack", gun_Idle = "GunIdle", gun_Shoot = "GunFire",
        gun_Replica = "GunGateway", gun_Escape = "GunBlast", knife_Idle = "KnifeIndle", knife_Stab = "KnifeStab", Knife_SummonSpike = "KnifeSpike",
        knife_FireDeathCurse = "KnifeDeath", knife_SuicidalLunge = "KnifeLunge", staff_Chant = "StaffChant", staff_Idle = "StaffIdle", 
        staff_Teleport = "StaffGrandTeleportation", staff_Ocean = "StaffOceanBeam", staff_Lance = "StaffLance", staff_Overload = "StaffOverload",
        staff_HolyRadiance = "StaffHoly";
}

public class StatusEffect
{
    public static string Null = "", Immobilized = "Stun", Unmoveable = "Unmoveable";
}