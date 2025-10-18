using System;
using Godot;
using PixelsRefactory.GraphEditor;

namespace PixelsRefactory.UI;

/// <summary>
/// Controls simulation playback (play/pause/speed).
/// </summary>
public partial class SimulationController : Control
{
	[Export] public GraphEditorController? GraphEditor { get; set; }

	private bool _isPlaying = false;
	private float _tickTimer = 0.0f;
	private float _currentTickRate = 10.0f; // ticks per second
	private float _speedMultiplier = 1.0f;

	private Button? _playButton;
	private Button? _pauseButton;
	private Button? _speedButton;
	private Label? _statusLabel;

	public override void _Ready()
	{
		// Find child controls (these will be created in the scene)
		_playButton = GetNodeOrNull<Button>("PlayButton");
		_pauseButton = GetNodeOrNull<Button>("PauseButton");
		_speedButton = GetNodeOrNull<Button>("SpeedButton");
		_statusLabel = GetNodeOrNull<Label>("StatusLabel");

		// Connect button signals if they exist
		if (_playButton != null)
		{
			_playButton.Pressed += OnPlayPressed;
		}

		if (_pauseButton != null)
		{
			_pauseButton.Pressed += OnPausePressed;
		}

		if (_speedButton != null)
		{
			_speedButton.Pressed += OnSpeedPressed;
		}

		UpdateUI();
		GD.Print("SimulationController ready");
	}

	public override void _Process(double delta)
	{
		if (!_isPlaying || GraphEditor == null)
		{
			return;
		}

		_tickTimer += (float)delta * _speedMultiplier;
		float tickInterval = 1.0f / _currentTickRate;

		while (_tickTimer >= tickInterval)
		{
			GraphEditor.SimulateTick();
			_tickTimer -= tickInterval;
		}

		UpdateUI();
	}

	private void OnPlayPressed()
	{
		if (GraphEditor == null)
		{
			GD.PrintErr("GraphEditor not assigned");
			return;
		}

		if (!_isPlaying)
		{
			GraphEditor.InitializeScheduler();
			_isPlaying = true;
			GD.Print("Simulation started");
			UpdateUI();
		}
	}

	private void OnPausePressed()
	{
		_isPlaying = false;
		GD.Print("Simulation paused");
		UpdateUI();
	}

	private void OnSpeedPressed()
	{
		// Cycle through speeds: 1x -> 2x -> 5x -> 1x
		if (_speedMultiplier == 1.0f)
		{
			_speedMultiplier = 2.0f;
		}
		else if (_speedMultiplier == 2.0f)
		{
			_speedMultiplier = 5.0f;
		}
		else
		{
			_speedMultiplier = 1.0f;
		}

		GD.Print($"Speed: {_speedMultiplier}x");
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (_statusLabel != null && GraphEditor != null)
		{
			var ctx = GraphEditor.GetSimContext();
			string status = _isPlaying ? "▶ Running" : "⏸ Paused";
			_statusLabel.Text = $"{status} | Speed: {_speedMultiplier}x | Tick: {ctx.TickCount}";
		}

		if (_speedButton != null)
		{
			_speedButton.Text = $"{_speedMultiplier}x";
		}
	}
}
