using System;
using Godot;
using RunnerEnemyGD.Scripts.CustomNodes;
using System.Collections.Generic;

namespace RunnerEnemyGD.Scripts {


	/// <summary>
	/// Factory class for enemies
	/// </summary>
	public class EnemyFactory : FactoryNode {

		/// <summary>
		/// Target node insert instance
		/// </summary>
		[Export]
		public NodePath TargetInstance {
			get; private set;
		}

		/// <summary>
		/// Time to spawn enemies
		/// </summary>
		[Export(PropertyHint.Range, "0.01, 100")]
		public float TimeSpawn {
			get; private set;
		} = 3f;

		/// <summary>
		/// Minimum spawn time
		/// </summary>
		[Export(PropertyHint.Range, "0.01, 100")]
		public float MinTimeSpawn {
			get; private set;
		} = 1f;

		/// <summary>
		/// Time to spawn enemies
		/// </summary>
		[Export(PropertyHint.Range, "0.01, 3")]
		public float TimeDiscount {
			get; private set;
		} = 0.2f;

		/// <summary>
		/// Game controller
		/// </summary>
		public GameController Controller {
			get; private set;
		} = GameController.Controller;

		/// <summary>
		/// Save all instances
		/// </summary>
		public readonly List<Node2D> Instances = new List<Node2D>();

		/// <summary>
		/// Timer to controller spawn instances
		/// </summary>
		public float TimerSpawn {
			get; private set;
		} = 0f;

		/// <summary>
		/// Backup of initial time spawn
		/// </summary>
		private float _initialTimeSpawn;

		/// <summary>
		/// Ready method
		/// </summary>
		public override void _Ready() {
			// Initialize properties
			_initialTimeSpawn = TimeSpawn;
			// Connect cotroller configuration changed
			Controller.Connect(nameof(GameController.ConfigurationChanged), this, nameof(OnConfigurationChanged));
			// Connect new instances singnal
			Connect(nameof(InstancedNode), this, nameof(OnInstanceCreated));
		}

		/// <summary>
		/// Called each frame.
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process(float delta) {
			// Check if game is running or is not paused
			if (!Controller.Start || Controller.Paused)
				return;

			// Check time spawn
			if (TimerSpawn >= TimeSpawn) {
				MakeInstance();
				TimerSpawn = 0;
			}

			// Update timer
			TimerSpawn += delta;
		}

		/// <summary>
		/// Remove all instances
		/// </summary>
		private void RemoveAllInstances() {
			if (Instances.Count > 0) {
				foreach (Node2D localInstance in Instances) {
					localInstance.QueueFree();
				}
				Instances.RemoveAll(RemovePredicate);
			}
		}

		/// <summary>
		/// Called when enemy is deleted
		/// </summary>
		/// <param name="node"></param>
		private void OnNodeDeleted(Node2D node) {
			if (Instances.Contains(node))
				Instances.Remove(node);
		}

		/// <summary>
		/// Remove nodes predicate
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private bool RemovePredicate(Node2D node) {
			return node.IsInGroup("Enemy");
		}

		/// <summary>
		/// Handle configuration change
		/// </summary>
		/// <param name="field"></param>
		private void OnConfigurationChanged(string field) {
			// compare all cases
			switch (field) {
				case "VelocityScale":
					TimeSpawn = Mathf.Clamp(TimeSpawn - TimeDiscount, MinTimeSpawn, 100);
					break;
				case "Start":
					if (!Controller.Start)
						RemoveAllInstances();
					else
						TimeSpawn = _initialTimeSpawn;
					break;
			}
		}

		/// <summary>
		/// Handle new instances
		/// </summary>
		/// <param name="node"></param>
		private void OnInstanceCreated(Node node) {
			Node2D target = node as Node2D;

			target.Position = Position;
			target.RotationDegrees = RotationDegrees;
			target.Scale = Scale;

			Instances.Add(target);
			target.Connect(nameof(EnemyController.NodeDeleted), this, nameof(OnNodeDeleted));

			if (TargetInstance == null)
				GetParent().CallDeferred("add_child", target);
			else
				GetNode(TargetInstance).CallDeferred("add_child", target);
		}

	}


}
