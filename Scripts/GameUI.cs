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
		/// Node path to pause button
		/// </summary>
		[Export]
		public NodePath PauseButtonPath {
			get; private set;
		}

		/// <summary>
		/// Layer with pause text
		/// </summary>
		[Export]
		public NodePath PauseLayer {
			get; private set;
		}

		/// <summary>
		/// Pause image icon
		/// </summary>
		[Export]
		public Texture PauseTexture {
			get; private set;
		}

		/// <summary>
		/// Play image icon
		/// </summary>
		[Export]
		public Texture PlayTexture {
			get; private set;
		}

		/// <summary>
		/// Game controller
		/// </summary>
		public GameController Controller {
			get; private set;
		} = GameController.Controller;

		/// <summary>
		/// Score node ui label.
		/// </summary>
		private Label _scoreLabelNode;

		/// <summary>
		/// Fps node ui label.
		/// </summary>
		private Label _fpsLabelNode;

		/// <summary>
		/// Pause node ui button.
		/// </summary>
		private Button _pauseButton;

		/// <summary>
		/// Pause node ui layer panel
		/// </summary>
		private Panel _pauseLayer;

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
			// Set pause button
			_pauseButton = GetNode<Button>(PauseButtonPath);
			// Set pause layer
			_pauseLayer = GetNode<Panel>(PauseLayer);
			// Connect signal
			Controller.Connect(nameof(GameController.ConfigurationChanged), this, nameof(OnChangeConfiguration));
			_pauseButton.Connect("pressed", this, nameof(OnButtonPauseClicked));
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
		/// Change button icon
		/// </summary>
		private void ChangeButtonIcon() {
			if (Controller.Paused)
				_pauseButton.Icon = PlayTexture;
			if (!Controller.Paused)
				_pauseButton.Icon = PauseTexture;
		}

		/// <summary>
		/// Change pause layer
		/// </summary>
		private void ChangePauseLayer() {
			_pauseLayer.Visible = Controller.Paused;
		}

		/// <summary>
		/// Handle change configuration controller.
		/// </summary>
		/// <param name="field">property changed.</param>
		private void OnChangeConfiguration(string field) {
			// Check property changed
			switch (field) {
				case "Score":
					// Update score
					_gameScore = Controller.GetConfiguration<int>(field);
					// Change label content
					ChangeLabelScore();
					break;
				case "Paused":
					ChangeButtonIcon();
					ChangePauseLayer();
					break;
			}

		}

		/// <summary>
		/// Handle button input
		/// </summary>
		private void OnButtonPauseClicked() {
			if ( Controller.Start )
				Controller["Paused"] = !Controller.Paused;
		}

	}

}
