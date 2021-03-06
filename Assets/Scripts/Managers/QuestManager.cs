﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private QuestTimer questTimerPrefab;

    [SerializeField]
    private TextAsset questsJson;

    [SerializeField]
    private List<Quest> questPool;          // questPool is the actual list of quests that are in use.

    [SerializeField]
    private List<Quest> questArchive;

    private Quests quests;                  // quests is just a reference to the entire Quest JSON list. It shouldn't be used directly.
    private IncidentManager incidentManager;
    private Guildhall guildhall;
    private NotificationManager notificationManager;

    private readonly string failureMessage = "The challenges were too great, and I was defeated before completing my quest. I have returned to the Adventure Guild so that I may recover.";
    private string rewardMessage = "";

    public Quest CurrentQuest { get; set; }

    private void Start()
    {
        incidentManager = FindObjectOfType<IncidentManager>();
        guildhall = FindObjectOfType<Guildhall>();
        notificationManager = FindObjectOfType<NotificationManager>();
        quests = JsonUtility.FromJson<Quests>(questsJson.text);
        if (questPool == null)
        {
            questPool = new List<Quest>();
        }
        if (questArchive == null)
        {
            questArchive = new List<Quest>();
        }
        if (!SaveSystem.SaveFileExists() && questPool.Count == 0)
        {
            PopulateQuestPool(4);
        }
    }

    public void PopulateQuestPool(int numOfQuests)
    {
        List<Quest> questsToGet = new List<Quest>();
        for (int i = 0; i < numOfQuests; i++)
        {
            Quest quest = new Quest();
            Quest questToClone;
            do
            {
                questToClone = quests.GetRandomQuest();
            }
            while (!Helpers.IsUniqueMember(questToClone.questName, questsToGet.Select(q => q.questName).ToList())
                    || !Helpers.IsUniqueMember(questToClone.questName, questPool.Select(q => q.questName).ToList()));
            quest.questName = questToClone.questName;
            quest.contractor = questToClone.contractor;
            quest.description = questToClone.description;
            quest.commencement = questToClone.commencement;
            quest.completion = questToClone.completion;
            quest.id = questToClone.id;
            quest.difficulty = questToClone.difficulty;
            quest.time = questToClone.time;
            quest.Init();
            questsToGet.Add(quest);
        }
        foreach (Quest quest in questsToGet)
        {
            questPool.Add(quest);
        }
        SortQuestPool();
    }
    
    public Quest GetQuestById(int _id)
    {
        foreach (Quest quest in questPool)
        {
            if (quest.questInstanceId == _id)
            {
                return quest;
            }
        }
        Debug.Log("QuestManager.cs GetQuestById() could not find the requested Quest: " + _id);
        return null;
    }

    public List<Quest> GetQuestsByStatus(Quest.Status _status)
    {
        List<Quest> questList = new List<Quest>();
        foreach (Quest quest in questPool)
        {
            if (quest.State == _status)
            {
                questList.Add(quest);
            }
        }
        return questList;
    }

    public void SetAdventurer(GuildMember guildMember)
    {
        CurrentQuest.GuildMember = guildMember;
    }

    public void StartQuest()
    {
        QuestTimer questTimer = Instantiate(questTimerPrefab, transform);
        questTimer.SetQuest(CurrentQuest);
        questTimer.StartTimer();
        CurrentQuest.GuildMember.IsAvailable = false;
        CurrentQuest.startTime = DateTime.Now;
        CurrentQuest.State = Quest.Status.Active;
    }

    public void CompleteQuest(Quest quest)
    {
        notificationManager.CreateNotification(string.Format("The quest \"{0}\" has completed!", quest.questName), Notification.Type.Quest, Notification.Spirit.Good);
        quest.State = Quest.Status.Completed;
        rewardMessage = "Quest Completed!";
        quest.Incidents.Add(incidentManager.CreateCustomIncident(quest.completion, Incident.Result.Good, rewardMessage, DateTime.Now));
        ApplyQuestReward(quest);
        quest.GuildMember.IsAvailable= true;
        questArchive.Add(quest);
        questPool.Remove(quest);
        SortQuestPool();
        SortQuestArchive();
    }

    public void FailQuest(Quest quest)
    {
        notificationManager.CreateNotification(string.Format("The quest \"{0}\" has failed!", quest.questName), Notification.Type.Quest, Notification.Spirit.Bad);
        quest.State = Quest.Status.Failed;
        rewardMessage = "Quest Failed!";
        quest.Incidents.Add(incidentManager.CreateCustomIncident(failureMessage, Incident.Result.Bad, rewardMessage));
        quest.GuildMember.IsAvailable = true;
        questArchive.Add(quest);
        questPool.Remove(quest);
        SortQuestPool();
        SortQuestArchive();
    }

    public void ApplyQuestReward(Quest quest)
    {
        guildhall.AdjustGold(quest.Reward.Gold);
        guildhall.AdjustIron(quest.Reward.Iron);
        guildhall.AdjustWood(quest.Reward.Wood);
        guildhall.AdjustRenown(quest.Reward.Renown);
        quest.GuildMember.AddExp(quest.Reward.Exp);
    }

    public List<Quest> GetQuestPool()
    {
        return questPool;
    }

    public List<Quest> GetQuestArchive()
    {
        return questArchive;
    }

    public void SetQuestPool(List<Quest> questList)
    {
        questPool.Clear();
        questPool = questList;
    }

    public void SetQuestArchive(List<Quest> questList)
    {
        questArchive.Clear();
        questArchive = questList;
    }

    public void LoadQuestTimer(QuestTimerData questTimerData)
    {
        QuestTimer questTimer = Instantiate(questTimerPrefab, transform);
        questTimer.SetQuest(FindQuestById(questTimerData.questInstanceId));
        if (questTimer.GetQuest() == null)
        {
            Debug.Log("QuestManager LoadQuestTimer() could not find the Quest by ID!");
        }
        else
        {
            questTimer.TimeLimit = questTimerData.timeLimit;
            questTimer.StartTime = questTimerData.startTime;
            questTimer.IsTiming = questTimerData.isTiming;
            questTimer.IncidentTimer = questTimerData.incidentTimer;
        }
    }

    private Quest FindQuestById(int _id)
    {
        foreach(Quest quest in questPool)
        {
            if (quest.questInstanceId == _id)
            {
                return quest;
            }
        }
        foreach(Quest quest in questArchive)
        {
            if (quest.questInstanceId == _id)
            {
                return quest;
            }
        }
        Debug.Log("FindQuestById() did not return a Quest!");
        return null;
    }

    public void SortQuestPool()
    {
        List<Quest> sorted = questPool.OrderByDescending(quest => quest.startTime).ToList();
        questPool = sorted;
    }

        public void SortQuestArchive()
    {
        List<Quest> sorted = questArchive.OrderByDescending(quest => quest.startTime).ToList();
        questArchive = sorted;
    }
}
