using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InGameNote
{
    //Soit N, S, E ou O. Peut être composé de plusieurs lettres. Par exemple, une note qui fait apparaître un ennemi en haut et un à droite sera NE.
    public string direction;

    //L'index de l'ennemi qu'il faut faire apparaître dans le tableau d'ennemis
    public int enemyType;

    //Tick de la chanson à lequel l'ennemi doit apparaître
    public long tickToSpawnOn;

    //S'il y a deux ennemis ou non
    public bool isDoubleNote;

    public InGameNote(string direction, int enemyType, long tickToSpawnOn, bool isDoubleNote)
    {
        this.direction = direction;
        this.enemyType = enemyType;
        this.tickToSpawnOn = tickToSpawnOn;
        this.isDoubleNote = isDoubleNote;
    }
}