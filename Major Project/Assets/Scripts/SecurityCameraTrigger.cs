using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCameraTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool m_detected;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            m_detected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_detected = false;
        }
    }
}
