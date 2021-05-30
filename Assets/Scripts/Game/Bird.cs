// Author: maka4519 - Maximiliam Rosén
// Group 02: maka4519 - Maximiliam Rosén, vida6631 - Viktor Dahlberg, anbe5918 - Andreas Berzelius

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    /// <summary>
    /// The class for managing the Bird
    /// (red sphere)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bird : MonoBehaviour
    {
        // Event for when the bird dies
        public static Action OnBirdDiedEvent;
        // How much jump-force should be applied
        [SerializeField] private float jumpForce;
        // The birds rigidbody
        private Rigidbody2D _rigidbody2D;
        // The position of the lower left screen.
        private Vector2 _screenLowerLeftCorner;
        // The currentscore 
        private int currentScore;
        
        private void Start()
        {
            // Gets the rigidbody of the bird
            _rigidbody2D = GetComponent<Rigidbody2D>();
            // Gets the lower left corner of the screen's position
            _screenLowerLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        }
        // Triggers the jump function when a input is activated
        public void Jump(InputAction.CallbackContext context) => Jump();
        // Adds jump-force to the vertically to the bird 
        public void Jump() => _rigidbody2D.velocity = new Vector2(0, jumpForce);
        // If bird collides with a pipe then it dies.
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Pipe")) Die();
        }
        // If bird passes between two pipes then increaseScore
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("BetweenPipes")) IncreaseScore();
        }
        // Increases the current score
        private void IncreaseScore()
        {
            currentScore++;
            Debug.Log("CurrentScore: " + currentScore);
        }
        // Triggers the event that the bird died and deletes the bird
        private void Die()
        {
            OnBirdDiedEvent?.Invoke();
            Destroy(gameObject);
        }
        // If the bird is outside the bottom or the top of the screen the bird dies.
        private void Update()
        {
            if (transform.position.y <= _screenLowerLeftCorner.y || transform.position.y >= -_screenLowerLeftCorner.y) Die();
        }
    }
}
