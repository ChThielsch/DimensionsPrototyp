using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraMover : MonoBehaviour
{
    public float m_rotationSpeed =5;
    public AudioClip m_detectionSound;

    private GameObject m_player;
    private Light m_light;
    private SecurityCameraTrigger m_trigger;
    private int m_rotateValue =1;
    private AudioSource m_audioSource;



    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        m_light = GetComponentInChildren<Light>();
        m_trigger = GetComponentInChildren<SecurityCameraTrigger>();
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_trigger.m_detected == false)
        {
            transform.Rotate(m_rotateValue * (Vector3.up * Mathf.Cos(Time.deltaTime)) * m_rotationSpeed /100);
        }

        if (transform.rotation.y >0.7f)
        {
            m_rotateValue = -1;
        }

        if (transform.rotation.y < -0.7f)
        {
            m_rotateValue = 1;
        }

        if (m_trigger.m_detected == true)
        {
            transform.LookAt(m_player.transform.position + new Vector3(0, 2.5f, 0));
            m_audioSource.PlayOneShot(m_detectionSound);
        }
    }
}
