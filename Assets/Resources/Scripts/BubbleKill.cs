using UnityEngine;

public class BubbleKill : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    [SerializeField] private WaterLevel waterlevel;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        if (waterlevel == null)
            waterlevel = FindObjectOfType<WaterLevel>();
    }

    private void Update()
    {

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {
            if (particles[i].position.y > waterlevel.WaterLevelPosition.position.y)
            {
                particles[i].remainingLifetime = -1.0f;
            }
        }
        ps.SetParticles(particles, count);
    }
}

