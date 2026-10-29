using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 50 Progressive Piano Lessons - Complete Piano Mastery
/// VR piano with 22 white keys (C4=60 to A6=93)
/// </summary>
public static class LessonData
{
    public static List<Lesson> GetAllLessons()
    {
        return new List<Lesson>
        {
            // ═══════════════════════════════════════════════════════════════
            // MODULE 1: OCTAVA 4 - PRIMELE NOTE (1-5)
            // ═══════════════════════════════════════════════════════════════
            
            new Lesson { id = 0, title = "Descoperă Pianul", description = "Familiarizează-te cu pianul", estimatedMinutes = 4,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Atinge orice 10 clape să simți cum răspund.", taskType = TaskType.PlayAnyKeys, requiredCount = 10, showHints = false },
                    new LessonPhase { instruction = "Găsește DO4 (cea mai din stânga).", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 60 }, requiredCount = 1, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Apasă DO4 de 5 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 60 }, requiredCount = 5, showHints = true, hintDelay = 3f }
                }, completionMessage = "Ai găsit prima notă!" },

            new Lesson { id = 1, title = "DO RE MI", description = "Primele 3 note", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 60 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "RE4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 62 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "MI4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 64 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO → RE → MI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "MI → RE → DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60 }, requiredCount = 3, showHints = true, hintDelay = 3f }
                }, completionMessage = "Ai învățat DO, RE, MI!" },

            new Lesson { id = 2, title = "FA și SOL", description = "Completează 5 degete", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Recapitulare: DO-RE-MI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64 }, requiredCount = 2, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "FA4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 65 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SOL4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 67 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO-RE-MI-FA-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67 }, requiredCount = 3, showHints = true, hintDelay = 4f }
                }, completionMessage = "Cunoști 5 note!" },

            new Lesson { id = 3, title = "Poziția 5 Degete", description = "Exersează primele 5 note", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Sus: DO-RE-MI-FA-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Jos: SOL-FA-MI-RE-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 65, 64, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Sus și jos!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 65, 64, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 5f }
                }, completionMessage = "Poziția perfectă!" },

            new Lesson { id = 4, title = "Exerciții 5 Note", description = "Consolidează", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Salturi: DO-MI-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 64, 67 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "SOL-MI-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 64, 60 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "Fără ajutor: DO-RE-MI-FA-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67 }, requiredCount = 2, showHints = false }
                }, completionMessage = "Ai stăpânit 5 note!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 2: COMPLETARE OCTAVA 4 (6-8)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 5, title = "LA și SI", description = "Note noi", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "LA4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 69 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SI4 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 71 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SOL-LA-SI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 69, 71 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "FA-SOL-LA-SI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 65, 67, 69, 71 }, requiredCount = 3, showHints = true, hintDelay = 4f }
                }, completionMessage = "Ai învățat LA și SI!" },

            new Lesson { id = 6, title = "DO5 - Completare", description = "Octava completă", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 72 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "LA-SI-DO5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 69, 71, 72 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "Compară: DO4 și DO5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 72, 60, 72 }, requiredCount = 2, showHints = true, hintDelay = 4f }
                }, completionMessage = "Octava 4 completă!" },

            new Lesson { id = 7, title = "Scala DO Major", description = "8 note complete", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala sus: DO-RE-MI-FA-SOL-LA-SI-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Scala jos: DO-SI-LA-SOL-FA-MI-RE-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Sus și jos complet!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Scala fără ajutor!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72 }, requiredCount = 1, showHints = false }
                }, completionMessage = "Scala DO Major perfectă!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 3: PRIMELE MELODII (9-12)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 8, title = "Hot Cross Buns", description = "Prima melodie", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI-RE-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "MI-RE-DO (x2)", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 64, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "DO-DO-DO-DO, RE-RE-RE-RE", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 60, 60, 60, 62, 62, 62, 62 }, requiredCount = 2, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "MELODIA COMPLETĂ!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 64, 62, 60, 60, 60, 60, 60, 62, 62, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Prima melodie!" },

            new Lesson { id = 9, title = "Mary Had a Little Lamb", description = "Melodie clasică", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI-RE-DO-RE-MI-MI-MI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 62, 64, 64, 64 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "RE-RE-RE, MI-SOL-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 62, 62, 62, 64, 67, 67 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Prima jumătate", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 62, 64, 64, 64, 62, 62, 62, 64, 67, 67 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "A doua jumătate", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 62, 64, 64, 64, 64, 62, 62, 64, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "MELODIA COMPLETĂ!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 62, 64, 64, 64, 62, 62, 62, 64, 67, 67, 64, 62, 60, 62, 64, 64, 64, 64, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Mary e mândră de tine!" },

            new Lesson { id = 10, title = "Oda Bucuriei", description = "Beethoven", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI-MI-FA-SOL-SOL-FA-MI-RE", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO-DO-RE-MI-MI-RE-RE", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 60, 62, 64, 64, 62, 62 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO-DO-RE-MI-RE-DO-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "ODA COMPLETĂ!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Beethoven aplaudă!" },

            new Lesson { id = 11, title = "Twinkle Twinkle", description = "Cu salturi", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Practică: DO-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 67, 60, 67, 60, 67 }, requiredCount = 2, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "DO-DO-SOL-SOL-LA-LA-SOL", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 60, 67, 67, 69, 69, 67 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "FA-FA-MI-MI-RE-RE-DO", taskType = TaskType.PlaySequence, targetNotes = new int[] { 65, 65, 64, 64, 62, 62, 60 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SOL-SOL-FA-FA-MI-MI-RE (x2)", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 67, 65, 65, 64, 64, 62, 67, 67, 65, 65, 64, 64, 62 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "TWINKLE COMPLET!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60, 67, 67, 65, 65, 64, 64, 62, 67, 67, 65, 65, 64, 64, 62, 60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Steluța strălucește!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 4: INTRODUCERE RITM (13-15)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 12, title = "Simte Ritmul", description = "30 BPM", estimatedMinutes = 6, useMetronome = true, bpm = 30,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Apasă DO pe fiecare TIC.", taskType = TaskType.PlayOnBeat, targetNotes = new int[] { 60 }, requiredCount = 8, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 500f },
                    new LessonPhase { instruction = "DO-RE pe beat", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62 }, requiredCount = 4, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 500f },
                    new LessonPhase { instruction = "DO-RE-MI pe beat", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64 }, requiredCount = 3, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 450f }
                }, completionMessage = "Ai simțit ritmul!" },

            new Lesson { id = 13, title = "Scala pe Ritm", description = "30 BPM", estimatedMinutes = 6, useMetronome = true, bpm = 30,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO-RE-MI-FA-SOL pe beat", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67 }, requiredCount = 2, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 450f },
                    new LessonPhase { instruction = "Scala completă pe beat", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72 }, requiredCount = 2, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 450f }
                }, completionMessage = "Scala pe ritm!" },

            new Lesson { id = 14, title = "Hot Cross Buns - Ritm", description = "30 BPM", estimatedMinutes = 7, useMetronome = true, bpm = 30,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI-RE-DO pe beat", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 62, 60 }, requiredCount = 3, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 450f },
                    new LessonPhase { instruction = "MELODIA pe beat!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 62, 60, 64, 62, 60, 60, 60, 60, 60, 62, 62, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 450f }
                }, completionMessage = "Prima melodie pe ritm!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 5: OCTAVA 5 (16-19)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 15, title = "Octava 5: DO5 RE5 MI5", description = "Note înalte", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 72 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "RE5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 74 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "MI5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 76 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO5-RE5-MI5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "Compară: DO4-RE4-MI4 apoi DO5-RE5-MI5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 72, 74, 76 }, requiredCount = 2, showHints = true, hintDelay = 4f }
                }, completionMessage = "Note înalte!" },

            new Lesson { id = 16, title = "Octava 5: FA5 SOL5", description = "Continuă", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "FA5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 77 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SOL5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 79 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO5-RE5-MI5-FA5-SOL5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76, 77, 79 }, requiredCount = 3, showHints = true, hintDelay = 4f }
                }, completionMessage = "5 note din Octava 5!" },

            new Lesson { id = 17, title = "Octava 5: LA5 SI5 DO6", description = "Completează", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "LA5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 81 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SI5 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 83 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO6 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 84 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SOL5-LA5-SI5-DO6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 79, 81, 83, 84 }, requiredCount = 3, showHints = true, hintDelay = 4f }
                }, completionMessage = "Octava 5 completă!" },

            new Lesson { id = 18, title = "Scala Octava 5", description = "8 note", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala 5 sus: DO5-RE5-MI5-FA5-SOL5-LA5-SI5-DO6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76, 77, 79, 81, 83, 84 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Scala 5 jos", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 83, 81, 79, 77, 76, 74, 72 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Sus și jos!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76, 77, 79, 81, 83, 84, 83, 81, 79, 77, 76, 74, 72 }, requiredCount = 2, showHints = true, hintDelay = 5f }
                }, completionMessage = "Scala Octava 5!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 6: MELODII OCTAVA 5 (20-22)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 19, title = "Hot Cross Buns - Înalt", description = "Octava 5", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI5-RE5-DO5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 76, 74, 72 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "HOT CROSS BUNS în Octava 5!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 76, 74, 72, 76, 74, 72, 72, 72, 72, 72, 74, 74, 74, 74, 76, 74, 72 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Hot Cross Buns înalt!" },

            new Lesson { id = 20, title = "Oda Bucuriei - Înalt", description = "Octava 5", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI5-MI5-FA5-SOL5-SOL5-FA5-MI5-RE5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 76, 76, 77, 79, 79, 77, 76, 74 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO5-DO5-RE5-MI5-MI5-RE5-RE5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 72, 74, 76, 76, 74, 74 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "ODA în Octava 5!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 76, 76, 77, 79, 79, 77, 76, 74, 72, 72, 74, 76, 76, 74, 74, 76, 76, 77, 79, 79, 77, 76, 74, 72, 72, 74, 76, 74, 72, 72 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Beethoven înalt!" },

            new Lesson { id = 21, title = "Twinkle - Înalt", description = "Octava 5", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO5-DO5-SOL5-SOL5-LA5-LA5-SOL5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 72, 79, 79, 81, 81, 79 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "FA5-FA5-MI5-MI5-RE5-RE5-DO5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 77, 77, 76, 76, 74, 74, 72 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Twinkle în Octava 5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 72, 79, 79, 81, 81, 79, 77, 77, 76, 76, 74, 74, 72 }, requiredCount = 2, showHints = true, hintDelay = 4f }
                }, completionMessage = "Steluța înaltă!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 7: OCTAVA 6 (23-26)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 22, title = "Octava 6: DO6 RE6 MI6", description = "Cele mai înalte", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO6 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 84 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "RE6 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 86 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "MI6 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 88 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "DO6-RE6-MI6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 86, 88 }, requiredCount = 3, showHints = true, hintDelay = 3f }
                }, completionMessage = "Note super înalte!" },

            new Lesson { id = 23, title = "Octava 6: FA6 SOL6 LA6", description = "Ultimele note", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "FA6 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 89 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "SOL6 - apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 91 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "LA6 - ULTIMA NOTĂ! Apasă de 3 ori.", taskType = TaskType.PlaySpecificNote, targetNotes = new int[] { 93 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "MI6-FA6-SOL6-LA6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 88, 89, 91, 93 }, requiredCount = 3, showHints = true, hintDelay = 4f }
                }, completionMessage = "CUNOȘTI TOATE NOTELE!" },

            new Lesson { id = 24, title = "Scala Octava 6", description = "6 note înalte", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala 6: DO6-RE6-MI6-FA6-SOL6-LA6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 86, 88, 89, 91, 93 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Scala 6 jos: LA6-SOL6-FA6-MI6-RE6-DO6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 93, 91, 89, 88, 86, 84 }, requiredCount = 2, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "Sus și jos!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 86, 88, 89, 91, 93, 91, 89, 88, 86, 84 }, requiredCount = 2, showHints = true, hintDelay = 5f }
                }, completionMessage = "Scala Octava 6!" },

            new Lesson { id = 25, title = "Exerciții Octava 6", description = "Consolidare", estimatedMinutes = 5,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO6-MI6-SOL6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 88, 91 }, requiredCount = 3, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "DO6-RE6-MI6-RE6-DO6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 86, 88, 86, 84 }, requiredCount = 3, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Compară 3 DO-uri: DO4-DO5-DO6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 72, 84, 72, 60 }, requiredCount = 2, showHints = true, hintDelay = 4f }
                }, completionMessage = "Octava 6 stăpânită!" },

            // ═══════════════════════════════════════════════════════════════
            // MODULE 8: CONECTARE OCTAVE (26-29)
            // ═══════════════════════════════════════════════════════════════

            new Lesson { id = 26, title = "Conectare Octava 4+5", description = "2 octave", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Octava 4 completă", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72 }, requiredCount = 1, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Continuă cu Octava 5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76, 77, 79, 81, 83, 84 }, requiredCount = 1, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "2 OCTAVE: DO4 până la DO6!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84 }, requiredCount = 2, showHints = true, hintDelay = 5f }
                }, completionMessage = "2 octave conectate!" },

            new Lesson { id = 27, title = "Conectare Octava 5+6", description = "Note înalte", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Octava 5 completă", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76, 77, 79, 81, 83, 84 }, requiredCount = 1, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "Continuă cu Octava 6", taskType = TaskType.PlaySequence, targetNotes = new int[] { 84, 86, 88, 89, 91, 93 }, requiredCount = 1, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "OCTAVA 5+6: DO5 până la LA6!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 72, 74, 76, 77, 79, 81, 83, 84, 86, 88, 89, 91, 93 }, requiredCount = 2, showHints = true, hintDelay = 5f }
                }, completionMessage = "Octavele înalte conectate!" },

            new Lesson { id = 28, title = "Scala pe Tot Pianul", description = "22 note!", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "ÎN SUS: DO4 până la LA6!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84, 86, 88, 89, 91, 93 }, requiredCount = 1, showHints = true, hintDelay = 5f },
                    new LessonPhase { instruction = "ÎN JOS: LA6 până la DO4!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 93, 91, 89, 88, 86, 84, 83, 81, 79, 77, 76, 74, 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 5f }
                }, completionMessage = "22 de note - FENOMENAL!" },

            // MODULE 9: MELODII NOI (29-33)
            new Lesson { id = 29, title = "Happy Birthday", description = "2 octave", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "SOL4-SOL4-LA4-SOL4-DO5-SI4", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 67, 69, 67, 72, 71 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "HAPPY BIRTHDAY COMPLET!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 67, 69, 67, 72, 71, 67, 67, 69, 67, 74, 72, 67, 67, 79, 76, 72, 71, 69, 77, 77, 76, 72, 74, 72 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "LA MULȚI ANI! 🎂" },

            new Lesson { id = 30, title = "Frère Jacques", description = "Melodie franceză", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO-RE-MI-DO (x2)", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 60, 60, 62, 64, 60 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "FRÈRE JACQUES COMPLET!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 60, 60, 62, 64, 60, 64, 65, 67, 64, 65, 67, 67, 69, 67, 65, 64, 60, 67, 69, 67, 65, 64, 60, 60, 67, 60, 60, 67, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Magnifique! 🇫🇷" },

            new Lesson { id = 31, title = "Jingle Bells", description = "Crăciun", estimatedMinutes = 7,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MI-MI-MI, MI-MI-MI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 64, 64, 64, 64, 64 }, requiredCount = 2, showHints = true, hintDelay = 3f },
                    new LessonPhase { instruction = "JINGLE BELLS COMPLET!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 64, 64, 64, 64, 64, 64, 67, 60, 62, 64, 65, 65, 65, 65, 65, 64, 64, 64, 64, 62, 62, 64, 62, 67 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "HO HO HO! 🎄" },

            new Lesson { id = 32, title = "Can Can", description = "Offenbach", estimatedMinutes = 6,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "DO-MI-DO-MI-DO-MI-SOL-MI", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 64, 60, 64, 60, 64, 67, 64 }, requiredCount = 2, showHints = true, hintDelay = 4f },
                    new LessonPhase { instruction = "CAN CAN COMPLET!", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 64, 60, 64, 60, 64, 67, 64, 62, 65, 62, 65, 62, 65, 69, 65, 60, 64, 60, 64, 60, 64, 67, 64 }, requiredCount = 1, showHints = true, hintDelay = 3f }
                }, completionMessage = "Can Can! 💃" },

            // MODULE 10: RITM MEDIU (33-37)
            new Lesson { id = 33, title = "Ritm 40 BPM", description = "Scala pe ritm", estimatedMinutes = 6, useMetronome = true, bpm = 40,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala pe 40 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72 }, requiredCount = 2, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 400f }
                }, completionMessage = "40 BPM!" },

            new Lesson { id = 34, title = "Oda - 40 BPM", description = "Beethoven pe ritm", estimatedMinutes = 7, useMetronome = true, bpm = 40,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "ODA pe 40 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 400f }
                }, completionMessage = "Beethoven pe ritm! 🎼" },

            new Lesson { id = 35, title = "Mary - 45 BPM", description = "Pe ritm", estimatedMinutes = 7, useMetronome = true, bpm = 45,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "MARY pe 45 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 62, 60, 62, 64, 64, 64, 62, 62, 62, 64, 67, 67, 64, 62, 60, 62, 64, 64, 64, 64, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 350f }
                }, completionMessage = "Mary pe ritm!" },

            new Lesson { id = 36, title = "Twinkle - 50 BPM", description = "Steluța pe ritm", estimatedMinutes = 7, useMetronome = true, bpm = 50,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Twinkle pe 50 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 350f }
                }, completionMessage = "Steluța pe 50 BPM! ⭐" },

            new Lesson { id = 37, title = "Frère Jacques - 55 BPM", description = "Pe ritm", estimatedMinutes = 7, useMetronome = true, bpm = 55,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "FRÈRE JACQUES pe 55 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 60, 60, 62, 64, 60, 64, 65, 67, 64, 65, 67, 67, 69, 67, 65, 64, 60, 67, 69, 67, 65, 64, 60, 60, 67, 60, 60, 67, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 300f }
                }, completionMessage = "Sonnez les matines! 🔔" },

            // MODULE 11: RITM RAPID (38-44)
            new Lesson { id = 38, title = "Scala - 60 BPM", description = "Un beat/secundă", estimatedMinutes = 6, useMetronome = true, bpm = 60,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala pe 60 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 280f }
                }, completionMessage = "60 BPM - viteză!" },

            new Lesson { id = 39, title = "Hot Cross Buns - 65 BPM", description = "Rapid", estimatedMinutes = 6, useMetronome = true, bpm = 65,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "HOT CROSS BUNS pe 65 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 62, 60, 64, 62, 60, 60, 60, 60, 60, 62, 62, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 260f }
                }, completionMessage = "Viteză impresionantă!" },

            new Lesson { id = 40, title = "Oda - 70 BPM", description = "Beethoven rapid", estimatedMinutes = 7, useMetronome = true, bpm = 70,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "ODA pe 70 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 250f }
                }, completionMessage = "Beethoven impresionat! 🎹" },

            new Lesson { id = 41, title = "Jingle Bells - 75 BPM", description = "Clopotele rapide", estimatedMinutes = 7, useMetronome = true, bpm = 75,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "JINGLE BELLS pe 75 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 64, 64, 64, 64, 64, 64, 67, 60, 62, 64, 65, 65, 65, 65, 65, 64, 64, 64, 64, 62, 62, 64, 62, 67 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 230f }
                }, completionMessage = "Clopotele sună rapid! 🔔" },

            new Lesson { id = 42, title = "Happy Birthday - 80 BPM", description = "Rapid!", estimatedMinutes = 7, useMetronome = true, bpm = 80,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "HAPPY BIRTHDAY pe 80 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 67, 67, 69, 67, 72, 71, 67, 67, 69, 67, 74, 72, 67, 67, 79, 76, 72, 71, 69, 77, 77, 76, 72, 74, 72 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 220f }
                }, completionMessage = "La mulți ani rapid! 🎂" },

            new Lesson { id = 43, title = "Scala Completă - 60 BPM", description = "22 note pe ritm", estimatedMinutes = 8, useMetronome = true, bpm = 60,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "TOATE notele pe 60 BPM!", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84, 86, 88, 89, 91, 93 }, requiredCount = 1, showHints = true, hintDelay = 4f, timingRequired = true, timingToleranceMs = 280f }
                }, completionMessage = "22 note pe ritm - MAESTRU!" },

            // MODULE 12: RECITALURI (44-50)
            new Lesson { id = 44, title = "🎹 Recital Octava 4", description = "Melodii joase", estimatedMinutes = 10,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Hot Cross Buns", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 64, 62, 60, 60, 60, 60, 60, 62, 62, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 2f },
                    new LessonPhase { instruction = "Mary Had a Little Lamb", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 62, 60, 62, 64, 64, 64, 62, 62, 62, 64, 67, 67, 64, 62, 60, 62, 64, 64, 64, 64, 62, 62, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 2f },
                    new LessonPhase { instruction = "Oda Bucuriei", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = true, hintDelay = 2f }
                }, completionMessage = "🌟 Recital Octava 4! 🌟" },

            new Lesson { id = 45, title = "🎹 Recital Octava 5", description = "Melodii înalte", estimatedMinutes = 10,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Hot Cross Buns - Octava 5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 76, 74, 72, 76, 74, 72, 72, 72, 72, 72, 74, 74, 74, 74, 76, 74, 72 }, requiredCount = 1, showHints = true, hintDelay = 2f },
                    new LessonPhase { instruction = "Oda Bucuriei - Octava 5", taskType = TaskType.PlaySequence, targetNotes = new int[] { 76, 76, 77, 79, 79, 77, 76, 74, 72, 72, 74, 76, 76, 74, 74, 76, 76, 77, 79, 79, 77, 76, 74, 72, 72, 74, 76, 74, 72, 72 }, requiredCount = 1, showHints = true, hintDelay = 2f }
                }, completionMessage = "🌟 Recital Octava 5! 🌟" },

            new Lesson { id = 46, title = "🎹 Recital pe Ritm - 50 BPM", description = "Melodii pe ritm lent", estimatedMinutes = 12, useMetronome = true, bpm = 50,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala pe 50 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 350f },
                    new LessonPhase { instruction = "Oda pe 50 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = true, hintDelay = 2f, timingRequired = true, timingToleranceMs = 350f }
                }, completionMessage = "🌟 Recital pe ritm lent! 🌟" },

            new Lesson { id = 47, title = "🎹 Recital pe Ritm - 70 BPM", description = "Melodii pe ritm rapid", estimatedMinutes = 12, useMetronome = true, bpm = 70, isFinalExam = true,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala completă pe 70 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84, 86, 88, 89, 91, 93 }, requiredCount = 1, showHints = true, hintDelay = 3f, timingRequired = true, timingToleranceMs = 250f },
                    new LessonPhase { instruction = "Oda pe 70 BPM", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = true, hintDelay = 2f, timingRequired = true, timingToleranceMs = 250f }
                }, completionMessage = "🌟 Recital pe ritm rapid! 🌟" },

            new Lesson { id = 48, title = "🎹🌟 RECITAL COMPLET 🌟🎹", description = "Fără ajutor!", estimatedMinutes = 15, isFinalExam = true,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala completă (fără ajutor!)", taskType = TaskType.PlaySequence, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84, 86, 88, 89, 91, 93, 93, 91, 89, 88, 86, 84, 83, 81, 79, 77, 76, 74, 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 1, showHints = false },
                    new LessonPhase { instruction = "Oda Bucuriei (fără ajutor!)", taskType = TaskType.PlaySequence, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = false },
                    new LessonPhase { instruction = "Happy Birthday (fără ajutor!)", taskType = TaskType.PlaySequence, targetNotes = new int[] { 67, 67, 69, 67, 72, 71, 67, 67, 69, 67, 74, 72, 67, 67, 79, 76, 72, 71, 69, 77, 77, 76, 72, 74, 72 }, requiredCount = 1, showHints = false }
                }, completionMessage = "🎉 RECITAL COMPLET FĂRĂ AJUTOR! 🎉" },

            new Lesson { id = 49, title = "🎹🌟 MAESTRUL PIANULUI 🌟🎹", description = "EXAMEN FINAL - 80 BPM!", estimatedMinutes = 20, isFinalExam = true, useMetronome = true, bpm = 80,
                phases = new List<LessonPhase> {
                    new LessonPhase { instruction = "Scala pe 80 BPM (fără ajutor!)", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79, 81, 83, 84, 86, 88, 89, 91, 93, 93, 91, 89, 88, 86, 84, 83, 81, 79, 77, 76, 74, 72, 71, 69, 67, 65, 64, 62, 60 }, requiredCount = 1, showHints = false, timingRequired = true, timingToleranceMs = 220f },
                    new LessonPhase { instruction = "Oda pe 80 BPM (fără ajutor!)", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 64, 62, 62, 64, 64, 65, 67, 67, 65, 64, 62, 60, 60, 62, 64, 62, 60, 60 }, requiredCount = 1, showHints = false, timingRequired = true, timingToleranceMs = 220f },
                    new LessonPhase { instruction = "Twinkle pe 80 BPM (fără ajutor!)", taskType = TaskType.PlaySequenceOnBeat, targetNotes = new int[] { 60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60, 67, 67, 65, 65, 64, 64, 62, 67, 67, 65, 65, 64, 64, 62, 60, 60, 67, 67, 69, 69, 67, 65, 65, 64, 64, 62, 62, 60 }, requiredCount = 1, showHints = false, timingRequired = true, timingToleranceMs = 220f },
                    new LessonPhase { instruction = "IMPROVIZAȚIE: 100 de note din inimă!", taskType = TaskType.PlayAnyKeys, requiredCount = 100, showHints = false }
                }, completionMessage = "🎉🎹🌟 FELICITĂRI, MAESTRULE! 🌟🎹🎉\n\nAi absolvit TOATE cele 50 de lecții!\nCunoști TOATE cele 22 de note!\nCânți pe ritm la 80 BPM!\n\nEȘTI UN PIANIST ADEVĂRAT!" }
        };
    }
}

[System.Serializable]
public class Lesson
{
    public int id;
    public string title;
    public string description;
    public int estimatedMinutes;
    public List<LessonPhase> phases;
    public string completionMessage;
    public bool useMetronome = false;
    public int bpm = 60;
    public bool isFinalExam = false;
}

[System.Serializable]
public class LessonPhase
{
    public string instruction;
    public TaskType taskType;
    public int[] targetNotes;
    public int requiredCount;
    public bool showHints;
    public float hintDelay;
    public bool timingRequired = false;
    public float timingToleranceMs = 250f;
}

public enum TaskType
{
    PlayAnyKeys,
    PlaySpecificNote,
    PlaySequence,
    PlayOnBeat,
    PlaySequenceOnBeat
}