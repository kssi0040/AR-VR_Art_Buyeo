using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        StartCoroutine(IDestroyMe());
    }

    IEnumerator IDestroyMe()
    {
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
