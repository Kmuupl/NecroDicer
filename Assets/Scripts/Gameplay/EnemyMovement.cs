using UnityEngine;
using System.Collections;

namespace NecroDice.Gameplay
{
    public class EnemyMovement : MonoBehaviour
    {
        public float stepSize = 1f;
        public float stepDuration = 0.15f;

        public IEnumerator StepMove()
        {
            // Простая логика: случайное направление
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            Vector2 dir = directions[Random.Range(0, directions.Length)];

            Vector2 targetPos = (Vector2)transform.position + dir;

            // Проверка коллизий
            Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);
            if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Enemy") || hit.CompareTag("Player")))
                yield break;

            // Пошаговое движение
            Vector3 startPos = transform.position;
            Vector3 endPos = targetPos;
            float elapsed = 0f;
            while (elapsed < stepDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / stepDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;
        }
    }
}
