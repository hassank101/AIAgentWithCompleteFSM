using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] float turnSpeed = 45.0f;
    [SerializeField] float movementSpeed = 20.0f;
    private float turningInput;
    private float movementInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = Input.GetAxis("Vertical");
        turningInput = Input.GetAxis("Horizontal");
        //We need the transform to move
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime * movementInput);
        transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime * turningInput);

    }
}
