using System;
using Godot;

namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Player controller class
	/// </summary>
	class PlayerController : RigidBody2D {

		/// <summary>
		/// Forse to jump
		/// </summary>
		[Export]
		public Vector2 ImpulseJump {
			get; private set;
		} = new Vector2(0, -300f);

		/// <summary>
		/// Animation controller node.
		/// </summary>
		public AnimationTree AnimationController {
			get; private set;
		}

		/// <summary>
		/// State machine playback
		/// </summary>
		public AnimationNodeStateMachinePlayback StateMachine {
			get; private set;
		}

		/// <summary>
		/// Check if player can jump.
		/// </summary>
		private bool _canJump = true;

		/// <summary>
		/// Ready method.
		/// </summary>
		public override void _Ready() {
			AnimationController = GetNode<AnimationTree>("AnimationController");
			StateMachine = AnimationController.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
			// Connect collision
			Connect("body_entered", this, nameof(OnBodyCollisionEntered));
		}

		/// <summary>
		/// Called each frame.
		/// </summary>
		/// <param name="delta">Elapsed time between frames.</param>
		public override void _Process(float delta) {
			// Check jump state
			if (Input.IsActionJustPressed("ui_jump"))
				PlayerJump();
		}

		/// <summary>
		/// Check unhandled input method.
		/// </summary>
		/// <param name="event">Input event</param>
		public override void _Input(InputEvent @event) {
			// Check if is touch input
			if (@event is InputEventScreenTouch)
				OnTouchEvent(@event as InputEventScreenTouch);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		private void OnTouchEvent(InputEventScreenTouch touchEvt) {
			if (touchEvt.Pressed)
				PlayerJump();
		}

		/// <summary>
		/// Action to player jump.
		/// </summary>
		private void PlayerJump() {
			// Check if player can jump
			if (!_canJump)
				return;

			// Apply impulse
			StateMachine.Travel("Jump");
			ApplyImpulse(Vector2.Zero, ImpulseJump);
			_canJump = false;
		}

		/// <summary>
		/// Check if collision detect other body.
		/// </summary>
		/// <param name="body">Collision body</param>
		private void OnBodyCollisionEntered(Node body) {
			// Check if node is floor
			if (body.IsInGroup("Floor")) {
				_canJump = true;
				StateMachine.Travel("Walk");
			}
				
		}

	}

}
