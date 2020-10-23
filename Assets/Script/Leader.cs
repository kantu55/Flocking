using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    GameObject target; // 目標とするオブジェクト
    Astar astar; // 経路探索用のファイル
    List<Vector3> points; // 経路探索で出たポイントを格納するリスト
    int currentPoint; // 現在向かっている経路ポイント
    int pointLength; // 目的地までの経路探索ポイント数
    public int wayLength; // 現在の探索ポイントから次の探索ポイントの距離
    Vector3 direction;

    void Start()
    {
        target = GameObject.Find("Target");
        astar = GetComponent<Astar>();
        points = new List<Vector3>();
        points = astar.Search(this.gameObject, target, wayLength);
        pointLength = points.Count;
        currentPoint = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sheep")
        {
            currentPoint++;
            if (currentPoint >= pointLength)
            {
                target = GameObject.Find("Target");
                points = astar.Search(this.gameObject, target, wayLength);
                pointLength = points.Count;
                currentPoint = 1;
                transform.position = points[currentPoint];
            }
            transform.position = points[currentPoint];
        }
    }
    
    // ただのデバッグ
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // なんかエラー出る。。。
        if (currentPoint > 0)
        {
            Gizmos.DrawSphere(points[currentPoint], 0.5f);
        }
    }
}
