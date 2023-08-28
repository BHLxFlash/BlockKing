using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InGameNote
{
    //Soit N, S, E ou O. Peut �tre compos� de plusieurs lettres. Par exemple, une note qui fait appara�tre un ennemi en haut et un � droite sera NE.
    public string direction;

    //L'index de l'ennemi qu'il faut faire appara�tre dans le tableau d'ennemis
    public int enemyType;

    //Tick de la chanson � lequel l'ennemi doit appara�tre
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