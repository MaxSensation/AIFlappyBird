// Author: maka4519 - Maximiliam Rosén
// Group 02: maka4519 - Maximiliam Rosén, vida6631 - Viktor Dahlberg, anbe5918 - Andreas Berzelius

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    /// <summary>
    /// The Menu class
    /// </summary>
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
        // Registers listening to event onBirdScored to know when to add score
        private void Start()
        {
            //Bird.OnBirdDiedEvent += GameOver;
            MLAgentBird.OnBirdScored += AddScore;
        }
        // Sets the score text 'score' value
        private void AddScore(int score)
        {
            scoreUI.SetText(score.ToString());
        }
        // Enables the game over text and sets the boolean _isGameOVer to true
        private void GameOver()
        {
            gameOverTextUI.SetActive(true);
            _isGameOver = true;
        }
        // Restarts the game if it is game over
        public void Restart(InputAction.CallbackContext context)
        {
            if (_isGameOver) StartNewGame();
        }
        // Starts a new game
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
        // Get the amount of birds
        public int GetTotalBirds()
        {
            return totalBirds;
        }
        // Spawns 'i' amount of birds
        private void SpawnBirds(int i)
        {
            for (var j = 0; j < i; j++)
            {
                Instantiate(birdPrefab);
            }
        }
    }
}