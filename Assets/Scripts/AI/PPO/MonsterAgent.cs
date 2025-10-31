using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterAgent : Agent
{
    Rigidbody2D rBody;

    [Header("References")]
    public Transform player;                // 타깃
    public Transform[] obstacles;           // 장애물들

    [Header("Movement Settings")]
    public float moveForce = 50f;            // 이동 파워
           // 속도 제한

    Vector2 startPos;                       // 시작 위치 저장

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // 매 에피소드마다 초기화
    public override void OnEpisodeBegin()
    {
        rBody.linearVelocity = Vector2.zero;
        // 에이전트 위치 랜덤
        transform.position = GetRandomClearPosition();

        // 플레이어 위치 랜덤 (장애물과 겹치지 않게)
        player.position = GetRandomClearPosition();
    }

    // 관측값 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        // 자기 자신의 속도
        sensor.AddObservation(rBody.linearVelocity);

        // 플레이어와의 상대 위치
        Vector2 toPlayer = player.position - transform.position;
        sensor.AddObservation(toPlayer.normalized);
        sensor.AddObservation(toPlayer.magnitude / 10f); // 거리 정규화

        // 가까운 장애물들 상대 위치 (2~3개 정도)
        foreach (Transform obs in obstacles)
        {
            Vector2 toObs = obs.position - transform.position;
            sensor.AddObservation(toObs.normalized);
            sensor.AddObservation(toObs.magnitude / 10f);
        }
    }

    // 행동 수행
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector2 moveDir = new Vector2(moveX, moveY);
        rBody.AddForce(moveDir * moveForce);

        // 플레이어와의 거리 보상
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        AddReward(-0.001f * distanceToPlayer); // 가까워질수록 보상 ↑

        // 가만히 있으면 약간의 패널티
        if (rBody.linearVelocity.magnitude < 0.1f)
            AddReward(-0.001f);
    }

    // 히트 이벤트
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            AddReward(+1.0f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-0.2f);
            EndEpisode();
        }
    }
    private Vector2 GetRandomClearPosition()
    {
        Vector2 pos;
        int attempts = 0;
        bool valid;

        do
        {
            pos = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
            valid = true;

            // 장애물들과 겹치는지 검사
            foreach (Transform obs in obstacles)
            {
                if (Vector2.Distance(pos, obs.position) < 1.5f) // 최소 거리 1.5f
                {
                    valid = false;
                    break;
                }
            }

            attempts++;
            if (attempts > 30) // 너무 오래 돌면 그냥 현재 pos 반환
            {
                Debug.LogWarning("⚠️ 위치 찾기 실패. 기본 위치 사용");
                break;
            }
        }
        while (!valid);

        return pos;
    }


    // 휴리스틱 (테스트용 수동 조작)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
