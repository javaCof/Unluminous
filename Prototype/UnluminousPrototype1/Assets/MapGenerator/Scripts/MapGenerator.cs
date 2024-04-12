using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    [Header("MAP DATA")]
    public Transform mapPos;
    public Vector2Int mapSize;
    public int maxDivideDepth = 4;
    public float minDividePer = 0.4f;
    public float maxDividePer = 0.6f;

    [Header("MAP TILE")]
    public float tileSize = 4f;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject cornerPrefab;
    public GameObject pillarPrefab;

    [Header("MAP OBJECT")]
    public GameObject playerPrefab;
    public GameObject monsterPrefab;

    [Header("DEBUG")]
    public bool useDebug = false;
    public LineRenderer lineRnd;
    public LineRenderer rectRnd;

    public enum RoomType { START, MONSTER, TREASURE, BOSS }
    public enum TileType { EMPTY, FLOOR, WALL, CORNER, PILLAR, PATH }
    public enum ObjType { PLAYER, ENEMY }

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
        public Transform pos;

        public RoomInfo(RoomType type, RectInt rect, Transform pos)
        {
            this.type = type;
            this.rect = rect;
            this.pos = pos;
        }
    }

    [System.Serializable]
    public class ObjInfo
    {
        public ObjType objID;
        public int roomID;
        public Vector3 pos;

        public ObjInfo(ObjType objID, int roomID, Vector3 pos)
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

    List<RectInt> rooms;
    List<PathInfo> paths;
    List<RoomInfo> roomInfos;
    [SerializeReference] List<ObjInfo> objects;

    Transform tilePos;
    Transform objectPos;
    Transform debugPos;

    Vector3 playerSpawnPoint;

    TileType[] mapTiles;

    ObjectPool floorPool;
    ObjectPool wallPool;
    ObjectPool cornerPool;
    ObjectPool pillarPool;
    ObjectPool monsterPool;

    PhotonView pv;
    PhotonReady pr;

    private void Awake()
    {
        rooms = new List<RectInt>();
        paths = new List<PathInfo>();
        roomInfos = new List<RoomInfo>();
        objects = new List<ObjInfo>();
        mapTiles = new TileType[mapSize.x * mapSize.y];

        (tilePos = new GameObject("tile").transform).parent = mapPos;
        (objectPos = new GameObject("object").transform).parent = mapPos;
        (debugPos = new GameObject("debug").transform).parent = mapPos;

        pv = GetComponent<PhotonView>();
        pr = GetComponent<PhotonReady>();
    }
    private IEnumerator Start()
    {
        
        if (PhotonNetwork.inRoom) PhotonNetwork.isMessageQueueRunning = true;
        yield return new WaitForSeconds(1);


        Transform poolPos = new GameObject("pool").transform;
        int mapSizeInt = mapSize.x * mapSize.y;
        floorPool = new ObjectPool(floorPrefab, mapSizeInt, poolPos);
        wallPool = new ObjectPool(wallPrefab, mapSizeInt / 2, poolPos);
        cornerPool = new ObjectPool(cornerPrefab, mapSizeInt / 2, poolPos);
        pillarPool = new ObjectPool(pillarPrefab, mapSizeInt / 2, poolPos);



        if (PhotonNetwork.isMasterClient)
            monsterPool = new ObjectPool("Enemy", 100, poolPos);

        StartCoroutine(LoadLevel());
    }

    [ContextMenu("Reset Level")]
    public void ResetLevel()
    {
        StartCoroutine(LoadLevel());
    }
    IEnumerator LoadLevel()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        yield return op;

        if (PhotonNetwork.inRoom)
        {
            yield return StartCoroutine(GenerateRandomMapMulti());
        }
        else
        {
            yield return StartCoroutine(GenerateRandomMapLocal());
        }

        SceneManager.UnloadSceneAsync("LoadingScene", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
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
            pv.RPC("ReadyOK", PhotonTargets.All);

            yield return StartCoroutine(pr.WaitForReady());

            pv.RPC("GeneratePlayer", PhotonTargets.All);
        }

        yield return null;
    }

    [PunRPC]
    void ResetMap()
    {
        rooms.Clear();
        paths.Clear();
        roomInfos.Clear();
        objects.Clear();

        floorPool.Reset();
        wallPool.Reset();
        cornerPool.Reset();
        pillarPool.Reset();

        foreach (Transform pos in objectPos)
        {
            Destroy(pos.gameObject);
        }
    }
    
    /*------------MAP DATA------------*/
    void GenerateMapData()
    {
        RoomNode map = new RoomNode(new RectInt(0, 0, mapSize.x, mapSize.y));

        if (useDebug) DrawRect(map.rect, Color.red);

        DivideRect(map);
        CreateRoom(map);
        DecideRoomType();
        CreatePath(map);
        SetObjectData();
    }
    void DivideRect(RoomNode node, int pDivAxis = -1, int divDepth = 0,
        bool hRev = false, bool vRev = false, int n = 0)
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

            if (useDebug)
            {
                DrawLine(
                new Vector2Int(node.rect.x, node.rect.y + down_h),
                new Vector2Int(node.rect.x + node.rect.width, node.rect.y + down_h),
                Color.red);
            }
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

            if (useDebug)
            {
                DrawLine(
                new Vector2Int(node.rect.x + left_w, node.rect.y),
                new Vector2Int(node.rect.x + left_w, node.rect.y + node.rect.height),
                Color.red);
            }
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

            if (useDebug)
            {
                if (rooms.Count == 1)
                    DrawRect(room, Color.blue);
                else if (rooms.Count == (int)Mathf.Pow(2, maxDivideDepth))
                    DrawRect(room, Color.red);
                else DrawRect(room, Color.white);
            }
        }
        else
        {
            node.left.room = CreateRoom(node.left, n + 1);
            node.right.room = CreateRoom(node.right, n + 1);
            room = node.left.room;
        }

        return room;
    }
    void DecideRoomType()
    {
        for (int i = 0; i < (int)Mathf.Pow(2, maxDivideDepth); i++)
        {
            RoomType type;

            if (i == (int)Mathf.Pow(2, maxDivideDepth) - 1)
                type = RoomType.START;
            else if (i == 0)
                type = RoomType.BOSS;
            else
                type = RoomType.MONSTER;

            Transform roomPos = new GameObject("room_" + i).transform;
            roomPos.parent = objectPos;
            roomInfos.Add(new RoomInfo(type, rooms[i], roomPos));
        }
    }
    void CreatePath(RoomNode node, int n = 0)
    {
        if (n == maxDivideDepth) return;

        paths.Add(new PathInfo(node.left.Center, node.right.Center, node.line));

        if (useDebug)
        {
            if (node.line.xAxis)
            {
                DrawLine(node.left.Center, new Vector2Int(node.left.Center.x, node.line.xy), Color.blue, -0.1f);
                DrawLine(node.right.Center, new Vector2Int(node.right.Center.x, node.line.xy), Color.blue, -0.1f);
                DrawLine(new Vector2Int(node.left.Center.x, node.line.xy), new Vector2Int(node.right.Center.x, node.line.xy), Color.blue, -0.1f);
            }
            else
            {
                DrawLine(node.left.Center, new Vector2Int(node.line.xy, node.left.Center.y), Color.blue, -0.1f);
                DrawLine(node.right.Center, new Vector2Int(node.line.xy, node.right.Center.y), Color.blue, -0.1f);
                DrawLine(new Vector2Int(node.line.xy, node.left.Center.y), new Vector2Int(node.line.xy, node.right.Center.y), Color.blue, -0.1f);
            }
        }

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
    void SetObjectData()
    {
        for (int i = 0; i < roomInfos.Count; i++)
        {
            RoomInfo info = roomInfos[i];
            switch (info.type)
            {
                case RoomType.START:
                    {
                        Vector3 pos = new Vector3(info.rect.center.x * tileSize, 5f, info.rect.center.y * tileSize);

                        objects.Add(new ObjInfo(ObjType.PLAYER, i, pos));
                    }
                    break;
                case RoomType.MONSTER:
                    {
                        RectInt objRect = new RectInt(info.rect.x + 1, info.rect.y + 1, info.rect.width - 2, info.rect.height - 2);
                        CombinationRect objComb = new CombinationRect(objRect);

                        for (int j = 0; j < 5; j++)
                        {
                            Vector2Int combPos = objComb.GetRandom();
                            Vector3 pos = new Vector3(combPos.x * tileSize, 5f, combPos.y * tileSize);

                            objects.Add(new ObjInfo(ObjType.ENEMY, i, pos));
                        }

                        //Vector3 pos = new Vector3(info.rect.center.x * tileSize, 5f, info.rect.center.y * tileSize);
                    }
                    break;
                case RoomType.TREASURE:
                    break;
                case RoomType.BOSS:
                    break;
            }
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
                    GameObject inst = floorPool.GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                }
                else if (type == TileType.WALL || type == TileType.PATH)
                {   //Wall
                    GameObject inst = wallPool.GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationW(tiles, i, j), 0), Space.Self);
                }
                else if (type == TileType.CORNER)
                {   //Corner
                    GameObject inst = cornerPool.GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationL(tiles, i, j), 0), Space.Self);
                }
                else if (type == TileType.PILLAR)
                {   //Pillar
                    GameObject inst = pillarPool.GetObject(Vector3.zero, Quaternion.identity, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationC(tiles, i, j), 0), Space.Self);
                }
            }
        }
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

    /*------------MAP OBJECT------------*/
    [PunRPC] void GenerateMapObject(string json)
    {
        List<ObjInfo> objs = JsonUtility.FromJson<ObjInfoList>(json).objs;
        foreach (ObjInfo obj in objs)
        {
            switch (obj.objID)
            {
                case ObjType.PLAYER:
                    {
                        playerSpawnPoint = obj.pos;
                    }
                    break;
                case ObjType.ENEMY:
                    {
                        if (PhotonNetwork.isMasterClient)
                        {
                            GameObject go = GenerateObject(obj);
                            Rect objRoom = new Rect(
                                rooms[obj.roomID].x * tileSize, rooms[obj.roomID].y * tileSize,
                                rooms[obj.roomID].width * tileSize, rooms[obj.roomID].height * tileSize
                                );
                            go.GetComponent<EnemyAction>().SetRoom(obj.roomID, objRoom);
                            
                        }
                    }
                    break;
            }
        }
    }
    GameObject GenerateObject(ObjInfo info)
    {
        return monsterPool.GetObject(info.pos, Quaternion.identity, roomInfos[info.roomID].pos);
        //return monsterPool.GetObject(info.pos, Quaternion.identity, null);
        //return PhotonNetwork.InstantiateSceneObject("Enemy", info.pos, Quaternion.identity, 0, null);
    }
    [PunRPC] void GeneratePlayer()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.Instantiate("PhotonPlayer", playerSpawnPoint, Quaternion.identity, 0);
        }
        else
        {
            GameObject.Instantiate(playerPrefab, playerSpawnPoint, Quaternion.identity);
        }
    }
    [PunRPC] void ReadyOK()
    {
        pr.Ready();
    }

    /*------------DEBUG------------*/
    void DrawLine(Vector2Int from, Vector2Int to, Color col, float zOrder = 0)
    {
        LineRenderer rnd = GameObject.Instantiate(lineRnd, debugPos);
        rnd.material.color = col;

        rnd.SetPosition(0, new Vector3(from.x, from.y, zOrder));
        rnd.SetPosition(1, new Vector3(to.x, to.y, zOrder));
    }
    void DrawRect(RectInt rect, Color col, float zOrder = 0)
    {
        LineRenderer rnd = GameObject.Instantiate(rectRnd, debugPos);
        rnd.material.color = col;

        rnd.SetPosition(0, new Vector3(rect.x, rect.y, zOrder));
        rnd.SetPosition(1, new Vector3(rect.x + rect.width, rect.y, zOrder));
        rnd.SetPosition(2, new Vector3(rect.x + rect.width, rect.y + rect.height, zOrder));
        rnd.SetPosition(3, new Vector3(rect.x, rect.y + rect.height, zOrder));
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
}
