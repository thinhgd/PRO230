using UnityEngine;

public class ConstructionProgress : MonoBehaviour
{
    public float totalBuildTime = 5f;
    public GameObject finalBuildingPrefab;
    public Transform parentOnComplete;

    private float currentBuildProgress = 0f;
    private BuilderAI assignedBuilder;

    public bool IsFullyBuilt() => currentBuildProgress >= totalBuildTime;
    public bool HasBuilderAssigned() => assignedBuilder != null;

    public void AssignBuilder(BuilderAI builder)
    {
        assignedBuilder = builder;
    }

    public void WorkOnConstruction(BuilderAI builder)
    {
        if (IsFullyBuilt()) return;

        currentBuildProgress += 1f;

        if (currentBuildProgress >= totalBuildTime)
        {
            FinishConstruction();
            builder.FinishConstruction();
        }
    }

    private void FinishConstruction()
    {
        GameObject finished = Instantiate(finalBuildingPrefab, transform.position, Quaternion.identity, parentOnComplete);
        Destroy(gameObject);
    }
    public void StartConstruction(Vector3 position)
    {
        transform.position = position;
        currentBuildProgress = 0f;
    }

}
