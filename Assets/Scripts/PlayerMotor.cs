using UnityEngine;
using System.Collections;

public class PlayerMotor : MonoBehaviour {

    public Camera UICam, GameCam;
    public Transform PlayerTank, MuzzlePoint, Turret;
    public Transform JoystickRoot, Joystick;
    public float PlayerSpeed;

    public Transform BulletPref;

    private Vector3 AdditionnalVector, dir, CurrentTouch;
    private bool OnJoystick, OnPause, CanFire;
    private int MoveStickID, PauseID;
    private Vector3[] StartPos = new Vector3[10];
    private float Distance, Angle, FireBudget;

	void Start () {
        MoveStickID = -10;
	}
	
	void Update () {
        if (GameMaster.Instance.Paused)
            return;

        if (FireBudget < 2.0f)
            FireBudget += Time.deltaTime;

        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    Ray ray = UICam.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 10.0f)) {
                        if (hit.transform.gameObject.tag == "VirtualPause") {
                            OnPause = true;
                            PauseID = touch.fingerId;
                        }
                    }
                    else {
                        StartPos[touch.fingerId] = touch.position;
                        CanFire = true;
                    }
                }
                else if (touch.phase == TouchPhase.Moved && !OnJoystick) {
                    if (Vector2.Distance(StartPos[touch.fingerId], touch.position) > 10.0f) {
                        CanFire = false;
                        OnJoystick = true;
                        MoveStickID = touch.fingerId;

                        JoystickRoot.GetComponent<SpriteRenderer>().enabled = true;
                        Joystick.GetComponent<SpriteRenderer>().enabled = true;
                        JoystickRoot.position = UICam.ScreenToWorldPoint(touch.position) + Vector3.forward;
                    }
                }
                else if (OnJoystick) {
                    if (MoveStickID == touch.fingerId) {
                        CurrentTouch = UICam.ScreenToWorldPoint(touch.position);
                        CurrentTouch.z += 1.0f;

                        Distance = Mathf.Clamp(Vector3.Distance(JoystickRoot.position, CurrentTouch), 0.0f, 0.9f);
                        Angle = Vector3.Angle(Vector3.right, CurrentTouch - JoystickRoot.position);

                        if (CurrentTouch.y < JoystickRoot.position.y)
                            Angle = 180.0f + (180.0f - Angle);

                        AdditionnalVector.x = Mathf.Cos(Angle * Mathf.Deg2Rad) * Distance;
                        AdditionnalVector.y = Mathf.Sin(Angle * Mathf.Deg2Rad) * Distance;

                        Joystick.position = JoystickRoot.position + AdditionnalVector;
                        dir = new Vector3(AdditionnalVector.x, 0.0f, AdditionnalVector.y).normalized;

                        Vector3 pos = PlayerTank.position;
                        PlayerTank.Translate(dir * Time.deltaTime * PlayerSpeed, Space.World);
                        PlayerTank.LookAt(PlayerTank.position + dir);
                    }
                }
                if (touch.phase == TouchPhase.Ended) {
                    if (MoveStickID == touch.fingerId) {
                        JoystickRoot.GetComponent<SpriteRenderer>().enabled = false;
                        Joystick.GetComponent<SpriteRenderer>().enabled = false;
                        Joystick.position = JoystickRoot.position;
                        OnJoystick = false;
                        MoveStickID = -10;
                    }
                    else if (PauseID == touch.fingerId) {
                        Ray ray = UICam.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 10.0f)) {
                            if (hit.transform.gameObject.tag == "VirtualPause") {
                                OnPause = false;
                                PauseID = -10;
                                JoystickRoot.GetComponent<SpriteRenderer>().enabled = false;
                                Joystick.GetComponent<SpriteRenderer>().enabled = false;
                                Joystick.position = JoystickRoot.position;
                                OnJoystick = false;
                                MoveStickID = -10;
                                GameMaster.Instance.Pause();
                            }
                        }
                    }
                    else if (CanFire) {
                        Fire(touch.position);
                        CanFire = false;
                    }
                }
            }
        }
	}

    void Fire(Vector3 touchPos) {
        if (FireBudget < 0.0f) return;

        Ray ray = GameCam.ScreenPointToRay(touchPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            if (hit.transform.gameObject.tag == "Floor") { //The player touched an existing part of the world
                Vector3 hitPoint = hit.point;
                hitPoint.y = MuzzlePoint.position.y;
                Turret.LookAt(hitPoint);

                Transform _bullet = (Transform)Instantiate(BulletPref, MuzzlePoint.position, Quaternion.identity);
                _bullet.LookAt(hitPoint);

                FireBudget -= 0.6f;
            }
        }
    }
}
