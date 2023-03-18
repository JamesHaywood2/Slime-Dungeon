using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraOffset = 2;
    

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        //this.player = GameObject.Find("Skeleton");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = this.player.transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + cameraOffset, transform.position.z);
    }
}
