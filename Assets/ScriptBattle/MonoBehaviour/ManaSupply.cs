using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSupply : MonoBehaviour
{
    public GameObject Riptide;

    private void Start()
    {

        StartCoroutine(CauseRiptide());
    }

    IEnumerator CauseRiptide()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            lock (this)
            {
                GameObject obj = Instantiate(Riptide);
                obj.transform.position = transform.position;
                obj.transform.eulerAngles = transform.eulerAngles;
                obj.transform.SetParent(transform.parent.transform);
            }
        }
    }
}
