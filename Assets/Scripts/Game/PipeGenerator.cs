using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PipeGenerator : MonoBehaviour
    {    
        // Clear all pipes event
        public static Action ClearAllPipes;
        // Pipe prefab
        [SerializeField] private GameObject pipePrefab;
        // Pipe spawn frequency
        [SerializeField] private float spawnFrequency;
        // Start gap size between pipes
        [SerializeField] private float maxGapSize;
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
            Bird.OnBirdDiedEvent += Stop;
            _gapSize = maxGapSize;
        }

        private void Stop() => _running = false;

        private void Update()
        {
            if (!_running) return;
            if (!(Time.time - _lastSpawnedTime >= spawnFrequency)) return;
            Spawn();
            _lastSpawnedTime = Time.time;
        }

        private void Spawn()
        {
            _totalSpawnedPipes++;
            if (_totalSpawnedPipes % pipesBetweenEachGapReduction == 0)
            {
                _gapSize -= gapReductionSize;
                if (_gapSize <= minGapSize) _gapSize = minGapSize;
            }
            var newPipe = Instantiate(pipePrefab);
            newPipe.GetComponent<Pipe>().Init(_gapSize);
            _pipes.Enqueue(newPipe);
        }

        public void Clear()
        {
            ClearAllPipes?.Invoke();
            _gapSize = maxGapSize;
            _running = true;
            _pipes.Clear();
            _removeList.Clear();
        }

        public GameObject GetClosestPipe()
        {
            var isSearching = true;
            GameObject pipe = null;
            while (isSearching)
            {
                pipe = _pipes.Peek();
                if (pipe != null && pipe.transform.position.x <= -6)
                {
                    _removeList.Add(_pipes.Dequeue());
                }
                else
                {
                    isSearching = false;
                }
            }
            RemoveOutOfBonce();
            return pipe;
        }
        // Remove out of bounds pipes
        private void RemoveOutOfBonce()
        {
            var minXPosition = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
            var removeIndexes = new List<int>();
            for (var index = 0; index < _removeList.Count; index++)
            {
                if (_removeList[index].transform.position.x <= minXPosition)
                {
                    removeIndexes.Add(index);
                }
            }

            foreach (var rIndex in removeIndexes)
            {
                _removeList[rIndex].GetComponent<Pipe>().DestroyMe();
                _removeList.RemoveAt(rIndex);
            }
        }

        public bool IsEmpty()
        {
            return _pipes.Count == 0;
        }

        public bool hasTwoPipes()
        {
            return _pipes.Count > 1;
        }

        public GameObject GetSecoundClosestPipe()
        {
            return _pipes.ElementAt(1);
        }
    }
}
