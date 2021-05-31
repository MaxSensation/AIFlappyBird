using System;
using Game;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgentBird : Agent
{
    public static Action<int> OnBirdScored;
    private static Action OnAllBirdDied;
    
    [SerializeField] private float jumpForce;
    [SerializeField] private bool resetScoreForEachTry;
    
    private static int _currentHighScore;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _screenLowerLeftCorner;
    
    private static int _totalBirdsDied;
    private static int _totalBirds;
    private bool _isDead;
    private Menu _menu;
    private PipeGenerator _pipeGenerator;
    private float _totalTimeAlive;
    private int _currentScore;
    private SpriteRenderer _spriteRenderer;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _menu = FindObjectOfType<Menu>();
        _totalBirds = _menu.GetTotalBirds();
        _pipeGenerator = FindObjectOfType<PipeGenerator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _screenLowerLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        OnAllBirdDied += EndEpisode;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (_isDead) return;
        // Birds position
        sensor.AddObservation(transform.position);
        // Birds velocity
        sensor.AddObservation(_rigidbody2D.velocity);
        if (!_pipeGenerator.IsEmpty())
        {
            var closestPipe = _pipeGenerator.GetClosestPipe().GetComponent<Pipe>();
            sensor.AddObservation(closestPipe.GetUpperPipePosition());
            sensor.AddObservation(closestPipe.GetLowerPipePosition());
            sensor.AddObservation(closestPipe.GetInBetweenPipesPosition());
            sensor.AddObservation(closestPipe.GetGapSize());
            sensor.AddObservation(closestPipe.GetPadding());
            sensor.AddObservation(closestPipe.GetSpeed());
            sensor.AddObservation(closestPipe.GetWidth());
            if (_pipeGenerator.hasTwoPipes())
            {
                var secoundClosestPipe = _pipeGenerator.GetSecondClosestPipe().GetComponent<Pipe>();
                sensor.AddObservation(secoundClosestPipe.GetUpperPipePosition());
                sensor.AddObservation(secoundClosestPipe.GetLowerPipePosition());
                sensor.AddObservation(secoundClosestPipe.GetInBetweenPipesPosition());
                sensor.AddObservation(secoundClosestPipe.GetGapSize());
                sensor.AddObservation(secoundClosestPipe.GetPadding());
                sensor.AddObservation(secoundClosestPipe.GetSpeed());
                sensor.AddObservation(secoundClosestPipe.GetWidth());
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_isDead) return; 
        if (actions.DiscreteActions[0] == 1) Jump();
    }

    public void Jump() => _rigidbody2D.velocity = new Vector2(0, jumpForce);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return; 
        if (other.CompareTag("Pipe") && Time.time - _totalTimeAlive > 1f)
        {
            Die();
            AddReward(-2);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isDead) return; 
        if (other.CompareTag("BetweenPipes")) IncreaseScore();
    }

    private void IncreaseScore()
    {
        _currentScore++;
        if (_currentScore > _currentHighScore)
        {
            _currentHighScore = _currentScore;
            OnBirdScored?.Invoke(_currentHighScore);
        }
        AddReward(100);
    }

    private void Die()
    {
        if (_isDead) return;
        AddReward(-5);
        AddReward(Time.time - _totalTimeAlive);
        _totalBirdsDied++;
        _isDead = true;
        _spriteRenderer.enabled = false;
        if (_totalBirdsDied >= _totalBirds)
        {
            OnAllBirdDied?.Invoke();
            _pipeGenerator.Clear();
            _totalBirdsDied = 0;
        }
    }

    public override void OnEpisodeBegin()
    {
        _totalBirdsDied = 0;
        if (resetScoreForEachTry)
        {
            _currentHighScore = 0;
            OnBirdScored?.Invoke(_currentHighScore);
        }
        _isDead = false;
        _spriteRenderer.enabled = true;
        transform.position = new Vector3(-6, 0);
        _rigidbody2D.velocity = Vector2.zero;
        _currentScore = 0;
        _totalTimeAlive = Time.time;
    }

    private void Update()
    {
        if (_isDead) return;   
        if (transform.position.y <= _screenLowerLeftCorner.y || transform.position.y >= -_screenLowerLeftCorner.y)
        {
            Die();
        }
    }
}