// Author: maka4519 - Maximiliam Rosén
// Group 02: maka4519 - Maximiliam Rosén, vida6631 - Viktor Dahlberg, anbe5918 - Andreas Berzelius

using System;
using Game;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
/// <summary>
/// The bird class with implemented Machine Learning system
/// </summary>
public class MLAgentBird : Agent
{
    // Event for when bird scored
    public static Action<int> OnBirdScored;
    // Event for when all birds are dead
    private static Action OnAllBirdDied;
    // How much jumpForce should be applied
    [SerializeField] private float jumpForce;
    // Boolean for managing a new episode starts
    [SerializeField] private bool resetScoreForEachTry;
    // Current high score
    private static int _currentHighScore;
    // Birds rigidbody
    private Rigidbody2D _rigidbody2D;
    // Previous velocity
    private Vector2 _oldVelocity;
    // Position of lower left corner
    private Vector2 _screenLowerLeftCorner;
    // Total amount of birds that died
    private static int _totalBirdsDied;
    // The amount of total birds
    private static int _totalBirds;
    // Boolean for if the bird is dead
    private bool _isDead;
    // Menu instance
    private Menu _menu;
    // PipeGenerator instance
    private PipeGenerator _pipeGenerator;
    // total time bird's been alive
    private float _totalTimeAlive;
    // the current score
    private int _currentScore;
    // Birds Spriterenderer
    private SpriteRenderer _spriteRenderer;
    private void Start()
    {
        // Get birds Spriterenderer
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // Get the menu
        _menu = FindObjectOfType<Menu>();
        // Get the total amount of birds
        _totalBirds = _menu.GetTotalBirds();
        // Get the pipeGenerator
        _pipeGenerator = FindObjectOfType<PipeGenerator>();
        // Get the Rigidbody
        _rigidbody2D = GetComponent<Rigidbody2D>();
        // Get the velocity of the bird
        _oldVelocity = _rigidbody2D.velocity;
        // Get the lower left corner position
        _screenLowerLeftCorner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        // Register that on all bird died end episode
        OnAllBirdDied += EndEpisode;
    }

    ///* Maximiliam config
    public override void CollectObservations(VectorSensor sensor)
    {
        if (_isDead) return;
        // Birds position
        sensor.AddObservation(transform.position);
        // Birds velocity
        sensor.AddObservation(_rigidbody2D.velocity);
        // Birds acceleration
        //sensor.AddObservation(GetAcceleration());
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
    }//*/
    
    /* Viktor config
    public override void CollectObservations(VectorSensor sensor)
    {
	    if (_isDead) return;
	    // Birds position
	    sensor.AddObservation(transform.position);
	    // Birds velocity
	    sensor.AddObservation(_rigidbody2D.velocity);
	    // Birds acceleration
	    sensor.AddObservation(GetAcceleration());
	    if (!_pipeGenerator.IsEmpty())
	    {
		    var closestPipe = _pipeGenerator.GetClosestPipe().GetComponent<Pipe>();
		    sensor.AddObservation(closestPipe.GetUpperPipePosition());
		    sensor.AddObservation(closestPipe.GetLowerPipePosition());
		    sensor.AddObservation(closestPipe.GetInBetweenPipesPosition());
		    //sensor.AddObservation(closestPipe.GetGapSize());
		    //sensor.AddObservation(closestPipe.GetPadding());
		    sensor.AddObservation(closestPipe.GetSpeed());
		    sensor.AddObservation(closestPipe.GetWidth());
		    if (_pipeGenerator.hasTwoPipes())
		    {
			    var secoundClosestPipe = _pipeGenerator.GetSecoundClosestPipe().GetComponent<Pipe>();
			    sensor.AddObservation(secoundClosestPipe.GetUpperPipePosition());
			    sensor.AddObservation(secoundClosestPipe.GetLowerPipePosition());
			    sensor.AddObservation(secoundClosestPipe.GetInBetweenPipesPosition());
			    //sensor.AddObservation(secoundClosestPipe.GetGapSize());
			    //sensor.AddObservation(secoundClosestPipe.GetPadding());
			    sensor.AddObservation(secoundClosestPipe.GetSpeed());
			    sensor.AddObservation(secoundClosestPipe.GetWidth());
		    }
	    }
    }*/
    // If not dead and recieved action do jump
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_isDead) return; 
        if (actions.DiscreteActions[0] == 1) Jump();
    }
    // Makes the bird jump with the applied jumpForce
    public void Jump() => _rigidbody2D.velocity = new Vector2(0, jumpForce);
    // If bird touches pipe and has been alive more than 1 second then kill bird
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return; 
        if (other.CompareTag("Pipe") && Time.time - _totalTimeAlive > 1f)
        {
            Die();
            AddReward(-2);
        }
    }
    // If bird goes between pipes then increase score
    private void OnTriggerExit2D(Collider2D other)
    {
        if (_isDead) return; 
        if (other.CompareTag("BetweenPipes")) IncreaseScore();
    }
    // Increase score and add reward
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
    // Kill bird and add penalty reward
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
    // Get acceleration of bird
    private Vector2 GetAcceleration()
    {
	    var velocity = _rigidbody2D.velocity;
	    var tmp = (velocity - _oldVelocity) / Time.fixedDeltaTime;
	    _oldVelocity = velocity;
	    return tmp;
    }
    // Resets everything on a new episode
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
        // If bird is outside the screen kill bird
        if (transform.position.y <= _screenLowerLeftCorner.y || transform.position.y >= -_screenLowerLeftCorner.y)
        {
            Die();
        }
    }
}