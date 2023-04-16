using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public GameObject EnemySet;
    public GameObject Blast;
    public AudioSource BlastSound;

    ParticleSystem ps;

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        // iterate through the particles which entered the trigger and make them red
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            EnemySet.GetComponent<Turtle>()._health--;
            if (EnemySet.GetComponent<Turtle>()._health == 0)
            {
                Destroy(EnemySet);
            }
            p.remainingLifetime = 0;
            enter[i] = p;
            Blast.transform.position = EnemySet.transform.position;
            Blast.GetComponent<ParticleSystem>().Play();
            BlastSound.Play();
        }

        // re-assign the modified particles back into the particle system
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}
