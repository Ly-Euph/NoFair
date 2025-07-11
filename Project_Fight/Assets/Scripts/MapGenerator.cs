using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 5x5�P�ʂ̕���(Room_)��ʘH(Path_)�v���t�@�u���A�אڃ��[���ɏ]���Ď����z�u����X�N���v�g
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("�}�b�v�ݒ�")]
    public int width = 5;         // �}�b�v���Z����
    public int height = 5;        // �}�b�v�c�Z����
    public float tileSize = 5f;   // 1�}�X�̃T�C�Y�i�P�ʁj

    private enum TileType { None, Room, Path }
    private TileType[,] map; // �^�C�v���i�[����2�����z��

    private List<GameObject> roomPrefabs = new();
    private List<GameObject> pathPrefabs = new();

    void Start()
    {
        LoadPrefabs();
        GenerateTileMap();
        InstantiateTiles();
    }

    /// <summary>
    /// Resources����Room_��Path_�ŕ��ނ��ēǂݍ���
    /// </summary>
    void LoadPrefabs()
    {
        var allPrefabs = Resources.LoadAll<GameObject>("");

        foreach (var prefab in allPrefabs)
        {
            if (prefab.name.StartsWith("Room_"))
                roomPrefabs.Add(prefab);
            else if (prefab.name.StartsWith("Path_"))
                pathPrefabs.Add(prefab);
        }

        if (roomPrefabs.Count == 0 || pathPrefabs.Count == 0)
            Debug.LogWarning("Room_�܂���Path_�̃v���t�@�u��Resources���ɑ��݂��܂���");
    }

    /// <summary>
    /// �אڃ��[���Ɋ�Â����}�b�v�̘_���\�z�iTileType�z��j
    /// </summary>
    void GenerateTileMap()
    {
        map = new TileType[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int roomCount = CountAdjacent(x, y, TileType.Room);
                int pathCount = CountAdjacent(x, y, TileType.Path);

                if (roomCount >= 1 && pathCount == 0)
                {
                    map[x, y] = TileType.Path; // �����ׂ̗͒ʘH��
                }
                else if (pathCount >= 1 && Random.value < 0.5f)
                {
                    map[x, y] = TileType.Room; // �ʘH�ׂ̗ɕ��������₷��
                }
                else
                {
                    map[x, y] = Random.value < 0.4f ? TileType.Room : TileType.Path;
                }
            }
        }

        // ���S�ɍŒ�1�̕����������ݒu
        map[width / 2, height / 2] = TileType.Room;
    }

    /// <summary>
    /// TileType�ɉ������v���t�@�u�𐶐�����
    /// </summary>
    void InstantiateTiles()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject prefab = null;

                switch (map[x, y])
                {
                    case TileType.Room:
                        prefab = GetRandomPrefab(roomPrefabs);
                        break;
                    case TileType.Path:
                        prefab = GetRandomPrefab(pathPrefabs);
                        break;
                }

                if (prefab != null)
                {
                    Quaternion rot = GetRotationFromName(prefab.name); // �C�ӂŕ����Ή�
                    Instantiate(prefab, pos, rot, this.transform);
                }
            }
        }
    }

    /// <summary>
    /// �w��^�C�v�̎��̓^�C�������J�E���g
    /// </summary>
    int CountAdjacent(int x, int y, TileType type)
    {
        int count = 0;
        Vector2Int[] dirs = {
            new Vector2Int(0, 1),   // ��
            new Vector2Int(0, -1),  // ��
            new Vector2Int(1, 0),   // �E
            new Vector2Int(-1, 0),  // ��
        };

        foreach (var dir in dirs)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                if (map[nx, ny] == type)
                    count++;
            }
        }
        return count;
    }

    /// <summary>
    /// �v���t�@�u���X�g���烉���_����1�擾
    /// </summary>
    GameObject GetRandomPrefab(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// �v���t�@�u���Ɋ܂܂�����������ŉ�]�������ݒ�i�I�v�V�����j
    /// </summary>
    Quaternion GetRotationFromName(string name)
    {
        if (name.Contains("_NS")) return Quaternion.identity;
        if (name.Contains("_EW")) return Quaternion.Euler(0, 90, 0);
        if (name.Contains("_NE")) return Quaternion.Euler(0, 0, 0);
        if (name.Contains("_SE")) return Quaternion.Euler(0, 90, 0);
        if (name.Contains("_SW")) return Quaternion.Euler(0, 180, 0);
        if (name.Contains("_NW")) return Quaternion.Euler(0, 270, 0);
        return Quaternion.identity;
    }
}
