using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMODUnityResonance;
using System.IO;
using System.Runtime.InteropServices;
using System;

public class MusicAnalyser : MonoBehaviour
{
    public void CreateEnemyDataFromFile(string songNameWithExtension)
    {
        string filePath = Application.dataPath + "/Songs/" + songNameWithExtension;
        UnityEngine.Debug.Log("Analyse du fichier: " + songNameWithExtension);

        FMOD.RESULT FModResult; //La variable qu'on utilise pour stocker nos résultats au fur et à mesure qu'on analyse le fichier.

        FMOD.System FModSystem;
        FMOD.Factory.System_Create(out FModSystem);
        FModResult = FModSystem.init(1, FMOD.INITFLAGS.NORMAL, new IntPtr(0));
        if (FModResult != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.Log("Erreur lors de la création du FModSystem! " + FModResult.ToString());
            return;
        }

        FMOD.Sound song = new Sound();
        FModResult = FModSystem.createSound(filePath, FMOD.MODE.ACCURATETIME, out song);
        if (FModResult != FMOD.RESULT.OK)
        {
            UnityEngine.Debug.Log("Erreur lors de la recherche du fichier de chanson! " + FModResult.ToString());
            UnityEngine.Debug.Log(filePath);
            return;
        }
        
        song.seekData(0); // Seek to the beginning of the sound data    
        FModResult = song.getLength(out uint remainingLength, TIMEUNIT.PCMBYTES);
        if (FModResult == FMOD.RESULT.OK)
        {
            int compteur = 0;

            do
            {
                compteur++;
                byte[] SoundData = new byte[65536];                             // Create a max-length buffer for the audio data
                uint LenToRead;                                                 // Specify the length is at most 65,536, or to the end of the data
                if (remainingLength > SoundData.Length)
                {
                    LenToRead = 65536;
                }
                else
                {
                    LenToRead = remainingLength;
                }
                uint LenRead = LenToRead;
                IntPtr BufferPtr = Marshal.AllocHGlobal((int)LenToRead);        // Allocate the buffer and get its pointer
                                                                                // Read the "LenToRead" bytes into the buffer and update LenRead with the number of bytes read
                FModResult = song.readData(BufferPtr, LenToRead, out LenRead);
                if (FModResult != FMOD.RESULT.OK)
                    UnityEngine.Debug.Log("Erreur critique lors de la lecture #" + compteur);
                Marshal.Copy(BufferPtr, SoundData, 0, (int)LenRead);            // Copy the data out of unmanaged memory into the SoundData byte[] array
                Marshal.FreeHGlobal(BufferPtr);
                remainingLength -= LenRead;                                              // Subtract what we read from the remaining length of data.
            } while ((remainingLength > 0) && (FModResult == FMOD.RESULT.OK));           // As long as we have no errors and still more data to read
        }

        song.release();
    }
}
