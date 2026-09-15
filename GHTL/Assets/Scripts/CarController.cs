using System;
using Unity.VisualScripting;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rbCar;
    [SerializeField] private Transform[] rayPoints; //Location of each tire of the car
    [SerializeField] private LayerMask driveable; //Whether or not a piece of terrain is actually driveable

    [Header("Suspension")]
    [SerializeField] private float springStiffness;  //Maximum force spring exerts when fully compressed
    [SerializeField] private float damperStiffness;  //How much of a dampening effect there is basically
    [SerializeField] private float restLength; //The 'normal' length of the spring when not compressed or stretched
    [SerializeField] private float springTravel; //maximum amount of distance the spring can compress or stretch
    [SerializeField] private float wheelRadius;  


    private void Start()
    {
        rbCar = GetComponent<Rigidbody>(); //grab RB on this object
    }


    //CALL ALL PHYSICS AND SOUND RELATED SHIT HERE,
    //Called every 1/60th of a second meaning important triggers and calcs are DETATCHED from frame rate
    private void FixedUpdate()
    {
        Suspension(); 
    }

    private void Suspension()
    {
        foreach(Transform point in rayPoints)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel; //Absolute longest the theoretical spring can be
            
            //Cast a ray from the position of each wheel, cap it where the tire ends, and check against drivable terrain
            if(Physics.Raycast(point.position,  -point.up,  out hit,  maxLength + wheelRadius,  driveable))
            {
                //Determine how compressed the spring is
                float currentSpringLength = hit.distance - wheelRadius; 
                float springCompression = (restLength - currentSpringLength) / springTravel; //Normalize

                float springVelocity = Vector3.Dot(rbCar.GetPointVelocity(point.position), point.up); //Calculate the velocity of our imaginary spring
                float dampForce = springVelocity * damperStiffness; //Calculate the force being applied based on velocity

                float springForce = springStiffness * springCompression; //Calculating the force based on the normalized springCompression ratio

                float netForce = springForce - dampForce;
                rbCar.AddForceAtPosition(netForce * point.up,  point.position); //Apply to car at the desired point

                Debug.DrawLine(point.position, hit.point, Color.salmon); //Debug lines to see it in action, salmon for contact because lol salmon
            }
            else
            {
                //Debug just to see what happens if we hit NOTHING
                Debug.DrawLine(point.position, point.position + (wheelRadius + maxLength) * -point.up, Color.green);
            }
                
        }
    }
}
