using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    // Rotation speed
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float collisionRadius = 5f;
    private Vector3 targetPosition;
    private bool is_coner = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // Camera horizontal movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // using a ray cast to handle collision
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, collisionRadius, transform.forward, out hit, moveSpeed * Time.deltaTime))
            {
                // a shorter movement is possible (otherwise, it is too close to the wall)
                if (hit.distance > collisionRadius)
                {
                    targetPosition = hit.point - transform.forward * collisionRadius;
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                }
                is_coner = true;

            }
            else 
            {
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                is_coner = false;
            }
                
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // using a ray cast to handle collision
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, collisionRadius, -transform.forward, out hit, moveSpeed * Time.deltaTime))
            {
                // a shorter movement is possible (otherwise, it is too close to the wall)
                if (hit.distance > collisionRadius)
                {
                    targetPosition = hit.point + transform.forward * collisionRadius;
                    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
                }
                is_coner = true;
            }
            else 
            {
                transform.position -= transform.forward * moveSpeed * Time.deltaTime;
                is_coner = false;
            }
        }

        is_coner = true;
        //Debug.Log("is_corner:" + is_coner);
        // Camera rotation around y-axis
        if (Input.GetKey(KeyCode.LeftArrow) && is_coner)
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow) && is_coner)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

    }
}
