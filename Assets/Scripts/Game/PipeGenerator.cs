using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PipeGenerator : MonoBehaviour
    {
        public static Action ClearAllPipes;
        
        [SerializeField] private GameObject pipePrefab;
        [SerializeField] private float spawnFrequency;
        [SerializeField] private float maxGapSize;
        [SerializeField] private float minGapSize;
        [SerializeField] private int pipesBetweenEachGapReduction;
        [SerializeField] private float gapReductionSize;

        private bool _running;
        private float _gapSize;
        private float _lastSpawnedTime;
        private int _totalSpawnedPipes;
        private readonly Queue<GameObject> _pipes = new Queue<GameObject>();
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
