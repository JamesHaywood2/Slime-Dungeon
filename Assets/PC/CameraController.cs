using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraOffset = 2.5f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float upLookOffset = 3f;
    [SerializeField] private float downLookOffset = 1f;
    private float cameraY;
    private Vector3 velocity = Vector3.zero;
    
    private GameObject player;

    private GameObject background;
    

    private void Start() {
        this.player = GameObject.Find("Player");
        transform.position = new Vector3(this.player.transform.position.x, this.player.transform.position.y + cameraOffset, transform.position.z);

        //find the Background game object which is the child of the camera.
        background = transform.GetChild(0).gameObject;
        //get the size of the camera in pixels.
        float camSizeY = Camera.main.pixelHeight;
        float camSizeX = Camera.main.pixelWidth;

        //Get the target size of the background image by taking the size of the camera and dividing it by the size of the background image.
        float targetSizeY = camSizeY / background.GetComponent<SpriteRenderer>().sprite.texture.height;
        float targetSizeX = camSizeX / background.GetComponent<SpriteRenderer>().sprite.texture.width;

        //get camera size
        float camSize = Camera.main.orthographicSize;

        Vector3 targetSize = new Vector3(targetSizeX* (1 + camSize/20f), targetSizeY*(1 + camSize/20f), 1);

        //Set the background image to the target size.
        background.transform.localScale = targetSize;
        

        

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
