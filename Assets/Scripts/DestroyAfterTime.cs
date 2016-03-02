using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {
    public float time;
	// Use this for initialization
	void Start () {
        StartCoroutine(deathClock());
	}
    IEnumerator deathClock()
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

}
