extends RigidBody3D

@export_range(750.0, 3000.0) var thrust: float = 1000.0
@export var torque_thrust: float = 200.0

func _init():
	contact_monitor = true
	axis_lock_linear_z = true
	axis_lock_angular_x = true
	axis_lock_angular_y = true

func _process(delta: float) -> void:
	if Input.is_action_pressed("boost"):
		apply_central_force(basis.y * delta * thrust)

	if Input.is_action_pressed("rotate_left"):
		apply_torque(Vector3(0.0, 0.0, torque_thrust * delta))

	if Input.is_action_pressed("rotate_right"):
		apply_torque(Vector3(0.0, 0.0, -torque_thrust * delta))

func _on_body_entered(body: Node) -> void:
	if "Goal" in body.get_groups():
		complete_level(body.file_path)
	elif "Obstacle" in body.get_groups():
		crash_sequence()

func crash_sequence() -> void:
	print("Boom")
	get_tree().reload_current_scene()

func complete_level(next_level_file: String) -> void:
	print("Level complete")
	get_tree().change_scene_to_file(next_level_file)
