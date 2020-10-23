using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    GameObject target; // 目標とするオブジェクト
    Astar astar; // 経路探索用のファイル
    List<Vector3> points; // 経路探索で出たポイントを格納するリスト
    int currentPoint; // 現在向かっている経路ポイント
    int pointLength; // 目的地までの経路探索ポイント数
    public int wayLength; // 現在の探索ポイントから次の探索ポイントの距離
    Vector3 direction;
    float length;
    Quaternion rotation; // 回転

    void Start()
    {
        target = GameObject.Find("Target");
        astar = GetComponent<Astar>();
        // 目標地点までの経路探索を行う
        points = astar.Search(this.gameObject, target, wayLength);
        pointLength = points.Count;
        currentPoint = 1;
    }
    
    void Update()
    {
        direction = points[currentPoint] - transform.position;
        // 方向転換
        rotation = Quaternion.LookRotation(direction);
        transform.rotation = new Quaternion(transform.rotation.x, Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2).y,
            transform.rotation.z, transform.rotation.w);
        // 前方ベクトルに進む
        transform.Translate(0, 0, Time.deltaTime * 2);

        // 経路ポイントに近づいたら次の経路ポイントに進む
        if (direction.magnitude <= (transform.localScale.z * 0.5f))
        {
            ++currentPoint;
            // 目標地点まで進んだら新しく目的地まで経路探索を行う
            if (currentPoint > pointLength)
            {
                currentPoint = 1;
                points = astar.Search(this.gameObject, target, wayLength);
                pointLength = points.Count;
                length = (points[currentPoint] - transform.position).magnitude;
            }
        }
    }

    // ただのデバッグ
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        // なんかエラー出る。。。
        if (currentPoint > 1)
        {
            Gizmos.DrawSphere(points[currentPoint], 0.5f);
        }
    }
}
