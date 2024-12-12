using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour
{
    public string Name;
    public List<Transform> InteractionLocations;
}

public class ActivityTracker
{
    public int CurrentProgress;
    public Transform[] Locations;
}

