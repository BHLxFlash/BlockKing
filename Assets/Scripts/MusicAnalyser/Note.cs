using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note
{
    public float timeIntoSongInSeconds = 0f;
    public SpawnDirection spawnDirection = SpawnDirection.North;
    public int nbOfAssociatedNotes = 0;

    public Note(float timeIntoSongInSeconds, SpawnDirection spawnDirection, int nbOfAssociatedNotes)
    {
        this.timeIntoSongInSeconds = timeIntoSongInSeconds;
        this.spawnDirection = spawnDirection;
        this.nbOfAssociatedNotes = nbOfAssociatedNotes;
    }
}

public enum SpawnDirection
{
    North,
    South,
    East,
    West
}