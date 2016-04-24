using UnityEngine;
using System.Collections;

public class EnemyTank : MonoBehaviour {

    public Transform Player;
    public LayerMask shootMask;

    public int DestroyReward;
    public float Speed;
    public float Health;
    public float FireRate;
    public Transform DieFX, BulletPref, ShootFX, ScoreFX, BombPref;
    public Transform LifeBarRoot, LifeBar, TankTurret;
    public Transform MuzzlePoint;
    public Vector3 LifeBarInset;

    public float minX = -5.8f, maxX = 5.8f, minY = -3.6f, maxY = 3.6f;

    private bool Turning;
    private float tunrLerp;
    private float OriHealth, OriScale;
    private float LastShot, deltaShot;
    private float timeBeforeChange;
    private Transform _myTrans;
    public Vector3 dir, target, prevDir;

    void Start() {
        _myTrans = this.transform;
        OriHealth = Health;
        OriScale = LifeBar.localScale.x;

        deltaShot = Random.Range(FireRate * 1.2f, FireRate * 2.0f) / 2.0f;
        LastShot = Time.time;

        target = new Vector3(Random.Range(minX, maxX), _myTrans.position.y, Random.Range(minY, maxY));
        dir = (target - _myTrans.position).normalized;
        timeBeforeChange = Vector3.Distance(_myTrans.position, target) / Speed;
        prevDir = _myTrans.forward;
        Turning = true;
        LifeBarRoot.position = _myTrans.position + LifeBarInset;
        LifeBarRoot.eulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
    }

    void Update() {
        if (GameMaster.Instance.IsLoosing || GameMaster.Instance.Paused)
            return;

        if (Turning) {
            tunrLerp += Time.deltaTime * 2.5f;
            _myTrans.forward = Vector3.Lerp(prevDir, dir, tunrLerp);
            LifeBarRoot.position = _myTrans.position + LifeBarInset;
            LifeBarRoot.eulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
            if (tunrLerp > 1.0f) {
                tunrLerp = 0.0f;
                Turning = false;
            }
        }
        else {
            timeBeforeChange -= Time.deltaTime;
            _myTrans.Translate(dir * Speed * Time.deltaTime, Space.World);
        }

        if (timeBeforeChange <= 0) {
            target = new Vector3(Random.Range(minX, maxX), _myTrans.position.y, Random.Range(minY, maxY));
            dir = (target - _myTrans.position).normalized;
            timeBeforeChange = Vector3.Distance(_myTrans.position, target) / Speed;
            prevDir = _myTrans.forward;
            Turning = true;
            LifeBarRoot.position = _myTrans.position + LifeBarInset;
            LifeBarRoot.eulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
        }

        if (Time.time - LastShot >= deltaShot)
            Fire();
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Crate" || other.gameObject.tag == "EnemyTank") {
            int iterations = 0;
            Vector3 contactPoint = other.contacts[0].point;

            float Angle = Vector3.Angle(Vector3.right, other.contacts[0].normal);
            if (_myTrans.position.z < contactPoint.z) Angle = 360.0f - Angle;

            Vector3 Left = new Vector3(), Right = new Vector3();
            Left.x = contactPoint.x + Mathf.Cos((Angle - 45.0f) * Mathf.Deg2Rad) * 1.2f;
            Left.z = contactPoint.z + Mathf.Sin((Angle - 45.0f) * Mathf.Deg2Rad) * 1.2f;
            Right.x = contactPoint.x + Mathf.Cos((Angle + 45.0f) * Mathf.Deg2Rad) * 1.2f;
            Right.z = contactPoint.z + Mathf.Sin((Angle + 45.0f) * Mathf.Deg2Rad) * 1.2f;

            bool OK = false;
            Vector2 newTar = new Vector2();
            while (!OK) {
                iterations++;
                if (iterations > 10) {
                    break;
                }
                newTar = Vector3.Lerp(Right, Left, Random.Range(0.0f, 1.0f));
                newTar.y = _myTrans.position.y;
                RaycastHit2D hit = Physics2D.Raycast(_myTrans.position, (Vector3)newTar - _myTrans.position, 1.2f, shootMask);
                if (hit.collider == null) {
                    OK = true;
                    iterations = 0;
                }
            }
            target = newTar;
            dir = (target - _myTrans.position).normalized;
            timeBeforeChange = Vector3.Distance(target, _myTrans.position) / Speed;
            prevDir = _myTrans.forward;
            Turning = true;
            LifeBarRoot.position = _myTrans.position + LifeBarInset;
            LifeBarRoot.eulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
        }
    }

    void Fire() {
        Vector3 dir = Player.position - _myTrans.position;
        TankTurret.forward = dir;
        //Transform _smoke = (Transform)Instantiate(ShootFX, MuzzlePoint.position, Quaternion.identity);
        //_smoke.forward = dir;

        Transform _bull = (Transform)Instantiate(BulletPref, MuzzlePoint.position, Quaternion.identity);
        _bull.LookAt(MuzzlePoint.position + dir);

        deltaShot = Random.Range(FireRate * 1.2f, FireRate * 2.0f) / 2.0f;
        LastShot = Time.time;
    }

    public void Hit(float Damage) {
        Health -= Damage;
        if (Health <= 0.0f) {
            GameMaster.Instance.ScorePoints(DestroyReward);
            Instantiate(DieFX, _myTrans.position, Quaternion.identity);
            Transform _score = (Transform)Instantiate(ScoreFX, _myTrans.position, Quaternion.identity);
            _score.eulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
            Destroy(this.gameObject);
        }
        else {
            LifeBar.localScale = new Vector3(OriScale * Health / OriHealth, LifeBar.localScale.y, LifeBar.localScale.z);
        }
    }
}
