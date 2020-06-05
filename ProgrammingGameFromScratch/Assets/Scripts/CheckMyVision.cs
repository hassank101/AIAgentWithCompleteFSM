using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMyVision : MonoBehaviour
{
    //Level of sensitivity
    public enum enumSensitivity { HIGH, LOW };

    //Variable to check sensitivity
    public enumSensitivity sensitivity = enumSensitivity.HIGH;

    //Is the target in sight?
    public bool targetInSight = false;

    //Field of vision
    public float fieldOfVision = 90.0f;

    //Reference to the target
    private Transform target = null;

    //Reference to the eyes
    public Transform eyes = null;

    //Transform component
    public Transform npcTransform = null;

    //Sphere collider
    private SphereCollider npcSphereCollider = null;

    //Last knows location of object
    public Vector3 lastKnownLocation = Vector3.zero;

    private void Awake()
    {
        npcTransform = GetComponent<Transform>();
        npcSphereCollider = GetComponent<SphereCollider>();
        lastKnownLocation = npcTransform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    }

    //Is the target within the field of view?
    bool InFieldOfVision()
    {
        Vector3 targetDirection = target.position - eyes.position;

        //Get angle between eyes' forward direction and target direction
        float angle = Vector3.Angle(eyes.forward, targetDirection);

        //Is the target within the field of view?
        if (angle <= fieldOfVision)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Is the target within the line of sight?
    bool InLineOfSight()
    {
        RaycastHit hit;

        if (Physics.Raycast(eyes.position, (target.position - eyes.position).normalized, out hit, npcSphereCollider.radius))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void UpdateSight()
    {
        switch (sensitivity)
        {
            case enumSensitivity.HIGH:
                targetInSight = InFieldOfVision() && InLineOfSight();
                break;
            case enumSensitivity.LOW:
                targetInSight = InFieldOfVision() || InLineOfSight();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        UpdateSight();

        //Update the last known sighting
        if (targetInSight)
        {
            lastKnownLocation = target.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        targetInSight = false;
    }

}
