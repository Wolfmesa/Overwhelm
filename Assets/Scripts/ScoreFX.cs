using UnityEngine;
using System.Collections;

public class ScoreFX : MonoBehaviour {

    public float Speed, Duration;
    public SpriteRenderer _mySprite;
    public Transform _myTrans;
    public Color CristalColor;

    private float LerpAmount;

	void Update () {
        _myTrans.Translate(_myTrans.up * Speed * Time.deltaTime, Space.World);

        LerpAmount += Time.deltaTime / Duration;
        _mySprite.color = Color.Lerp(Color.white, CristalColor, LerpAmount);
        if (LerpAmount >= 1.0f)
            Destroy(this.gameObject);
	}
}
