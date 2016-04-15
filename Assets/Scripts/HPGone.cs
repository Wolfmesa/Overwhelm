using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HPGone : MonoBehaviour {

    public Transform _myTrans;
    public Image _myImg;
    public Color CristalColor;
    public Vector3 TargetScale;
    private float LerpAmount;

	void Update () {
        LerpAmount += Time.deltaTime * 2.0f;
        _myTrans.localScale = Vector3.Lerp(Vector3.one, TargetScale, LerpAmount);
        _myImg.color = Color.Lerp(Color.white, CristalColor, LerpAmount);

        if (LerpAmount >= 1.0f)
            Destroy(this.gameObject);
	}
}
