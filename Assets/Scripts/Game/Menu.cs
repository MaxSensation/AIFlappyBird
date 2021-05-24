using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject birdPrefab;
        [SerializeField] private PipeGenerator pipeGenerator;
        [SerializeField] private GameObject playGameUIButton;
        [SerializeField] private GameObject gameOverTextUI;
        [SerializeField] private TextMeshProUGUI scoreUI;
        [SerializeField] private int totalBirds;

        private static bool _isGameOver;
        private static int _currentScore;
        private void Start()
        {
            //Bird.OnBirdDiedEvent += GameOver;
            MLAgentBird.OnBirdScored += AddScore;
        }

        private void AddScore(int score)
        {
            scoreUI.SetText(score.ToString());
        }

        private void GameOver()
        {
            gameOverTextUI.SetActive(true);
            _isGameOver = true;
        }

        public void Restart(InputAction.CallbackContext context)
        {
            if (_isGameOver) StartNewGame();
        }

        public void StartNewGame()
        {
            playGameUIButton.SetActive(false);
            _currentScore = 0;
            scoreUI.SetText(_currentScore.ToString());
            _isGameOver = false;
            gameOverTextUI.SetActive(false);
            pipeGenerator.Clear();
            SpawnBirds(totalBirds);
        }

        public int GetTotalBirds()
        {
            return totalBirds;
        }

        private void SpawnBirds(int i)
        {
            for (var j = 0; j < i; j++)
            {
                Instantiate(birdPrefab);
            }
        }
    }
}