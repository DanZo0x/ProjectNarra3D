using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Transition References")]
    [SerializeField] private Animator _transitionAnim;
    [SerializeField] private GameObject _transitionCanvas;

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
        _transitionAnim?.SetTrigger("TransitionStart");
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(_sceneToLoad);
    }
}