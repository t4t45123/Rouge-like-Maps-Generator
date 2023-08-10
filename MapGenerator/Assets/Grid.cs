using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] float roomSize = 30f;
    [SerializeField] int nodeCount = 16;
    //[SerializeField] List<GameObject> rooms = new List<GameObject>();

    [SerializeField] int spawnAttempsPerRoom = 25;
    public HashSet<Vector2> RoomLogicalSpace = new HashSet<Vector2>();
    
    public List<Rooms> roomTypes = new List<Rooms>();
    List<Vector2> PosibleStartPositions = new List<Vector2>();
    

    [System.Serializable]
    public class Rooms {
        public string name;
        [Range(0,100)]
        public int percentageOfToalRooms;
        public List<GameObject> rooms;
        Rooms(List<GameObject> _rooms)
        {
            
            rooms = _rooms;
        }
    }
    

    [ContextMenu("instance")]
    public void instanceRooms()
    {
        Vector2 startPosition = Vector2.zero;

        for (int i = 0; i < spawnAttempsPerRoom; i++)
        {
            Vector2 tempRoomPosition = (GenerateDirectionVector() + startPosition);

            if (!RoomLogicalSpace.Contains(tempRoomPosition))
            {
                InstanceSingleRoom(tempRoomPosition);
            }
        }
    }
    public void InstanceSingleRoom(Vector2 LogicalPosition)
    {
        RoomLogicalSpace.Add(LogicalPosition);
        PosibleStartPositions.Add(LogicalPosition);
    }
    Vector2 GenerateDirectionVector()
    {
        float rotation = Random.Range(0, 2*Mathf.PI);
        
        return new Vector2 (Mathf.RoundToInt(Mathf.Sin(rotation)), Mathf.RoundToInt(Mathf.Cos(rotation)));
    }
    private void Start()
    {
        PosibleStartPositions.Add(Vector2.zero);
    }
    private void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        foreach (Vector2 pos in PosibleStartPositions)
        {
            Gizmos.DrawSphere(pos, 1f);
        }
    }
}
