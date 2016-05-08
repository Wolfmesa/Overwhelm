using UnityEngine;
using System.Collections;

public class PlayerPhysX : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) { 
            case "Shield_Gem":
                GameMaster.Instance.GemPickedUp(other.transform.position);
                Destroy(other.gameObject);
                break;
            case "ObusPerf":
                PlayerMotor.Instance.PickupItem("Perforant");
                Destroy(other.gameObject);
                break;
            case "ObusGren":
                PlayerMotor.Instance.PickupItem("Grenade");
                Destroy(other.gameObject);
                break;
            case "ObusNuke":
                PlayerMotor.Instance.PickupItem("Nuke");
                Destroy(other.gameObject);
                break;
        }
    }
}
