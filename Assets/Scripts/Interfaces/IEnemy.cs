using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEnemy : MonoBehaviour
{
    [SerializeField] public bool IsAggro;
    [SerializeField] public float AggroTime;
    [SerializeField] public LayerMask PlayerMask;
    [SerializeField] private GameObject damageNumberPrefab;
    public float aggroRange = 2f;

    public event Action<float, float> OnHitEnemy = delegate { };

    private IEnumerator aggroRangeRoutine;

    protected void RaiseOnHitEnemyEvent(float health, float maxHealth)
    {
        var eh = OnHitEnemy;
        if (eh != null)
            OnHitEnemy(health, maxHealth);
    }

    public virtual IEnumerator AggroRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, aggroRange, PlayerMask);
        if (player == null)
        {
            yield return new WaitForSeconds(AggroTime);
            IsAggro = false;
            StopCoroutine(aggroRangeRoutine);
        }
        yield return new WaitForSeconds(0.5f);
        if (aggroRangeRoutine != null)
            StopCoroutine(aggroRangeRoutine);

        aggroRangeRoutine = AggroRange();
        StartCoroutine(aggroRangeRoutine);
    }

    public void Aggro()
    {
        IsAggro = true;
        if (aggroRangeRoutine != null)
            StopCoroutine(aggroRangeRoutine);

        aggroRangeRoutine = AggroRange();
        StartCoroutine(aggroRangeRoutine);
    }

    public void SpawnDamagePoints(int damage)
    {
        float x = UnityEngine.Random.Range(transform.position.x - 1f, transform.position.x + 1f);
        float y = UnityEngine.Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
        GameObject damageNum = Instantiate(damageNumberPrefab, new Vector3(x, y, 0), Quaternion.identity);
        damageNum.GetComponent<DamageNumber>().Setup(damage);
    }

    public abstract bool CanSeePlayer();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
