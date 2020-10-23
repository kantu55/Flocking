using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : MonoBehaviour
{
    List<Vector3> PointsList;
    int pointIndex;
    Vector3 vec;
    RaycastHit hit;

    public List<Vector3> Search(GameObject start, GameObject target, float distance)
    {
        int loop = 0;
        // 経路ポイントの初期化
        pointIndex = 0;
        PointsList = new List<Vector3>();
        PointsList.Add(start.transform.position);

        // 目的地にたどり着くまでポイントを追加する
        while (true)
        {
            // 経路ポイントが見つからなさそうだったらループから抜ける
            loop++;
            if(loop >= 1000)
            {
                Debug.Log("経路が見つかりませんでした。");
                break;
            }

            // 経路ポイントと目標ポイントのベクトル
            vec = target.transform.position - PointsList[pointIndex];
            // 自身の大きさ分のBOXを目標位置まで飛ばす
            if (Physics.BoxCast(PointsList[pointIndex], start.transform.localScale * 0.5f , vec, out hit, Quaternion.LookRotation(vec), distance))
            {
                // 目的地に着いたらwhile文から抜ける
                if (hit.collider.gameObject == target.gameObject)
                {
                    PointsList.Add(hit.collider.gameObject.transform.position);
                    break;
                }
                // 目標地点以外のオブジェクトに当たったらオブジェクトを避ける
                else
                {
                    var bb = hit.collider.gameObject.transform;
                    // 4隅の座標を取得
                    Vector3[] corners = new Vector3[]
                    {
                        new Vector3(bb.position.x + (bb.localScale.x / 2), start.transform.position.y, bb.position.z + (bb.localScale.z / 2)),
                        new Vector3(bb.position.x + (bb.localScale.x / 2), start.transform.position.y, bb.position.z - (bb.localScale.z / 2)),
                        new Vector3(bb.position.x - (bb.localScale.x / 2) , start.transform.position.y, bb.position.z + (bb.localScale.z / 2)),
                        new Vector3(bb.position.x - (bb.localScale.x / 2) , start.transform.position.y, bb.position.z - (bb.localScale.z / 2))
                    };
                    int cornerIndex = 0;
                    float length = 1000;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 dirCorner = (corners[i] - PointsList[pointIndex]).normalized;
                        // 4隅のうち自身から見える位置を取得
                        if (Physics.Linecast(PointsList[pointIndex], corners[i], out hit) && (corners[i] - PointsList[pointIndex]).magnitude <= 0.1f)
                        {
                            // 見えた位置の中でターゲットに1番近い位置を取得
                            float targetDistance = (corners[i] - target.transform.position).magnitude;
                            if (targetDistance <= length && targetDistance >= 0.5f)
                            {
                                length = targetDistance;
                                cornerIndex = i;
                            }
                        }
                    }
                    // オブジェクトにめり込まないように位置を調整
                    corners = new Vector3[]
                    {
                        new Vector3(bb.position.x + (bb.localScale.x), start.transform.position.y, bb.position.z + (bb.localScale.z)),
                        new Vector3(bb.position.x + (bb.localScale.x), start.transform.position.y, bb.position.z - (bb.localScale.z)),
                        new Vector3(bb.position.x - (bb.localScale.x) , start.transform.position.y, bb.position.z + (bb.localScale.z)),
                        new Vector3(bb.position.x - (bb.localScale.x) , start.transform.position.y, bb.position.z - (bb.localScale.z))
                    };
                    // 四隅のうち目標地点に近い位置を経路ポイントとして格納する
                    PointsList.Insert(pointIndex, corners[cornerIndex]);
                    break;
                }
            }
            // Rayに何も当たらなけらばRayの先をポイントとして格納する
            else
            {
                // 2点のベクトルをノーマライズ化
                var length = vec.magnitude;
                var direction = vec / length;
                Vector3 point = PointsList[pointIndex] + (direction * distance);
                point = new Vector3(point.x, start.transform.position.y, point.z);
                // スタート地点からdistance分進んだ位置を格納
                PointsList.Add(point);
                ++pointIndex;
            }
        }
        // 格納したポイントを返す
        return PointsList;
    }
}
