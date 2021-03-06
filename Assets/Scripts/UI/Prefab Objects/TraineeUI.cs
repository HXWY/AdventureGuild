﻿using UnityEngine;

public class TraineeUI : MonoBehaviour
{
    private TrainingManager trainingManager;
    private MenuManager menuManager;

    private void Start()
    {
        trainingManager = FindObjectOfType<TrainingManager>();
        menuManager = FindObjectOfType<MenuManager>();
    }

    public void StartTraining()
    {
        trainingManager.SetGuildMember(GetComponent<PersonUI>().GuildMember);
        menuManager.OpenMenu("Menu_Training");
        trainingManager.StartGame();
    }
}
