using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Cell의 사이즈
    public float cellSize;
    public int numOfRows;
    public int numOfcolumns;

    //길 찾기
    private ArrayList pathList;

    //장애물
    private GameObject[] obstacles;

    //게임의 시작 위치
    private Vector3 origin = new Vector3();

    //시작과 끝 위치
    private Transform startTransform, endTransform;

    //Node 객체들
    private Node[,] nodes;

    //GameController 싱글톤
    private static GameController instance = null;

    //GameController 싱글톤 프로퍼티
    public static GameController Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GameController)) as GameController;
                if (!instance)
                {
                    Debug.Log("GameController 가 존재하지 않습니다.");
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        //장애물 정보 설정
        obstacles = GameObject.FindGameObjectsWithTag("obstacle");

        //Nodes 정보를 설정
        InitNodes();

        startTransform = GameObject.FindGameObjectWithTag("entrance").GetComponent<Transform>();
        endTransform = GameObject.FindGameObjectWithTag("exit").GetComponent<Transform>();

        //Queue에 들어갈때 인식이 안되서 추가되는 코드
        int nodeIndex, nodeRowIndex, nodecolumnIndex;
        nodeIndex = GetnodeIndex(startTransform.position);
        nodeRowIndex = GetRowIndex(nodeIndex);
        nodecolumnIndex = GetcolumnIndex(nodeIndex);

        Node startNode = nodes[nodeRowIndex, nodecolumnIndex];

        nodeIndex = GetnodeIndex(endTransform.position);
        nodeRowIndex = GetRowIndex(nodeIndex);
        nodecolumnIndex = GetcolumnIndex(nodeIndex);

        Node endNode = nodes[nodeRowIndex, nodecolumnIndex];

        //FindPath로 찾은 경로를 받음
        pathList = AStar.FindPath(startNode, endNode);

        Test();
    }

    private void InitNodes()
    {
        nodes = new Node[numOfRows, numOfcolumns];

        //Node의 인덱스
        int index = 0;

        for (int i = 0; i < numOfRows; i++)
        {
            for (int j = 0; j < numOfcolumns; j++)
            {
                Vector3 nodePosition = GetNodePosition(index);
                Node node = new Node(nodePosition, index);
                nodes[i, j] = node;
                index++;
            }
        }

        //장애물 위치 설정
        if (obstacles != null && obstacles.Length > 0)
        {
            foreach (GameObject obstacle in obstacles)
            {
                int nodeIndex = GetnodeIndex(obstacle.transform.position);
                int columnIndex = GetcolumnIndex(nodeIndex);
                int rowIndex = GetRowIndex(nodeIndex);

                nodes[rowIndex, columnIndex].isObstacle = true;
            }
        }
    }

    //특정 좌표 기준으로 전/후/좌/우 방향에 장애물이 있는지 여부 확인
    //없으면 이동 가능한 영역으로 판단해서 반환
    public ArrayList GetAvailableNodes(Node node)
    {
        ArrayList resultList = new ArrayList();

        Vector3 nodePosition = node.position;

        int nodeIndex = GetnodeIndex(nodePosition);
        int rowIndex = GetRowIndex(nodeIndex);
        int columnIndex = GetcolumnIndex(nodeIndex);

        //위,아래를 찾고 싶으면 rowIndex
        //오른쪽,왼쪽을 찾고 싶으면 columnIndex
        int nodeRowIndex;
        int nodecolumnIndex;

        //위
        nodeRowIndex = rowIndex + 1;
        nodecolumnIndex = columnIndex;

        if (IsAvailableNode(nodeRowIndex, nodecolumnIndex))
        {
            resultList.Add(nodes[nodeRowIndex, nodecolumnIndex]);
        }

        //아래
        nodeRowIndex = rowIndex - 1;
        nodecolumnIndex = columnIndex;

        if (IsAvailableNode(nodeRowIndex, nodecolumnIndex))
        {
            resultList.Add(nodes[nodeRowIndex, nodecolumnIndex]);
        }

        //오른쪽
        nodeRowIndex = rowIndex;
        nodecolumnIndex = columnIndex + 1;

        if (IsAvailableNode(nodeRowIndex, nodecolumnIndex))
        {
            resultList.Add(nodes[nodeRowIndex, nodecolumnIndex]);
        }

        //왼쪽
        nodeRowIndex = rowIndex;
        nodecolumnIndex = columnIndex - 1;

        if (IsAvailableNode(nodeRowIndex, nodecolumnIndex))
        {
            resultList.Add(nodes[nodeRowIndex, nodecolumnIndex]);
        }

        return resultList;
    }
    //특정 Row, column index의 Node가 유효한지 확인하는 함수
    private bool IsAvailableNode(int rowIndex, int columnIndex)
    {
        //해당 row, column이 범위 밖이라면 false
        if (!IsAvailableIndex(rowIndex, columnIndex)) return false;

        //해당 row, column이 장해물인지 확인
        //if(장애물이라면) false;
        Node node = nodes[rowIndex, columnIndex];
        if (!node.isObstacle) return true;

        return false;
    }

    //지정한 범위내에 위치하는지 검사하는 함수
    private bool IsAvailablePosition(Vector3 position)
    {
        float availableWidht = numOfcolumns * cellSize;
        float availableHight = numOfRows * cellSize;

        if (position.x >= origin.x && position.x <= origin.x + availableWidht && position.z >= origin.z && position.z <= origin.z + availableHight)
        {
            return true;
        }
        return false;
    }

    //row와 column 인덱스가 유효한지 확인하는 함수, 유효한 인덱스인지 검사 함수
    private bool IsAvailableIndex(int rowindex, int columnIndex)
    {
        if (rowindex > -1 && columnIndex > -1 && rowindex < numOfRows && columnIndex < numOfcolumns)
        {
            return true;
        }
        return false;
    }

    //셀에 인덱스를 넣으면 그 셀의 좌표값을 얻어오는 함수
    private Vector3 GetNodePosition(int index)
    {
        int rowIndex = GetRowIndex(index);
        int columnIndex = GetcolumnIndex(index);

        float xPosition = (columnIndex * cellSize) + (cellSize / 2f);
        float zPosition = (rowIndex * cellSize) + (cellSize / 2f);

        return new Vector3(xPosition, 1f, zPosition);
    }

    //현재위치가 몇번 Index인지 찾는 함수 (셀의 Size가 필요하다)
    private int GetnodeIndex(Vector3 position)
    {
        if (!IsAvailablePosition(position))
        {
            return -1;
        }

        int columnIndex = (int)(position.x / cellSize);
        int rowIndex = (int)(position.z / cellSize);

        return (rowIndex * numOfcolumns + columnIndex);
    }

    private int GetRowIndex(int nodeIndex)
    {
        int rowIndex = nodeIndex / numOfcolumns;
        return rowIndex;
    }

    private int GetcolumnIndex(int nodeIndex)
    {
        int columnIndex = nodeIndex % numOfcolumns;
        return columnIndex;
    }

    //기즈모로 표현
    private void OnDrawGizmos()
    {
        if (pathList != null && pathList.Count > 0)
        {
            int index = 1;
            foreach(Node node in pathList)
            {
                if(index < pathList.Count)
                {
                    Node next = (Node)pathList[index];
                    Debug.DrawLine(node.position, next.position, Color.red);
                    index++;
                }
            }
        }
    }

    void Test()
    {
        if (pathList != null && pathList.Count > 0)
        {
            int index = 1;

            foreach (Node node in pathList)
            {
                if (index < pathList.Count)
                {
                    Node next = (Node)pathList[index];

                    float dis = Vector3.Distance(startTransform.position, next.position);

                    if(dis > 0)
                    { 
                        StartCoroutine(MoveCube(startTransform.position, next.position));
                        dis = Vector3.Distance(startTransform.position, next.position);
                    }

                    if (dis <= 0.1f)
                    {
                        index++;
                    }
                }
            }
        }
    }

    IEnumerator MoveCube(Vector3 startPos, Vector3 nextPos)
    {
        float currentTime = 0f;
        float duration = Time.deltaTime * 2.5f;

        while (currentTime <= duration)
        {
            startTransform.position = Vector3.Lerp(startPos, nextPos, duration);

            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
