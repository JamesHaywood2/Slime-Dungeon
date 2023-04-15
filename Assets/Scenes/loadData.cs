using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loadData : MonoBehaviour
{
    string loadFrom;

    private void Awake() {
        loadFrom = GetComponentInParent<LoadZoneInfo>().Destination;
        gameObject.name = "load_from_"+loadFrom;
    }

    //Draw a gizmo to show where the loadspot is.
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.9f, 2.0f,0f));



    }
        

}
