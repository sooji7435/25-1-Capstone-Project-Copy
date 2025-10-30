using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SwordSlash")]
public class Enemy_SwordSlash : EnemyAttackPattern
{
  
    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.IsAttacking = true;
        enemy.GetAnimatorController().PlayAttack();
        enemy.enemyShaderController.OnOutline();
        Vector2 attackDir = (PlayerScript.Instance.transform.position - enemy.transform.position).normalized;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        yield return new WaitForSeconds(attackChargeSec);
        EffectPooler.Instance.SpawnFromPool("AttackSlashParticle", enemy.transform.position, Quaternion.Euler(0f, 0f, angle));
        Vector2 boxCenter = (Vector2)enemy.transform.position + attackDir * 0.5f;
        Vector2 boxSize = new Vector2(1f, 1f);

        // 단일 대상 판정
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, angle, LayerMask.GetMask("Player"));

        if (hit != null && hit.TryGetComponent<PlayerScript>(out var player))
        {
            player.TakeDamage(enemy); // 예시

        }
        yield return new WaitForSeconds(attackDuration);
        enemy.enemyShaderController.OffOutline();
        yield return new WaitForSeconds(attackPostDelay);
        enemy.IsAttacking = false;
        
    }



}
