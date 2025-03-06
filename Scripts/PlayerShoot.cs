using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public Transform bulletSpawn; // Reference to the bullet spawn point
    public float fireRate = 0.5f; // Interval between shots in seconds
    private float nextFireTime = 0f; // Time when the player can fire next

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the left mouse button is pressed and if the cooldown period has passed
        if (Input.GetMouseButton(0) && Time.time > nextFireTime)
        {
            // Instantiate the bullet at the bullet spawn position and rotation
            Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            // Set the next fire time
            nextFireTime = Time.time + fireRate;
        }
    }
}
