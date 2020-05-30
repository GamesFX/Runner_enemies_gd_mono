using Godot;
using System;
using System.Collections.Generic;

namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Game controller 
	/// </summary>
	[Serializable]
	public sealed class GameController : Node {

		/// <summary>
		/// Property change signal.
		/// </summary>
		/// <param name="property">signal to change</param>
		[Signal]
		public delegate void ConfigurationChanged(string property);

		/// <summary>
		/// Node instance.
		/// </summary>
		public static GameController Controller {
			get; private set;
		}

		/// <summary>
		/// Game controller configuration.
		/// </summary>
		private readonly Dictionary<string, object> _configuration;

		/// <summary>
		/// Access to configuration controller property.
		/// </summary>
		/// <param name="key">configuration key.</param>
		/// <returns><c>object</c> with configuration data or <c>null</c> if not exists.</returns>
		public object this[string key] {
			get => (_configuration.ContainsKey(key) && key != null) ? _configuration[key] : null;
			set {
				_configuration[key] = value;
				EmitSignal(nameof(ConfigurationChanged), key);
			}
		}

		/// <summary>
		/// Paused property
		/// </summary>
		public bool Paused {
			get => GetConfiguration<bool>("Paused");
			private set => this["Paused"] = value;
		}

		/// <summary>
		/// Start property
		/// </summary>
		public bool Start {
			get => GetConfiguration<bool>("Start");
			private set => this["Start"] = value;
		}

		/// <summary>
		/// Save factory player
		/// </summary>
		private PlayerFactory _factoryPlayer;

		/// <summary>
		/// Primary constructor.
		/// </summary>
		private GameController() : base() {
			// Set property instance
			Controller = this;
			// Create properties
			_configuration = new Dictionary<string, object>();
		}

		/// <summary>
		/// Get singlenton of <c>GameController</c>.
		/// </summary>
		/// <returns><c>GameController</c> instance</returns>
		public static GameController GetInstance() {
			// Check if instance exists.
			if (Controller != null)
				return Controller;

			// Create new instance.
			return new GameController();
		}

		/// <summary>
		/// Get configuration casted to target class.
		/// </summary>
		/// <typeparam name="T">Target class</typeparam>
		/// <param name="property">Target property</param>
		/// <returns><c>T</c> class with data</returns>
		public T GetConfiguration<T>(string property) {
			try {
				return (T)this[property];
			} catch (Exception err) {
				GD.PrintErr(property, ": ", err);
				return default;
			}
		}

		/// <summary>
		/// Initialize all properties.
		/// </summary>
		public override void _Ready() {
			ResetConfiguration();
			this["Start"] = true;

			// Search factory player
			SearchFactoryPlater();
			// Connect configuration changed
			Connect(nameof(ConfigurationChanged), this, nameof(OnConfigurationChanged));
		}

		/// <summary>
		/// Called each frame.
		/// </summary>
		/// <param name="delta">delta time game time</param>
		public override void _Process(float delta) {
			// Manage pause behaviour
			PauseBahaviour();

			// Check if you can play
			if (!Start || Paused)
				return;

			// Save current time
			float localCurrentTime = GetConfiguration<float>("CurrentTime");

			// Check if time is out (one second)
			if (localCurrentTime >= 1f) {
				// Add score
				AddScore((int)Math.Round(localCurrentTime));

				// Saved change velocity time
				int currentScore = GetConfiguration<int>("Score");
				int changedVelocityTime = GetConfiguration<int>("VelocityChangeTime");

				// Change velocity scale
				if (currentScore > 0 && currentScore % changedVelocityTime == 0) {
					float currentScale = GetConfiguration<float>("VelocityScale");
					float scaleAmount = GetConfiguration<float>("VelocityAmout");
					float maxScale = GetConfiguration<float>("MaxVelocityScale");

					float resultScale = Mathf.Clamp(currentScale + scaleAmount, 1f, maxScale);

					this["VelocityScale"] = resultScale;
				}

				// Reset currentTime
				_configuration["CurrentTime"] = 0f;
			}

			// Update current time
			_configuration["CurrentTime"] = GetConfiguration<float>("CurrentTime") + delta;
		}

		/// <summary>
		/// Reset all confituration
		/// </summary>
		private void ResetConfiguration() {
			this["Score"] = 0;
			this["VelocityScale"] = 1f;
			this["MaxVelocityScale"] = 10f;
			this["VelocityAmout"] = 1f;
			this["VelocityChangeTime"] = 10;

			this["CurrentTime"] = 0f;

			this["Paused"] = false;
		}

		/// <summary>
		/// Add score to game.
		/// </summary>
		/// <param name="amount">Score amount</param>
		private void AddScore(int amount) {
			int localScoreGame = GetConfiguration<int>("Score");
			this["Score"] = localScoreGame + amount;
		}

		/// <summary>
		/// Search factory player
		/// </summary>
		private void SearchFactoryPlater() {
			Godot.Collections.Array nodes = GetTree().GetNodesInGroup("p_spawn");

			// Check empty
			if (nodes.Count == 0)
				return;
			// Check node type
			Node target = nodes[0] as Node;

			// Set factory
			if (typeof(Position2D).IsInstanceOfType(target)) {
				_factoryPlayer = target as PlayerFactory;
			}
		}

		/// <summary>
		/// Manage pause
		/// </summary>
		private void PauseBahaviour() {
			if (Input.IsActionJustPressed("ui_pause") && Start)
				this["Paused"] = !Paused;
		}

		/// <summary>
		/// Called when configuration changed
		/// </summary>
		/// <param name="field"></param>
		private void OnConfigurationChanged(string field) {
			if (field != "Start")
				return;
			if (Start)
				return;

			ResetConfiguration();
		}


	}

}
