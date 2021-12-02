using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject sky;
    public GameObject mountains;
    public GameObject hills;
    public GameObject mounds;
    public GameObject ground;
    public GameObject foreground;

    public RectTransform healthbar;
    public GameObject help;

    private PlayerController player;
    private Inventory inventory;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        if (!inventory.alreadyShowedHelp && help)
        {
            help.SetActive(true);
            inventory.alreadyShowedHelp = true;
        }
    }

    void Update()
    {
        healthbar.offsetMax = new Vector2(healthbar.offsetMax.x, -GetHealth());
        if (help && help.activeInHierarchy && player.transform.localPosition.x >= -10)
            help.SetActive(false);
    }

    int GetHealth()
    {
        float percentHealth = (float)player.health / inventory.flaskHealth;
        return 237 - Mathf.FloorToInt(237 * percentHealth) + 8;
    }

    private void LateUpdate()
    {
        float x = Camera.main.transform.localPosition.x;
        sky.transform.localPosition = new Vector3(x * .8f, 0, 0);
        mountains.transform.localPosition = new Vector3(x * 0.55f, 0, 0);
        hills.transform.localPosition = new Vector3(x * 0.15f, 0, 0);
        foreground.transform.localPosition = new Vector3(x * -0.55f, 0, 0);
    }
}
