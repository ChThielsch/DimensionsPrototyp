using UnityEngine;
using TMPro;

public class BuildNumberUpdater : MonoBehaviour
{
    private TextMeshProUGUI m_buildNumber;

    private void Awake()
    {
        m_buildNumber = GetComponent<TextMeshProUGUI>();
        m_buildNumber.text = "Build: " + Application.version;
    }
}
