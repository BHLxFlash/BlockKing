using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;


public class MIDIReader : MonoBehaviour
{
    public string midiName;
    private string midiPath;

    private FMOD.System system;
    private Sound sound;
    private Channel channel;
    ChannelGroup masterCG;

    FileConfig fileConfig;

    // Start is called before the first frame update
    void Start()
    {
        //midiName = @".\BlockKingAtelier_Data\Ressources\giorno.mid";

        //à échanger pour que ça marche dans le build
        //midiName = @".\Assets\Songs\Test\giorno.mid";
        
        //midiPath = FileHandler.GetPath("giorno.mid");

        OpenConfig(StateLevelSelector.levelSelected);
        midiPath = fileConfig.pathToMidi;

        system = InitializeFMOD();
        sound = CreateSound();

        StartCoroutine(InitialDelay());
    }

    // Update is called once per frame
    void Update()
    {
        system.update();
    }

    private FMOD.System InitializeFMOD()
    {
        FMOD.System t_system = new FMOD.System();
        FMOD.RESULT result = new FMOD.RESULT();

        // Create FMOD interface object
        result = FMOD.Factory.System_Create(out t_system);

        System.IntPtr temp2 = new System.IntPtr();
        // Initialise FMOD
        result = t_system.init(100, FMOD.INITFLAGS.NORMAL, temp2);

        return t_system;
    }

    public void PlayMidi()
    {
        system.playSound(sound, masterCG, false, out channel);
    }

    public void PauseMidi()
    {
        masterCG.setPaused(true);

    }
    public void ResumeMidi()
    {
        masterCG.setPaused(false);
    }

    public void EndMidi()
    {
        system.release();
    }

    private Sound CreateSound()
    {
        //By default (FMOD_CREATESAMPLE) FMOD will try to load and decompress the whole sound into memory! Use FMOD_CREATESTREAM to open it as a stream and have it play back in realtime!
        //RESULT System.createSound(midiName, FMOD_CREATESTREAM, out Sound sound);
        Sound t_sound;

        system.createSound(midiPath, MODE.CREATESAMPLE, out t_sound);

        channel = new Channel();

        system.getMasterChannelGroup(out masterCG);

        return t_sound;
    }

    private void OnDestroy()
    {
        system.release();
    }

    IEnumerator InitialDelay()
    {
        yield return new WaitForSeconds(2);
        MidiMusicManager midiMusicManager = GetComponent<MidiMusicManager>();
        midiMusicManager.enabled = true;
        yield return new WaitForSeconds(3);
        PlayMidi();
    }



    public void OpenConfig(string filename)
    {
        try
        {
            fileConfig = FileHandler.ReadFromJSON<FileConfig>(filename + "_config.json");
        }
        catch
        {
            UnityEngine.Debug.Log("Could not find config");
        }
    }
}
