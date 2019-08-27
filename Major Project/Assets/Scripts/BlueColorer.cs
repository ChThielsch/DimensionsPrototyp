using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BlueColorer : MonoBehaviour
{
    [HideInInspector]
    public Material m_material;

    /// <summary>
    /// the playerController Script in the Scene
    /// </summary>
    private PlayerController m_playerController;

    void Awake()
    {
        m_material = gameObject.GetComponent<Renderer>().material;

        m_playerController = FindObjectOfType<PlayerController>();

        m_playerController.AddColorerList(this);
    }

    private void OnDestroy()
    {
        m_playerController.RemoveColorerList(this);
    }
}
