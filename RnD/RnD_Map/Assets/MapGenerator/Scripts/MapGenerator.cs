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

    [Header("MAP TEXTURE")]
    public Color emptyColor = Color.black;
    public Color floorColor = Color.white;
    public Color wallColor = Color.red;
    public Color cornerColor = Color.green;
    public Color pillarColor = Color.blue;
    public Color pathColor = Color.yellow;

    [Header("MAP TILE")]
    public float tileSize = 4f;
    public GameObject prefabFloor;
    public GameObject prefabWall;
    public GameObject prefabCorner;
    public GameObject prefabPillar;

    [Header("MAP OBJECT")]
    public GameObject playerPref;
    public GameObject monsterPref;

    [Header("DEBUG")]
    public bool useDebug = false;
    public LineRenderer lineRnd;
    public LineRenderer rectRnd;

    public enum RoomType { START, MONSTER, TREASURE, BOSS }

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

    List<RectInt> rooms;
    List<PathInfo> paths;
    List<RoomInfo> roomInfos;

    Texture2D mapTexture;

    Transform tilePos;
    Transform objectPos;
    Transform debugPos;

    PhotonView pv;

    private void Awake()
    {
        rooms = new List<RectInt>();
        paths = new List<PathInfo>();
        roomInfos = new List<RoomInfo>();
        mapTexture = new Texture2D(mapSize.x, mapSize.y);

        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {




        //master -> map data
        //client -> map gen
        //all client ready -> player gen (RPC)
        















        PhotonNetwork.isMessageQueueRunning = true;
        StartCoroutine(GenerateRandomMap());
    }

    public void Run()
    {

        //create new level
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
        yield return op;

        yield return StartCoroutine(GenerateRandomMap());
        //yield return new WaitForSeconds(1);
        
        SceneManager.UnloadSceneAsync("LoadingScene", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    }
    IEnumerator GenerateRandomMap()
    {
        ClearMap();
        SetUpMap();

        GenerateMapData();
        PaintMapTile();
        GenerateMapTile();
        GenerateMapObject();

        yield return null;
    }

    void ClearMap()
    {
        foreach (Transform child in mapPos)
            Destroy(child.gameObject);

        rooms.Clear();
        paths.Clear();
        roomInfos.Clear();
    }
    void SetUpMap()
    {
        (tilePos = new GameObject("tile").transform).parent = mapPos;
        (objectPos = new GameObject("object").transform).parent = mapPos;
        (debugPos = new GameObject("debug").transform).parent = mapPos;
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
            {
                type = RoomType.MONSTER;
            }

            Transform roomPos = new GameObject("room_" + (rooms.Count - 1)).transform;
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
    
    /*------------MAP PAINT------------*/
    void PaintMapTile()
    {
        int map_w = mapTexture.width;
        int map_h = mapTexture.height;

        //back
        for (int i = 0; i < map_h; i++)
            for (int j = 0; j < map_w; j++)
                mapTexture.SetPixel(j, i, emptyColor);

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
                    mapTexture.SetPixel(j, i, floorColor);

            //wall
            for (int j = x1; j < x2; j++)
            {
                mapTexture.SetPixel(j, y1, wallColor);
                mapTexture.SetPixel(j, y2 - 1, wallColor);
            }
            for (int i = y1; i < y2; i++)
            {
                mapTexture.SetPixel(x1, i, wallColor);
                mapTexture.SetPixel(x2 - 1, i, wallColor);
            }

            //corner
            mapTexture.SetPixel(x1, y1, cornerColor);
            mapTexture.SetPixel(x1, y2 - 1, cornerColor);
            mapTexture.SetPixel(x2 - 1, y1, cornerColor);
            mapTexture.SetPixel(x2 - 1, y2 - 1, cornerColor);
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

        mapTexture.Apply();
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
                SetCurvePixel(cur, 0, 1, cornerColor);
                SetCurvePixel(cur, 1, 0, pillarColor);
            }
            else
            {
                SetCurvePixel(cur, 0, 0, cornerColor);
                SetCurvePixel(cur, 1, 1, pillarColor);
            }
        }
        else
        {
            if (p1.y < p2.y)
            {
                SetCurvePixel(cur, 1, 1, cornerColor);
                SetCurvePixel(cur, 0, 0, pillarColor);
            }
            else
            {
                SetCurvePixel(cur, 1, 0, cornerColor);
                SetCurvePixel(cur, 0, 1, pillarColor);
            }
        }
    }
    void SetPathPixel(int x, int y)
    {
        if (mapTexture.GetPixel(x, y) == wallColor) mapTexture.SetPixel(x, y, pillarColor);
        else if (mapTexture.GetPixel(x, y) == emptyColor) mapTexture.SetPixel(x, y, pathColor);
    }
    void SetCurvePixel(Vector2Int cur, int offset_x, int offset_y, Color col)
    {
        mapTexture.SetPixel(cur.x + offset_x, cur.y + offset_y, col);
    }

    /*------------MAP TILE------------*/
    void GenerateMapTile()
    {
        float multiplierFactor = tileSize + float.Epsilon;
        Color[] pixels = mapTexture.GetPixels();

        for (int i = 0; i < mapSize.y; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                Color pixelColor = pixels[i * mapSize.y + j];

                if (pixelColor == floorColor)
                {   //Floor
                    GameObject inst = Instantiate(prefabFloor, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                }
                else if (pixelColor == wallColor || pixelColor == pathColor)
                {   //Wall
                    GameObject inst = Instantiate(prefabWall, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationW(pixels, i, j), 0), Space.Self);
                }
                else if (pixelColor == cornerColor)
                {   //Corner
                    GameObject inst = Instantiate(prefabCorner, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationL(pixels, i, j), 0), Space.Self);
                }
                else if (pixelColor == pillarColor)
                {   //Pillar
                    GameObject inst = Instantiate(prefabPillar, tilePos);
                    inst.transform.position = new Vector3(j * multiplierFactor, 0, i * multiplierFactor);
                    inst.transform.Rotate(new Vector3(0, FindRotationC(pixels, i, j), 0), Space.Self);
                }
            }
        }
    }
    float FindRotationW(Color[] pixels, int i, int j)
    {
        //void on the right
        float Rotation = 0f;
        //void on the bottom
        if (i - 1 >= 0 && (pixels[(i - 1) * mapSize.y + j] == Color.black || pixels[(i - 1) * mapSize.y + j] == Color.cyan))
        {
            Rotation = 90f;
        }
        //void on the left
        else if (j - 1 >= 0 && (pixels[i * mapSize.y + (j - 1)] == Color.black || pixels[i * mapSize.y + (j - 1)] == Color.cyan))
        {
            Rotation = 180f;
        }
        else if (i + 1 < mapSize.y && (pixels[(i + 1) * mapSize.y + j] == Color.black || pixels[(i + 1) * mapSize.y + j] == Color.cyan))
        {
            Rotation = -90f;
        }
        return Rotation;
    }
    float FindRotationC(Color[] pixels, int i, int j)
    {
        //void is on right/up
        float Rotation = 0f;
        //void is on right/down
        if (i - 1 >= 0 && j + 1 < mapSize.x && (pixels[(i - 1) * mapSize.y + (j + 1)] == Color.black || pixels[(i - 1) * mapSize.y + (j + 1)] == Color.cyan))
            Rotation = 90f;
        //voif is on bottom/left
        else if (i - 1 >= 0 && j - 1 >= 0 && (pixels[(i - 1) * mapSize.y + (j - 1)] == Color.black || pixels[(i - 1) * mapSize.y + (j - 1)] == Color.cyan))
            Rotation = 180f;
        else if (i + 1 < mapSize.y && j - 1 >= 0 && (pixels[(i + 1) * mapSize.y + (j - 1)] == Color.black || pixels[(i + 1) * mapSize.y + (j - 1)] == Color.cyan))
            Rotation = -90f;
        return Rotation;
    }
    float FindRotationL(Color[] pixels, int i, int j)
    {
        //void is in the top And right
        float rotation = 0;
        //void is in the right And bottom
        if (((pixels[i * mapSize.y + j - 1] == Color.black || pixels[i * mapSize.y + j - 1] == Color.cyan)) && ((pixels[(i - 1) * mapSize.y + j] == Color.black) || (pixels[(i - 1) * mapSize.y + j] == Color.cyan)))
            rotation = 180;
        //void is up And Left
        if (((pixels[i * mapSize.y + j - 1] == Color.black) || (pixels[i * mapSize.y + j - 1] == Color.cyan)) && ((pixels[(i + 1) * mapSize.y + j] == Color.black) || (pixels[(i + 1) * mapSize.y + j] == Color.cyan)))
            rotation = -90;
        if (((pixels[i * mapSize.y + j + 1] == Color.black) || (pixels[i * mapSize.y + j + 1] == Color.cyan)) && ((pixels[(i - 1) * mapSize.y + j] == Color.black) || (pixels[(i - 1) * mapSize.y + j] == Color.cyan)))
            rotation = 90;
        return rotation;
    }

    /*------------MAP OBJECT------------*/
    void GenerateMapObject()
    {
        foreach (RoomInfo info in roomInfos)
        {
            switch (info.type)
            {
                case RoomType.START:
                    {
                        Vector3 pos = new Vector3(info.rect.center.x * tileSize, 5f, info.rect.center.y * tileSize);

                        //GameObject.Instantiate(playerPref, pos, Quaternion.identity, info.pos);
                        GameObject player = PhotonNetwork.Instantiate("PhotonPlayer", pos, Quaternion.identity, 0);
                        player.transform.parent = info.pos;
                    }
                    break;
                case RoomType.MONSTER:
                    {
                        Vector3 pos = new Vector3(info.rect.center.x * tileSize, 5f, info.rect.center.y * tileSize);
                        GameObject.Instantiate(monsterPref, pos, Quaternion.identity, info.pos);
                    }
                    break;
                case RoomType.TREASURE:
                    break;
            }
        }
    }

    /*------------DEBUG------------*/
    void DrawLine(Vector2Int from, Vector2Int to, Color col, float zOrder = 0)
    {
        LineRenderer rnd = Instantiate(lineRnd, debugPos);
        rnd.material.color = col;

        rnd.SetPosition(0, new Vector3(from.x, from.y, zOrder));
        rnd.SetPosition(1, new Vector3(to.x, to.y, zOrder));
    }
    void DrawRect(RectInt rect, Color col, float zOrder = 0)
    {
        LineRenderer rnd = Instantiate(rectRnd, debugPos);
        rnd.material.color = col;

        rnd.SetPosition(0, new Vector3(rect.x, rect.y, zOrder));
        rnd.SetPosition(1, new Vector3(rect.x + rect.width, rect.y, zOrder));
        rnd.SetPosition(2, new Vector3(rect.x + rect.width, rect.y + rect.height, zOrder));
        rnd.SetPosition(3, new Vector3(rect.x, rect.y + rect.height, zOrder));
    }
}
