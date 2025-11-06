using System;
using UnityEngine;

public class Follow_player : MonoBehaviour
{
    [Tooltip("Transform of the player to follow")]
    public Transform player;

    [Tooltip("Offset from the player's position")]
    public Vector3 offset = new Vector3(0, 20, 0);

    [Tooltip("Base smooth time used when player is stationary")]
    public float baseSmoothTime = 0.25f;

    [Tooltip("How much the smoothTime increases per unit of player speed")]
    public float speedToSmoothTime = 0.02f;

    [Tooltip("Minimum smooth time clamp")]
    public float minSmoothTime = 0.02f;

    [Tooltip("Maximum smooth time clamp")]
    public float maxSmoothTime = 1.0f;

    [Tooltip("Camera height above the player")] // Hated the fact i had this hardcoded and it gave no room for movement.
    public float cameraHeight = 40f;

  
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 lastPlayerPosition;

    [SerializeField] private float cameraFollowSpeed = 10f;

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Follow_player: player Transform is not assigned.");
            lastPlayerPosition = transform.position;
        }
        else
        {
            lastPlayerPosition = player.position;
            // Not keen on this, but it prevents a jarring snap on start. 
            transform.position = player.position + offset;
        }
    }
    void FixedUpdate()
    {
        if (player == null) // Mainly checking for myself. Also useful if something goes horribly wrong for the sake of debugging.
            throw new InvalidOperationException("Follow_player: player Transform is not assigned.");

        transform.position += (lastPlayerPosition - transform.position).normalized * Mathf.Clamp(cameraFollowSpeed * Time.fixedDeltaTime, 0, (lastPlayerPosition - transform.position).magnitude); // Nice and smooth with no shakiness (Is that a word?)
        transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z); 


        lastPlayerPosition = player.position;
    }
}