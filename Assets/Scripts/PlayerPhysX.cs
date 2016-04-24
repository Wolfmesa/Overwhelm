using UnityEngine;
using System.Collections;

public class PlayerPhysX : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) { 
            case "Shield_Gem":
                GameMaster.Instance.GemPickedUp(other.transform.position);
                Destroy(other.gameObject);
                break;
        }
    }
}
