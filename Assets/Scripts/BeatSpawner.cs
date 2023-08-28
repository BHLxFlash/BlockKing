using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    private bool beatSpawnArmed = false;

    SpawnerManager spawnerManager;
    /*[SerializeField]
    private Track[] tracks;*/

    private string lastMarkerName;

    private void Start()
    {
        spawnerManager = gameObject.GetComponent<SpawnerManager>();

        if (lastMarkerName != null)
            lastMarkerName = (string)MusicManager.me.timelineInfo.lastMarker;
    }

    private void Update()
    {

        if (lastMarkerName != (string)MusicManager.me.timelineInfo.lastMarker)
        {
            beatSpawnArmed = true;
            lastMarkerName = (string)MusicManager.me.timelineInfo.lastMarker;
        }

        if (beatSpawnArmed && lastMarkerName != string.Empty)
        {
            spawnerManager.SpawnEnemyRandom();

            /*
            switch (lastMarkerName[0].ToString())
            {
                case "1":
                    if (tracks[0].isActive)
                        tracks[0].CreateNote(MusicManager.me.timelineInfo.currentPosition);
                    break;
                case "2":
                    if (tracks[1].isActive)
                        tracks[1].CreateNote(MusicManager.me.timelineInfo.currentPosition);
                    break;
                case "3":
                    if (tracks[2].isActive)
                        tracks[2].CreateNote(MusicManager.me.timelineInfo.currentPosition);
                    break;
                case "4":
                    if (tracks[3].isActive)
                        tracks[3].CreateNote(MusicManager.me.timelineInfo.currentPosition);
                    break;
                default:
                    Debug.Log("No valid track");
                    break;
            }*/
            beatSpawnArmed = false;
        }
    }
}
