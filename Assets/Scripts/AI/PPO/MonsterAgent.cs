using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterAgent : Agent
{
    Rigidbody2D rBody;

    [Header("References")]
    public Transform player;                // 타깃 (Inspector에서 지정)
    public Transform[] obstacles;           // 장애물들 (자동 인식)

    [Header("Movement Settings")]
    public float moveForce = 50f;           // 이동 파워

    Vector2 startPos;                       // 시작 위치 저장

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        // ✅ 태그 기반 장애물 자동 검색
        GameObject[] obsObjects = GameObject.FindGameObjectsWithTag("Obstacle");
        obstacles = new Transform[obsObjects.Length];

        Debug.Log($"🔍 장애물 개수: {obsObjects.Length}");
        for (int i = 0; i < obsObjects.Length; i++)
        {
            obstacles[i] = obsObjects[i].transform;
            Debug.Log($"   ✅ [{i}] {obsObjects[i].name} / 위치: {obsObjects[i].transform.position}");
        }
    }

    // 에피소드 초기화
    public override void OnEpisodeBegin()
    {
        rBody.linearVelocity = Vector2.zero;

        // 1️⃣ 에이전트 위치 설정
        Vector2 agentPos = GetRandomClearPosition();
        transform.position = agentPos;

        // 2️⃣ 플레이어 위치 설정 (에이전트와 겹치지 않게)
        Vector2 playerPos = GetRandomClearPosition(agentPos);
        player.position = playerPos;
    }

    // 관측값 수집
    public override void CollectObservations(VectorSensor sensor)
    {
        // 자기 속도
        sensor.AddObservation(rBody.linearVelocity);

        // 타깃 상대 위치
        Vector2 toPlayer = player.position - transform.position;
        sensor.AddObservation(toPlayer.normalized);
        sensor.AddObservation(toPlayer.magnitude / 10f); // 거리 정규화

        // 장애물 상대 위치
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

        // 플레이어와 거리 기반 보상
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        AddReward(-0.001f * distanceToPlayer);

        // 정지 패널티
        if (rBody.linearVelocity.magnitude < 0.1f)
            AddReward(-0.001f);
    }

    // 충돌 이벤트
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"💥 충돌한 객체: {collision.collider.name} / 태그: {collision.collider.tag}");

        if (collision.collider.CompareTag("Player"))
        {
            AddReward(+1.0f);
            Debug.Log("🎯 Player 닿음 → 에피소드 종료");
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
            Debug.Log("🚧 장애물 닿음 → 에피소드 종료");
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-0.2f);
            Debug.Log("🧱 벽 닿음 → 에피소드 종료");
            EndEpisode();
        }
    }

    // ✅ 겹치지 않는 랜덤 위치 반환 함수
    private Vector2 GetRandomClearPosition(Vector2? otherPos = null)
{
    Vector2 pos = startPos; // 실패 시 기본값으로 돌아가기
    int attempts = 0;
    bool valid = false;

    while (attempts < 50)
    {
        Vector2 candidate = new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
        valid = true;

        // 장애물들과 거리 검사
        foreach (Transform obs in obstacles)
        {
            if (Vector2.Distance(candidate, obs.position) < 1.5f)
            {
                valid = false;
                break;
            }
        }

        // 다른 오브젝트(플레이어나 에이전트)와 거리 검사
        if (otherPos.HasValue && Vector2.Distance(candidate, otherPos.Value) < 3.5f)
            valid = false;

        if (valid)
        {
            pos = candidate;
            break;
        }

        attempts++;
    }

    if (!valid)
    {
        Debug.LogWarning($"⚠️ {gameObject.name}의 위치 배치 실패: 안전한 좌표를 찾지 못해 기본 위치로 복귀함");
    }

    return pos;
}


    // 휴리스틱 (키보드 조작)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
