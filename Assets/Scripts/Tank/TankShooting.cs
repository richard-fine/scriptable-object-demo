using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public Rigidbody m_Shell;                   // Prefab of the shell.
    public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
    public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
    public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
    public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
    public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
    public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
    public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
    public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.


    private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool m_Charging;

	public bool IsCharging
	{
		get { return m_Charging; }
	}
	
    private void OnEnable()
    {
        // When the tank is turned on, reset the launch force and the UI
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start ()
    {
        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

	public void BeginChargingShot()
	{
		if (m_Charging) return;

		m_CurrentLaunchForce = m_MinLaunchForce;

		// Change the clip to the charging clip and start it playing.
		m_ShootingAudio.clip = m_ChargingClip;
		m_ShootingAudio.Play();

		m_Charging = true;
	}

	public void FireChargedShot()
	{
		if (!m_Charging) return;

		Fire();
		m_Charging = false;
	}


	private void Update()
	{
		if (m_Charging)
		{
			m_CurrentLaunchForce = Mathf.Min(m_MaxLaunchForce, m_CurrentLaunchForce + m_ChargeSpeed*Time.deltaTime);
			m_AimSlider.value = m_CurrentLaunchForce;
		}
		else
		{
			m_AimSlider.value = m_MinLaunchForce;
		}
	}


    private void Fire ()
    {
        // Create an instance of the shell and store a reference to it's rigidbody.
        Rigidbody shellInstance =
            Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        // Set the shell's velocity to the launch force in the fire position's forward direction.
        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

        // Change the clip to the firing clip and play it.
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play ();

        // Reset the launch force.  This is a precaution in case of missing button events.
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}