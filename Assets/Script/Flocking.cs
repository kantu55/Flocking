using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    GameObject target; // 目標地点
    List<GameObject> fellow; // 仲間

    // 自身が適用する最大の推進力
    [Range(0, 1)]
    public float maxForce;
    // 最大速度
    [Range(0, 100)]
    public float maxSpeed;
    // 回転速度
    [Range(0, 10)]
    public float maxTurnRate;
    // 分離行動の強さ
    [Range(0, 10)]
    public float weightSep;
    // 整列行動の強さ
    [Range(0, 10)]
    public float weightAli;
    // 結合行動の強さ
    [Range(0, 10)]
    public float weightCoh;
    // ターゲットに向かう強さ
    [Range(0, 10)]
    public float weightToTarget; 
    Vector3 velocity; // 速度
    Vector3 acceleration; // 推進力
    Quaternion rotation; // 回転
    Vector3 direction;
    Vector3 sum;
    
    private void OnTriggerStay(Collider other)
    {
        // 範囲内に仲間がいればリストに追加する
        if (other.gameObject.tag == "Sheep")
        {
            for (int i = 0; i < fellow.Count; i++)
            {
                if (fellow[i] != other.gameObject)
                {
                    fellow.Add(other.gameObject);
                }
            }
        }
    }

    void Start()
    {
        sum = new Vector3(0, 0, 0);
        target = GameObject.Find("LeadActor");
        fellow = new List<GameObject>();
        fellow.Add(this.gameObject);
    }

    void FixedUpdate()
    {
        // 目標地点とのベクトルを取得
        Vector3 targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        direction = target.transform.position - transform.position;
        Debug.Log(direction);
        // 操舵力を取得
        Flock(fellow);
        velocity = direction;
        MoveTo();
        // リストに追加した仲間のオブジェクトをリセットする
        fellow.Clear();
        fellow = new List<GameObject>();
        fellow.Add(this.gameObject);
    }

    void Flock(List<GameObject> other)
    {
        Vector3 sep;
        sep = Separate(fellow);
        ApplyForce(sep * weightSep);
        Vector3 ali;
        ali = Align(fellow);
        ApplyForce(ali * weightAli);
        Vector3 coh;
        coh = Cohesion(fellow);
        ApplyForce(coh * weightCoh);
    }

    // 分離行動・・・仲間たちから離れる
    Vector3 Separate(List<GameObject> other)
    {
        Vector3 steer = new Vector3(0, 0, 0);
        int count = 0;
        for(int i = 1; i < other.Count; i++)
        {
            Vector3 vec = transform.position - other[i].transform.position;
            // ベクトルのスケーリング
            if (vec.magnitude > 0)
            {
                steer += vec.normalized / vec.magnitude;
                count++;
            }
        }
        if(count > 0)
        {
            // ベクトルの平均化
            steer /= count;
            // 平均化したベクトルと今進んでる方向を減算して仲間たちから離れた方向へ行く
            if (steer.magnitude > 0)
            {
                return Vector3.ClampMagnitude(steer.normalized - velocity, maxForce);
            }
        }
        return Vector3.zero;
    }

    // 整列行動・・・仲間達と同じ方向、速度で進むようにする
    Vector3 Align(List<GameObject> other)
    {
        Vector3 sum = new Vector3(0, 0, 0);
        int count = 0;
        for(int i = 1; i < other.Count; i++)
        {
            Vector3 otherPos = other[i].transform.position;
            // 近くにいる仲間の向いている方向を取得して変数に加算
            if (Vector3.Distance(transform.position, otherPos) > 0)
            {
                sum += other[i].GetComponent<Rigidbody>().velocity;
                ++count;
            }
        }
        if(count > 0)
        {
            // 加算したベクトルをcountで除算することによりそれぞれのオブジェクトに向いている方向の平均を取得
            sum /= count;
            sum = sum.normalized * maxSpeed;
            sum -= velocity;
            return Vector3.ClampMagnitude(sum, maxForce);
        }
        return Vector3.zero;
    }

    // 結合行動・・・重心を求めてそこに密集する
    Vector3 Cohesion(List<GameObject> other)
    {
        int count = 0;
        for(int i = 1; i < other.Count; i++)
        {
            Vector3 otherPos = other[i].transform.position;
            // 範囲内の仲間の座標を集める
            if (Vector3.Distance(transform.position, otherPos) > 0)
            {
                sum += other[i].transform.position;
                ++count;
            }
        }
        // 集めた座標を除算して仲間たちとの重心を求める
        if (count > 0)
        {
            sum /= count;
            return Vector3.ClampMagnitude((sum - transform.position).normalized * maxSpeed - velocity, maxForce);
        }
        return Vector3.zero;
    }
    
    void MoveTo()
    {
        // 各行動に対する向くべき方向を求める
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        // 方向転換
        rotation = Quaternion.LookRotation(velocity);
        transform.rotation = new Quaternion(transform.rotation.x, Quaternion.Slerp(transform.rotation, rotation, maxTurnRate * Time.deltaTime).y,
            transform.rotation.z, transform.rotation.w);

        // 前方ベクトルに進む
        transform.Translate(0, 0, Time.deltaTime * maxSpeed);
        // 推進力をリセット
        acceleration *= 0.0f;
    }

    // 方向ベクトルと各行動の重みを掛けたものを引数にしている
    void ApplyForce(Vector3 force)
    {
        // 推進力として加算している
        acceleration += force;
    }
}
