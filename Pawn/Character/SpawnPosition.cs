using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Assumes For only one scene, Figure out for scene changing or something
public class SpawnPosition : MonoBehaviour
{
    private static List<SpawnPosition> _spawnPositionList= new List<SpawnPosition>();
    
    public static List<SpawnPosition> SpawnPositionList{get {return _spawnPositionList;}}
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private void Reset()
    {
        _spawnPositionList.Clear();
    }
    private void OnEnable()
    {
        AddToSpawnList(this);
    }
    private void OnDisable()
    {        
        RemoveFromSpawnList(this);
    }
    public void TeleportHere(Transform targetTransform) 
    {
        targetTransform.position=this.transform.position;
    }

    public static void SpawnAtAnyPosition(Transform targetTransform) 
    {
        if(_spawnPositionList.Count<=0)
        {
            Debug.LogError("No Spawn Positions Exist.");
            return;
        }
        int index = Random.Range(0, _spawnPositionList.Count);
        targetTransform.position = _spawnPositionList[index].transform.position;
    }
    public static void SpawnAtAnyPosition(List<Transform> targetTransforms) 
    {
        if(_spawnPositionList.Count<=0)
        {
            Debug.LogWarning("No Spawn Positions Exist.");
            return;
        }
        int index = Random.Range(0, _spawnPositionList.Count);
        foreach(var targetTransform in targetTransforms)
        {
            targetTransform.position = _spawnPositionList[index].transform.position;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw a semitransparent blue cube at the transforms position
        float size=0.5f;
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawSphere(transform.position, size); 
    }
#endif

    private void OnDestroy()
    {
        RemoveFromSpawnList(this);
    }
    private void AddToSpawnList(SpawnPosition playerSpawnPosition)
    {
        if(!_spawnPositionList.Contains(playerSpawnPosition))
        {
            _spawnPositionList.Add(playerSpawnPosition);
        }

    }

    private void RemoveFromSpawnList(SpawnPosition playerSpawnPosition)
    {
        if(_spawnPositionList.Contains(playerSpawnPosition))
        {
            _spawnPositionList.Remove(playerSpawnPosition);
        }
    }
}
