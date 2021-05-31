// Author: maka4519 - Maximiliam Rosén
// Group 02: maka4519 - Maximiliam Rosén, vida6631 - Viktor Dahlberg, anbe5918 - Andreas Berzelius

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A class for generating the pipes
    /// </summary>
    public class PipeGenerator : MonoBehaviour
    {    
        // Clear all pipes event
        public static Action ClearAllPipes;
        // Pipe prefab
        [SerializeField] private GameObject pipePrefab;
        // Pipe spawn frequency
        [SerializeField] private float spawnFrequency;
        // Start gap size between pipes
        [SerializeField] private float startGapSize;
        // Min gap size between pipes
        [SerializeField] private float minGapSize;
        // Total pipes before gap reduces
        [SerializeField] private int pipesBetweenEachGapReduction;
        // How much gap reduces when certain amount of pipes have been passed
        [SerializeField] private float gapReductionSize;
        // Boolean if the game is running
        private bool _running;
        // Current gap size
        private float _gapSize;
        // Time when the last pipe spawned
        private float _lastSpawnedTime;
        // Total amount of spawned pipes
        private int _totalSpawnedPipes;
        // Queue of spawned pipes
        private readonly Queue<GameObject> _pipes = new Queue<GameObject>();
        // Remove list of pipes to remove
        private readonly List<GameObject> _removeList = new List<GameObject>();

        private void Start()
        {
            // Registers OnBirdDiedEvent should trigger Stop
            Bird.OnBirdDiedEvent += Stop;
            // Sets current gap size to start gap size between pipes
            _gapSize = startGapSize;
        }
        // Sets boolean running to false
        private void Stop() => _running = false;

        private void Update()
        {
            // If it's not running return
            if (!_running) return;
            // If it's not time to spawn a pipe then return
            if (!(Time.time - _lastSpawnedTime >= spawnFrequency)) return;
            Spawn();
            _lastSpawnedTime = Time.time;
        }

        private void Spawn()
        {
            // Increments total spawned pipes
            _totalSpawnedPipes++;
            // If total spawned pipes modulo pipes between each gap reduction equals zero reduce gap size
            if (_totalSpawnedPipes % pipesBetweenEachGapReduction == 0)
            {
                _gapSize -= gapReductionSize;
                if (_gapSize <= minGapSize) _gapSize = minGapSize;
            }
            // Instantiates a new pipe and puts it in the pipe queue
            var newPipe = Instantiate(pipePrefab);
            newPipe.GetComponent<Pipe>().Init(_gapSize);
            _pipes.Enqueue(newPipe);
        }
        // Removes all pipes and resets gap size
        public void Clear()
        {
            ClearAllPipes?.Invoke();
            _gapSize = startGapSize;
            _running = true;
            _pipes.Clear();
            _removeList.Clear();
        }
        // Gets the closest pipe
        public GameObject GetClosestPipe()
        {
            var isSearching = true;
            GameObject pipe = null;
            // While isSearching is true
            while (isSearching)
            {
                // Sets the pipe to the beginning GameObject in _pipes queue
                pipe = _pipes.Peek();
                /*
                 * If pipe is not null and x value of pipe position is less or equal to minus six
                 * add pipe to _removeList and remove pipe from the _pipes queue
                 */
                if (pipe != null && pipe.transform.position.x <= -6)
                {
                    _removeList.Add(_pipes.Dequeue());
                }
                // Else stop searching
                else
                {
                    isSearching = false;
                }
            }
            RemoveOutOfBounce();
            // Return pipe
            return pipe;
        }
        // Remove out of bounds pipes
        private void RemoveOutOfBounce()
        {
            var minXPosition = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
            var removeIndexes = new List<int>();
            for (var index = 0; index < _removeList.Count; index++)
            {    
                // If pipe is outside of screen add index number to removeIndexes list
                if (_removeList[index].transform.position.x <= minXPosition)
                {
                    removeIndexes.Add(index);
                }
            }
            // Remove pipes at the indexes in the removeIndexes list
            foreach (var rIndex in removeIndexes)
            {
                _removeList[rIndex].GetComponent<Pipe>().DestroyMe();
                _removeList.RemoveAt(rIndex);
            }
        }
        // Return if pipes queue is empty or not
        public bool IsEmpty()
        {
            return _pipes.Count == 0;
        }
        // Return if theres two pipes in the queue or not
        public bool hasTwoPipes()
        {
            return _pipes.Count > 1;
        }
        // Gets the second pipe in the pipe queue
        public GameObject GetSecondClosestPipe()
        {
            return _pipes.ElementAt(1);
        }
    }
}
