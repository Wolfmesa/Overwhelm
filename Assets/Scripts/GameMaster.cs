using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;
    public Transform PlayerTank;
    public bool Paused;
    public Transform CanvasRoot;
    public Transform TankPosesRoot, GemPosesRoot, BonusPosesRoot;
    public Transform[] TanksPrefs;
    public Transform[] BonusPrefs;
    public Text ScoreTxt;
    public Image[] Hearts, Shields;
    public Animation PauseLayer, GameOverLayer;
    public float MinTank, MaxTank;
    public float MinBonus, MaxBonus;

    public Sprite HeartFull, HeartEmpty, ShieldFull, ShieldEmpty;
    public Transform HeartLostFX, ShieldLostFX, ShieldGem, ShieldFX;

    private int Score;
    private int Lives, BackupLives, Armors, BackupArmors;
    private float Delay, ShieldDelay, deltaTank, deltaBonus;
    private bool IsPausing;
    [HideInInspector] public bool IsLoosing;

	void Start () {
        Instance = this;
        Lives = BackupLives = PlayerPrefs.GetInt("Loaded_Lives", 5);
        Armors = BackupArmors = PlayerPrefs.GetInt("Loaded_Armors", 4);
        ShieldDelay = Random.Range(15.0f, 30.0f);
        deltaTank = Random.Range(2, 4);
        deltaBonus = Random.Range(MinBonus, MaxBonus);

        for (int i = BackupLives; i < Hearts.Length; i++)
            Hearts[i].enabled = false;
        for (int i = BackupArmors; i < Shields.Length; i++)
            Shields[i].enabled = false;
	}
	
	void Update () {
        ShieldDelay -= Time.deltaTime;
        deltaTank -= Time.deltaTime;
        deltaBonus -= Time.deltaTime;
        if (ShieldDelay <= 0.0f) {
            ShieldDelay = Random.Range(15.0f, 30.0f);
            Vector3 pos = GemPosesRoot.GetChild(Random.Range(0, GemPosesRoot.childCount)).position;
            Transform _sg = (Transform)Instantiate(ShieldGem, pos, Quaternion.identity);
            _sg.Rotate(Vector3.right, 55.0f);
        }
        if (deltaTank <= 0.0f) {
            deltaTank = Random.Range(MinTank, MaxTank);
            Vector3 pos = TankPosesRoot.GetChild(Random.Range(0, TankPosesRoot.childCount)).position;
            Transform _tk = (Transform)Instantiate(TanksPrefs[Random.Range(0, TanksPrefs.Length)], pos, Quaternion.identity);
            _tk.GetComponent<EnemyTank>().Player = PlayerTank;
        }
        if (deltaBonus <= 0.0f) {
            deltaBonus = Random.Range(MinBonus, MaxBonus);
            Vector3 pos = BonusPosesRoot.GetChild(Random.Range(0, BonusPosesRoot.childCount)).position;
            Transform bonus = null;

            int Choice = Random.Range(0, 100);
            if (Choice % 2 == 0)
                bonus = (Transform)Instantiate(BonusPrefs[0], pos, Quaternion.identity); //Créer une caisse d'obus perçant avec 50% de chance
            else if(Choice % 5 == 0)
                bonus = (Transform)Instantiate(BonusPrefs[1], pos, Quaternion.identity); //Créer une caisse d'obus grenade avec 20% de chance
            else if(Choice % 29 == 0)
                bonus = (Transform)Instantiate(BonusPrefs[2], pos, Quaternion.identity); //Créer une caisse d'obus nucléaire avec 3% de chance

            bonus.Rotate(Vector3.right, -90.0f);
            //Si aucune de ces conditions n'est respectée, aucun bonus n'appaîtra à l'écran
        }

        if (IsPausing) {
            Delay -= Time.deltaTime;
            if (Delay <= 0.0f) { 
                Paused = true;
                IsPausing = false;
                Time.timeScale = 0.0f; 
            }
        }
        if (IsLoosing) {
            Delay -= Time.deltaTime;
            if (Delay <= 0.0f) {
                Paused = true;
                IsLoosing = false;
                Time.timeScale = 0.0f;
            }
        }
	}

    void Loose() {
        Debug.Log("Lost!");
        GameOverLayer.Play("Appear");
        IsLoosing = true;
        Delay = 0.5f;

        if (Score > PlayerPrefs.GetInt("Highscore", 0))
            PlayerPrefs.SetInt("Highscore", Score);

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject _bull in bullets)
            Destroy(_bull);
    }

    public void PlayerHit() {
        if (Armors > 0) {
            Armors--;
            Shields[Armors].sprite = ShieldEmpty;

            Transform _sd = (Transform)Instantiate(ShieldLostFX, Shields[Armors].rectTransform.position, Quaternion.identity);
            _sd.SetParent(CanvasRoot, true);
        }
        else {
            Lives--;
            Transform _ht = (Transform)Instantiate(HeartLostFX, Hearts[Lives].rectTransform.position, Quaternion.identity);
            _ht.SetParent(CanvasRoot, true);
            for (int i = Lives; i < BackupLives; i++)
                Hearts[i].sprite = HeartEmpty;

            if (Lives == 0)
                Loose();
        }
    }

    public void ScorePoints(int Reward) {
        Score += Reward;
        ScoreTxt.text = Score.ToString("000");
    }

    public void GemPickedUp(Vector3 gemPos) {
        Instantiate(ShieldFX, gemPos, Quaternion.identity);
        Armors = Mathf.Clamp(Armors + 1, 0, BackupArmors);
        Shields[Armors - 1].sprite = ShieldFull;
    }

    public void Pause() {
        Delay = 0.5f;
        IsPausing = true;
        PauseLayer.Play("Appear");
    }

    public void UnPause() {
        Paused = false;
        Time.timeScale = 1.0f;
        PauseLayer.Play("Disappear");
    }

    public void Replay() {
        GameOverLayer.Play("Disappear");
        Paused = false;
        Score = 0;
        ScoreTxt.text = "000";
        Time.timeScale = 1.0f;
        deltaTank = Random.Range(2.0f, 4.0f);

        Lives = BackupLives;
        Armors = BackupArmors;
        for (int i = 0; i < BackupLives; i++)
            Hearts[i].enabled = false;
        for (int i = 0; i < BackupArmors; i++)
            Shields[i].enabled = false;

        GameObject[] _enemies = GameObject.FindGameObjectsWithTag("EnemyTank");
        foreach (GameObject _en in _enemies)
            Destroy(_en);
    }

    public void Quit() {
        Application.Quit();
    }
}
