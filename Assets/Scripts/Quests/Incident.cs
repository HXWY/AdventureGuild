﻿using System;

[System.Serializable]
public class Incident
{
    public enum Result { Good, Bad }

    public string time, description;
    public Result result;

    public void Init()
    {
        time = DateTime.Now.ToShortTimeString();
    }
}