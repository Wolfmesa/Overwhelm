using UnityEngine;
using System.Collections;

public class BulletHandler : MonoBehaviour {

    public bool IsFriend;
    public float Speed;
    public float TriggerDist;
    public float Damage;
    public Transform ExplodeFX;

    private Transform _myTrans;

	void Start () {
        _myTrans = this.transform;
	}
	
	void Update () {
        _myTrans.Translate(_myTrans.forward * Speed * Time.deltaTime, Space.World);

        Ray ray = new Ray(_myTrans.position, _myTrans.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, TriggerDist)) {
            switch (hit.transform.gameObject.tag) { 
                case "Wall":
                    Instantiate(ExplodeFX, _myTrans.position, Quaternion.identity);
                    Destroy(this.gameObject);
                    break;
                case "Player":
                    if (!IsFriend) {
                        GameMaster.Instance.PlayerHit();

                        Instantiate(ExplodeFX, _myTrans.position, Quaternion.identity);
                        Destroy(this.gameObject);
                    }
                    break;
                case "EnemyTank":
                    if (IsFriend) {
                        hit.transform.GetComponent<EnemyTank>().Hit(Damage);

                        Instantiate(ExplodeFX, _myTrans.position, Quaternion.identity);
                        Destroy(this.gameObject);
                    }
                    break;
            }
        }
	}
}
