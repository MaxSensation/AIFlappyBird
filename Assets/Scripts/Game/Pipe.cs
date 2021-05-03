using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class Pipe : MonoBehaviour
    {
        public static Action OnBirdScored;
        
        [SerializeField] private GameObject upperPipe;
        [SerializeField] private GameObject lowerPipe;
        [SerializeField] private float padding;
        [SerializeField] private float pipeWidth;
        [SerializeField] private float speed;
        [SerializeField] private float birdPositionX;

        private bool _hasScored;
        private Vector2 _downLeftCorner;
        private float _screenHeight;

        private void Update()
        {
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
            if (transform.position.x < _downLeftCorner.x - pipeWidth/2) Destroy(gameObject);
            if (_hasScored || !(transform.position.x <= birdPositionX)) return;
            OnBirdScored?.Invoke();
            _hasScored = true;
        }

        public void Init(float gapSize)
        {
            if (Camera.main is { }) _downLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
            _screenHeight = -_downLeftCorner.y * 2;
            var totalPipeHeight = _screenHeight - gapSize;
            if (totalPipeHeight < padding * 2) totalPipeHeight = padding * 2;
            var upperPipeHeight = Random.Range(padding, totalPipeHeight - padding);
            var lowerPipeHeight = totalPipeHeight - upperPipeHeight;
            upperPipe.transform.localScale = new Vector3(pipeWidth, upperPipeHeight, 1);
            lowerPipe.transform.localScale = new Vector3(pipeWidth, lowerPipeHeight, 1);
            upperPipe.transform.position = new Vector2(0 , -_downLeftCorner.y - upperPipe.transform.localScale.y/2);
            lowerPipe.transform.position = new Vector2(0 , _downLeftCorner.y + lowerPipe.transform.localScale.y/2);
            transform.position = new Vector2(-_downLeftCorner.x + pipeWidth/2, 0);
        }
    }
}
