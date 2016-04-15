using UnityEngine;
using System.Collections;

public class CamGizmos : MonoBehaviour {

    public Vector3 Pos, Size;

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(Pos, Size);
    }
}
