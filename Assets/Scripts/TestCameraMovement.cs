using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraMovement : MonoBehaviour
{

    [SerializeField] float speed = 10f;
    
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 position = transform.position;
        
        transform.Translate(input * speed * Time.deltaTime);
    }
}
