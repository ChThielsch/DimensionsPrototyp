using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEvent : MonoBehaviour
{
    private PlayerController m_playerController;

    // Start is called before the first frame update
    void Start()
    {
        m_playerController = FindObjectOfType<PlayerController>();
    }

    public void Jump()
    {
        m_playerController.m_jumpEvent = true;
    }

    public void Climb()
    {
        m_playerController.m_climbEvent = true;
    }

    public void FrezeInputs()
    {
        m_playerController.m_freezeInputs = true;
    }

    public void UnfrezeInputs()
    {
        m_playerController.m_freezeInputs = false;
    }
}
