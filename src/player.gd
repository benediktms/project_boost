extends RigidBody3D

@export_range(750.0, 3000.0) var thrust: float = 1000.0
@export var torque_thrust: float = 200.0

@onready var explosion_audio: AudioStreamPlayer = $ExplosionAudio
@onready var state_complete_audio: AudioStreamPlayer = $StageCompleteAudio
@onready var rocket_audio: AudioStreamPlayer3D = $RocketAudio

@onready var booster_particles: GPUParticles3D = $BoosterParticles
@onready var booster_particles_right: GPUParticles3D = $BoosterParticlesRight
@onready var booster_particles_left: GPUParticles3D = $BoosterParticlesLeft

@onready var explosion_particles: GPUParticles3D = $ExplosionParticles
@onready var success_particles: GPUParticles3D = $SuccessParticles

var is_transitioning: bool = false

func _init():
	contact_monitor = true
	axis_lock_linear_z = true
	axis_lock_angular_x = true
	axis_lock_angular_y = true

func _process(delta: float) -> void:
	if Input.is_action_pressed("boost"):
		apply_central_force(basis.y * delta * thrust)
		booster_particles.emitting = true
		if rocket_audio.playing == false:
			rocket_audio.play()
	else:
		rocket_audio.stop()
		booster_particles.emitting = false

	if Input.is_action_pressed("rotate_left"):
		apply_torque(Vector3(0.0, 0.0, torque_thrust * delta))
		booster_particles_right.emitting = true
	else:
		booster_particles_right.emitting = false

	if Input.is_action_pressed("rotate_right"):
		apply_torque(Vector3(0.0, 0.0, -torque_thrust * delta))
		booster_particles_left.emitting = true
	else:
		booster_particles_left.emitting = false

func _on_body_entered(body: Node) -> void:
	if is_transitioning: return

	if "Goal" in body.get_groups():
		complete_level(body.file_path)
	elif "Obstacle" in body.get_groups():
		crash_sequence()

func crash_sequence() -> void:
	print("Boom")
	explosion_audio.play()
	explosion_particles.emitting = true
	is_transitioning = true
	set_process(false)
	var tween = create_tween()
	tween.tween_interval(explosion_audio.stream.get_length())
	tween.tween_callback(get_tree().reload_current_scene)

func complete_level(next_level_file: String) -> void:
	print("Level complete")
	state_complete_audio.play()
	success_particles.emitting = true
	is_transitioning = true
	set_process(false)
	var tween = create_tween()
	tween.tween_interval(state_complete_audio.stream.get_length())
	tween.tween_callback(get_tree().change_scene_to_file.bind(next_level_file))
