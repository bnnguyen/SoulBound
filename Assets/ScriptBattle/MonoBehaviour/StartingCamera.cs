using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingCamera : MonoBehaviour
{
    public GameObject VictoryScreen, DrawScreen;
    public SpriteRenderer Player, Weapons;
    public Sprite[] PlayerSprite, WeaponSprites;
    public GameObject Target1, Target2, Target3, Target4;
    private Vector2 UpperLeftBorder, LowerRightBorder;
    public float MaximumCameraLimitArea = 7.5f, CameraExtension = 7.5f;
    private float MovementRate = 35f, MovementInterval = 0.01f;
    private Vector3 TutorialPos = new(6.19f, -18.71f);
    public Vector3[] P1MapPos = new Vector3[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) };
    public Vector3[] P2MapPos = new Vector3[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) };
    public Vector3[] P3MapPos = new Vector3[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) };
    public Vector3[] P4MapPos = new Vector3[] { new(0, 0), new(0, 0), new(0, 0), new(0, 0) };
    private int[] PlayerId = new int[4] { 1, 2, 3, 4 };
    public int mapId = 0, maxMaps = 4;
    [Space][Space][Space][Space][Space]
    public GameObject[] Maps;
    [Space][Space][Space][Space][Space]
    public float[] CameraLimitAreaMaps, CameraExtensionMaps, CameraIntialSize;
    private Camera myCamera;
    private bool Finish = false;

    void Start()
    {
        Target2.GetComponent<Player>().Flip();
        Target4.GetComponent<Player>().Flip();
        myCamera = GetComponent<Camera>();
        StartCoroutine(Follow());
    }

    private void Update()
    {
        SetBorder();
        SetAlive();
        if (Input.GetKeyDown(KeyCode.Z)) { Target1.GetComponent<Player>().m_PlayerMode = !Target1.GetComponent<Player>().m_PlayerMode; Target1.GetComponent<Player>().ActivatePlayerMode(); }
        if (Input.GetKeyDown(KeyCode.X)) {Target2.GetComponent<Player>().m_PlayerMode = !Target2.GetComponent<Player>().m_PlayerMode; Target2.GetComponent<Player>().ActivatePlayerMode(); }
        if (Input.GetKeyDown(KeyCode.C)) {Target3.GetComponent<Player>().m_PlayerMode = !Target3.GetComponent<Player>().m_PlayerMode; Target3.GetComponent<Player>().ActivatePlayerMode(); }
        if (Input.GetKeyDown(KeyCode.V)) {Target4.GetComponent<Player>().m_PlayerMode = !Target4.GetComponent<Player>().m_PlayerMode; Target4.GetComponent<Player>().ActivatePlayerMode(); }
        int PlayerAlive = 4, id = 0;
        if (!Target1) PlayerAlive--;
        else id = 1;
        if (!Target2) PlayerAlive--;
        else id = 2;
        if (!Target3) PlayerAlive--;
        else id = 3;
        if (!Target4) PlayerAlive--;
        else id = 4;
        if (PlayerAlive == 1 && !Finish)
        {
            if (id == 1) { Target1.GetComponent<Player>().m_ImmobilizeStack++; id = Target1.GetComponent<Player>().Job; }
            else if (id == 2) { Target2.GetComponent<Player>().m_ImmobilizeStack++; id = Target2.GetComponent<Player>().Job; }
            else if (id == 3) { Target3.GetComponent<Player>().m_ImmobilizeStack++; id = Target3.GetComponent<Player>().Job; }
            else if (id == 4) { Target4.GetComponent<Player>().m_ImmobilizeStack++; id = Target4.GetComponent<Player>().Job; }
            VictoryScreen.transform.localPosition = new Vector3(0,0,5);
            Player.sprite = PlayerSprite[id - 1];
            Weapons.sprite = WeaponSprites[id - 1];
            Finish = true;
            FindObjectOfType<AudioManager>().Stop("BattleLoop");
            FindObjectOfType<AudioManager>().Play("Victory");
            myCamera.orthographicSize = 10;
            transform.position = new Vector3(100, 0,-10);
            int xxx = 0;
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
            ItemList.Gems += 20;
        }
        else if (PlayerAlive == 0 && !Finish)
        {
            if (id == 1) { Target1.GetComponent<Player>().m_ImmobilizeStack++; id = Target1.GetComponent<Player>().Job; }
            else if (id == 2) { Target2.GetComponent<Player>().m_ImmobilizeStack++; id = Target2.GetComponent<Player>().Job; }
            else if (id == 3) { Target3.GetComponent<Player>().m_ImmobilizeStack++; id = Target3.GetComponent<Player>().Job; }
            else if (id == 4) { Target4.GetComponent<Player>().m_ImmobilizeStack++; id = Target4.GetComponent<Player>().Job; }
            VictoryScreen.transform.localPosition = new Vector3(0, 0, 5);
            Player.sprite = PlayerSprite[id - 1];
            Weapons.sprite = WeaponSprites[id - 1];
            Finish = true;
            FindObjectOfType<AudioManager>().Stop("BattleLoop");
            FindObjectOfType<AudioManager>().Play("Defeat");
            myCamera.orthographicSize = 10;
            transform.position = new Vector3(100, 0, -10);
            int xxx = 0;
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
            ItemList.Gems += 10;
        }
    }

    IEnumerator Follow()
    {
        //LoadPlayerLoadout(SaveFileDirectory.Player1LoadOut,Target1.GetComponent<Player>());
        //LoadPlayerLoadout(SaveFileDirectory.Player2LoadOut,Target2.GetComponent<Player>());
        //LoadPlayerLoadout(SaveFileDirectory.Player3LoadOut,Target3.GetComponent<Player>());
        //LoadPlayerLoadout(SaveFileDirectory.Player4LoadOut,Target4.GetComponent<Player>());
        if (!SaveFileDirectory.Tutorial)
        {
            mapId = Random.Range(0, 3);
            while (mapId == 1) mapId = Random.Range(0, 2);
            myCamera.orthographicSize = CameraIntialSize[mapId];
            for (int i = 1; i <= 10; i++)
            {
                GameObject p1 = Target1, p2 = Target2;
                int t1 = Random.Range(1, 5), t2 = Random.Range(1, 5);
                while (t1 == t2) t2 = Random.Range(1, 5);
                if (t1 == 2) p1 = Target2;
                if (t1 == 3) p1 = Target3;
                if (t1 == 4) p1 = Target4;
                //
                if (t2 == 1) p2 = Target1;
                if (t2 == 3) p2 = Target3;
                if (t2 == 4) p2 = Target4;
                //
                if (t1 == 1) Target1 = p2;
                if (t1 == 2) Target2 = p2;
                if (t1 == 3) Target3 = p2;
                if (t1 == 4) Target4 = p2;
                //
                if (t2 == 1) Target1 = p1;
                if (t2 == 2) Target2 = p1;
                if (t2 == 3) Target3 = p1;
                if (t2 == 4) Target4 = p1;
            }
            Target1.transform.position = P1MapPos[mapId];
            Target2.transform.position = P2MapPos[mapId];
            Target3.transform.position = P3MapPos[mapId];
            Target4.transform.position = P4MapPos[mapId];
            MaximumCameraLimitArea = CameraLimitAreaMaps[mapId];
            CameraExtension = CameraExtensionMaps[mapId];
            Maps[mapId].transform.position = new Vector3(0, 0, 0);
            yield return new WaitForSeconds(0.1f);//1

            float xChange, yChange;
            if (Target1)
            {
                xChange = Target1.transform.position.x - transform.position.x; yChange = Target1.transform.position.y - transform.position.y;
                bool SizeBool = (myCamera.orthographicSize == CameraIntialSize[mapId]);
                for (int i = 1; i <= MovementRate; i++)
                {
                    transform.position += new Vector3(xChange / MovementRate, yChange / MovementRate);
                    if (SizeBool) myCamera.orthographicSize -= (CameraIntialSize[mapId] - 3) / MovementRate;
                    yield return new WaitForSeconds(MovementInterval);
                }
                yield return new WaitForSeconds(.1f);
            }

            if (Target2)
            {
                xChange = Target2.transform.position.x - transform.position.x; yChange = Target2.transform.position.y - transform.position.y;
                bool SizeBool = (myCamera.orthographicSize == CameraIntialSize[mapId]);
                for (int i = 1; i <= MovementRate; i++)
                {
                    transform.position += new Vector3(xChange / MovementRate, yChange / MovementRate);
                    if (SizeBool) myCamera.orthographicSize -= (CameraIntialSize[mapId] - 3) / MovementRate;
                    yield return new WaitForSeconds(MovementInterval);
                }
                yield return new WaitForSeconds(.1f);
            }

            if (Target3)
            {
                xChange = Target3.transform.position.x - transform.position.x; yChange = Target3.transform.position.y - transform.position.y;
                bool SizeBool = (myCamera.orthographicSize == CameraIntialSize[mapId]);
                for (int i = 1; i <= MovementRate; i++)
                {
                    transform.position += new Vector3(xChange / MovementRate, yChange / MovementRate);
                    if (SizeBool) myCamera.orthographicSize -= (CameraIntialSize[mapId] - 3) / MovementRate;
                    yield return new WaitForSeconds(MovementInterval);
                }
                yield return new WaitForSeconds(.1f);
            }

            if (Target4)
            {
                xChange = Target4.transform.position.x - transform.position.x; yChange = Target4.transform.position.y - transform.position.y;
                bool SizeBool = (myCamera.orthographicSize == CameraIntialSize[mapId]);
                for (int i = 1; i <= MovementRate; i++)
                {
                    transform.position += new Vector3(xChange / MovementRate, yChange / MovementRate);
                    if (SizeBool) myCamera.orthographicSize -= (CameraIntialSize[mapId] - 3) / MovementRate;
                    yield return new WaitForSeconds(MovementInterval);
                }
                yield return new WaitForSeconds(.1f);
            }

            SetBorder();
            float SizeInit = -UpperLeftBorder.y + LowerRightBorder.y, BaseSize = myCamera.orthographicSize;
            if (UpperLeftBorder.x - LowerRightBorder.x > SizeInit) SizeInit = UpperLeftBorder.x - LowerRightBorder.x;
            SizeInit = SizeInit / 5 + 2;
            xChange = (UpperLeftBorder.y + LowerRightBorder.y) / 2 - transform.position.x; yChange = (UpperLeftBorder.x + LowerRightBorder.x) / 2 - transform.position.y;
            for (int i = 1; i <= MovementRate; i++)
            {
                transform.position += new Vector3(xChange / MovementRate, yChange / MovementRate);
                myCamera.orthographicSize += (SizeInit - BaseSize) / MovementRate;
                yield return new WaitForSeconds(MovementInterval);
            }
            yield return new WaitForSeconds(.1f);
        }
        if (Target1)
            Target1.GetComponent<Player>().m_ImmobilizeStack--;
        if (Target2)
            Target2.GetComponent<Player>().m_ImmobilizeStack--;
        if (Target3)
            Target3.GetComponent<Player>().m_ImmobilizeStack--;
        if (Target4)
            Target4.GetComponent<Player>().m_ImmobilizeStack--;
        if (SaveFileDirectory.Tutorial)
        {
            MaximumCameraLimitArea = CameraLimitAreaMaps[0];
            CameraExtension = CameraExtensionMaps[0];
            Maps[2].transform.position = new Vector3(0, 0, 0);
        }
        while (!Finish)
        {
            myCamera.transform.position = new Vector3((UpperLeftBorder.y + LowerRightBorder.y) / 2, (UpperLeftBorder.x + LowerRightBorder.x) / 2, -10);
            if (transform.position.x < -MaximumCameraLimitArea) transform.position = new Vector3(-MaximumCameraLimitArea, transform.position.y, -10);
            if (transform.position.x > MaximumCameraLimitArea) transform.position = new Vector3(MaximumCameraLimitArea, transform.position.y, -10);
            if (transform.position.y > MaximumCameraLimitArea) transform.position = new Vector3(transform.position.x, MaximumCameraLimitArea, -10);
            if (transform.position.y < -MaximumCameraLimitArea) transform.position = new Vector3(transform.position.x, -MaximumCameraLimitArea, -10);
            float Size = -UpperLeftBorder.y + LowerRightBorder.y;
            if (UpperLeftBorder.x - LowerRightBorder.x > Size) Size = UpperLeftBorder.x - LowerRightBorder.x;
            //Debug.Log(Size);
            Size = Size / 5 + 2;
            myCamera.orthographicSize = Size;
            yield return new WaitForSeconds(0.002f);
        }
    }

    private void SetBorder()
    {
        UpperLeftBorder = new Vector2(0, 0);
        LowerRightBorder = new Vector2(0, 0);
        if (Target1)
            if (Target1.transform.position.x < 100)
                SetBorderWithTarget(Target1);
        if (Target2) if (Target2.transform.position.x < 100) SetBorderWithTarget(Target2);
        if (Target3) if (Target3.transform.position.x < 100) SetBorderWithTarget(Target3);
        if (Target4) if (Target4.transform.position.x < 100) SetBorderWithTarget(Target4);
        //Debug.Log(UpperLeftBorder.x + " " + LowerRightBorder.x + " / x : " + UpperLeftBorder.y + " " + LowerRightBorder.y);
    }

    private void SetAlive()
    {
        if (!Target1) Target1 = null;
        else if (Target1.GetComponent<Player>().GetHp() <= 0 && Target1.GetComponent<Player>().m_Dead == true) { Target1 = null; /*Debug.Log("Player 1 Dead");*/}
        if (!Target2) Target2 = null;
        else if (Target2.GetComponent<Player>().GetHp() <= 0 && Target2.GetComponent<Player>().m_Dead == true) {Target2 = null; /*Debug.Log("Player 2 Dead");*/}
        if (!Target3) Target3 = null;
        else if (Target3.GetComponent<Player>().GetHp() <= 0 && Target3.GetComponent<Player>().m_Dead == true) {Target3 = null; /*Debug.Log("Player 3 Dead");*/}
        if (!Target4) Target4 = null;
        else if (Target4.GetComponent<Player>().GetHp() <= 0 && Target4.GetComponent<Player>().m_Dead == true) {Target4 = null; /*Debug.Log("Player 4 Dead");*/}
    }

    private void SetBorderWithTarget(GameObject Target)
    {
        if (Target.transform.position.x- CameraExtension < UpperLeftBorder.y) UpperLeftBorder.y = Target.transform.position.x- CameraExtension;
        if (Target.transform.position.y+ CameraExtension > UpperLeftBorder.x) UpperLeftBorder.x = Target.transform.position.y+ CameraExtension;
        if (Target.transform.position.x+ CameraExtension > LowerRightBorder.y) LowerRightBorder.y = Target.transform.position.x+ CameraExtension;
        if (Target.transform.position.y-CameraExtension < LowerRightBorder.x) LowerRightBorder.x = Target.transform.position.y- CameraExtension;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Demo");
    }

    public void Home()
    {
        SceneManager.LoadScene("TrueMainMenu");
    }

    public bool LoadPlayerLoadout(string Directory, Player player)
    {
        if (File.Exists(Application.persistentDataPath
                       + "/" + Directory))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/" + Directory, FileMode.Open);
            PlayerLoadout data = (PlayerLoadout)bf.Deserialize(file);
            file.Close();
            player.m_PlayerMode = data.PlayerMode; player.m_Enable = data.Enable;
            player.Priority = data.Priority;
            player.kc_Down = data.down; player.kc_Up = data.up; player.kc_Left = data.left; player.kc_Right = data.right;
            player.ac_AirSlash = data.AirSlash; player.ac_Slash = data.Slash; player.ac_Dash = data.Dash; player.ac_FuriosRush = data.FuriousRush;
            player.ac_SkyCurrent = data.SkyCurrent; player.ac_GroundSlam = data.GroundSlam; player.ac_Shoot = data.Shoot; 
            player.ac_EscapeBlast = data.EscapeBlast; player.ac_Whirlwind = data.Whirlwind; player.ac_Gateway = data.Gateway;
            player.ac_FlareBurst = data.FlareBurst; player.ac_BulletTornado = data.BulletTornado; player.ac_SnipePoint = data.SnipePoint;
            player.ac_Backstab = data.Backstab; player.ac_ShadowCloak = data.ShadowCloak; player.ac_DarkSpike = data.DarkSpike;
            player.ac_SoulSurge = data.SoulSurge; player.ac_DeathCurse = data.DeathCurse; player.ac_SuicidalLunge = data.SuicidalLunge;
            player.ac_Chant = data.Chant; player.ac_GrandTeleportation = data.GrandTeleportation; player.ac_OceanBeam = data.OceanBeam;
            player.ac_EarthenLances = data.EarthenLances; player.ac_Overload = data.Overload; player.ac_HolyRadiance = data.HolyRadiance;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void SavePlayerLoadout(string Directory, Player player)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + "/" + Directory);
        PlayerLoadout data = new();
        data.PlayerMode = player.m_PlayerMode; data.Enable = player.m_Enable;
        data.Priority = player.Priority;
        data.down = player.kc_Down; data.up = player.kc_Up; data.left = player.kc_Left; data.right = player.kc_Right;
        data.AirSlash = player.ac_AirSlash; data.Slash = player.ac_Slash; data.Dash = player.ac_Dash; data.FuriousRush = player.ac_FuriosRush;
        data.SkyCurrent = player.ac_SkyCurrent; data.GroundSlam = player.ac_GroundSlam; data.Shoot = player.ac_Shoot;
        data.EscapeBlast = player.ac_EscapeBlast; data.Whirlwind = player.ac_Whirlwind; data.Gateway = player.ac_Gateway;
        data.FlareBurst = player.ac_FlareBurst; data.BulletTornado = player.ac_BulletTornado; data.SnipePoint = player.ac_SnipePoint;
        data.Backstab = player.ac_Backstab; data.ShadowCloak = player.ac_ShadowCloak; data.DarkSpike = player.ac_DarkSpike;
        data.SoulSurge = player.ac_SoulSurge; data.DeathCurse = player.ac_DeathCurse; data.SuicidalLunge = player.ac_SuicidalLunge;
        data.Chant = player.ac_Chant; data.GrandTeleportation = player.ac_GrandTeleportation; data.OceanBeam = player.ac_OceanBeam;
        data.EarthenLances = player.ac_EarthenLances; data.Overload = player.ac_Overload; data.HolyRadiance = player.ac_HolyRadiance;
        bf.Serialize(file, data);
        file.Close();
    }
}

