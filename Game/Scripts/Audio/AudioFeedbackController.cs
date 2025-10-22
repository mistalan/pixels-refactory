using Godot;

namespace PixelsRefactory.Audio;

/// <summary>
/// Manages audio feedback for player interactions and simulation events.
/// </summary>
public partial class AudioFeedbackController : Node
{
	private AudioStreamPlayer? _itemCompletePlayer;
	private AudioStreamPlayer? _bottleneckWarningPlayer;
	private AudioStreamPlayer? _achievementPlayer;

	private int _itemCounter = 0;
	private const int PlayEveryNthItem = 5; // Avoid audio spam

	public override void _Ready()
	{
		// Create audio players
		_itemCompletePlayer = new AudioStreamPlayer
		{
			Name = "ItemCompletePlayer",
			VolumeDb = -10.0f
		};
		AddChild(_itemCompletePlayer);

		_bottleneckWarningPlayer = new AudioStreamPlayer
		{
			Name = "BottleneckWarningPlayer",
			VolumeDb = -10.0f
		};
		AddChild(_bottleneckWarningPlayer);

		_achievementPlayer = new AudioStreamPlayer
		{
			Name = "AchievementPlayer",
			VolumeDb = -5.0f
		};
		AddChild(_achievementPlayer);

		// Generate placeholder audio
		GeneratePlaceholderAudio();

		GD.Print("AudioFeedbackController ready");
	}

	/// <summary>
	/// Play sound when an item completes processing.
	/// Only plays every Nth item to avoid audio spam.
	/// </summary>
	public void PlayItemComplete()
	{
		_itemCounter++;
		if (_itemCounter % PlayEveryNthItem == 0 && _itemCompletePlayer != null)
		{
			_itemCompletePlayer.Play();
		}
	}

	/// <summary>
	/// Play warning sound for bottleneck detected.
	/// </summary>
	public void PlayBottleneck()
	{
		if (_bottleneckWarningPlayer != null && !_bottleneckWarningPlayer.Playing)
		{
			_bottleneckWarningPlayer.Play();
		}
	}

	/// <summary>
	/// Play success sound for achievement unlocked.
	/// </summary>
	public void PlayAchievement()
	{
		if (_achievementPlayer != null)
		{
			_achievementPlayer.Play();
		}
	}

	private void GeneratePlaceholderAudio()
	{
		// Create simple sine wave tones as placeholder
		// In production, replace with actual audio files

		// Item complete: short high-pitched "ping" (880 Hz, 0.1s)
		var pingStream = CreateSineWave(880.0f, 0.1f);
		if (_itemCompletePlayer != null)
		{
			_itemCompletePlayer.Stream = pingStream;
		}

		// Bottleneck warning: low-pitched "warning" (220 Hz, 0.3s)
		var warningStream = CreateSineWave(220.0f, 0.3f);
		if (_bottleneckWarningPlayer != null)
		{
			_bottleneckWarningPlayer.Stream = warningStream;
		}

		// Achievement: pleasant chord (440 Hz, 0.5s)
		var achievementStream = CreateSineWave(440.0f, 0.5f);
		if (_achievementPlayer != null)
		{
			_achievementPlayer.Stream = achievementStream;
		}
	}

	private AudioStreamWav CreateSineWave(float frequency, float duration)
	{
		int sampleRate = 44100;
		int sampleCount = (int)(sampleRate * duration);
		var samples = new byte[sampleCount * 2]; // 16-bit samples

		for (int i = 0; i < sampleCount; i++)
		{
			float t = (float)i / sampleRate;
			float value = Mathf.Sin(2.0f * Mathf.Pi * frequency * t);
			
			// Apply envelope (fade out)
			float envelope = 1.0f - (float)i / sampleCount;
			value *= envelope;

			// Convert to 16-bit PCM
			short sample = (short)(value * short.MaxValue * 0.3f); // 30% volume
			samples[i * 2] = (byte)(sample & 0xFF);
			samples[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
		}

		var stream = new AudioStreamWav
		{
			Data = samples,
			Format = AudioStreamWav.FormatEnum.Format16Bits,
			MixRate = sampleRate,
			Stereo = false
		};

		return stream;
	}
}
