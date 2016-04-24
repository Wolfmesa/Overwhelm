using UnityEngine;
using System.Collections;

public class SpudHandler : MonoBehaviour {

    public Transform ExplodeFX;
    public Vector3 TargetPos;
    public float Speed;
    public float TriggerDist;
    public float ExplosionRadius;
    public float Damage;

    private Transform _myTrans;
    private float LifeTime;

	void Start () {
        _myTrans = this.transform;
        LifeTime = Vector3.Distance(TargetPos, _myTrans.position) / Speed;
	}
	
	void Update () {
        LifeTime -= Time.deltaTime;
        _myTrans.position = Vector3.Lerp(_myTrans.position, TargetPos, Time.deltaTime * Speed);
        if (Vector3.Distance(_myTrans.position, TargetPos) <= TriggerDist || LifeTime <= 0.0f) {
            Instantiate(ExplodeFX, _myTrans.position, Quaternion.identity);
            Collider[] cols = Physics.OverlapSphere(_myTrans.position, ExplosionRadius);
            foreach (Collider _col in cols) {
                if (_col.gameObject.tag == "EnemyTank")
                    _col.GetComponent<EnemyTank>().Hit(Damage);
            }
            Destroy(this.gameObject);
        }
	}
}
