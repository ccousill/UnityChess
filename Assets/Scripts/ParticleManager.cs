using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] public ParticleSystem particleSystemPrefab;
    
    void Start(){
        ParticleSystem.MainModule mainModule = particleSystemPrefab.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World; // Set simulation space to world
    }

    public void PlayParticles(Tile tile){
        ParticleSystem particles = Instantiate(particleSystemPrefab,tile.transform.position,Quaternion.LookRotation(Vector3.up),tile.transform);
    }

    public void DeleteParticles(Tile tile)
    {
        // Find and destroy the ParticleSystem associated with the specified piece
        ParticleSystem particle = tile.GetComponentInChildren<ParticleSystem>();
        if (particle != null)
        {
            Destroy(particle.gameObject);
        }
    }
}
