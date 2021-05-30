using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class Pipe : MonoBehaviour
    {
        public static Action<GameObject> OnOutsideBonceEvent;
        // Upper pipe
        [SerializeField] private GameObject upperPipe;
        // Gap between pipes
        [SerializeField] private GameObject inBetweenPipes;
        // Lower pipe
        [SerializeField] private GameObject lowerPipe;
        // Distance from screen edges (top & bottom)
        [SerializeField] private float padding;
        // Pipe width
        [SerializeField] private float pipeWidth;
        // Pipe speed
        [SerializeField] private float speed;
        // Lower left corner of screen
        private Vector2 _downLeftCorner;
        private float _screenHeight;
        private float _gapSize;

        private void Update()
        {
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
            if (transform.position.x < _downLeftCorner.x - pipeWidth/2) OnOutsideBonceEvent?.Invoke(gameObject);
        }

        public void Init(float gapSize)
        {
            PipeGenerator.ClearAllPipes += DestroyMe;
            _gapSize = gapSize;
            if (Camera.main is { }) _downLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
            _screenHeight = -_downLeftCorner.y * 2;
            var totalPipeHeight = _screenHeight - gapSize;
            if (totalPipeHeight < padding * 2) totalPipeHeight = padding * 2;
            var upperPipeHeight = Random.Range(padding, totalPipeHeight - padding);
            var lowerPipeHeight = totalPipeHeight - upperPipeHeight;
            upperPipe.transform.localScale = new Vector3(pipeWidth, upperPipeHeight, 1);
            inBetweenPipes.transform.localScale = new Vector3(pipeWidth, gapSize, 1);
            lowerPipe.transform.localScale = new Vector3(pipeWidth, lowerPipeHeight, 1);
            upperPipe.transform.position = new Vector2(0 , -_downLeftCorner.y - upperPipe.transform.localScale.y/2);
            inBetweenPipes.transform.position = new Vector2(0 , upperPipe.transform.position.y - upperPipeHeight/2 - inBetweenPipes.transform.localScale.y/2);
            lowerPipe.transform.position = new Vector2(0 , _downLeftCorner.y + lowerPipe.transform.localScale.y/2);
            transform.position = new Vector2(-_downLeftCorner.x + pipeWidth/2, 0);
        }

        public void DestroyMe()
        {
            PipeGenerator.ClearAllPipes -= DestroyMe;
            Destroy(gameObject);
        }

        public Vector2 GetUpperPipePosition()
        {
            return new Vector2(upperPipe.transform.position.x,upperPipe.transform.position.y - upperPipe.transform.localScale.y/2);
        }
        
        public Vector2 GetInBetweenPipesPosition()
        {
            return inBetweenPipes.transform.position;
        }
        
        public Vector2 GetLowerPipePosition()
        {
            return new Vector2(lowerPipe.transform.position.x,lowerPipe.transform.position.y + lowerPipe.transform.localScale.y/2);
        }
        
        public float GetGapSize()
        {
            return _gapSize;
        }

        public float GetWidth()
        {
            return pipeWidth;
        }

        public float GetPadding()
        {
            return padding;
        }

        public float GetSpeed()
        {
            return speed;
        }
    }
}
