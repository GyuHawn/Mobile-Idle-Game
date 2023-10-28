using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private FixedJoystick joystick;

    // �⺻ ü�� ��..
    private float baseMaxHealth; // ü��
    private float basePower; // ���ݷ�
    private float baseDefense; // ����

    // ü�� ��..
    public float maxHealth; // ü�� = �⺻ü�� + ����ü��
    public float currentHealth;
    public float power; // ���ݷ� = �⺻���� + ���۰���
    private float defense; // ���� = �⺻��� + ���۹��
    public int money = 10; // ��

    public int level; // ���۷��� (ü + �� + �� ����)
    public float totalPower; // ���� = ���ݷ�(100%) + ���(50%) + ü��(50%)
    public float damege; // ������ = ���ݷ� - ����

    // ���� ����
    public int upgradeHealth = 1;
    public int upgradePower = 1;
    public int upgradeDefense = 1;
    
    // �̵�
    public float spd;
    public Vector2 inputVec;

    // ����
    public Transform pos;
    public Vector2 BoxSize;
    private Collider2D currentTarget;  // ���� Ÿ��

    // ����
    public GameObject bulletPrefab;
    public Transform shoot;
    public float bulletSpeed;

    // ���
    private bool isDead;

    private Rigidbody2D rigib;

    private void Awake()
    {
        rigib = GetComponent<Rigidbody2D>();
        joystick = FindObjectOfType<FixedJoystick>();
    }

    private void Start()
    {
        spd = 0.1f;
        bulletSpeed = 10;

        // �⺻ ���� ����
        baseMaxHealth = 100;
        basePower = 15;
        baseDefense = 1;

        // ���� ����
        SetStats();

        power = basePower; // �ϴ� ���� ó���� ���� ����
        StartCoroutine(AutoShoot());
    }

    IEnumerator AutoShoot() // �ڵ� ���
    {
        while (true)
        {
            if (IsValidTarget(currentTarget))
            {
                ShootBullet();
            }
            yield return new WaitForSeconds(.5f); // ���� �ӵ� ����   
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootBullet();
        }

        // Ÿ���� ������ ���ο� �� Ÿ��
        if (!IsValidTarget(currentTarget))
        {
            currentTarget = GetRandomMonster();
        }
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
        Die();    
    }

    void Move()
    {
        //inputVec.x = Input.GetAxisRaw("Horizontal"); Ű���� �̵�
        //inputVec.y = Input.GetAxisRaw("Vertical");
        inputVec.x = joystick.Horizontal; // ���̽�ƽ �̵�
        inputVec.y = joystick.Vertical;

        Vector2 nextVec = inputVec.normalized * spd;
        rigib.MovePosition(rigib.position + nextVec);
    }

    void Turn()
    {
        if (inputVec.x != 0)
        {
            float rotationY = inputVec.x > 0 ? 0f : 180f;
            transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        }
    }

 
    private Collider2D GetRandomMonster()  // ���� ���� ����
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos.position, BoxSize, 0f);

        List<Collider2D> monsters = new List<Collider2D>();

        foreach (Collider2D collider in colliders)
            if (collider.CompareTag("Monster"))
                monsters.Add(collider);

        return monsters.Count > 0 ? monsters[Random.Range(0, monsters.Count)] : null;
    }

    private bool IsValidTarget(Collider2D target) // ���� ����
    {
        // Ÿ���� null �̰ų� Monster �±װ� ������ false ��ȯ
        return target != null && target.CompareTag("Monster");
    }

    private void ShootBullet() // ���
    {
        if (IsValidTarget(currentTarget))
        {
            GameObject bullet = Instantiate(bulletPrefab, shoot.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity =
                (currentTarget.transform.position - shoot.position).normalized * bulletSpeed;
        }
    }
    
    void Die()
    {
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            transform.position = new Vector2(0, 0);
            MonsterSpwan spawner = GameObject.Find("Manager").GetComponent<MonsterSpwan>();
            StageManger stageManger = GameObject.Find("Manager").GetComponent<StageManger>();
            if (spawner != null)
            {
                spawner.ResetStage();
            }
            stageManger.deadMonster = 0;
            SetStats();
            isDead = false;
        }
    }

    public void SetStats()
    {
        level = upgradeHealth + upgradePower + upgradeDefense;
        maxHealth = baseMaxHealth + upgradeHealth;
        currentHealth = maxHealth;
        power = basePower + upgradePower;
        defense = baseDefense + upgradeDefense;
        totalPower = (int)(power + (defense / 0.5f) + (maxHealth / 0.3f));
    }
        private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            MonsterScript monsterScript = collision.gameObject.GetComponent<MonsterScript>();

            if (monsterScript != null)
            {
                damege = monsterScript.power - defense;
                currentHealth -= damege;
            }
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos.position, BoxSize);
    }*/
}