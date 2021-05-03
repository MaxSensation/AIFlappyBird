using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PipeGenerator : MonoBehaviour
    {
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
        private readonly List<GameObject> _pipes = new List<GameObject>();

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
            _pipes.Add(newPipe);
        }

        public void Clear()
        {
            foreach (var pipe in _pipes) Destroy(pipe);
            _gapSize = maxGapSize;
            _running = true;
        }
    }
}
