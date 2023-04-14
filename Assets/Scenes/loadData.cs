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
        

}
