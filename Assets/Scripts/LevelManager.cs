using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Transition References")]
    private Animator _transitionAnim;
    private GameObject _transitionCanvas;

    private void Start()
    {
        _transitionCanvas = GameObject.FindGameObjectWithTag("TransitionCanvas");
        _transitionAnim = _transitionCanvas?.GetComponent<Animator>();
    }


    public void LoadLevel(string _levelToLoad)
    {
        StartCoroutine(CoroutineLoadLevel(_levelToLoad));
    }

    IEnumerator CoroutineLoadLevel(string _sceneToLoad)
    {
        Debug.Log("Loaded level: " + _sceneToLoad);
        _transitionAnim?.SetTrigger("Start");
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(_sceneToLoad);
    }
}