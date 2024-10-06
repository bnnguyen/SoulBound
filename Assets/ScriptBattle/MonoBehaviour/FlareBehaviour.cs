using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlareBehaviour : MonoBehaviour
{
    //Unity
    public GameObject caster, pf_FlareTargetZone, pf_FlareExplosion, pf_Ghost;
    public Vector3 u_GroundCheck;
    public float m_Limit = 120f;
    //Custom
    private Vector3 m_HitBox = Vector3.zero;
    public GameObject Ground, Zone;
    private List<GameObject> m_EnemiesHitList = new List<GameObject>();
    private bool m_Activated = false;
    private int m_Count = 0;
    // /2.4

    //System Functions

    private void Start()
    {
        GroundCheck();
        if (!Ground) Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (m_HitBox.x != 0)
        Gizmos.DrawWireCube(transform.position, m_HitBox);
    }

    private void Update()
    {
        if (!m_Activated)
        {
            m_Count++;
            if (m_Count % 10 == 0)
            {
                GameObject obj = Instantiate(pf_Ghost);
                obj.GetComponent<Expane>().Reverse = true;
                obj.GetComponent<PropelBackward>().U_Change = new Vector3(Random.Range(0.0028f, 0.0032f), Random.Range(0.0028f, 0.0032f));
                obj.transform.position = transform.position;
                obj.transform.localScale = transform.localScale;
                obj.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                obj.GetComponent<SpriteRenderer>().sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
                obj.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
            }
            m_Limit -= Time.deltaTime;
            if (m_Count > 100) m_Count = 0;
            if (m_Limit <= 0) Destroy(gameObject);
        }
    }

    public void Trigger()
    {
        m_Activated = true;
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 0);
        Zone = Instantiate(pf_FlareTargetZone);
        Zone.transform.localScale = new Vector3(0.1f, 0.1f);
        Zone.transform.position = new Vector3(transform.position.x, Ground.transform.position.y + Ground.transform.localScale.y/1.9f);
        Zone.transform.Rotate(0, 0, Ground.transform.eulerAngles.z);
        float x = Ground.transform.localScale.x / 2.4f;
        if (x > 2f) x = 2f;
        Zone.GetComponent<Expane>().U_MaxSize = new Vector3(x,1);
        StartCoroutine(Burst());
    }

    IEnumerator Burst()
    {
        float xSizeConstant = 1.3f;
        Vector3 Size = new Vector3(Zone.transform.localScale.x/xSizeConstant, Zone.transform.localScale.x/ xSizeConstant), Pos = Zone.transform.position + new Vector3(0, Size.y / 1.5f);
        float z = Zone.transform.eulerAngles.z;
        while (Zone)
        {
            Size = new Vector3(Zone.transform.localScale.x/ xSizeConstant, Zone.transform.localScale.x/ xSizeConstant); Pos = Zone.transform.position + new Vector3(0, Size.y / 1.5f);
            m_HitBox = new Vector3(Size.x*4.5f, Size.y*2.5f);
            yield return new WaitForSeconds(0.1f);
        }
        Zone = Instantiate(pf_FlareExplosion);
        Zone.transform.position = Pos;
        Zone.transform.localScale = Size;
        Zone.transform.Rotate(0, 0, z);
        lock (this)
        {
            EnemyCheck();
        }
        Destroy(gameObject);
    }

    //Custom Functions

    void GroundCheck()
    {
        if (Physics2D.OverlapBox(transform.position, u_GroundCheck, 0, LayerMask.GetMask("Ground")).gameObject) 
            Ground = Physics2D.OverlapBox(transform.position, u_GroundCheck, 0, LayerMask.GetMask("Ground")).gameObject;
    }

    void EnemyCheck()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position, m_HitBox, Zone.transform.eulerAngles.z, LayerMask.GetMask("Player"));
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
}
