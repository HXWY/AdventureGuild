﻿using TMPro;
using UnityEngine;

public class HeroMaker : MonoBehaviour
{
    [SerializeField]
    private Sprite defaultHeroSprite;

    [SerializeField]
    TextMeshProUGUI nameTextBox;

    private GuildMember hero;
    private PopulationManager populationManager;

    private void Start()
    {
        populationManager = FindObjectOfType<PopulationManager>();
    }

    public void CreateHero()
    {
        GameObject heroObj = GameObject.FindGameObjectWithTag("Hero");
        if (heroObj.GetComponent<GuildMember>() == null)
        {
            heroObj.AddComponent<GuildMember>();
        }
        hero = heroObj.GetComponent<GuildMember>();
        hero.person = new Person {
            gender = "MALE"
        };
        hero.UpdateHealth(100);
        hero.IncreaseLevel();
        hero.SetAvatar(defaultHeroSprite);
        hero.SetVocation(new Adventurer());
        hero.IsAvailable(true);
        populationManager.GuildMembers.Add(hero);
    }

    public void SetHeroGender(string _gender)
    {
        hero.person.gender = _gender;
    }

    public void SetHeroName()
    {
        hero.person.name = nameTextBox.text;
    }
}
