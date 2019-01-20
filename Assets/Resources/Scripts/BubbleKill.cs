using UnityEngine;

public class BubbleKill : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    private void OnParticleTrigger()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Water");
        GameObject closesttilemap;

        if (objects.Length >= 1)
        {
            closesttilemap = objects[0];
            if (objects.Length > 1)
            {
                for (int i = 1; i < objects.Length; i++)
                {
                    if (Vector3.Distance(objects[i].transform.position, transform.position) < Vector3.Distance(closesttilemap.transform.position, transform.position))
                        closesttilemap = objects[i];
                }
            }
        }
        else
        {
            Debug.LogError(this.name + " : no water map found");
            return;
        }

        GameObject movingWaterTilemap = GameObject.FindGameObjectWithTag("MovingWater");

        int count = ps.GetParticles(particles);
        for (int i = 0; i < count; i++)
        {

            /* if (particles[i].position.y > (closesttilemap.GetComponent<CompositeCollider2D>().bounds.center.y + closesttilemap.GetComponent<CompositeCollider2D>().bounds.extents.y))
             {
                 particles[i].remainingLifetime = -1.0f;
             }*/
            Vector2 pos = particles[i].position;
        if (!closesttilemap.GetComponent<CompositeCollider2D>().OverlapPoint(pos) && !movingWaterTilemap.GetComponent<CompositeCollider2D>().OverlapPoint(pos))
                particles[i].remainingLifetime = -1.0f;
        }
        ps.SetParticles(particles, count);

    }

}

