using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MIDIAnalyser : MonoBehaviour
{
    //Whoopsie des variables globales à la classe. J'aurais pu les passer en paramètres pis les trainer partout mais c'est moins cancer à créer comme ça
    long lastTimeAdded = -1;
    int lastDirection = 0;

    public GameObject loadText;

    [SerializeField]
    SongUploadMenu songUploadMenu;

    public IEnumerator AnalyzeMIDI(string midiName, string midiPath)
    {
        if (midiName == null || midiName == "" || midiPath == null || midiPath == "")
        {
            loadText.GetComponent<TextMeshProUGUI>().text = "Invalid path, please try selecting it again";
        }
        else
        {
            loadText.GetComponent<TextMeshProUGUI>().text = "Analysing file...";

            string jsonFileName = midiName + ".json";
            MidiFile midiFile = MidiFile.Read(midiPath);

            IEnumerable<Note> notes = midiFile.GetNotes(); //.AtTime(new BarBeatTicksTimeSpan(0), tempoMap);

            //128 car la variable NoteNumber (qui représente la note en tant que telle) va de 0-127
            int[] globalNoteFrequencyArray = new int[128];
            AddToNoteFrequencyArray(ref globalNoteFrequencyArray, notes);


            //************************************************ Trouver le nombre total de bars de la chanson ************************************************\\
            TempoMap tempoMap = midiFile.GetTempoMap();
            BarBeatTicksTimeSpan barBeatTimeOfLastEvent = midiFile.GetTimedEvents().Last().TimeAs<BarBeatTicksTimeSpan>(tempoMap);

            long totalBars = barBeatTimeOfLastEvent.Bars;

            // If a bar is partially occupied, we need add it to the total count of bars
            if (barBeatTimeOfLastEvent.Beats > 0 || barBeatTimeOfLastEvent.Ticks > 0)
                totalBars = barBeatTimeOfLastEvent.Bars + 1;

            // Convert time span corresponding to total count of bars to ticks
            MidiTimeSpan totalBarsInTicks = TimeConverter.ConvertTo<MidiTimeSpan>(new BarBeatTicksTimeSpan(totalBars, 0), tempoMap);

            MetricTimeSpan totalBarsInMetric = TimeConverter.ConvertTo<MetricTimeSpan>((long)totalBarsInTicks, tempoMap);
            long totalBarsInMicroseconds = totalBarsInMetric.TotalMicroseconds;

            long nbOfTicksInBars = totalBarsInTicks / totalBars; //GetNumberOfTicksInBar(notes, totalBars, tempoMap);


            //****************************************** Remplir des tableaux faciles d'accès contenant chaque bar ******************************************\\
            List<Chord>[] barsChordsArray;
            Debug.Log("Getting all chords and separating them per bar");
            barsChordsArray = GetAllChordsInBarsV2(midiFile, nbOfTicksInBars, totalBars);
            yield return null;

            List<Note>[] barsNotesArray;
            Debug.Log("Getting all notes and separating them per bar");
            barsNotesArray = GetAllNotesInBarsV2(midiFile, nbOfTicksInBars, totalBars);
            yield return null;

            //******************************************************** Analyser chaque bar une à une ********************************************************\\
            List<InGameNote> inGameNotes = new List<InGameNote>();
            for (int currentBar = 0; currentBar < totalBars; currentBar++)
            {
                Debug.Log("Analysing bar #" + currentBar);
                loadText.GetComponent<TextMeshProUGUI>().text = "Analysing bar #" + currentBar + "...";
                if (barsChordsArray[currentBar].Count > 0)
                    inGameNotes.AddRange(AnalyseBar(currentBar, nbOfTicksInBars, barsChordsArray, barsNotesArray));
                yield return null;
            }

            WriteSongToFile(inGameNotes, jsonFileName);
            CreateConfigFile(midiName, midiPath, totalBarsInTicks, totalBarsInMicroseconds);
            Debug.Log("We did it!");
            loadText.GetComponent<TextMeshProUGUI>().text = "Done!";

            songUploadMenu.Clean();
            songUploadMenu.Populate();
        }
    }

    /// <summary>
    /// Fonction qui permet de savoir combien de ticks se trouvent dans une seule bar de la chanson
    /// </summary>
    /// <returns></returns>
    private long GetNumberOfTicksInBar(IEnumerable<Note> notes, long totalNumberOfBarsInSong, TempoMap tempoMap)
    {
        long nbOfTicksInBar = 0;

        int barNumber = 0;
        while (nbOfTicksInBar == 0 && barNumber < totalNumberOfBarsInSong)
        {
            long timeOfNoteInFirstBar = notes.AtTime(new BarBeatTicksTimeSpan(barNumber), tempoMap).GetObjects(ObjectType.Note).First().Time;
            long timeOfNoteInSecondBar = notes.AtTime(new BarBeatTicksTimeSpan(barNumber + 1), tempoMap).GetObjects(ObjectType.Note).First().Time;

            nbOfTicksInBar = timeOfNoteInSecondBar - timeOfNoteInFirstBar;

            barNumber++;
        }

        if (nbOfTicksInBar <= 0)
            Debug.LogError("There was an error during the analysis of the MIDI file. Please try another file.");

        return nbOfTicksInBar;
    }

    /// <summary>
    /// Fonction qui permet d'avoir toutes les notes qui se trouvent entre deux bars. Simplement mettre deux fois le même chiffre pour avoir les notes d'une bar spécifique
    /// </summary>
    /// <param name="midiFile">Le fichier midi à analyser</param>
    /// <param name="startingBarNumber">Numéro de la bar de début, inclusif. La première bar de la chanson est la bar 0</param>
    /// <param name="endingBarNumber">Numéro de la bar de début, inclusif également. La première bar de la chanson est la bar 0</param>
    /// <param name="barDurationInTicks">Laisser vide pour que la fonction calcule par elle même le nombre de ticks par bar</param>
    /// <returns></returns>
    private List<Note> GetAllNotesInBars(MidiFile midiFile, int startingBarNumber, int endingBarNumber = -1, long nbOfTicksInBar = -1)
    {
        List<Note> notesInBar = new List<Note>();
        IEnumerable<Note> notes = midiFile.GetNotes();

        if (endingBarNumber == -1)
            endingBarNumber = startingBarNumber;

        foreach (Note note in notes.GetObjects(ObjectType.Note))
        {
            if ((note.Time >= nbOfTicksInBar * startingBarNumber) && (note.Time <= nbOfTicksInBar * (endingBarNumber + 1)))
            {
                notesInBar.Add(note);
            }
        }

        return notesInBar;
    }

    /// <summary>
    /// Fonction qui permet d'avoir toutes les notes qui se trouvent entre deux bars. Simplement mettre deux fois le même chiffre pour avoir les notes d'une bar spécifique
    /// </summary>
    /// <param name="midiFile">Le fichier midi à analyser</param>
    /// <param name="startingBarNumber">Numéro de la bar de début, inclusif. La première bar de la chanson est la bar 0</param>
    /// <param name="endingBarNumber">Numéro de la bar de début, inclusif également. La première bar de la chanson est la bar 0</param>
    /// <param name="barDurationInTicks">Laisser vide pour que la fonction calcule par elle même le nombre de ticks par bar</param>
    /// <returns></returns>
    private List<Chord> GetAllChordsInBars(MidiFile midiFile, int startingBarNumber, int endingBarNumber = -1, long nbOfTicksInBar = -1)
    {
        List<Chord> chordsInBar = new List<Chord>();
        IEnumerable<Chord> chords = midiFile.GetChords();

        if (endingBarNumber == -1)
            endingBarNumber = startingBarNumber;

        bool reachedChord = false;

        foreach (Chord chord in chords.GetObjects(ObjectType.Chord))
        {
            if ((chord.Time >= nbOfTicksInBar * startingBarNumber) && (chord.Time <= nbOfTicksInBar * (endingBarNumber + 1)))
            {
                reachedChord = true;
                chordsInBar.Add(chord);
            }
            else if (reachedChord)
                return chordsInBar;
        }

        return chordsInBar;
    }

    /// <summary>
    /// Version 2 de la fonction qui retourne toutes les chords triées en bars
    /// </summary>
    /// <param name="midiFile">Le fichier midi</param>
    /// <param name="nbOfTicksInBar">Le nombre de ticks par bar</param>
    /// <param name="nbOfBars">Le nombre total de bars</param>
    /// <returns></returns>
    private List<Note>[] GetAllNotesInBarsV2(MidiFile midiFile, long nbOfTicksInBar, long nbOfBars)
    {
        List<Note>[] allBarsArray = new List<Note>[nbOfBars];
        int index;

        for (int i = 0; i < nbOfBars; i++)
            allBarsArray[i] = new List<Note>();

        IEnumerable<Note> notes = midiFile.GetNotes();
        foreach (Note note in notes.GetObjects(ObjectType.Note))
        {
            index = Mathf.FloorToInt(note.Time / nbOfTicksInBar);

            allBarsArray[index].Add(note);
        }

        return allBarsArray;
    }

    /// <summary>
    /// Version 2 de la fonction qui retourne toutes les chords triées en bars
    /// </summary>
    /// <param name="midiFile">Le fichier midi</param>
    /// <param name="nbOfTicksInBar">Le nombre de ticks par bar</param>
    /// <param name="nbOfBars">Le nombre total de bars</param>
    /// <returns></returns>
    private List<Chord>[] GetAllChordsInBarsV2(MidiFile midiFile, long nbOfTicksInBar, long nbOfBars)
    {
        List<Chord>[] allBarsArray= new List<Chord>[nbOfBars];
        int index;

        for (int i = 0; i < nbOfBars; i++)
            allBarsArray[i] = new List<Chord>();

        IEnumerable<Chord> chords = midiFile.GetChords();
        foreach (Chord chord in chords.GetObjects(ObjectType.Chord))
        {
            index = Mathf.FloorToInt(chord.Time / nbOfTicksInBar);

            allBarsArray[index].Add(chord);
        }

        return allBarsArray;
    }

    /// <summary>
    /// Fonction qui compare 2 bars et qui retourne la similitude entre les deux, de 0 à 1
    /// </summary>
    /// <returns></returns>
    private float CompareBars(List<Chord> bar1, List<Chord> bar2, long nbOfTicksInBar)
    {
        int bar1Index = 0, bar2Index = 0;

        if (bar1.Count == 0 || bar2.Count == 0)
            return 0;
        
        float likeness = 1;

        Chord chord1 = bar1[bar1Index], chord2 = bar2[bar2Index];
        int totalNbOfChords = bar1.Count + bar2.Count;
        long bar1RelativeTime = bar1[bar1Index].Time;
        long bar2RelativeTime = bar2[bar2Index].Time;

        while (chord1 != null && chord2 != null)
        {
            //Comparer les notes des chords pour comparer le % de similarité
            //Après, si le temps est pareil, avancer dans les deux listes. Sinon seulement avancer dans celle qui est la moins "loin" dans la chanson.
            //Évidemment, il ne faut pas compter en double les notes pour le % final si on n'avance pas une bar

            float chordDifference = 0;

            //Calculer la différence de temps entre les deux chords
            long earlierTime = Convert.ToInt64(Mathf.Min(chord1.Time - bar1RelativeTime, chord2.Time - bar2RelativeTime));
            long laterTime = Convert.ToInt64(Mathf.Max(chord1.Time - bar1RelativeTime, chord2.Time - bar2RelativeTime));
            long timeDifference = laterTime - earlierTime;
            chordDifference += (float)timeDifference / (float)nbOfTicksInBar;

            //Calculer la différence dans les notes elles-mêmes
            int nbOfNotesInChords = chord1.Notes.Count<Note>() + chord2.Notes.Count<Note>();
            foreach (Note note1 in chord1.Notes)
            {
                bool noteFound = false;
                foreach (Note note2 in chord2.Notes)
                {
                    if (note1.NoteNumber == note2.NoteNumber)
                        noteFound = true;
                }
                if (!noteFound)
                    chordDifference += 1f / (float)nbOfNotesInChords;
            }
            
            likeness -= (float)chordDifference / (float)totalNbOfChords;

            if (bar1Index == bar1.Count - 1 || bar2Index == bar2.Count - 1)
            {
                int nbOfRemainingChords = (bar1.Count - bar1Index - 1) + (bar2.Count - bar2Index - 1);
                likeness -= (float)nbOfRemainingChords / (float)totalNbOfChords;

                return Mathf.Clamp01(likeness);
            }
            else
            {
                //La structure des if est telle que si les Time sont égaux, les deux index seront avancés. Pas besoin de if/elseif/else dans ce cas
                if (chord1.Time - bar1RelativeTime <= chord2.Time - bar2RelativeTime)
                    chord1 = bar1[++bar1Index];
                
                if (chord1.Time - bar1RelativeTime >= chord2.Time - bar2RelativeTime)
                    chord2 = bar2[++bar2Index];
            }
        }

        return Mathf.Clamp01(likeness);
    }

    /// <summary>
    /// Fonction qui prépare le tableau qui contiendra la fréquence de chaque note dans la chanson complète
    /// </summary>
    private void AddToNoteFrequencyArray(ref int[] array, IEnumerable<Note> notes)
    {
        foreach (Note note in notes)
        {
            array[note.NoteNumber]++;
        }
    }

    /// <summary>
    /// Fonction qui analyse une bar spécifique et qui retourne les notes qui seront utilisées in-game
    /// </summary>
    /// <param name="barNumber">Le numéro de la mesure</param>
    /// <param name="nbOfTicksInBar">Le nombre de tick dans chaque mesure de la chanson</param>
    /// <param name="chordsArray">Un tableau contenant toutes les chords de la chanson</param>
    /// <param name="notesArray">Un tableau contenant toutes les notes de la chanson</param>
    /// <returns></returns>
    private List<InGameNote> AnalyseBar(int barNumber, long nbOfTicksInBar, List<Chord>[] chordsArray, List<Note>[] notesArray)
    {
        List<InGameNote> inGameNotes = new List<InGameNote>();

        int[] surroundingBarsNoteFrequencyArray = new int[128];
        List<Chord> barChords = chordsArray[barNumber];
        List<Note> barNotes = notesArray[barNumber];

        int startingBar = Mathf.Clamp(barNumber - 4, 0, Convert.ToInt32(chordsArray.Length));
        int endingBar = Mathf.Clamp(barNumber + 4, 0, Convert.ToInt32(chordsArray.Length));
        for (int currentComparison = startingBar; currentComparison < endingBar; currentComparison++)
        {
            List<Chord> barToCompareChords = chordsArray[currentComparison];
            List<Note> barToCompareNotes = notesArray[currentComparison];

            //Cette partie sert à éliminer les bars qui sont complètement différentes de celle que l'on examine
            float likeness = CompareBars(barChords, barToCompareChords, nbOfTicksInBar);
            if (likeness >= 0.33f)
            {
                AddToNoteFrequencyArray(ref surroundingBarsNoteFrequencyArray, barToCompareNotes);
            }
        }

        //Donner un score préliminaire aux notes
        float[] noteScore = new float[barNotes.Count];
        for (int noteIndex = 0, chordIndex = 0; noteIndex < barNotes.Count; noteIndex++)
        {
            // Gérer les bars pour qu'elles ne soient pas décalées
            if (noteIndex != 0 && barNotes[noteIndex].Time > barNotes[noteIndex - 1].Time)
                chordIndex++;

            //Proximité aux notes qui apparaissent souvent dans le tableau de fréquence
            noteScore[noteIndex] += 15f * Mathf.Clamp01(NoteFrequencyProximity(surroundingBarsNoteFrequencyArray, 5, barNotes[noteIndex].NoteNumber));

            //Fréquence d'apparition de la note elle-même dans les bars proches
            noteScore[noteIndex] += 15f * Mathf.Clamp01(surroundingBarsNoteFrequencyArray[barNotes[noteIndex].NoteNumber] / surroundingBarsNoteFrequencyArray.Max());

            //Si la note fait partie d'une chord et, si oui, combien de notes composent cette chord
            int maxNbOfNotesInChord = 0;
            for (int i = 0; i < barChords.Count; i++)
                maxNbOfNotesInChord = Mathf.Max(maxNbOfNotesInChord, barChords[i].Notes.Count());
            noteScore[noteIndex] += 50f * Mathf.Clamp01(barChords[chordIndex].Notes.Count() / maxNbOfNotesInChord);

            //Si la note est seule après (ou avant) un moment de silence. Si c'est la première note de la chanson, il faut placer un ennemi là
            long optimalSilenceAmount = nbOfTicksInBar / 4;
            if (chordIndex == 0 && barNumber == 0)
                noteScore[noteIndex] += 75f;
            else if (chordIndex == 0)
                //Il y a un opérateur ternaire dans le milieu de la ligne. C'est juste pour vérifier s'il y a au moins une chord dans la bar d'avant. Si non, donner le plus grand score
                noteScore[noteIndex] += 75f * Mathf.Clamp((barChords[chordIndex].Time - (chordsArray[barNumber - 1].Count > 0 ? chordsArray[barNumber - 1].Last().Time : 0)) / optimalSilenceAmount, 0f, 2f);
            else
                noteScore[noteIndex] += 75f * Mathf.Clamp((barChords[chordIndex].Time - barChords[chordIndex - 1].Time) / optimalSilenceAmount, 0f, 2f);

            //Plus la note est haute, plus elle est a de chance de faire partie de la mélodie principale
            noteScore[noteIndex] += 125f * Mathf.Clamp01(Mathf.Pow(barNotes[noteIndex].NoteNumber, 2f) / Mathf.Pow(128f, 2f));
        }

        //Choisir les notes les plus élevées et ajuster les scores en fonction
        float scoreAverage = noteScore.Average();
        float standardDeviation = FindStandardDeviation(noteScore);
        long minimumGapBetweenNotes = nbOfTicksInBar / 8;

        for (int i = 0; i < barNotes.Count; i++)
        {
            if ((noteScore[i] > scoreAverage + standardDeviation * 1.20f) && (lastTimeAdded != barNotes[i].Time) && (barNotes[i].Time - lastTimeAdded >= minimumGapBetweenNotes))
            {
                bool isDoubleNote = (noteScore[i] > scoreAverage + standardDeviation * 1.80f);

                inGameNotes.Add(new InGameNote(DetermineDirection(barNotes[i].NoteNumber, barNumber + 1, nbOfTicksInBar, barNotes[i].Time), DetermineEnemyType(), barNotes[i].Time, isDoubleNote));
                lastTimeAdded = barNotes[i].Time;
            }
        }

        return inGameNotes;
    }

    /// <summary>
    /// Fonction qui analyse la fréquences des notes à proximité et retourne le taux de similitude à celle la plus utilisée
    /// </summary>
    /// <param name="noteFrequencyArray">Le tableau qui contient la fréquence des notes</param>
    /// <param name="acceptableProximity">Int qui indique la proximité acceptable. Plus le chiffre est haut, plus la recherche est large</param>
    /// <param name="noteNumber">L'index de la note qui nous intéresse</param>
    /// <returns></returns>
    private float NoteFrequencyProximity(int[] noteFrequencyArray, int acceptableProximity, int noteNumber)
    {
        float proximity = 1;
        int maxFrequency = noteFrequencyArray.Max();

        int minIndex = Mathf.Clamp(noteNumber - acceptableProximity, 0, 127);
        int maxIndex = Mathf.Clamp(noteNumber + acceptableProximity, 0, 127);

        int indexDifference = -1;
        int highestFrequencyNearby = -1;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (noteFrequencyArray[i] > highestFrequencyNearby)
            {
                indexDifference = Mathf.Abs(i - noteNumber);
                highestFrequencyNearby = noteFrequencyArray[i];
            }
        }

        proximity += (highestFrequencyNearby / maxFrequency) * (1 - (indexDifference / acceptableProximity));

        return proximity;
    }

    /// <summary>
    /// Fonction qui détermine de quelle direction un ennemi devrait arriver selon quelques critères arbitraires
    /// </summary>
    /// <param name="noteNumber">Le NoteNumber de la note</param>
    /// <param name="nbOfTicksInBar">Le nombre de ticks dans une mesure</param>
    /// <param name="timeOfNote">Le temps exact (en ticks) à lequel la note joue</param>
    /// <returns></returns>
    private string DetermineDirection(long noteNumber, int barNumber, long nbOfTicksInBar, long timeOfNote)
    {
        //0 = N, 1 = S, 2 = E, 3 = O
        float[] directionPreference = new float[4];
        int indexOfPreferredDirection = Mathf.FloorToInt(noteNumber * barNumber % 4);
        directionPreference[indexOfPreferredDirection] += 2;

        if (indexOfPreferredDirection == lastDirection && timeOfNote - lastTimeAdded < nbOfTicksInBar / 8)
        {
            switch (lastDirection)
            {
                case 0:
                    directionPreference[1] += 2;
                    directionPreference[2] += 1;
                    break;
                case 1:
                    directionPreference[0] += 2;
                    directionPreference[3] += 1;
                    break;
                case 2:
                    directionPreference[1] += 1;
                    directionPreference[3] += 2;
                    break;
                case 3:
                    directionPreference[0] += 1;
                    directionPreference[2] += 2;
                    break;
                default:
                    break;
            }
        }

        int tieBreaker = indexOfPreferredDirection;
        for (int i = 0; i < 4; i++)
        {
            if (directionPreference[i] >= 3)
            {
                lastDirection = i;
                return ConvertDirectionIndexToString(i);
            }
            else if (directionPreference[i] == 1)
                tieBreaker = i;
        }

        lastDirection = tieBreaker;
        return ConvertDirectionIndexToString(tieBreaker);
    }

    //On pourrait utiliser un dictionary mais je suis vache lol
    private string ConvertDirectionIndexToString(int directionIndex)
    {
        switch(directionIndex)
        {
            case 0:
                return "N";
            case 1:
                return "S";
            case 2:
                return "E";
            case 3:
                return "O";
            default:
                return "N";
        }
    }

    private int DetermineEnemyType()
    {
        return 0;
    }

    /// <summary>
    /// Trouver l'écart-type de plusieurs variables float
    /// </summary>
    /// <param name="values">Les valeurs</param>
    /// <returns></returns>
    private float FindStandardDeviation(IEnumerable<float> values)
    {
        // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/
        float mean = 0.0f;
        float sum = 0.0f;
        float stdDev = 0.0f;
        int n = 0;
        foreach (float val in values)
        {
            n++;
            float delta = val - mean;
            mean += delta / n;
            sum += delta * (val - mean);
        }
        if (n > 1)
            stdDev = (float)Math.Sqrt(sum / (n - 1));
        else if (n == 1)
            stdDev = (float)Math.Sqrt(sum / (n));

        return stdDev;
    }

    /// <summary>
    /// Fonction qui, à partir d'une liste de notes, créé un fichier JSON que nous pouvons utiliser pour recréer les notes de la chanson par la suite
    /// </summary>
    /// <param name="inGameNotes">La liste qui contient les notes (le résultat de l'analyse, en gros)</param>
    /// <param name="jsonFileName">Nom du fichier JSON créé, avec l'extension</param>
    private void WriteSongToFile(List<InGameNote> inGameNotes, string jsonFileName)
    {
        FileHandler.SaveToJSON(inGameNotes, jsonFileName);
    }
    private void CreateConfigFile(string songName, string pathToMIDI, long totalNbOfTicks, long lengthInMicroseconds)
    {
        FileConfig fileConfig = new FileConfig(songName, "", totalNbOfTicks, lengthInMicroseconds, pathToMIDI);
        FileHandler.SaveToJSON(fileConfig, songName + "_config.json");
    }
}
