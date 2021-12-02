using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    public Sprite[] bigRocks;
    public Sprite[] smallRocks;

    public bool useBigRocks;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = -Mathf.FloorToInt(transform.position.y * 100);
        float scale = 0.08f * (-transform.position.y - 1) + 0.9f;
        transform.localScale = new Vector3(transform.localScale.x * scale, transform.localScale.y * scale, 0);
        if(useBigRocks)
        {
            sr.sprite = bigRocks[Random.Range(0, bigRocks.Length)];
        }
        else
        {
            sr.sprite = smallRocks[Random.Range(0, smallRocks.Length)];
        }
        int i = Random.Range(0, 2);
        sr.flipX = i == 0;
    }
}
