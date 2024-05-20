using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("MAP SETTING")]
    public Camera mainCam;                          //카메라
    public Transform mapPos;                        //맵 위치
    public Texture2D mapTexture;                    //맵 텍스쳐
    public Vector2Int mapSize;                      //맵 크기
    public float tileSize = 4f;                     //타일 크기
    public int maxDivideDepth = 4;                  //맵 생성 깊이
    public float minDividePer = 0.4f;               //최소 맵 자르기 비율
    public float maxDividePer = 0.6f;               //최대 맵 자르기 비율

    [Header("MAP OBJECT SETTING")]
    public int normalMonsterCount = 5;
    public int chestCount = 5;

    [Header("MAP MATERAILS")]
    public List<Material> mapFloorMats;
    public List<Material> mapWallMats;

    [Header("MAP TILES")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject cornerPrefab;
    public GameObject pillarPrefab;

    [Header("MAP DECOS")]
    public List<GameObject> decoPrefabs;

    [Header("MAP OBJECTS")]
    public string chestResName;
    public string traderResName;
    public string potalResName;

    [Header("ITEM OBJECTS")]
    public string itemResName;               //TEST

    public enum RoomType { START, BATTLE, ELITE, TREASURE, TRADER, POTAL, BOSS }            //방 타입
    public enum TileType { EMPTY, FLOOR, WALL, CORNER, PILLAR, PATH }                       //타일 타입
    public enum TileID { FLOOR = 100, WALL, CORNER, PILLAR }                                //타일 ID
    public enum MapObjectID { CHEST=300, TRADER, POTAL }

    public const int MapDecoID = 400;

    [SerializeField] private Color[] tileColors = { Color.white, Color.white, Color.black, Color.black, Color.black, Color.black };
    [SerializeField] private Color myPlayerColor = Color.red;
    [SerializeField] private Color othPlayerColor = Color.blue;

    [HideInInspector] public Transform tilePos;     //타일 위치
    [HideInInspector] public Transform objectPos;   //오브젝트 위치
    [HideInInspector] public Transform poolPos;     //메모리풀 위치

    class RoomNode
    {
        public RectInt rect;
        public RectInt room;
        public RoomNode left;
        public RoomNode right;
        public AxisLine line;

        public RoomNode(RectInt rect) => this.rect = rect;
        public Vector2Int Center { get { return new Vector2Int(room.x + (room.width - 1) / 2, room.y + (room.height - 1) / 2); } }
    }
    public struct AxisLine
    {
        public int xy;
        public bool xAxis;

        public AxisLine(int xy, bool xAxis)
        {
            this.xy = xy;
            this.xAxis = xAxis;
        }
    }
    public struct PathInfo
    {
        public Vector2Int begin;
        public Vector2Int end;
        public AxisLine line;

        public PathInfo(Vector2Int begin, Vector2Int end, AxisLine line)
        {
            this.begin = begin;
            this.end = end;
            this.line = line;
        }
    }
    public struct RoomInfo
    {
        public RoomType type;
        public RectInt rect;

        public RoomInfo(RoomType type, RectInt rect)
        {
            this.type = type;
            this.rect = rect;
        }
    }

    [System.Serializable]
    public class ObjInfo
    {
        public int objID;
        public int roomID;
        public Vector3 pos;

        public ObjInfo(int objID, int roomID, Vector3 pos)
        {
            this.objID = objID;
            this.roomID = roomID;
            this.pos = pos;
        }
    }
    [System.Serializable]
    public class ObjInfoList
    {
        public List<ObjInfo> objs;
        public ObjInfoList(List<ObjInfo> objs) => this.objs = objs;
    }

    List<RectInt> rooms;                //방 리스트
    List<PathInfo> paths;               //통로 리스트
    List<RoomInfo> roomInfos;           //방 정보 리스트
    List<ObjInfo> objects;              //오브젝트 리스트

    GameManager game;
    PhotonView pv;
    PhotonReady pr;

    TileType[] mapTiles;                //맵 타일
    Texture2D tileTexture;              //타일 텍스쳐

    int mapMatIdx = -1;

    Vector3 playerSpawnPoint;
    Dictionary<int, ObjectPool> objectsPool;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
        pv = GetComponent<PhotonView>();
        pr = GetComponent<PhotonReady>();

        rooms = new List<RectInt>();
        paths = new List<PathInfo>();
        roomInfos = new List<RoomInfo>();
        objects = new List<ObjInfo>();

        mapTiles = new TileType[mapSize.x * mapSize.y];
        tileTexture = new Texture2D(mapSize.x, mapSize.y);

        (tilePos = new GameObject("tile").transform).parent = mapPos;
        (objectPos = new GameObject("object").transform).parent = mapPos;
        (poolPos = new GameObject("pool").transform).parent = mapPos;

        objectsPool = new Dictionary<int, ObjectPool>();
    }
    IEnumerator Start()
    {
        game.curScene = gameObject.scene.name;

        if (PhotonNetwork.inRoom) PhotonNetwork.isMessageQueueRunning = true;

        yield return game.UpdateLoadingText("타일 Pool 생성 중...");

        int mapSizeInt = mapSize.x * mapSize.y;
        CreateObjectPool(floorPrefab, (int)TileID.FLOOR, mapSizeInt);
        CreateObjectPool(wallPrefab, (int)TileID.WALL, mapSizeInt / 2);
        CreateObjectPool(cornerPrefab, (int)TileID.CORNER, mapSizeInt / 2);
        CreateObjectPool(pillarPrefab, (int)TileID.PILLAR, mapSizeInt / 2);

        yield return game.UpdateLoadingText("몬스터 Pool 생성 중...");

        foreach (var unit in FirebaseManager.units)
        {
            switch (unit.Value.type)
            {
                case -1:    //player
                    CreateObjectPool(unit.Value.res, unit.Key, 1, PhotonPool.PhotonInstantiateOption.STANDARD);
                    break;
                case 0:     //normal monster
                    CreateObjectPool(unit.Value.res, unit.Key, 100, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);
                    break;
                case 1:     //elite monster
                    CreateObjectPool(unit.Value.res, unit.Key, 10, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);
                    break;
                case 2:     //boss monster
                    CreateObjectPool(unit.Value.res, unit.Key, 1, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);
                    break;
            }
        }

        yield return game.UpdateLoadingText("맵 오브젝트 Pool 생성 중...");

        CreateObjectPool(chestResName, (int)MapObjectID.CHEST, 30, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);
        CreateObjectPool(traderResName, (int)MapObjectID.TRADER, 30, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);
        CreateObjectPool(potalResName, (int)MapObjectID.POTAL, 1, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);

        yield return game.UpdateLoadingText("장식 Pool 생성 중...");

        for (int i = 0; i < decoPrefabs.Count; i++)
            CreateObjectPool(decoPrefabs[i], MapDecoID + i, 50);

        yield return game.UpdateLoadingText("아이템 Pool 생성 중...");

        CreateObjectPool(itemResName, 1000, 50, PhotonPool.PhotonInstantiateOption.SCENE_OBJECT);

        StartCoroutine(LoadLevel());
    }
    private void Update()
    {
        UpdateMapTexture();
    }

    [ContextMenu("Reset Map")] [PunRPC] public void ResetLevel()
    {
        StartCoroutine(LoadLevel());
    }
    IEnumerator LoadLevel()
    {
        yield return game.StartLoading();

        yield return game.UpdateLoadingText("맵 생성 중...");

        ChangeTileMat();

        if (PhotonNetwork.inRoom)
        {
            //멀티용 맵 생성
            yield return StartCoroutine(GenerateRandomMapMulti());
        }
        else
        {
            //싱글용 맵 생성
            yield return StartCoroutine(GenerateRandomMapLocal());
        }

        //로딩화면 제거
        yield return game.EndLoading();
    }

    void ChangeTileMat()
    {
        int matIdx;
        do matIdx = Random.Range(0, mapFloorMats.Count);
        while (matIdx == mapMatIdx);
        mapMatIdx = matIdx;
    }

    void CreateObjectPool(string name, int id, int n, PhotonPool.PhotonInstantiateOption option)
    {
        if (option == PhotonPool.PhotonInstantiateOption.LOCAL || !PhotonNetwork.inRoom)
            objectsPool[id] = new ResourcePool(name, id, n, poolPos);
        else if (option == PhotonPool.PhotonInstantiateOption.STANDARD ||
            (option == PhotonPool.PhotonInstantiateOption.SCENE_OBJECT && PhotonNetwork.isMasterClient))
            objectsPool[id] = new PhotonPool(name, id, n, option);
    }
    void CreateObjectPool(GameObject obj, int id, int n)
    {
        objectsPool[id] = new ResourcePool(obj, id, n, poolPos);
    }

    IEnumerator GenerateRandomMapLocal()
    {
        ResetMap();

        GenerateMapData();
        PaintMapTile();

        GenerateMapTile(mapTiles);
        GenerateMapObject(JsonUtility.ToJson(new ObjInfoList(objects)));
        GeneratePlayer();

        yield return null;
    }
    IEnumerator GenerateRandomMapMulti()
    {
        if (PhotonNetwork.isMasterClient)
        {
            pv.RPC("ResetMap", PhotonTargets.All);

            GenerateMapData();
            PaintMapTile();

            pv.RPC("GenerateMapTile", PhotonTargets.All, mapTiles);
            pv.RPC("GenerateMapObject", PhotonTargets.All, JsonUtility.ToJson(new ObjInfoList(objects)));
            pv.RPC("MapReadyOK", PhotonTargets.All);
        }

        yield return StartCoroutine(pr.WaitForReady());

        if (PhotonNetwork.isMasterClient)
            pv.RPC("GeneratePlayer", PhotonTargets.All);
    }
    [PunRPC] void ResetMap()
    {
        rooms.Clear();
        paths.Clear();
        roomInfos.Clear();
        objects.Clear();

        foreach (var pool in objectsPool)
        {
            pool.Value.Reset();
        }
    }

    /*------------MAP DATA------------*/
    void GenerateMapData()
    {
        RoomNode map = new RoomNode(new RectInt(0, 0, mapSize.x, mapSize.y));

        DivideRect(map);
        CreateRoom(map);
        CreatePath(map);

        DecideRoomType();
        SetObjectData();
    }
    void DivideRect(RoomNode node, int pDivAxis = -1, int divDepth = 0, bool hRev = false, bool vRev = false, int n = 0)
    {
        if (n == maxDivideDepth) return;

        RoomNode lRoom, rRoom;
        bool bH = hRev, bV = vRev;
        float dividePer = Random.Range(minDividePer, maxDividePer);

        //horizontal : 0, vertical : 1
        int divideAxis = (node.rect.width < node.rect.height) ? 0 : 1;
        if (divideAxis == pDivAxis) divDepth++;
        else divDepth = 1;

        if (divDepth > 2) divideAxis = (divideAxis == 1) ? 0 : 1;

        if (divideAxis == 0)
        {   //divide horizontal
            int down_h = (int)(node.rect.height * dividePer);
            lRoom = new RoomNode(new RectInt(
                node.rect.x, node.rect.y,
                node.rect.width, down_h
                ));
            rRoom = new RoomNode(new RectInt(
                node.rect.x, node.rect.y + down_h,
                node.rect.width, node.rect.height - down_h
                ));
            node.line = new AxisLine(node.rect.y + down_h - 1, true);

            if (hRev)
            {
                node.left = rRoom;
                node.right = lRoom;
            }
            else
            {
                node.left = lRoom;
                node.right = rRoom;
            }
            bH = true;
        }
        else
        {   //divide vertical
            int left_w = (int)(node.rect.width * dividePer);
            lRoom = new RoomNode(new RectInt(
                node.rect.x, node.rect.y,
                left_w, node.rect.height
                ));
            rRoom = new RoomNode(new RectInt(
                node.rect.x + left_w, node.rect.y,
                node.rect.width - left_w, node.rect.height
                ));
            node.line = new AxisLine(node.rect.x + left_w - 1, false);

            if (vRev)
            {
                node.left = rRoom;
                node.right = lRoom;
            }
            else
            {
                node.left = lRoom;
                node.right = rRoom;
            }
            bV = true;
        }

        DivideRect(node.left, divideAxis, divDepth, bH, bV, n + 1);
        DivideRect(node.right, divideAxis, divDepth, hRev, vRev, n + 1);
    }
    RectInt CreateRoom(RoomNode node, int n = 0)
    {
        RectInt room;

        if (n == maxDivideDepth)
        {
            RectInt rect = node.rect;

            int w = rect.width - 4;
            int h = rect.height - 4;
            int x = rect.x + rect.width - w - 2;
            int y = rect.y + rect.height - h - 2;

            room = new RectInt(x, y, w, h);
            rooms.Add(room);
        }
        else
        {
            node.left.room = CreateRoom(node.left, n + 1);
            node.right.room = CreateRoom(node.right, n + 1);
            room = node.left.room;
        }

        return room;
    }
    void CreatePath(RoomNode node, int n = 0)
    {
        if (n == maxDivideDepth) return;

        paths.Add(new PathInfo(node.left.Center, node.right.Center, node.line));

        CreatePath(node.left, n + 1);
        CreatePath(node.right, n + 1);
    }
    void SetMapTile(int x, int y, TileType type)
    {
        mapTiles[mapSize.x * y + x] = type;
    }
    TileType GetMapTile(int x, int y)
    {
        return mapTiles[mapSize.x * y + x];
    }

    /*------------OBJ DATA------------*/
    void DecideRoomType()
    {
        for (int i = 0; i < (int)Mathf.Pow(2, maxDivideDepth); i++)
        {
            RoomType type;

            if (i == (int)Mathf.Pow(2, maxDivideDepth) - 1)
                type = RoomType.START;
            else if (i == 0)
                type = RoomType.POTAL;
            else
                type = RoomType.BATTLE;
                //type = (RoomType)Random.Range((int)RoomType.BATTLE, (int)RoomType.POTAL);
            

            roomInfos.Add(new RoomInfo(type, rooms[i]));
        }
    }
    void SetObjectData()
    {
        int playerCount = PhotonNetwork.inRoom ? PhotonNetwork.room.PlayerCount : 1;
        for (int i = 0; i < playerCount; i++)
            objects.Add(null);

        for (int i = 0; i < roomInfos.Count; i++)
        {
            RoomInfo info = roomInfos[i];
            RectInt objRect = new RectInt(info.rect.x + 1, info.rect.y + 1, info.rect.width - 2, info.rect.height - 2);

            CombinationRect combine = new CombinationRect(objRect);

            switch (info.type)
            {
                case RoomType.START:
                    {
                        if (PhotonNetwork.inRoom)
                            AddObjectsRandom((int)DB_INFO.PLAYER_ID, PhotonNetwork.room.PlayerCount, i, combine, true);
                        else
                            AddObjectCenter((int)DB_INFO.PLAYER_ID, i, combine, true);
                    }
                    break;
                case RoomType.BATTLE:
                    {
                        for (int j = 0; j < normalMonsterCount; j++)
                        {
                            int id = Random.Range((int)DB_INFO.NORMAL_MONSTER_BEGIN, (int)DB_INFO.NORMAL_MONSTER_NEXT);
                            AddObjectRandom(id, i, combine);
                        }

                        //for (int jj = 0; jj < 3; jj++)
                        //    AddObjectRandom(1000, i, combine);

                        for (int j = 0; j < 5; j++)
                        {
                            int id = Random.Range(MapDecoID, MapDecoID + decoPrefabs.Count);
                            AddObjectRandom(id, i, combine);
                        }
                            
                    }
                    break;
                case RoomType.TREASURE:
                    {
                        for (int j = 0; j < chestCount; j++)
                        {
                            AddObjectRandom((int)MapObjectID.CHEST, i, combine);
                        }
                    }
                    break;
                case RoomType.BOSS:
                    {
                        int id = Random.Range((int)DB_INFO.BOSS_MONSTER_BEGIN, (int)DB_INFO.BOSS_MONSTER_NEXT);
                        AddObjectCenter(id, i, combine);
                    }
                    break;
                case RoomType.ELITE:
                    {
                        int id = Random.Range((int)DB_INFO.ELITE_MONSTER_BEGIN, (int)DB_INFO.ELITE_MONSTER_NEXT);
                        AddObjectCenter(id, i, combine);
                    }
                    break;
                case RoomType.TRADER:
                    {
                        AddObjectCenter((int)MapObjectID.TRADER, i, combine);
                    }
                    break;
                case RoomType.POTAL:
                    {
                        AddObjectCenter((int)MapObjectID.POTAL, i, combine);
                    }
                    break;
            }
        }
    }
    void AddObjectCenter(int objId, int roomId, CombinationRect combin, bool addHead = false)
    {
        Vector2Int combPos = combin.GetCenter();
        Vector3 pos = new Vector3(combPos.x * tileSize, 0, combPos.y * tileSize);

        if (addHead) objects[0] = new ObjInfo(objId, roomId, pos);
        else objects.Add(new ObjInfo(objId, roomId, pos));
    }
    void AddObjectRandom(int objId, int roomId, CombinationRect combin, bool addHead = false)
    {
        Vector2Int combPos = combin.GetRandom();
        Vector3 pos = new Vector3(combPos.x * tileSize, 0, combPos.y * tileSize);

        if (addHead) objects[0] = new ObjInfo(objId, roomId, pos);
        else objects.Add(new ObjInfo(objId, roomId, pos));
    }
    void AddObjectsRandom(int objId, int count, int roomId, CombinationRect combin, bool addHead = false)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2Int combPos = combin.GetRandom();
            Vector3 pos = new Vector3(combPos.x * tileSize, 0, combPos.y * tileSize);

            if (addHead) objects[i] = new ObjInfo(objId, roomId, pos);
            else objects.Add(new ObjInfo(objId, roomId, pos));
        }
    }

    /*------------MAP PAINT------------*/
    void PaintMapTile()
    {
        int map_w = mapSize.x;
        int map_h = mapSize.y;

        //back
        for (int i = 0; i < map_h; i++)
            for (int j = 0; j < map_w; j++)
                SetMapTile(j, i, TileType.EMPTY);

        //room
        foreach (RectInt room in rooms)
        {
            int x1 = room.x;
            int y1 = room.y;
            int x2 = room.x + room.width;
            int y2 = room.y + room.height;

            //floor
            for (int i = y1; i < y2; i++)
                for (int j = x1; j < x2; j++)
                    SetMapTile(j, i, TileType.FLOOR);

            //wall
            for (int j = x1; j < x2; j++)
            {
                SetMapTile(j, y1, TileType.WALL);
                SetMapTile(j, y2 - 1, TileType.WALL);
            }
            for (int i = y1; i < y2; i++)
            {
                SetMapTile(x1, i, TileType.WALL);
                SetMapTile(x2 - 1, i, TileType.WALL);
            }

            //corner
            SetMapTile(x1, y1, TileType.CORNER);
            SetMapTile(x1, y2 - 1, TileType.CORNER);
            SetMapTile(x2 - 1, y1, TileType.CORNER);
            SetMapTile(x2 - 1, y2 - 1, TileType.CORNER);
        }

        //path
        foreach (PathInfo path in paths)
        {
            if (path.line.xAxis)
            {
                Vector2Int beginCurve = new Vector2Int(path.begin.x, path.line.xy);
                Vector2Int endCurve = new Vector2Int(path.end.x, path.line.xy);

                PaintLine(path.begin, beginCurve, Color.yellow);
                PaintLine(path.end, endCurve, Color.yellow);
                PaintLine(beginCurve, endCurve, Color.green);

                PaintCurve(path.begin, endCurve, beginCurve);
                PaintCurve(path.end, beginCurve, endCurve);
            }
            else
            {
                Vector2Int beginCurve = new Vector2Int(path.line.xy, path.begin.y);
                Vector2Int endCurve = new Vector2Int(path.line.xy, path.end.y);

                PaintLine(path.begin, beginCurve, Color.yellow);
                PaintLine(path.end, endCurve, Color.yellow);
                PaintLine(beginCurve, endCurve, Color.green);

                PaintCurve(endCurve, path.begin, beginCurve);
                PaintCurve(beginCurve, path.end, endCurve);
            }
        }
    }
    void PaintLine(Vector2Int begin, Vector2Int end, Color col)
    {
        if (begin.x == end.x)
        {
            int fromY = Mathf.Min(begin.y, end.y), toY = Mathf.Max(begin.y, end.y) + 1;
            for (int y = fromY; y <= toY; y++)
            {
                SetPathPixel(begin.x, y);
                SetPathPixel(begin.x + 1, y);
            }
        }
        else if (begin.y == end.y)
        {
            int fromX = Mathf.Min(begin.x, end.x), toX = Mathf.Max(begin.x, end.x) + 1;
            for (int x = fromX; x <= toX; x++)
            {
                SetPathPixel(x, begin.y);
                SetPathPixel(x, begin.y + 1);
            }
        }
        else return;
    }
    void PaintCurve(Vector2Int p1, Vector2Int p2, Vector2Int cur)
    {
        if (p1.x == p2.x || p1.y == p2.y) return;

        if (p1.x < p2.x)
        {
            if (p1.y < p2.y)
            {
                SetCurvePixel(cur, 0, 1, TileType.CORNER);
                SetCurvePixel(cur, 1, 0, TileType.PILLAR);
            }
            else
            {
                SetCurvePixel(cur, 0, 0, TileType.CORNER);
                SetCurvePixel(cur, 1, 1, TileType.PILLAR);
            }
        }
        else
        {
            if (p1.y < p2.y)
            {
                SetCurvePixel(cur, 1, 1, TileType.CORNER);
                SetCurvePixel(cur, 0, 0, TileType.PILLAR);
            }
            else
            {
                SetCurvePixel(cur, 1, 0, TileType.CORNER);
                SetCurvePixel(cur, 0, 1, TileType.PILLAR);
            }
        }
    }
    void SetPathPixel(int x, int y)
    {
        if (GetMapTile(x, y) == TileType.WALL) SetMapTile(x, y, TileType.PILLAR);
        else if (GetMapTile(x, y) == TileType.EMPTY) SetMapTile(x, y, TileType.PATH);
    }
    void SetCurvePixel(Vector2Int cur, int offset_x, int offset_y, TileType type)
    {
        SetMapTile(cur.x + offset_x, cur.y + offset_y, type);
    }

    /*------------MAP TILE------------*/
    [PunRPC] void GenerateMapTile(TileType[] tiles)
    {
        float multiplierFactor = tileSize + float.Epsilon;

        for (int i = 0; i < mapSize.y; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                TileType type = tiles[i * mapSize.y + j];

                if (type == TileType.FLOOR)
                {   //Floor
                    GameObject inst = objectsPool[(int)TileID.FLOOR].GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.GetComponent<TileObject>().ChangeMat(mapFloorMats[mapMatIdx], mapWallMats[mapMatIdx]);
                }
                else if (type == TileType.WALL || type == TileType.PATH)
                {   //Wall
                    GameObject inst = objectsPool[(int)TileID.WALL].GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationW(tiles, i, j), 0), Space.Self);
                    inst.GetComponent<TileObject>().ChangeMat(mapFloorMats[mapMatIdx], mapWallMats[mapMatIdx]);
                }
                else if (type == TileType.CORNER)
                {   //Corner
                    GameObject inst = objectsPool[(int)TileID.CORNER].GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationL(tiles, i, j), 0), Space.Self);
                    inst.GetComponent<TileObject>().ChangeMat(mapFloorMats[mapMatIdx], mapWallMats[mapMatIdx]);
                }
                else if (type == TileType.PILLAR)
                {   //Pillar
                    GameObject inst = objectsPool[(int)TileID.PILLAR].GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationC(tiles, i, j), 0), Space.Self);
                    inst.GetComponent<TileObject>().ChangeMat(mapFloorMats[mapMatIdx], mapWallMats[mapMatIdx]);
                }
            }
        }

        SetTileTexture(tiles);
    }
    float FindRotationW(TileType[] tiles, int i, int j)
    {
        float rotation = 0f;
        if (i - 1 >= 0 && (tiles[(i - 1) * mapSize.y + j] == TileType.EMPTY))
            rotation = 90f;
        else if (j - 1 >= 0 && (tiles[i * mapSize.y + (j - 1)] == TileType.EMPTY))
            rotation = 180f;
        else if (i + 1 < mapSize.y && (tiles[(i + 1) * mapSize.y + j] == TileType.EMPTY))
            rotation = -90f;
        return rotation;
    }
    float FindRotationC(TileType[] tiles, int i, int j)
    {
        float rotation = 0f;
        if (i - 1 >= 0 && j + 1 < mapSize.x && (tiles[(i - 1) * mapSize.y + (j + 1)] == TileType.EMPTY))
            rotation = 90f;
        else if (i - 1 >= 0 && j - 1 >= 0 && (tiles[(i - 1) * mapSize.y + (j - 1)] == TileType.EMPTY))
            rotation = 180f;
        else if (i + 1 < mapSize.y && j - 1 >= 0 && (tiles[(i + 1) * mapSize.y + (j - 1)] == TileType.EMPTY))
            rotation = -90f;
        return rotation;
    }
    float FindRotationL(TileType[] tiles, int i, int j)
    {
        float rotation = 0;
        if ((tiles[i * mapSize.y + j - 1] == TileType.EMPTY) && (tiles[(i - 1) * mapSize.y + j] == TileType.EMPTY))
            rotation = 180;
        if ((tiles[i * mapSize.y + j - 1] == TileType.EMPTY) && (tiles[(i + 1) * mapSize.y + j] == TileType.EMPTY))
            rotation = -90;
        if ((tiles[i * mapSize.y + j + 1] == TileType.EMPTY) && (tiles[(i - 1) * mapSize.y + j] == TileType.EMPTY))
            rotation = 90;
        return rotation;
    }

    /*------------MINIMAP------------*/
    void SetTileTexture(TileType[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tileTexture.SetPixel(i % mapSize.x, i / mapSize.x, tileColors[(int)(tiles[i])]);
        }
        tileTexture.Apply();
    }
    void UpdateMapTexture()
    {
        for (int i = 0; i < tileTexture.width * tileTexture.height; i++)
        {
            mapTexture.SetPixel(i % mapSize.x, i / mapSize.x, tileTexture.GetPixel(i % mapSize.x, i / mapSize.x));
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            Vector2Int pixel = ObjPosToMap(obj.transform.position);
            mapTexture.SetPixel(pixel.x, pixel.y, (!PhotonNetwork.inRoom || obj.GetComponent<PhotonView>().isMine) ? myPlayerColor : othPlayerColor);
        }
        mapTexture.Apply();
    }
    Vector2Int ObjPosToMap(Vector3 pos)
    {
        return new Vector2Int((int)(pos.x / tileSize), (int)(pos.z / tileSize));
    }

    /*------------MAP OBJECT------------*/
    [PunRPC] void GenerateMapObject(string json)
    {
        List<ObjInfo> objs = JsonUtility.FromJson<ObjInfoList>(json).objs;
        playerSpawnPoint = PhotonNetwork.inRoom ? objs[PhotonNetwork.player.ID - 1].pos : objs[0].pos;

        foreach (ObjInfo obj in objs)
        {
            if (obj.objID == (int)DB_INFO.PLAYER_ID) continue;

            if (!PhotonNetwork.inRoom || PhotonNetwork.isMasterClient)
            {
                GameObject go = GenerateObject(obj);
                Rect objRoom = new Rect(
                    rooms[obj.roomID].x * tileSize, rooms[obj.roomID].y * tileSize,
                    rooms[obj.roomID].width * tileSize, rooms[obj.roomID].height * tileSize
                    );

                if (go.tag == "Enemy")
                {
                    Enemy enemy = go.GetComponent<Enemy>();
                    enemy.originPos = obj.pos;
                    enemy.SetRoom(obj.roomID, objRoom);
                }
            }
        }
    }
    GameObject GenerateObject(ObjInfo info)
    {
        if (!objectsPool.ContainsKey(info.objID)) Debug.Log("object pool key not found : " + info.objID);
        return objectsPool[info.objID].GetObject(info.pos, Quaternion.identity, objectPos);
    }
    [PunRPC] void GeneratePlayer()
    {
        objectsPool[(int)DB_INFO.PLAYER_ID].GetObject(playerSpawnPoint, Quaternion.identity, objectPos);
    }
    [PunRPC] void MapReadyOK()
    {
        pr.Ready();
    }

    /*public method*/
    public int FindRoom(Vector3 pos)
    {
        for (int i = 0; i < roomInfos.Count; i++)
        {
            RectInt room = roomInfos[i].rect;
            if (pos.x > room.xMin * tileSize && pos.x < room.xMax * tileSize &&
                pos.z > room.yMin * tileSize && pos.z < room.yMax * tileSize)
                return i;
        }
        return -1;
    }
    public void RemoveObject(GameObject go, int id)
    {
        objectsPool[id].DisableObject(go);
    }
}
