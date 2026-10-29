using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Keyboard input for piano - maps keyboard keys to piano keys.
/// 
/// Mapping (cu salt de octavă pe fiecare rând):
///   Row 1 (Q W E R T Y U I) → DO4 .. DO5  (8 keys, MIDI 60..67) [last key = DO5? see note]
///   Row 2 (A S D F G H J)   → DO5 .. SI5  (7 keys, MIDI 72..78)
///   Row 3 (Z X C V B N M)   → DO6 .. FA#6 (7 keys, MIDI 84..90)
/// 
/// NOTE: Pentru rândul 1, "DO4 până la DO5" sunt 8 note albe pe pian (DO RE MI FA SOL LA SI DO),
/// dar dacă mapăm cromatic 8 clape consecutive de la MIDI 60 ajungem la MIDI 67 (SOL4), nu DO5.
/// Folosim 8 note ALBE consecutive (cum sunt clapele albe pe pianul fizic): DO4 RE4 MI4 FA4 SOL4 LA4 SI4 DO5.
/// La fel pentru celelalte rânduri (note albe consecutive).
/// 
/// Final mapping (white keys only, ca pe un pian real):
///   Row 1: Q=DO4(60) W=RE4(62) E=MI4(64) R=FA4(65) T=SOL4(67) Y=LA4(69) U=SI4(71) I=DO5(72)
///   Row 2: A=DO5(72) S=RE5(74) D=MI5(76) F=FA5(77) G=SOL5(79) H=LA5(81) J=SI5(83)
///   Row 3: Z=DO6(84) X=RE6(86) C=MI6(88) V=FA6(89) B=SOL6(91) N=LA6(93) M=FA#6(90)
/// </summary>
public class KeyboardPianoInput : MonoBehaviour
{
    [Header("=== PIANO KEYS REFERENCE ===")]
    [Tooltip("Reference to all piano keys in the scene (auto-found if empty)")]
    public PianoKeyEnhanced[] pianoKeys;

    [Header("=== SETTINGS ===")]
    [Tooltip("Enable keyboard input (disabled when not at piano)")]
    public bool inputEnabled = false;

    [Tooltip("Show key mapping help in console at start")]
    public bool logKeyMappingAtStart = true;

    // Keyboard mapping to MIDI notes
    private Dictionary<KeyCode, int> keyToMidiMap = new Dictionary<KeyCode, int>();

    // Track which keys are currently pressed (for clean release on disable)
    private HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

    // Cache: midi -> piano key (faster lookup, avoids per-frame linear search)
    private Dictionary<int, PianoKeyEnhanced> midiToKey = new Dictionary<int, PianoKeyEnhanced>();

    void Start()
    {
        InitializeKeyMapping();

        // Find all piano keys if not assigned
        if (pianoKeys == null || pianoKeys.Length == 0)
        {
            pianoKeys = FindObjectsByType<PianoKeyEnhanced>(FindObjectsSortMode.None);
        }

        // Build MIDI->Key cache
        foreach (var key in pianoKeys)
        {
            if (key != null && !midiToKey.ContainsKey(key.noteNumber))
                midiToKey.Add(key.noteNumber, key);
        }

        Debug.Log($"[KeyboardPiano] Found {pianoKeys.Length} piano keys, cached {midiToKey.Count} unique MIDI notes");

        if (logKeyMappingAtStart)
            Debug.Log(GetKeyMappingHelp());
    }

