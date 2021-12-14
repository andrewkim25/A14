using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

    [System.Serializable]
    public class AxelInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; 
        public bool steering; 
    }

    public List<AxelInfo> motorAxels;
    public List<AxelInfo> freeAxels;
    public ParticleSystem[] skidParticles;
    public float maxMotorTorque;
    public float boostImpulse;
    public float maxSteeringAngle;
    public float downForce;
    public float accelerometerSensitivity = 3;
    public float skidAngle = 30;

    protected Rigidbody theRigid;
    protected TrailEmitter[] trails;
    protected ParticleSystem eggCannon;

    void Start () {
        theRigid = GetComponent<Rigidbody>();
        trails = GetComponentsInChildren<TrailEmitter>();
        eggCannon = GetComponent<ParticleSystem>();
        foreach (ParticleSystem particle in skidParticles)
        {
            particle.Stop();
        }
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    public void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case TagManager.OIL_SPIL:
                gameObject.AddComponent<OilSpil>();
                break;
            case TagManager.ICE_ICE_BABY:
                gameObject.AddComponent<Ice>();
                break;
        }
    }

    public void Boost()
    {
        theRigid.AddForce(transform.forward * boostImpulse, ForceMode.Impulse);
        eggCannon.Play();
    }

    private void FixedUpdate()
    {
        foreach(TrailEmitter emitter in trails)
        {
            if (emitter.transform.position.y < transform.position.y)
            {
                emitter.EndTrail(); ///Temporary hack to be fixed in v1.1. Check if wheelCollider grounded.
            }
        }

        if (Mathf.Abs(Vector3.Angle(theRigid.velocity,transform.forward)) > skidAngle)
        {
            foreach(TrailEmitter trail in trails)
            {
                if (!trail.Active)
                {
                    trail.NewTrail();
                    foreach (ParticleSystem particle in skidParticles)
                    {
                        particle.Play();
                    }
                }
            }
        }
        else
        {
            foreach (TrailEmitter trail in trails)
            {
                trail.EndTrail();
                foreach (ParticleSystem particle in skidParticles)
                {
                    particle.Stop();
                }
            }
        }
        float motor = maxMotorTorque; ///AutoAccelerate
                                      ///Steer 
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        float steering = Mathf.Clamp(accelerometerSensitivity * maxSteeringAngle * Input.acceleration.x,-maxSteeringAngle,maxSteeringAngle);
#else
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
#endif
        foreach (AxelInfo axleInfo in motorAxels)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        foreach(AxelInfo axelInfo in freeAxels)
        {
            ApplyLocalPositionToVisuals(axelInfo.leftWheel);
            ApplyLocalPositionToVisuals(axelInfo.rightWheel);
        }

        theRigid.AddForceAtPosition(Vector3.down * theRigid.velocity.magnitude * downForce, transform.position + transform.right);
        theRigid.AddForceAtPosition(Vector3.down * theRigid.velocity.magnitude * downForce, transform.position - transform.right);
    }
}
