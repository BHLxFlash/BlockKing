using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private float beatTempo;

    private Direction comingFrom;

    public float BeatTempo
    {
        get
        {
            return beatTempo;
        }
        set
        {
            beatTempo = value;
        }
    }
    public Direction ComingFrom
    {
        get
        {
            return comingFrom;
        }
        set
        {
            comingFrom = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (comingFrom)
        {
            case Direction.North:
                this.transform.position -= new Vector3(beatTempo * Time.deltaTime, 0f, 0f);
                this.transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case Direction.South:
                this.transform.position += new Vector3(beatTempo * Time.deltaTime, 0f, 0f);
                this.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case Direction.East:
                this.transform.position += new Vector3(0f, 0f, beatTempo * Time.deltaTime);
                this.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.West:
                this.transform.position -= new Vector3(0f, 0f, beatTempo * Time.deltaTime);
                this.transform.eulerAngles = new Vector3(0, 180, 0);
                break;
        }
    }


    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