    void InitializeKeyMapping()
    {
        keyToMidiMap.Clear();

        // ─────────────────────────────────────────────────────────
        // Row 1: white keys DO4 → DO5 (8 keys)
        // Q W E R T Y U I = DO4 RE4 MI4 FA4 SOL4 LA4 SI4 DO5
        // ─────────────────────────────────────────────────────────
        keyToMidiMap[KeyCode.Q] = 60; // DO4
        keyToMidiMap[KeyCode.W] = 62; // RE4
        keyToMidiMap[KeyCode.E] = 64; // MI4
        keyToMidiMap[KeyCode.R] = 65; // FA4
        keyToMidiMap[KeyCode.T] = 67; // SOL4
        keyToMidiMap[KeyCode.Y] = 69; // LA4
        keyToMidiMap[KeyCode.U] = 71; // SI4
        keyToMidiMap[KeyCode.I] = 72; // DO5

        // ─────────────────────────────────────────────────────────
        // Row 2: white keys DO5 → SI5 (7 keys)
        // A S D F G H J = DO5 RE5 MI5 FA5 SOL5 LA5 SI5
        // ─────────────────────────────────────────────────────────
        keyToMidiMap[KeyCode.A] = 72; // DO5
        keyToMidiMap[KeyCode.S] = 74; // RE5
        keyToMidiMap[KeyCode.D] = 76; // MI5
        keyToMidiMap[KeyCode.F] = 77; // FA5
        keyToMidiMap[KeyCode.G] = 79; // SOL5
        keyToMidiMap[KeyCode.H] = 81; // LA5
        keyToMidiMap[KeyCode.J] = 83; // SI5

        // ─────────────────────────────────────────────────────────
        // Row 3: white keys DO6 → SI6 (7 keys); ultima e FA#6 cum ai cerut
        // Z X C V B N M = DO6 RE6 MI6 FA6 SOL6 LA6 FA#6
        // ─────────────────────────────────────────────────────────
        keyToMidiMap[KeyCode.Z] = 84; // DO6
        keyToMidiMap[KeyCode.X] = 86; // RE6
        keyToMidiMap[KeyCode.C] = 88; // MI6
        keyToMidiMap[KeyCode.V] = 89; // FA6
        keyToMidiMap[KeyCode.B] = 91; // SOL6
        keyToMidiMap[KeyCode.N] = 93; // LA6
        keyToMidiMap[KeyCode.M] = 90; // FA#6

        Debug.Log($"[KeyboardPiano] Initialized {keyToMidiMap.Count} key mappings");
    }

    void Update()
    {
        if (!inputEnabled) return;

        // Iterate via list of keys to allow safe modification of pressedKeys
        foreach (var kvp in keyToMidiMap)
        {
            KeyCode keyCode = kvp.Key;
            int midiNote = kvp.Value;

            if (Input.GetKeyDown(keyCode))
            {
                pressedKeys.Add(keyCode);
                TriggerPianoKey(midiNote, true);
            }
            else if (Input.GetKeyUp(keyCode))
            {
                pressedKeys.Remove(keyCode);
                TriggerPianoKey(midiNote, false);
            }
        }
    }

    void TriggerPianoKey(int midiNote, bool press)
    {
        PianoKeyEnhanced pianoKey = GetPianoKeyByMidi(midiNote);

        if (pianoKey == null)
        {
            Debug.LogWarning($"[KeyboardPiano] Piano key for MIDI {midiNote} not found in scene!");
            return;
        }

        if (press)
            pianoKey.PressKey();
        else
            pianoKey.ReleaseKey();
    }

    PianoKeyEnhanced GetPianoKeyByMidi(int midiNote)
    {
        if (midiToKey.TryGetValue(midiNote, out PianoKeyEnhanced key))
            return key;
        return null;
    }

    /// <summary>
    /// Enable/disable keyboard input (called by GhostMaestroManager).
    /// On disable, releases any currently-pressed keys cleanly.
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        if (!enabled && inputEnabled)
        {
            // Release all currently pressed keys cleanly before disabling
            foreach (var keyCode in pressedKeys)
            {
                if (keyToMidiMap.TryGetValue(keyCode, out int midiNote))
                    TriggerPianoKey(midiNote, false);
            }
            pressedKeys.Clear();
        }

        inputEnabled = enabled;
        Debug.Log($"[KeyboardPiano] Input {(enabled ? "ENABLED" : "DISABLED")}");
    }

    /// <summary>
    /// Check if a given KeyCode is mapped to a piano note.
    /// Useful for the manager to avoid double-handling X (sit down vs piano key).
    /// </summary>
    public bool IsPianoKey(KeyCode keyCode)
    {
        return keyToMidiMap.ContainsKey(keyCode);
    }

    /// <summary>
    /// Get list of all mapped keys for UI display
    /// </summary>
    public string GetKeyMappingHelp()
    {
        return "PIANO KEYS:\n" +
               "Row 1: Q W E R T Y U I  →  DO4 RE4 MI4 FA4 SOL4 LA4 SI4 DO5\n" +
               "Row 2: A S D F G H J    →  DO5 RE5 MI5 FA5 SOL5 LA5 SI5\n" +
               "Row 3: Z X C V B N M    →  DO6 RE6 MI6 FA6 SOL6 LA6 FA#6\n\n" +
               "X = Așează-te la pian (când ești aproape)\n" +
               "ESC = Ridică-te de la pian";
    }
}