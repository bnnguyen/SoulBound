using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DarkSpikeBehaviour : MonoBehaviour
{
    public float WaitTime = 0.5f, multiplier = 1;
    public bool m_RightDirection = false;
    public GameObject caster, pf_DarkSpike;
    public Sprite spSpike1, spSpike2, spSpike3;

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, 1);
        lock (this)
        {
            if (!CheckGround())
            {
                Destroy(gameObject);
            } else
            {
                if (!caster) Destroy(gameObject);
                FindObjectOfType<AudioManager>().Play("DarkSpike");
                caster.GetComponent<Player>().StartCoroutine(SpawnSpike(transform.position,m_RightDirection,caster,multiplier));
                int i = Random.Range(1, 4);
                GetComponent<SpriteRenderer>().sprite = spSpike1;
                if (i == 2) GetComponent<SpriteRenderer>().sprite = spSpike2;
                if (i == 3) GetComponent<SpriteRenderer>().sprite = spSpike3;
            }
        }
        CheckHit();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector2(0.25f, 0.35f) * multiplier);
        //Gizmos.DrawWireCube(transform.position + new Vector3(0, 1.5f), new Vector2(1f, 3f) * multiplier);
    }

    bool CheckGround()
    {
        return Physics2D.OverlapBox(transform.position, new Vector2(0.25f, 0.35f)*multiplier, 0, LayerMask.GetMask("Ground"));
    }

    void CheckHit()
    {
        Collider2D[] EnemyHit = Physics2D.OverlapBoxAll(transform.position + new Vector3(0,1.5f), new Vector2(1f, 3)*multiplier, 0, LayerMask.GetMask("Player"));
        foreach (Collider2D Enemy in EnemyHit)
        {
            if (Enemy.gameObject == caster) continue;
            if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().OnHit.Invoke();
            if (multiplier == 2)
            {
                if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().StartCoroutine(Enemy.GetComponent<Player>().WhenHitAfterTime(0.25f));
                if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
            }
            if (!caster.GetComponent<Player>().m_OutsideSoulGain)
            {
                if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().StartCoroutine(caster.GetComponent<Player>().CreateSouls(0.35f, 1, SoulName.Fight));
                if (multiplier == 2)
                {
                    if (!Enemy.GetComponent<Player>().m_Immunity) Enemy.GetComponent<Player>().InflictStatusEffect(StatusEffect.Immobilized, 1.5f);
                }
                if (!Enemy.GetComponent<Player>().m_Immunity) caster.GetComponent<Player>().m_OutsideSoulGain = true;
            }
        }
    }

    IEnumerator SpawnSpike(Vector3 pos, bool right, GameObject cast, float multi)
    {
        yield return new WaitForSeconds(WaitTime);
        GameObject obj = Instantiate(cast.GetComponent<Player>().pf_DarkSpike);
        obj.transform.position = pos;
        obj.transform.localScale *= multi;
        obj.transform.position += new Vector3(m_RightDirection ? 1f + ((multi==2)? 1f : 0f) : -1f - ((multi == 2) ? 1f : 0f), 0);
        obj.GetComponent<DarkSpikeBehaviour>().m_RightDirection = right;
        obj.GetComponent<DarkSpikeBehaviour>().caster = cast;
        obj.GetComponent<DarkSpikeBehaviour>().multiplier = multi;
    }
}
