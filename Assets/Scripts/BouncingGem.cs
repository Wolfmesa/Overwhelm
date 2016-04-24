using UnityEngine;
using System.Collections;

public class BouncingGem : MonoBehaviour {

    public float MoveSpeed, MoveAmplitude;

    private bool Up;
    private Transform _myTrans;
    private Vector3 Upvect, DownVect;

	void Start () {
        Up = true;
        _myTrans = this.transform;
        Upvect = _myTrans.position + _myTrans.up * MoveAmplitude * 0.5f;
        DownVect = _myTrans.position - _myTrans.up * MoveAmplitude * 0.5f;
	}
	
	void Update () {
        if (Up) {
            _myTrans.Translate(_myTrans.up * MoveSpeed * 0.5f * Time.deltaTime, Space.World);
            if (_myTrans.position.z > Upvect.z)
                Up = false;
        }
        else {
            _myTrans.Translate(-_myTrans.up * MoveSpeed * Time.deltaTime, Space.World);
            if (_myTrans.position.z < DownVect.z)
                Up = true;
        }
	}
}
