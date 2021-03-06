﻿using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    private Vector2[] quadrants = new Vector2[4];

    public float defaultRepositionDelay = 2.0f;

    private float repositionDelay;
    private float startTime;
    private float currentTime;
    private int currentQuadrant;
    private int newQuadrant;
    private bool striking = false;
    private TrainingManager trainingManager;
    private TrainingSword trainingSword;

    void Start()
    {
        trainingSword = FindObjectOfType<TrainingSword>();
        trainingManager = FindObjectOfType<TrainingManager>();
        startTime = Time.time;
        repositionDelay = defaultRepositionDelay;
        currentTime = repositionDelay;
    }

    private void FixedUpdate()
    {
        if (!trainingManager.GameOver)
        {
            if (currentTime < repositionDelay)
            {
                currentTime = Time.time - startTime;
            }
            else
            {
                ChangePosition();
                startTime = Time.time;
                currentTime = 0;
            }
        }
    }

    private void ChangePosition()
    {
        Vector2 newPos;
        do
        {
            newQuadrant = Random.Range(0, quadrants.Length);
            newPos = quadrants[newQuadrant];
        }
        while (newQuadrant == currentQuadrant);
        transform.position = newPos;
        currentQuadrant = newQuadrant;
        repositionDelay *= 0.98f;
        if (repositionDelay < 0.6f)
        {
            repositionDelay = 0.6f;
        }
    }

    public void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (!trainingManager.GameOver && hit.collider != null)
        {
            trainingSword.Swing(hit.point);
            StartCoroutine(StrikeShield());
        }
    }
    
    private IEnumerator StrikeShield()
    {
        if (!striking)
        {
            striking = true;
            int quadrant = currentQuadrant;
            yield return new WaitForSeconds(0.15f);
            if (quadrant == currentQuadrant)
            {
                FindObjectOfType<TrainingSword>().ClangSound();
                trainingManager.AddPoints(Mathf.FloorToInt(trainingManager.TimeLimit / trainingManager.TimeRemaining) * 10);
                currentTime = repositionDelay;
            }
            else
            {
                FindObjectOfType<TrainingSword>().WooshSound();
            }
            striking = false;
        }
    }

    public void ResetShieldSpeed()
    {
        repositionDelay = defaultRepositionDelay;
        currentTime = repositionDelay;
    }
}
