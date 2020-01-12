using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public float cellSize;
    public int numOfRows;
    public int numOfcolumns;

    //Gizmos표현 함수
    private void OnDrawGizmos()
    {
        float width = (numOfcolumns * cellSize);
        float height = (numOfRows * cellSize);

        //row, 가로 표현
        for(int i=0; i< numOfRows; i++)
        {
            Vector3 startTransform = transform.position + i * cellSize * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endTransform = startTransform + width * new Vector3(1.0f, 0.0f, 0.0f);
            Debug.DrawLine(startTransform, endTransform, Color.green);
        }

        //columns, 세로 표현
        for(int i=0; i<numOfcolumns; i++)
        {
            Vector3 startTransform = transform.position + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endTransform = startTransform + height * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startTransform, endTransform, Color.green);
        }
    }
}
