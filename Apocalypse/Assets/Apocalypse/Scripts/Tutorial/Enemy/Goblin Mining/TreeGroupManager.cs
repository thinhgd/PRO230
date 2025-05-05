using System.Collections.Generic;
using UnityEngine;

public class TreeGroupManager : MonoBehaviour
{
    private List<WorkerAI> workers = new List<WorkerAI>();
    private List<MineAI> miners = new List<MineAI>();
    private List<MeaterAI> meaters = new List<MeaterAI>();

    public void RegisterWorker(WorkerAI worker)
    {
        if (!workers.Contains(worker))
        {
            workers.Add(worker);
            Vector3 offset = GetOffsetPosition(workers.Count - 1);
            worker.SetOffsetPosition(transform.position + offset);
        }
    }
    public void RegisterMiner(MineAI mineAI)
    {
        if (!miners.Contains(mineAI))
        {
            miners.Add(mineAI);
            Vector3 offset = GetOffsetPositionMine(miners.Count - 1);
            mineAI.SetOffsetPosition(transform.position + offset);
        }
    }
    public void RegisterMeater(MeaterAI meaterAI)
    {
        if (!meaters.Contains(meaterAI))
        {
            meaters.Add(meaterAI);
            Vector3 offset = GetOffsetPositionMeater(meaters.Count - 1);
            meaterAI.SetOffsetPosition(transform.position + offset);
        }
    }

    public void UnregisterWorker(WorkerAI worker)
    {
        if (workers.Contains(worker))
        {
            workers.Remove(worker);
        }
    }
    public void UnregisterMiner(MineAI mineAI)
    {
        if (miners.Contains(mineAI))
        {
            miners.Remove(mineAI);
        }
    }
    public void UnregisterMeater(MeaterAI meaterAI)
    {
        if (meaters.Contains(meaterAI))
        {
            meaters.Remove(meaterAI);
        }
    }

    private Vector3 GetOffsetPosition(int index)
    {
        float radius = 1f;
        float angle = index * Mathf.PI * 2f / Mathf.Max(1, workers.Count);
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
    }
    private Vector3 GetOffsetPositionMine(int index)
    {
        float radius = 1f;
        float angle = index * Mathf.PI * 2f / Mathf.Max(1, miners.Count);
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
    }
    private Vector3 GetOffsetPositionMeater(int index)
    {
        float radius = 1f;
        float angle = index * Mathf.PI * 2f / Mathf.Max(1, meaters.Count);
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
    }
}
