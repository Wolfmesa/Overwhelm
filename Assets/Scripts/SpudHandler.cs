using UnityEngine;
using System.Collections;

public class SpudHandler : MonoBehaviour {

    public Transform ExplodeFX;
    public Vector3 TargetPos;
    public float Velocity;
    public float ExplosionRadius;
    public float Damage;
    public float Speed;

    private Vector3 StartPos, dir;
    private Transform _myTrans;
    private float LifeTime;
    private float BackupLifeTime;

	void Start () {
        _myTrans = this.transform;
        StartPos = _myTrans.position;
        LifeTime = 5.0f;
        BackupLifeTime = LifeTime;

        //La valeur 'Velocity' est fournie par le script 'PlayerMotor' au moment du tir
        //mais doit être réduite de 10% pour compenser l'inclinaison de la camera qui décale les Raycast
        Velocity *= 0.9f;

        dir = (TargetPos - _myTrans.position).normalized;
        dir.y = 0.0f;
	}
	
	void Update () {
        LifeTime -= Time.deltaTime * Speed;
        float Height = -4.905f * Mathf.Pow((BackupLifeTime - LifeTime), 2) + Mathf.Cos(45.0f * Mathf.Deg2Rad) * Velocity * (BackupLifeTime - LifeTime) + 2.231f;
        Vector3 newPos = StartPos + dir * Velocity * Mathf.Cos(45.0f * Mathf.Deg2Rad) * (BackupLifeTime - LifeTime);
        newPos.y = Height;

        _myTrans.position = newPos;

        if (_myTrans.position.y <= 0.7f || LifeTime <= 0) {
            Collider[] _cols = Physics.OverlapSphere(_myTrans.position, ExplosionRadius);
            foreach (Collider col in _cols) {
                if (col.gameObject.tag == "EnemyTank") {
                    col.GetComponent<EnemyTank>().Hit(25.0f);
                }
            }

            Instantiate(ExplodeFX, _myTrans.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
	}
}
