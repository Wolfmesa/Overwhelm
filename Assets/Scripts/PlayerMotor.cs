using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerMotor : MonoBehaviour {

    public enum CurrentWeapon { Regular, Spud, Perforant, Nuke };
    public CurrentWeapon currentWeapon = CurrentWeapon.Regular;
    public static PlayerMotor Instance;

    public Animation PlayerBase, RegHead, GrenHead, NukeHead, BonusLayer;
    public Camera UICam, GameCam;
    public Transform PlayerTank, MuzzlePoint, Turret;
    public Transform JoystickRoot, Joystick;
    public Image BonusImg;
    public Sprite GrenImg, PerfImg, NukeImg;
    public Text RegAmount, SpudAmount, PiercingAmount, NukeAmount, BonusText;
    public Transform SelectMark, RegMark, SpudMark, PerfMark, NukeMark;
    public float PlayerSpeed;

    public Transform BulletPref, SpudPref, PiercingPref, NukePref;

    private Vector3 AdditionnalVector, dir, CurrentTouch;
    private bool OnJoystick, OnPause, CanFire;
    private int MoveStickID, PauseID;
    private Vector3[] StartPos = new Vector3[10];
    private float Distance, Angle, FireBudget;
    private int RegQty, SpudQty, PercingQty, NukeQty;

	void Start () {
        MoveStickID = -10;
        PauseID = -10;

        RegQty = 999;
        SpudQty = 0;
        RegAmount.text = RegQty.ToString("000");
        SpudAmount.text = SpudQty.ToString("00");
        Instance = this;
	}
	
	void Update () {
        if (GameMaster.Instance.Paused)
            return;

        if (Input.GetButtonDown("Fire1")) {
            if(currentWeapon == CurrentWeapon.Spud)
                Fire(GameCam.WorldToScreenPoint(MuzzlePoint.position + MuzzlePoint.forward * 5.0f));
            else
                Fire(GameCam.WorldToScreenPoint(MuzzlePoint.position + MuzzlePoint.right * -5.0f));
        }
        if (Input.GetButtonDown("Weapon1"))
            ChangeWeapon("Regular");
        if (Input.GetButtonDown("Weapon2"))
            ChangeWeapon("Spud");
        if (Input.GetButtonDown("Weapon3"))
            ChangeWeapon("Perforant");
        if (Input.GetButtonDown("Weapon4"))
            ChangeWeapon("Nuke");

        if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f) {
            Vector3 movdir = new Vector3();
            movdir.x = Input.GetAxis("Horizontal");
            movdir.z = Input.GetAxis("Vertical") * -1.0f;
            movdir.Normalize();

            PlayerTank.Translate(movdir * Time.deltaTime * PlayerSpeed, Space.World);
            PlayerTank.LookAt(PlayerTank.position + movdir);
        }
        if (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f) {
            Vector3 lookdir = new Vector3();
            lookdir.x = Input.GetAxis("Mouse X");
            lookdir.z = Input.GetAxis("Mouse Y");
            lookdir.Normalize();

            Vector3 point = PlayerTank.position + lookdir;
            point.y = MuzzlePoint.position.y;
            Turret.LookAt(Turret.position + point);
            if (currentWeapon == CurrentWeapon.Spud) {
                Turret.Rotate(Vector3.up, -90.0f);
                Turret.localEulerAngles = new Vector3(270.0f, Turret.localEulerAngles.y, Turret.localEulerAngles.z);
            }
        }

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

                        PlayerBase.Play("Start");
                        PlayerBase.PlayQueued("Idle");

                        switch (currentWeapon) { 
                            case CurrentWeapon.Regular:
                                RegHead.Play("Start");
                                RegHead.PlayQueued("Idle");
                                break;
                            case CurrentWeapon.Spud:
                                //GrenHead.Play("Start");
                                //GrenHead.PlayQueued("Idle");
                                break;
                        }

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
                        PlayerBase.Play("Stop");
                        PlayerBase.PlayQueued("Idle");
                        RegHead.Play("Stop");
                        RegHead.PlayQueued("Idle");

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
                Turret.Rotate(Vector3.up, 90.0f);

                switch (currentWeapon) {
                    case CurrentWeapon.Regular:
                        RegQty--;
                        RegAmount.text = RegQty.ToString("000");
                        Transform _bullet = (Transform)Instantiate(BulletPref, MuzzlePoint.position, Quaternion.identity);
                        _bullet.LookAt(hitPoint);

                        RegHead.Play("Shoot");
                        RegHead.PlayQueued("Idle");
                        PlayerBase.Play("Shoot");
                        PlayerBase.PlayQueued("Idle");
                        break;
                    case CurrentWeapon.Spud:
                        Turret.Rotate(Vector3.up, -90.0f);
                        Turret.localEulerAngles = new Vector3(270.0f, Turret.localEulerAngles.y, Turret.localEulerAngles.z);

                        SpudQty--;
                        SpudAmount.text = SpudQty.ToString("000");
                        Transform _spud = (Transform)Instantiate(SpudPref, MuzzlePoint.position, Quaternion.identity);
                        float Distance = Vector3.Distance(PlayerTank.position, hit.point);

                        //Equation de velocité
                        //La velocité de la grenade est directement proportionnelle au carré de la distance séparant
                        //le point d'impact du tank, et à la demi-intensité de pesanteur
                        float Velocity = Mathf.Sqrt(((4.905f * Mathf.Pow(Distance, 2)) / Distance + 2.231f) / (Mathf.Pow(Mathf.Cos(45.0f * Mathf.Deg2Rad), 2)));

                        _spud.GetComponent<SpudHandler>().TargetPos = hit.point;
                        _spud.GetComponent<SpudHandler>().Velocity = Velocity;

                        if (SpudQty <= 0)
                            ResetWeapon();

                        //GrenHead.Play("Shoot");
                        //GrenHead.PlayQueued("Idle");
                        PlayerBase.Play("Shoot");
                        PlayerBase.PlayQueued("Idle");
                        break;
                    case CurrentWeapon.Perforant:
                        PercingQty--;
                        PiercingAmount.text = PercingQty.ToString("000");
                        Transform _perf = (Transform)Instantiate(PiercingPref, MuzzlePoint.position, Quaternion.identity);
                        _perf.LookAt(hitPoint);

                        if (PercingQty <= 0)
                            ResetWeapon();

                        RegHead.Play("Shoot");
                        RegHead.PlayQueued("Idle");
                        PlayerBase.Play("Shoot");
                        PlayerBase.PlayQueued("Idle");
                        break;
                }

                FireBudget -= 0.6f;
            }
        }
    }

    void ResetWeapon() {
        currentWeapon = CurrentWeapon.Regular;
        SelectMark.position = RegMark.position;

        RegHead.gameObject.SetActive(true);
        GrenHead.gameObject.SetActive(false);

        RegHead.Play("Idle");
        Turret = RegHead.transform.Find("Os_TankJ_TeteMaitre/Os_TankJ_TeteMoteurB/Os_TankJ_TeteRotation");
        MuzzlePoint = Turret.GetChild(0);
    }

    public void ChangeWeapon(string WeapID) {
        switch (WeapID) { 
            case "Regular":
                SelectMark.position = RegMark.position;
                currentWeapon = CurrentWeapon.Regular;

                RegHead.gameObject.SetActive(true);
                GrenHead.gameObject.SetActive(false);
                NukeHead.gameObject.SetActive(false);

                RegHead.Play("Idle");
                Turret = RegHead.transform.Find("Os_TankJ_TeteMaitre/Os_TankJ_TeteMoteurB/Os_TankJ_TeteRotation");
                MuzzlePoint = Turret.GetChild(0);
                break;
            case "Spud":
                if (SpudQty > 0) {
                    SelectMark.position = SpudMark.position;
                    currentWeapon = CurrentWeapon.Spud;

                    RegHead.gameObject.SetActive(false);
                    GrenHead.gameObject.SetActive(true);
                    NukeHead.gameObject.SetActive(false);
                    
                    //temporary
                    Turret = GrenHead.transform;
                    MuzzlePoint = Turret.GetChild(0);
                    //Turret = GrenHead.transform.Find("Os_TankJ_TeteMaitre/Os_TankJ_TeteMoteurB/Os_TankJ_TeteRotation");
                    //GrenHead.Play("Idle");
                }
                break;
            case "Perforant":
                SelectMark.position = PerfMark.position;
                currentWeapon = CurrentWeapon.Perforant;

                RegHead.gameObject.SetActive(true);
                GrenHead.gameObject.SetActive(false);
                NukeHead.gameObject.SetActive(false);

                RegHead.Play("Idle");
                Turret = RegHead.transform.Find("Os_TankJ_TeteMaitre/Os_TankJ_TeteMoteurB/Os_TankJ_TeteRotation");
                MuzzlePoint = Turret.GetChild(0);
                break;
        }
    }

    public void PickupItem(string ID) {
        if (ID == "Grenade") {
            BonusImg.sprite = GrenImg;
            BonusText.text = "You picked up 5 grenades!";
            BonusLayer.Play("Trigger");
            SpudQty = Mathf.Clamp(SpudQty + 5, 0, 10);
            SpudAmount.text = SpudQty.ToString("00");
        }
        else if (ID == "Perforant") {
            BonusImg.sprite = PerfImg;
            BonusText.text = "You picked up 5 piercing warheads!";
            BonusLayer.Play("Trigger");
            PercingQty = Mathf.Clamp(PercingQty + 10, 0, 20);
            PiercingAmount.text = PercingQty.ToString("00");
        }
        else if (ID == "Nuke") {
            BonusImg.sprite = NukeImg;
            BonusText.text = "Holy Shit! You picked up the Nuke!!!";
            BonusLayer.Play("Trigger");
            NukeQty = 1;
            NukeAmount.text = "1";
        }
    }
}
