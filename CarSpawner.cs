using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform counterPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float spawnInterval = 3f;

    private bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnCars());
    }

    private IEnumerator SpawnCars()
    {
        while (isSpawning)
        {
            SpawnCar();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCar()
    {
        // Instantiate the car prefab
        GameObject car = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);

        // Find the child object (carsss_0) and get the CarMovement component
        Transform carChild = car.transform.Find("carsss_0");
        if (carChild != null)
        {
            CarMovement carMovement = carChild.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                carMovement.Initialize(spawnPoint.position, counterPoint.position, exitPoint.position);
            }
            else
            {
                Debug.LogError("CarMovement component is missing on the child object (carsss_0)!");
            }
        }
        else
        {
            Debug.LogError("Child object (carsss_0) not found in the car prefab!");
        }
    }
}