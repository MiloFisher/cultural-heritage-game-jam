using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public GameObject messageDisplay;
    public float messageDuration;

    private SpriteRenderer sr;
    private GameObject shadow;

    void Start()
    {
        messageDisplay.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
        shadow = transform.GetChild(0).gameObject;
    }

    public void DisplayMessage()
    {
        if (!messageDisplay.activeInHierarchy)
            StartCoroutine(ShowMessage());
    }

    IEnumerator ShowMessage()
    {
        messageDisplay.SetActive(true);
        yield return new WaitForSeconds(messageDuration);
        messageDisplay.SetActive(false);
    }

    private void LateUpdate()
    {
        sr.sortingOrder = -Mathf.FloorToInt(shadow.transform.position.y * 100);
        messageDisplay.GetComponent<Canvas>().sortingOrder = sr.sortingOrder;
    }
}
