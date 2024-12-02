using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] levelPart; // Array of level parts to be spawned
    [SerializeField] private Vector3 nextPartPosition; // Position where the next level part will be spawned

    [SerializeField] private float distanceToSpawnl; // Distance threshold to trigger new part spawning
    [SerializeField] private float distanceToDelete; // Distance threshold to delete old parts
    [SerializeField] private Transform player; // Reference to the player's position for tracking

    void Update()
    {
        // Continuously checks for spawning and deleting platforms in the Update loop
        DeletePlatform();
        GeneratePlatform();
    }

    // Generates a new platform part when the player is close to the next spawn position
    private void GeneratePlatform()
    {
        // Loop to keep generating parts as long as player is within the spawn threshold
        while (Vector2.Distance(player.transform.position, nextPartPosition) < distanceToSpawnl)
        {
            // Choose a random part from the levelPart array
            Transform part = levelPart[Random.Range(0, levelPart.Length)];

            // Adjusts new part position by accounting for the starting position of the part
            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("Start_Position").position.x, 0);

            // Instantiate the selected part at the calculated position
            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);

            // Update the next part position using the end position of the newly instantiated part
            nextPartPosition = newPart.Find("End_Position").position;
        }
    }

    // Deletes old platform parts that are far away from the player
    private void DeletePlatform()
    {
        // Checks if there are child objects (platforms) to delete
        if (transform.childCount > 0)
        {
            // Get the first child (oldest spawned part) for deletion
            Transform partToDelete = transform.GetChild(0);

            // If the part is beyond the delete threshold, destroy it
            if (Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }
}