using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraOffset = 2.0f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float upLookOffset = 3f;
    [SerializeField] private float downLookOffset = 1f;
    private float cameraY;
    private Vector3 velocity = Vector3.zero;
    
    private GameObject player;
    

    private void Start() {
        this.player = GameObject.Find("Player");
        transform.position = new Vector3(this.player.transform.position.x, this.player.transform.position.y + cameraOffset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = this.player.transform.position;
        cameraY = playerPos.y + cameraOffset;

        //If the player is holding the up arrow key increase the camera's y position by upLookOffset.
        if (Input.GetKey(KeyCode.UpArrow)) {
            cameraY = playerPos.y + upLookOffset;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            cameraY = playerPos.y - downLookOffset;
        } else {
            cameraY = playerPos.y + cameraOffset;
        }

        //transform.position = new Vector3(playerPos.x, cameraY, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(playerPos.x, cameraY, transform.position.z), ref velocity, smoothTime);
    }
}
