using UnityEngine;

/// <summary>
/// Defines the <see cref="PathSpawnCollider" />
/// </summary>
public class PathSpawnCollider : MonoBehaviour
{
    /// <summary>
    /// Defines the PathSpawnPoints
    /// </summary>
    public Transform[] PathSpawnPoints;

    /// <summary>
    /// Defines the Path
    /// </summary>
    public GameObject Path;

    /// <summary>
    /// The OnTriggerEnter
    /// </summary>
    /// <param name="hit">The hit<see cref="Collider"/></param>
    internal void OnTriggerEnter(Collider hit)
    {
        //player has hit the collider
        if (hit.gameObject.tag == Constants.PlayerTag)
        {
            //find whether the next path will be straight, left or right
            int SpawnPoint = Random.Range(0, PathSpawnPoints.Length);
            for (int i = 0; i < PathSpawnPoints.Length; i++)
            {
                //instantiate the path
                if (i == SpawnPoint)
                    Instantiate(Path, PathSpawnPoints[i].position, PathSpawnPoints[i].rotation);
                else
                {
                    Vector3 position = PathSpawnPoints[i].position;
                }
            }

        }
    }
}
