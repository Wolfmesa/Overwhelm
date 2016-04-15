using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMaster : MonoBehaviour {

    public static GameMaster Instance;
    public bool Paused;
    public Transform CanvasRoot;
    public Text ScoreTxt;
    public Image[] Hearts;
    public Animation PauseLayer;

    public Sprite HeartFull, HeartEmpty;
    public Transform HeartLostFX;

    private int Score;
    private int Lives, BackupLives;
    private float Delay;
    private bool IsPausing;

	void Start () {
        Instance = this;
        Lives = BackupLives = 3;
	}
	
	void Update () {
        if (IsPausing) {
            Delay -= Time.deltaTime;
            if (Delay <= 0.0f) { 
                Paused = true;
                IsPausing = false;
                Time.timeScale = 0.0f; 
            }
        }
	}

    public void PlayerHit() {
        Lives--;
        Transform _ht = (Transform)Instantiate(HeartLostFX, Hearts[Lives].rectTransform.position, Quaternion.identity);
        _ht.SetParent(CanvasRoot, true);
        for (int i = Lives; i < BackupLives; i++)
            Hearts[i].sprite = HeartEmpty;

        if (Lives == 0)
            Debug.Log("Lost!");
    }

    public void ScorePoints(int Reward) {
        Score += Reward;
        ScoreTxt.text = Score.ToString("000");
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

    public void Quit() {
        Application.Quit();
    }
}
