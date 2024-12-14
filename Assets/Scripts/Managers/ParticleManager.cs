using UnityEngine;

//to make particle appear above canvas: canvas render mode -> screen space camera

[RequireComponent(typeof(ParticleSystem))]
public class ParticleManager : MonoBehaviour
{
    private ParticleSystem particles;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            particles.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);

            particles.Play();
        }
    }
}
