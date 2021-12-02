using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurveyManager : MonoBehaviour
{
    public GameObject survey;
    public GameObject objective;

    public int[] surveyPositions;

    private Inventory inventory;
    private GameObject player;
    private int current;
    private int completed;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
        inventory.objectiveCompleted = false;
        survey.SetActive(false);
        objective.transform.GetChild(1).GetComponent<Text>().text = "Complete Surveys (" + completed + "/" + surveyPositions.Length + ")";
    }

    // Update is called once per frame
    void Update()
    {
        survey.SetActive(false);
        for (int i = 0; i < surveyPositions.Length; i++)
        {
            if (Mathf.Abs(player.transform.localPosition.x - surveyPositions[i]) < 1.5f)
            {
                current = i;
                survey.SetActive(true);
            }
        }
    }

    public void TakeSurvey()
    {
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<AudioSource>().Play();
        surveyPositions[current] = 999;
        survey.SetActive(false);
        completed++;
        objective.transform.GetChild(1).GetComponent<Text>().text = "Complete Surveys (" + completed + "/" + surveyPositions.Length + ")";
        if (completed == surveyPositions.Length)
        {
            inventory.objectiveCompleted = true;
            objective.transform.GetChild(1).GetComponent<Text>().color = Color.green;
        }
    }
}
