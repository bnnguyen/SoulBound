using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class SaveFileDirectory : MonoBehaviour
{
    public static string Player1LoadOut = "Player1LoadOut.dat", Player2LoadOut = "Player2LoadOut.dat", 
        Player3LoadOut = "Player3LoadOut.dat", Player4LoadOut = "Player4LoadOut.dat";
    public static bool Tutorial = false;
}

[System.Serializable]
public class PlayerLoadout
{
    public bool PlayerMode, Enable;
    public int[] Priority = new int[4];
    public KeyCode left, down, right, up;
    public bool[] Slash = new bool[4], Dash = new bool[4], FuriousRush = new bool[4], Whirlwind = new bool[4], SkyCurrent = new bool[4],
        AirSlash = new bool[4], GroundSlam = new bool[4], Shoot = new bool[4], Gateway = new bool[4], FlareBurst = new bool[4],
        BulletTornado = new bool[4], EscapeBlast = new bool[4], SnipePoint = new bool[4], Backstab = new bool[4], ShadowCloak = new bool[4],
        DarkSpike = new bool[4], SoulSurge = new bool[4], DeathCurse = new bool[4], SuicidalLunge = new bool[4], Chant = new bool[4],
        GrandTeleportation = new bool[4], OceanBeam = new bool[4], EarthenLances = new bool[4], Overload = new bool[4], HolyRadiance = new bool[4];
}