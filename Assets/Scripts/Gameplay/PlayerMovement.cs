using UnityEngine;
using System.Collections;

namespace NecroDice.Gameplay
{
    public class PlayerMovement : MonoBehaviour
    {
        public float stepSize = 1f;
        public float stepDuration = 0.15f;
        public float swipeThreshold = 50f;

        private bool isMoving = false;
        private Vector2 touchStart;

        void Update()
        {
            if (GameManager.Instance == null) return;


            if (!GameManager.Instance.isPlayerTurn || isMoving) return;

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(TryMove(Vector2.up));
            if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(TryMove(Vector2.down));
            if (Input.GetKeyDown(KeyCode.A)) StartCoroutine(TryMove(Vector2.left));
            if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(TryMove(Vector2.right));
#endif

            HandleSwipe();
        }

        void HandleSwipe()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 delta = touch.position - touchStart;
                if (delta.magnitude < swipeThreshold) return;

                Vector2 direction;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    direction = delta.x > 0 ? Vector2.right : Vector2.left;
                else
                    direction = delta.y > 0 ? Vector2.up : Vector2.down;

                StartCoroutine(TryMove(direction));
            }
        }

        IEnumerator TryMove(Vector2 direction)
        {
            // Проверка коллизий с врагами или стенами
            Vector2 targetPos = (Vector2)transform.position + direction;
            Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f);
            if (hit != null && (hit.CompareTag("Wall") || hit.CompareTag("Enemy")))
                yield break;

            // Двигаем игрока
            yield return StartCoroutine(MoveStep(direction));

            // Сообщаем GameManager, что игрок закончил ход
            GameManager.Instance.OnPlayerMoveFinished();
        }

        IEnumerator MoveStep(Vector2 direction)
        {
            isMoving = true;

            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + (Vector3)(direction * stepSize);

            float elapsed = 0f;
            while (elapsed < stepDuration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, elapsed / stepDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
            isMoving = false;
        }
    }
}
