using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public Image screenBlack;
    //public float fadeTime;

    private bool fading;

    private void Start()
    {
        screenBlack.gameObject.SetActive(true);
        StartCoroutine(FadeFromBlack());
    }

    public void ChangeScene(string sceneName)
    {
        if(!fading)
            StartCoroutine(FadeToBlack(sceneName));
    }

    private IEnumerator FadeToBlack(string sceneName)
    {
        if (sceneName == "Museum" || sceneName == "Research Center" || sceneName == "Store" || sceneName == "Expedition Center" || sceneName == "Town")
            GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        fading = true;
        float fadeTime = 0.1f;
        screenBlack.color = new Color(0, 0, 0, 0);
        float smoothness = 50;
        Vector3 v;
        Vector3 a = Vector3.zero;
        Vector3 b = new Vector3(1, 0, 0);
        for (int i = 0; i < smoothness; i++)
        {
            v = Vector3.Lerp(a, b, i * (1/smoothness));
            screenBlack.color = new Color(0, 0, 0, v.x);
            yield return new WaitForSeconds(fadeTime/smoothness);
        }
        SceneManager.LoadScene(sceneName);
        fading = false;
    }

    private IEnumerator FadeFromBlack()
    {
        fading = true;
        float fadeTime = 0.1f;
        screenBlack.color = new Color(0, 0, 0, 1);
        float smoothness = 50;
        Vector3 v;
        Vector3 b = Vector3.zero;
        Vector3 a = new Vector3(1, 0, 0);
        for (int i = 0; i < smoothness; i++)
        {
            v = Vector3.Lerp(a, b, i * (1 / smoothness));
            screenBlack.color = new Color(0, 0, 0, v.x);
            yield return new WaitForSeconds(fadeTime / smoothness);
        }
        fading = false;
    }
}
