using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bird : MonoBehaviour
    {
        public static Action OnBirdDiedEvent;
        [SerializeField] private float jumpForce;
        
        private Rigidbody2D _rigidbody2D;
        private Vector2 _screenLowerLeftCorner;
        private int currentScore;
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _screenLowerLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        }

        public void Jump(InputAction.CallbackContext context) => Jump();
        public void Jump() => _rigidbody2D.velocity = new Vector2(0, jumpForce);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Pipe")) Die();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("BetweenPipes")) IncreaseScore();
        }

        private void IncreaseScore()
        {
            currentScore++;
            Debug.Log("CurrentScore: " + currentScore);
        }

        private void Die()
        {
            OnBirdDiedEvent?.Invoke();
            Destroy(gameObject);
        }

        private void Update()
        {
            if (transform.position.y <= _screenLowerLeftCorner.y || transform.position.y >= -_screenLowerLeftCorner.y) Die();
        }
    }
}
