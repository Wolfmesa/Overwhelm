using UnityEngine;
using System.Collections;

public class BoxHandler : MonoBehaviour {

    public float GrowUpSpeed;
    public float LifeTime;

    private float LerpAmount;
    private Transform _myTrans;

	void Start () {
        _myTrans = this.transform;
	}
	
	void Update () {
        LerpAmount += Time.deltaTime * GrowUpSpeed;
        _myTrans.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, LerpAmount - 0.2f);

        if (LerpAmount >= LifeTime)
            Destroy(_myTrans.parent.gameObject);
	}
}
