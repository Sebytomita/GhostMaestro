// SETUP INSTRUCTIONS FOR PARTICLE EFFECTS
// ==========================================

/*
QUICK PARTICLE SETUP (5 minutes):

1. CREATE CORRECT PARTICLE (Green):
   -----------------------------------
   - GameObject → Effects → Particle System → Rename "CorrectParticle"
   - Particle System settings:
     * Duration: 0.5
     * Start Lifetime: 0.3-0.5
     * Start Speed: 2-3
     * Start Size: 0.1-0.2
     * Start Color: Green (0.5, 1, 0.5, 1)
     * Emission → Rate: 20
     * Shape → Cone, Angle: 15
     * Color over Lifetime: Green → Transparent
   - Drag to Project to make Prefab
   
2. CREATE INCORRECT PARTICLE (Red):
   ----------------------------------
   - Duplicate "CorrectParticle" → Rename "IncorrectParticle"
   - Change:
     * Start Color: Red (1, 0.5, 0.5, 1)
     * Start Speed: 1-2 (slower)
     * Shape → Sphere
   - Drag to Project to make Prefab

3. ASSIGN TO VISUAL HINT SYSTEM:
   --------------------------------
   - Select VisualHintSystem GameObject
   - Drag "CorrectParticle" → Correct Particle Prefab
   - Drag "IncorrectParticle" → Incorrect Particle Prefab

ALTERNATIVE - NO PARTICLES:
---------------------------
If you don't want to create particles, just leave those fields empty.
The system will still work with color feedback on keys!

MATERIALS FOR HIGHLIGHTS:
------------------------
You may want to create these materials:

1. HighlightMaterial (Yellow/Gold):
   - Create → Material → "HighlightMaterial"
   - Color: (1, 1, 0.5, 0.8) - Yellow semi-transparent
   - Rendering Mode: Transparent
   - Emission: Enabled, Color: Yellow

2. Assign to each PianoKeyEnhanced:
   - Select all piano keys
   - In PianoKeyEnhanced → Highlight Material: HighlightMaterial

*/

using UnityEngine;

public class ParticleSetupHelper : MonoBehaviour
{
    [ContextMenu("Create Correct Particle Prefab")]
    void CreateCorrectParticle()
    {
        CreateParticlePrefab("CorrectParticle", new Color(0.5f, 1f, 0.5f, 1f), 20, 3f);
    }

    [ContextMenu("Create Incorrect Particle Prefab")]
    void CreateIncorrectParticle()
    {
        CreateParticlePrefab("IncorrectParticle", new Color(1f, 0.5f, 0.5f, 1f), 15, 1.5f);
    }

    void CreateParticlePrefab(string name, Color color, int emissionRate, float speed)
    {
        GameObject particleGO = new GameObject(name);
        ParticleSystem ps = particleGO.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = speed;
        main.startSize = 0.15f;
        main.startColor = color;

        var emission = ps.emission;
        emission.rateOverTime = emissionRate;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );

        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        Debug.Log($"Created {name} - Drag to Project folder to make Prefab!");
    }
}