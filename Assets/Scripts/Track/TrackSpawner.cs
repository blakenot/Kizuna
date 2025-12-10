using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    [Header("Orb Settings")]
    [SerializeField] GameObject redOrbPrefab;
    [SerializeField] GameObject greenOrbPrefab;
    [SerializeField] GameObject blueOrbPrefab;

    [SerializeField, Range(0f, 1f)] float orbSpawnChance = 0.25f;
    [SerializeField] float orbHeightOffset = 1.2f;

    // Prevent bundles
    [SerializeField] float minOrbRowDistance = 2f;
    float lastOrbZ = -999f;


    [Header("Track Prefabs")]
    [SerializeField] GameObject whiteTrack;
    [SerializeField] GameObject redTrack;
    [SerializeField] GameObject greenTrack;
    [SerializeField] GameObject blueTrack;
    [SerializeField] GameObject blackTrack;

    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Transform parentContainer;

    [Header("Track Settings")]
    [SerializeField] float segmentLength = 20f;
    [SerializeField] int rowsAhead = 10;
    [SerializeField] int rowsBehind = 2;
    [SerializeField] float spawnY = 0f;
    [SerializeField] float[] laneX = new float[] { -3.8f, 0f, 3.8f };
    [SerializeField] int whiteRowsAtStart = 2;

    readonly Dictionary<int, List<GameObject>> rows = new Dictionary<int, List<GameObject>>();
    float baseZ;
    int minRowIndex;
    int maxRowIndex;

    void Start()
    {
        if (!player)
        {
            enabled = false;
            return;
        }

        baseZ = Mathf.Floor(player.position.z / segmentLength) * segmentLength;
        minRowIndex = -rowsBehind;
        maxRowIndex = rowsAhead;

        for (int i = minRowIndex; i <= maxRowIndex; i++) SpawnRow(i);
    }

    void Update()
    {
        if (!player) return;

        int playerRow = Mathf.FloorToInt((player.position.z - baseZ) / segmentLength);

        while (maxRowIndex < playerRow + rowsAhead)
        {
            maxRowIndex++;
            SpawnRow(maxRowIndex);
        }

        while (minRowIndex < playerRow - rowsBehind)
        {
            DestroyRow(minRowIndex);
            rows.Remove(minRowIndex);
            minRowIndex++;
        }
    }

    void SpawnRow(int rowIndex)
    {
        if (rows.ContainsKey(rowIndex)) return;


      

        float z = baseZ + rowIndex * segmentLength;
        var rowList = new List<GameObject>();

        for (int i = 0; i < laneX.Length; i++)
        {
            GameObject prefab = ChoosePrefab(rowIndex);
            if (!prefab) continue;

            Vector3 pos = new Vector3(laneX[i], spawnY, z);
            GameObject seg = Instantiate(prefab, pos, Quaternion.identity, parentContainer);

            Rigidbody rb = seg.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            TrackSegment ts = seg.GetComponent<TrackSegment>();
            if (ts != null) ts.Initialize(pos);

            rowList.Add(seg);

            TrySpawnOrb(laneX[i], spawnY, z, ts);

        }

        rows[rowIndex] = rowList;
    }

    void DestroyRow(int rowIndex)
    {
        if (!rows.TryGetValue(rowIndex, out var rowList)) return;
        for (int i = 0; i < rowList.Count; i++)
        {
            if (rowList[i]) Destroy(rowList[i]);
        }
    }

    GameObject ChoosePrefab(int rowIndex)
    {
        if (rowIndex >= 0 && rowIndex < whiteRowsAtStart) return whiteTrack;

        float r = Random.value;
        if (r < 0.40f) return whiteTrack;
        if (r < 0.55f) return redTrack;
        if (r < 0.70f) return greenTrack;
        if (r < 0.85f) return blueTrack;
        return blackTrack;
    }

    void TrySpawnOrb(float laneX, float baseY, float z, TrackSegment segment)
    {
        if (!segment) return;

        
        if (Mathf.Abs(z - lastOrbZ) < minOrbRowDistance) return;

        if (Random.value > orbSpawnChance) return;

        
        GameObject orbPrefab = ChooseOrbForTile(segment);

        if (!orbPrefab) return;

        Vector3 orbPos = new Vector3(laneX, baseY + orbHeightOffset, z);
        GameObject orb = Instantiate(orbPrefab, orbPos, Quaternion.identity, parentContainer);

     
        var rb = orb.GetComponent<Rigidbody>();
        if (!rb) rb = orb.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        lastOrbZ = z; // store last orb row
    }

    GameObject ChooseOrbForTile(TrackSegment tile)
    {
        switch (tile.TileColor)
        {
            case TrackSegment.ColorState.Red:
                return redOrbPrefab;
            case TrackSegment.ColorState.Green:
                return greenOrbPrefab;
            case TrackSegment.ColorState.Blue:
                return blueOrbPrefab;

            case TrackSegment.ColorState.White:
            case TrackSegment.ColorState.Black:
                
                int r = Random.Range(0, 3);
                if (r == 0) return redOrbPrefab;
                if (r == 1) return greenOrbPrefab;
                return blueOrbPrefab;

            default:
                return null;
        }
    }



}
