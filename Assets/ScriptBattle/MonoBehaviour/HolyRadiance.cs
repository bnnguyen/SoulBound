using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyRadiance : MonoBehaviour
{
    //Unity
    public Vector3 k_HitBoxSizeOn, k_HitBoxOffsetOn;
    public GameObject pf_HolyExplosion;
    //Object
    public GameObject caster;
    private List<GameObject> m_EnemiesHitList = new(), m_EnemiesSlowList = new();
    public int m_Repeat;
    public float m_WaitTime;

    private void Start()
    {
        StartCoroutine(CreateExplosion());
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn);
    }

    void CheckEnemy()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(k_HitBoxOffsetOn + transform.position, k_HitBoxSizeOn, 0, LayerMask.GetMask("Player"));
        foreach (Collider2D Enemy in EnemyHit)
        {
            if (!Enemy) continue; 
            if (Enemy.gameObject == caster) continue;
            if (m_EnemiesSlowList.Contains(Enemy.gameObject)) continue;

            StartCoroutine(SlowEnemy(Enemy.GetComponent<Player>()));
            if (!Enemy.GetComponent<Player>().m_Immunity) m_EnemiesSlowList.Add(Enemy.gameObject);

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

    IEnumerator SlowEnemy(Player Enemy)
    {
        Enemy.m_MovementSpeed *= 0.25f;
        Enemy.m_JumpForce *= 0.5f;
        yield return new WaitForSeconds(.5f);
        Enemy.m_MovementSpeed /= 0.25f;
        Enemy.m_JumpForce /= 0.5f;
    }

    IEnumerator CreateExplosion()
    {
        while (m_Repeat > 0)
        {
            m_Repeat--;
            yield return new WaitForSeconds(m_WaitTime);
            lock (this)
            {
                GameObject obj = Instantiate(pf_HolyExplosion);
                obj.transform.position = transform.position;
                m_EnemiesSlowList.Clear();
                CheckEnemy();
            }
        }
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
