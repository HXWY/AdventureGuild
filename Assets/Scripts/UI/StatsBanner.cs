﻿using TMPro;
using UnityEngine;

public class StatsBanner : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gold, iron, wood, weapons, population, adventurers, artisans, peasants, renown;

    private Guildhall guildhall;
    private PopulationManager populationManager;

    private void FixedUpdate()
    {
        guildhall = FindObjectOfType<Guildhall>();
        populationManager = FindObjectOfType<PopulationManager>();
        gold.text = guildhall.Gold.ToString();
        iron.text = guildhall.Iron.ToString();
        wood.text = guildhall.Wood.ToString();
        weapons.text = guildhall.Weapons.ToString();

        population.text = populationManager.GuildMembers.Count.ToString();
        adventurers.text = populationManager.Adventurers().Count.ToString();
        artisans.text = populationManager.Artisans().Count.ToString();
        peasants.text = populationManager.Peasants().Count.ToString();
        renown.text = guildhall.Renown.ToString();
    }
}
