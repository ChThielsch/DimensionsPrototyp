using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    private int m_previousIndex = 0;

    private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void LoadScene(int _index)
    {
        //m_previousIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(_index);
    }

    public void LoadScene(string _name)
    {
        //m_previousIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(_name);
    }

    public void LoadPreviousIndex()
    {
        SceneManager.LoadScene(m_previousIndex);
    }

    public void LoadNextIndex()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlaySound(AudioClip _audio)
    {
        m_audioSource.PlayOneShot(_audio);
    }
}
