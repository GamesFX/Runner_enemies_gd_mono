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
		/// Save factory player
		/// </summary>
		private FactoryPlayer _factoryPlayer;

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
			this["Score"] = 0;
			this["VelocityScale"] = 1f;
			this["VelocityAmout"] = 0.2f;
			this["VelocityChangeTime"] = 10f;

			this["CurrentTime"] = 0f;
			this["VelocityTime"] = 0f;

			// Search factory player
			SearchFactoryPlater();
		}

		/// <summary>
		/// Called each frame.
		/// </summary>
		/// <param name="delta">delta time game time</param>
		public override void _Process(float delta) {
			// Save current time
			float localCurrentTime = GetConfiguration<float>("CurrentTime");

			// Check if time is out (one second)
			if (localCurrentTime >= 1f) {
				// Save velocity time
				float localCurrentVelocityTime = GetConfiguration<float>("VelocityTime");

				// Check if time is out
				if (localCurrentVelocityTime >= GetConfiguration<float>("VelocityChangeTime")) {
					this["VelocityScale"] = GetConfiguration<float>("VelocityScale") + GetConfiguration<float>("VelocityAmout");
					_configuration["VelocityTime"] = 0f;
				}

				// Add score
				AddScore((int)Math.Round(localCurrentTime));

				// Reset currentTime
				_configuration["CurrentTime"] = 0f;
				_configuration["VelocityTime"] = localCurrentVelocityTime + localCurrentTime;
			}

			// Update current time
			_configuration["CurrentTime"] = GetConfiguration<float>("CurrentTime") + delta;
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
				_factoryPlayer = target as FactoryPlayer;
			}
		}

	}

}
