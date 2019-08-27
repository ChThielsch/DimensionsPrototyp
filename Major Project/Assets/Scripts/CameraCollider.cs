using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    [Tooltip("The layerMask used to check collision")]
    public LayerMask m_layerMask;

    /// <summary>
    /// the playerController  script in the scene
    /// </summary>
    private PlayerController m_playerController;

    void Start()
    {
        m_playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        MeshRenderer renderer = other.GetComponent<MeshRenderer>();
        if (m_playerController.playerState == PlayerController.State.Side)
        {
            if (renderer != null)
            {
                if (((1 << renderer.gameObject.layer) & m_layerMask) != 0)
                {
                    renderer.enabled = false;
                }
            }
        }
        else
        {
            if (renderer != null)
            {
                renderer.enabled = true;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        MeshRenderer renderer = other.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = true;
        }

    }
}
