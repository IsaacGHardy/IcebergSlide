using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    [SerializeField] GameObject penguin;
    private float moveSpeed = 1.0f;
    [SerializeField] private Vector2 finalPosition;

    private void Start()
    {
        StartCoroutine(waitThenStart());
    }

    private IEnumerator waitThenStart()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Main Menu");
    }
}
