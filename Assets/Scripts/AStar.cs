using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//길을 찾는 용도
public class AStar
{
    private static SortingQueue<Node> closedQueue, openQueue;

    public static ArrayList FindPath(Node startNode, Node endNode)
    {
        int findCount = 0;

        // 탐색을 위한 Node를 담을 Queue 설정
        openQueue = new SortingQueue<Node>();
        openQueue.Enqueue(startNode);

        startNode.gScore = 0f;
        startNode.hScore = GetPostionScore(startNode, endNode);

        //탐색이 끝난 Node를 담을 Queue 설정, 자기가 있는 중심축
        closedQueue = new SortingQueue<Node>();

        Node node = null;

        while (openQueue.Count != 0)
        {
            //Queue는 먼저 들어간게 먼저 나온다.
            node = openQueue.Dequeue();

            // 목적지를 찾았다면
            if (node == endNode)
            {
                Debug.Log("Find: " + findCount);
                return GetReverseResult(node);
            }

            // Node를 기준으로 갈수있는 주변 길 찾기
            ArrayList availableNodes = GameController.Instance.GetAvailableNodes(node);

            foreach (Node availableNode in availableNodes)
            {
                //이미 찾았던 길이 아니라면 openQueue에 추가
                if (!closedQueue.Contains(availableNode))
                {
                    //다른애가 오픈큐에 넣었었는데 다시 똑같은 길을 찾아서 오픈큐에 넣으려는 중복을 방지하도록 처리
                    if (openQueue.Contains(availableNode))
                    {
                        float score = GetPostionScore(node, availableNode);
                        float newGScore = node.gScore + score;

                        //더 가까운 길이라면
                        if (availableNode.gScore > newGScore)
                        {
                            availableNode.gScore = newGScore;
                            availableNode.parent = node;
                        }
                    }
                    else
                    {
                        float score = GetPostionScore(node, availableNode);

                        float newGScore = node.gScore + score;
                        float newHScore = GetPostionScore(availableNode, endNode);

                        availableNode.gScore = newGScore;
                        availableNode.hScore = newHScore;
                        //GScore에 따라 부모가 바뀜
                        availableNode.parent = node;

                        openQueue.Enqueue(availableNode);
                        findCount++;
                    }
                }
            }
            closedQueue.Enqueue(node);
        }

        if (node == endNode)
        {
            Debug.Log("Find: " + findCount);
            return GetReverseResult(node);
        }

        return null;
    }

    private static ArrayList GetReverseResult(Node node)
    {
        ArrayList resultArrayList = new ArrayList();
        while (node != null)
        {
            resultArrayList.Add(node);
            node = node.parent;
        }
        //목적지에서 출발지까지의 ArrayList을 Reverse를 통해 반전시켜 출발지->목적지로 변환
        resultArrayList.Reverse();

        return resultArrayList;
    }

    //현재 지점과 목표지점과의거리 H 점수
    private static float GetPostionScore(Node currentNode, Node endNode)
    {
        Vector3 resultValue = currentNode.position - endNode.position;
        return resultValue.magnitude;
    }
}
