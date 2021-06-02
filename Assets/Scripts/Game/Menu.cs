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
        [SerializeField] private bool recording;
        [SerializeField] private bool manualMode;
        [SerializeField] private bool trainingMode;
        [SerializeField] private GameObject manualBirdPrefab;
        [SerializeField] private GameObject recordingBirdPrefab;
        [SerializeField] private GameObject trainingBirdPrefab;
        [SerializeField] private GameObject reinforcementBirdPrefab;
        [SerializeField] private GameObject imitationBirdPrefab;
        [SerializeField] private PipeGenerator pipeGenerator;
        [SerializeField] private GameObject playGameUIButton;
        [SerializeField] private GameObject gameOverTextUI;
        [SerializeField] private TextMeshProUGUI scoreUI;
        [SerializeField] private int totalTrainingBirds;
        
        private int _totalBirds;
        
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
            SpawnBirds(totalTrainingBirds);
        }
        // Get the amount of birds
        public int GetTotalBirds()
        {
            return _totalBirds;
        }
        // Spawns 'i' amount of birds
        private void SpawnBirds(int i)
        {
            if (!manualMode)
            {
                if(trainingMode)
                    for (var j = 0; j < i; j++)
                    {
                        Instantiate(trainingBirdPrefab);
                        _totalBirds++;
                    }
                else{
                    Instantiate(reinforcementBirdPrefab);
                    Instantiate(imitationBirdPrefab);
                    _totalBirds += 2;
                }
            }
            else
            {
                Instantiate(recording ? recordingBirdPrefab : manualBirdPrefab);
                _totalBirds++;
            }
        }
    }
}