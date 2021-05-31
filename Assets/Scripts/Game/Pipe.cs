// Author: maka4519 - Maximiliam Rosén
// Group 02: maka4519 - Maximiliam Rosén, vida6631 - Viktor Dahlberg, anbe5918 - Andreas Berzelius

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    /// <summary>
    /// The pipe class
    /// </summary>
    public class Pipe : MonoBehaviour
    {
        // Out of bounds Event
        public static Action<GameObject> OnOutsideBounceEvent;
        // Upper pipe
        [SerializeField] private GameObject upperPipe;
        // Gap between pipes
        [SerializeField] private GameObject inBetweenPipes;
        // Lower pipe
        [SerializeField] private GameObject lowerPipe;
        // Distance from screen edges (top & bottom)
        [SerializeField] private float distanceFromScreenEdges;
        // Pipe width
        [SerializeField] private float pipeWidth;
        // Pipe speed
        [SerializeField] private float speed;
        // Lower left corner of screen
        private Vector2 _downLeftCorner;
        // Screen height 
        private float _screenHeight;
        // current Gap size
        private float _gapSize;

        private void Update()
        {
            // Moves the pipe towards the bird
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
            // Triggers out of bounds event if pipes are outside of screen
            if (transform.position.x < _downLeftCorner.x - pipeWidth/2) OnOutsideBounceEvent?.Invoke(gameObject);
        }

        public void Init(float gapSize)
        {
            // Register event clearAllPipes to trigger function DestroyMe 
            PipeGenerator.ClearAllPipes += DestroyMe;
            // Sets current gapSize to 'gapSize'
            _gapSize = gapSize;
            // Gets mainCamera
            Camera mainCamera = Camera.main;
            // If mainCamera ... get position of _downLeftCorner
            if (mainCamera is { }) _downLeftCorner = mainCamera.ScreenToWorldPoint(Vector3.zero);
            // Set _screenHeight to downLeftCorner vertical value times 2
            _screenHeight = -_downLeftCorner.y * 2;
            // Get the totalPipeHeight
            var totalPipeHeight = _screenHeight - gapSize;
            // If totalPipeHeight is less than distance from screen edges * 2 then set totalPipeHeight to 
            if (totalPipeHeight < distanceFromScreenEdges * 2) totalPipeHeight = distanceFromScreenEdges * 2;
            // set the upperPipeHeight
            var upperPipeHeight = Random.Range(distanceFromScreenEdges, totalPipeHeight - distanceFromScreenEdges);
            // Set lowerPipeHeight
            var lowerPipeHeight = totalPipeHeight - upperPipeHeight;
            // Sets upper pipe scale
            upperPipe.transform.localScale = new Vector3(pipeWidth, upperPipeHeight, 1);
            // Sets scale in between pipes
            inBetweenPipes.transform.localScale = new Vector3(pipeWidth, gapSize, 1);
            // Sets lower pipe scale
            lowerPipe.transform.localScale = new Vector3(pipeWidth, lowerPipeHeight, 1);
            // Sets upper pipe position
            upperPipe.transform.position = new Vector2(0 , -_downLeftCorner.y - upperPipe.transform.localScale.y/2);
            // Sets position in between pipes
            inBetweenPipes.transform.position = new Vector2(0 , upperPipe.transform.position.y - upperPipeHeight/2 - inBetweenPipes.transform.localScale.y/2);
            // Sets lower pipe position
            lowerPipe.transform.position = new Vector2(0 , _downLeftCorner.y + lowerPipe.transform.localScale.y/2);
            // Sets pipe position
            transform.position = new Vector2(-_downLeftCorner.x + pipeWidth/2, 0);
        }
        // Unregisters clearAllPips event triggers DestroyMe function and then Destroys pipe
        public void DestroyMe()
        {
            PipeGenerator.ClearAllPipes -= DestroyMe;
            Destroy(gameObject);
        }
        // Gets upper pipe Position
        public Vector2 GetUpperPipePosition()
        {
	        return new Vector2(upperPipe.transform.position.x,upperPipe.transform.position.y - upperPipe.transform.localScale.y/2);
        }
        // Gets position in between pipes 
        public Vector2 GetInBetweenPipesPosition()
        {
            return inBetweenPipes.transform.position;
        }
        // Gets lower pipe position
        public Vector2 GetLowerPipePosition()
        {
            return new Vector2(lowerPipe.transform.position.x,lowerPipe.transform.position.y + lowerPipe.transform.localScale.y/2);
        }
        // Gets gap size 
        public float GetGapSize()
        {
            return _gapSize;
        }
        // Gets pipe width
        public float GetWidth()
        {
            return pipeWidth;
        }
        // Gets distance from screen edges (top & bottom)
        public float GetPadding()
        {
            return distanceFromScreenEdges;
        }
        // Gets the speed of the pipe
        public float GetSpeed()
        {
            return speed;
        }
    }
}
