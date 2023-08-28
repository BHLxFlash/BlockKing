using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FileConfig
{
    public string songName;

    public string artistName;

    public long timeTick;

    public long timeMicroSecond;

    public string pathToMidi;

    public FileConfig(string songName, string artistName, long timeTick, long timeMicroSecond, string pathToMidi)
    {
        this.songName = songName;
        this.artistName = artistName;
        this.timeTick = timeTick;
        this.timeMicroSecond = timeMicroSecond;
        this.pathToMidi = pathToMidi;
    }
}
