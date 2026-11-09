using System.Collections;
using UnityEngine;

public class WildMonster : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private int minLevel = 2;
    [SerializeField] private int maxLevel = 4;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private GameObject alertIcon;
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private float detectRadius = 3.5f;
    [SerializeField] private float wanderRadius = 2.5f;

    private GameServices services;
    private Transform playerTransform;
    private Vector3 spawnPoint;
    private StateMachine stateMachine;
    private MonsterInstance runtimeInstance;

    public MonsterData Data => monsterData;
    public bool IsBoss => isBoss;

    public void Construct(GameServices s, Transform player)
    {
        services = s;
        playerTransform = player;
        spawnPoint = transform.position;

        if (isBoss && services != null && services.Flags != null && services.Flags.Has(StoryFlags.BossDefeated))
        {
            gameObject.SetActive(false);
            return;
        }

        int lvl = Random.Range(minLevel, maxLevel + 1);
        runtimeInstance = new MonsterInstance(monsterData, lvl);

        if (body != null && monsterData != null && monsterData.Frames != null && monsterData.Frames.Length > 0)
        {
            body.sprite = monsterData.Frames[0];
        }

        if (alertIcon != null) alertIcon.SetActive(false);

        stateMachine = new StateMachine();
        if (isBoss)
        {
            stateMachine.ChangeState(new BossIdleState(this));
        }
        else
        {
            stateMachine.ChangeState(new WanderState(this));
        }
    }

    private void Update()
    {
        stateMachine?.Tick(Time.deltaTime);

        // Animate idle 2 frames
        if (monsterData != null && monsterData.Frames != null && monsterData.Frames.Length > 1 && body != null)
        {
            int frame = Mathf.FloorToInt(Time.time / 0.35f) % 2;
            body.sprite = monsterData.Frames[frame];
        }
    }

    public void TriggerBattle()
    {
        if (services == null || services.Scenes == null || services.Scenes.IsBusy) return;
        if (services.Battle != null && Time.time < services.Battle.SafeTimeUntil) return;
        if (services.Dialogue != null && services.Dialogue.IsRunning) return;
        if (services.Party == null || services.Party.Partner == null) return;

        if (isBoss)
        {
            StartCoroutine(BossRoarAndBattle());
        }
        else
        {
            services.Scenes.StartBattle(runtimeInstance, false);
        }
    }

    private IEnumerator BossRoarAndBattle()
    {
        services.Input?.Lock();
        if (services.Dialogue != null)
        {
            yield return services.Dialogue.Say("", "GRRRAAAH!! Raijugar menghalangi jalan, percikan listrik memenuhi gua!");
        }
        services.Scenes.StartBattle(runtimeInstance, true);
    }

    // --- FSM States ---
    private class WanderState : IState
    {
        private readonly WildMonster m;
        private Vector3 target;
        private float waitTimer;

        public WanderState(WildMonster monster) => m = monster;

        public void Enter() => PickTarget();

        public void Tick(float dt)
        {
            if (m.playerTransform == null) return;

            float distToPlayer = Vector3.Distance(m.transform.position, m.playerTransform.position);
            if (distToPlayer <= m.detectRadius && m.services != null && m.services.Party != null && m.services.Party.Partner != null)
            {
                m.stateMachine.ChangeState(new ChaseState(m));
                return;
            }

            if (waitTimer > 0f)
            {
                waitTimer -= dt;
                return;
            }

            m.transform.position = Vector3.MoveTowards(m.transform.position, target, 1.125f * dt);
            if (Vector3.Distance(m.transform.position, target) < 0.1f)
            {
                waitTimer = Random.Range(1f, 2.5f);
                PickTarget();
            }
        }

        public void Exit() { }

        private void PickTarget()
        {
            Vector2 offset = Random.insideUnitCircle * m.wanderRadius;
            target = m.spawnPoint + new Vector3(offset.x, offset.y, 0);
        }
    }

    private class ChaseState : IState
    {
        private readonly WildMonster m;

        public ChaseState(WildMonster monster) => m = monster;

        public void Enter()
        {
            if (m.alertIcon != null) m.alertIcon.SetActive(true);
        }

        public void Tick(float dt)
        {
            if (m.playerTransform == null) return;

            float distToPlayer = Vector3.Distance(m.transform.position, m.playerTransform.position);
            if (distToPlayer < 0.6f)
            {
                m.TriggerBattle();
                return;
            }

            if (distToPlayer > m.detectRadius * 1.5f)
            {
                m.stateMachine.ChangeState(new ReturnState(m));
                return;
            }

            m.transform.position = Vector3.MoveTowards(m.transform.position, m.playerTransform.position, 2.5f * dt);
            if (m.body != null)
            {
                m.body.flipX = (m.playerTransform.position.x < m.transform.position.x);
            }
        }

        public void Exit()
        {
            if (m.alertIcon != null) m.alertIcon.SetActive(false);
        }
    }

    private class ReturnState : IState
    {
        private readonly WildMonster m;

        public ReturnState(WildMonster monster) => m = monster;

        public void Enter() { }

        public void Tick(float dt)
        {
            m.transform.position = Vector3.MoveTowards(m.transform.position, m.spawnPoint, 1.5f * dt);
            if (Vector3.Distance(m.transform.position, m.spawnPoint) < 0.1f)
            {
                m.stateMachine.ChangeState(new WanderState(m));
            }
        }

        public void Exit() { }
    }

    private class BossIdleState : IState
    {
        private readonly WildMonster m;

        public BossIdleState(WildMonster monster) => m = monster;

        public void Enter() { }

        public void Tick(float dt)
        {
            if (m.playerTransform == null) return;
            float dist = Vector3.Distance(m.transform.position, m.playerTransform.position);
            if (dist < 1.4f)
            {
                m.TriggerBattle();
            }
        }

        public void Exit() { }
    }
}
