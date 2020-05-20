using Godot;

namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Game controller UI
	/// </summary>
	class GameUI : Control {

		/// <summary>
		/// Node path to score node.
		/// </summary>
		[Export]
		public NodePath ScoreLabelPath {
			get; private set;
		}

		/// <summary>
		/// Node path to fps node.
		/// </summary>
		[Export]
		public NodePath FpsLabelPath {
			get; private set;
		}

		/// <summary>
		/// Score node ui label.
		/// </summary>
		private Label _scoreLabelNode;

		/// <summary>
		/// Fps node ui label.
		/// </summary>
		private Label _fpsLabelNode;

		/// <summary>
		/// Score number.
		/// </summary>
		private int _gameScore;

		/// <summary>
		/// Save last fps result
		/// </summary>
		private float _lastFPS = Engine.GetFramesPerSecond();

		/// <summary>
		/// Ready method.
		/// </summary>
		public override void _Ready() {
			// Set score
			_gameScore = GameController.Controller.GetConfiguration<int>("Score");
			// Set score label
			_scoreLabelNode = GetNode<Label>(ScoreLabelPath);
			// Set fps label
			_fpsLabelNode = GetNode<Label>(FpsLabelPath);
			// Connect signal
			GameController.Controller.Connect(nameof(GameController.ConfigurationChanged), this, nameof(OnChangeConfiguration));
		}

		/// <summary>
		/// Called each frame.
		/// </summary>
		/// <param name="delta">Elapsed time between frames.</param>
		public override void _Process(float delta) {
			// Save current fps
			float localFPS = Engine.GetFramesPerSecond();

			// Check fps
			if (localFPS != _lastFPS) {
				_lastFPS = localFPS;
				_fpsLabelNode.Text = $"FPS: {_lastFPS}";
			}
		}

		/// <summary>
		/// Change score label text
		/// </summary>
		private void ChangeLabelScore() {
			// Format text
			if (_gameScore < 10)
				_scoreLabelNode.Text = $"0{_gameScore}";
			else
				_scoreLabelNode.Text = $"{_gameScore}";
		}

		/// <summary>
		/// Handle change configuration controller.
		/// </summary>
		/// <param name="field">property changed.</param>
		private void OnChangeConfiguration(string field) {
			// Check property changed
			if (field.Equals("Score")) {
				// Update score
				_gameScore = GameController.Controller.GetConfiguration<int>(field);
				// Change label content
				ChangeLabelScore();
			}
		}

	}

}
