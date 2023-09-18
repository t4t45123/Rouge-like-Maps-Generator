using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Transform roomParent;
    [SerializeField] float roomSize = 1f;
    [SerializeField] int minRoomAmount = 16;
    [SerializeField] int maxRoomAmount = 50;
    [SerializeField] Vector2 startPos = Vector2.zero;
    [Range(0, 1)]
    public int SpawnType = 0;
    //[SerializeField] List<GameObject> rooms = new List<GameObject>();

    [SerializeField] int spawnAttempsPerRoom = 25;
    public HashSet<Vector2> RoomLogicalSpace = new HashSet<Vector2>();
    
    public List<Rooms> roomTypes = new List<Rooms>();
    List<Vector2> CurrentRooms = new List<Vector2>();
    List<GameObject> RoomObjects = new List<GameObject>();
    
    
    

    [System.Serializable]
    public class Rooms {
        public string name;
        public int minAmount = 1;
        [Min(1)]
        public int maxAmount = 50;
        public int currentAmount = 0;
        [Range(0, 100)]
        public int percentageOfSpawningRoom;
        public List<GameObject> rooms;

        public void IncrementAmount()
        {
            currentAmount++;
        }
    }
    [ContextMenu("gen")]
    public void GenerateMap()
    {
        
        InstanceRooms(minRoomAmount);
        while (!IsAllRoomsInstanced() )
        {
            InstanceRooms(1);
        }
    }
    [ContextMenu("clear")]
    public void ResetMap()
    {
        foreach (var rooms in roomTypes)
        {
            rooms.currentAmount = 0;
        }
        foreach (GameObject obj in RoomObjects) {
            Destroy(obj);
        }
        RoomLogicalSpace.Clear();
        CurrentRooms.Clear();
        CurrentRooms.Add(Vector2.zero);
        RoomObjects.Clear();
        startPos = Vector2.zero;
    }
    
    public void InstanceRooms(int amount)
    {
        

        for (int i = 0; i < amount; i++)
        {
            
            for (int attempt = 0; attempt < CurrentRooms.Count * 2; attempt++)
            {
                Vector2 tempRoomPosition = Vector2.zero;
                
                bool spawned = false;
                
                if(SpawnType == 0)
                {
                    Vector2 tempStartPosition = CurrentRooms[Random.Range(0, CurrentRooms.Count - 1)];
                    tempRoomPosition = (GenerateDirectionVector() + tempStartPosition);
                }else if(SpawnType == 1)
                {
                    startPos += GenerateDirectionVector();
                    tempRoomPosition = startPos;
                }
                
                if (!RoomLogicalSpace.Contains(tempRoomPosition))
                {
                    InstanceSingleRoom(tempRoomPosition);
                    spawned = true;
                }
                if (spawned) { break; }
            }
        }
    }
    public bool IsAllRoomsInstanced()
    {
        bool isNotInstanced = false;
        foreach (var type in roomTypes)
        {
            
            if (type.currentAmount < type.minAmount)
            {
                isNotInstanced = true;
            }
        }
        return isNotInstanced ? false : true;
        
    }
    public void InstanceSingleRoom(Vector2 LogicalPosition)
    {
        int typeIndex = GetRoomTypeIndex();
        Rooms roomType = roomTypes[typeIndex];
        GameObject room = roomType.rooms[Random.Range(0, roomType.rooms.Count - 1)];
        GameObject roomOBJ = Instantiate(room, LogicalPosition * roomSize,Quaternion.identity, roomParent);
        RoomObjects.Add(roomOBJ);
        RoomLogicalSpace.Add(LogicalPosition);
        CurrentRooms.Add(LogicalPosition);
        roomTypes[typeIndex].IncrementAmount();
    }
    int GetRoomTypeIndex()
    {
        for(int type = 0; type < roomTypes.Count; type++)
        {
            
            int chance = Random.Range(0, 100);
            if (roomTypes[type].percentageOfSpawningRoom >= chance)
            {
                if (roomTypes[type].currentAmount >= roomTypes[type].maxAmount)
                {
                    return GetRoomTypeIndex();
                }
                else
                {
                    return type;
                }
                
            }
            
        }
        return 0;
    }
    Vector2 GenerateDirectionVector()
    {
        float rotation = Random.Range(0, 2*Mathf.PI);
        Vector2 tempPos = new Vector2(Mathf.RoundToInt(Mathf.Sin(rotation)), Mathf.RoundToInt(Mathf.Cos(rotation)));
        if(tempPos.x == 0 || tempPos.y == 0)
        {
            return tempPos;
        }
        else
        {
            return GenerateDirectionVector();
        }
        
    }
    private void Start()
    {
        CurrentRooms.Add(Vector2.zero);
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GenerateMap();
        }
        if(Input.GetMouseButtonDown(1))
        {
            ResetMap();
        }
    }
    private void OnDrawGizmos()
    {
        foreach (Vector2 pos in CurrentRooms)
        {
            Gizmos.DrawSphere(pos, 0.2f);
        }
    }
}
