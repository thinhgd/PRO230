// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MeatGroupManager : MonoBehaviour
// {
//     private List<MeaterAI> workers = new List<WorkerAI>();
//     private List<MineAI> miners = new List<MineAI>();

//     public void RegisterWorker(WorkerAI worker)
//     {
//         if (!workers.Contains(worker))
//         {
//             workers.Add(worker);
//             Vector3 offset = GetOffsetPosition(workers.Count - 1);
//             worker.SetOffsetPosition(transform.position + offset);
//         }
//     }
//     public void RegisterMiner(MineAI mineAI)
//     {
//         if (!miners.Contains(mineAI))
//         {
//             miners.Add(mineAI);
//             Vector3 offset = GetOffsetPosition(workers.Count - 1);
//             mineAI.SetOffsetPosition(transform.position + offset);
//         }
//     }

//     public void UnregisterWorker(WorkerAI worker)
//     {
//         if (workers.Contains(worker))
//         {
//             workers.Remove(worker);
//         }
//     }
//     public void UnregisterMiner(MineAI mineAI)
//     {
//         if (miners.Contains(mineAI))
//         {
//             miners.Remove(mineAI);
//         }
//     }

//     private Vector3 GetOffsetPosition(int index)
//     {
//         float radius = 1f;
//         float angle = index * Mathf.PI * 2f / Mathf.Max(1, workers.Count);
//         return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
//     }
//     private Vector3 GetOffsetPositionMine(int index)
//     {
//         float radius = 1f;
//         float angle = index * Mathf.PI * 2f / Mathf.Max(1, miners.Count);
//         return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
//     }
// }
