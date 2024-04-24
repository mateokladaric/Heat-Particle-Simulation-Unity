using UnityEngine;

public class Particles : MonoBehaviour
{
	public Sprite particleSprite;
	public float spriteSize = 8f;
	public float timeStep = 0.001f;
	public float chanceOfHeat = 0.1f;
	public int scale = 2;
	private float[,] particles;
	private GameObject[,] particleObjects;

	void Start()
	{
		// Initialize the particles array and particleObjects array
		particles = new float[scale, scale];
		particleObjects = new GameObject[scale, scale];

		// Generate random heat values for each particle
		for (int i = 0; i < scale; i++)
		{
			for (int j = 0; j < scale; j++)
			{
				if (Random.Range(0f, 1f) < chanceOfHeat)
				{
					particles[i, j] = 1f;
				}
				else
				{
					particles[i, j] = 0f;
				}
			}
		}

		// Create game objects for each particle and set their positions, scale, and sprite
		for (int i = 0; i < scale; i++)
		{
			for (int j = 0; j < scale; j++)
			{
				GameObject particle = new GameObject("particle");
				particle.transform.position = new Vector3((i / spriteSize) - (scale / 2 / spriteSize), (j / spriteSize) - (scale / 2 / spriteSize), 0);
				particle.transform.parent = transform;
				particle.transform.localScale = new Vector3(1f / spriteSize, 1f / spriteSize, 1f / spriteSize);

				SpriteRenderer spriteRenderer = particle.AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = particleSprite;

				particleObjects[i, j] = particle;

				// Set the color of the particle based on its heat value
				if (particles[i, j] > 0f)
				{
					spriteRenderer.color = new Color(particles[i, j] * 255f, 0, 0);
				}
				else
				{
					spriteRenderer.color = Color.blue;
				}
			}
		}
	}

	void Update()
	{
		// Update the heat values of each particle based on its neighbors
		for (int i = 0; i < scale; i++)
		{
			for (int j = 0; j < scale; j++)
			{
				float currentHeat = particles[i, j];
				float[] neighbourHeat = new float[4];

				// Get the heat values of the particle's neighbors
				neighbourHeat[0] = i > 0 ? particles[i - 1, j] : particles[i, j];
				neighbourHeat[1] = i < scale - 1 ? particles[i + 1, j] : particles[i, j];
				neighbourHeat[2] = j > 0 ? particles[i, j - 1] : particles[i, j];
				neighbourHeat[3] = j < scale - 1 ? particles[i, j + 1] : particles[i, j];

				float newHeat = currentHeat;
				for (int k = 0; k < 4; k++)
				{
					// Calculate the new heat value based on the difference between the current heat and the neighbor heat
					newHeat += (neighbourHeat[k] - currentHeat) * timeStep;

					// Reduce the neighbor heat based on the heat transfer
					if (neighbourHeat[k] > 0)
					{
						neighbourHeat[k] -= (neighbourHeat[k] - currentHeat) * timeStep;
						neighbourHeat[k] = Mathf.Max(neighbourHeat[k], 0);
					}
				}

				// Update the heat value of the particle
				particles[i, j] = newHeat;

				// Update the heat values of the neighboring particles
				if (i > 0 && j > 0 && i < (scale - 1) && j < (scale - 1))
				{
					particles[i - 1, j] = neighbourHeat[0];
					particles[i + 1, j] = neighbourHeat[1];
					particles[i, j - 1] = neighbourHeat[2];
					particles[i, j + 1] = neighbourHeat[3];
				}

				// Update the color of the particle based on its heat value
				if (particles[i, j] > 0f)
				{
					particleObjects[i, j].GetComponent<SpriteRenderer>().color = new Color(particles[i, j] * 1f, 0, 1f - (particles[i, j] * 1f));
				}
				else
				{
					particleObjects[i, j].GetComponent<SpriteRenderer>().color = new Color(0, 0, 1f);
				}
			}
		}
	}
}
