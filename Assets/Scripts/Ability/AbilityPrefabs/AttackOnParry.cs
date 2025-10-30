using UnityEngine;

public class AttackOnParry : PlayerAbility
{
    private PlayerScript player;

    public float attackRange = 3f;
    public int damage = 5;
    LayerMask enemyLayer = 6; // �� ���̾�... ���İ������

    public override void OnEquip(PlayerScript player)
    {
        Debug.Log("�������� �����Ƽ ������");
        this.player = player;
        player.OnParrySuccess += AttackAbility;
    }

    public override void OnUnequip(PlayerScript player)
    {
        player.OnParrySuccess -= AttackAbility;
    }

    private void AttackAbility()
    {
        Debug.Log("�����Ƽ(������)�ߵ�");
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attackRange, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            Debug.Log("�����Ƽ: ����");
            hit.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }

    // ���� �� ���� ���������� ���� �� ���� ����? Ȯ���ʿ�
    // �� �� ����� ���� Ȯ�� �ʿ�
}