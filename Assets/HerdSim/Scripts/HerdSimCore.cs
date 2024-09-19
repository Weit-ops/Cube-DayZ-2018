using System.Collections;
using UnityEngine;

public class HerdSimCore : MonoBehaviour
{
	private const float HerdLogicUpdateTime = 0.1f;

	public HerdSimController _controller;

	public Transform _scanner;

	public Transform _collider;

	public Transform _model;

	public Renderer _renderer;

	public float _hitPoints = 100f;

	public int _type;

	public float _minSize = 1f;

	public float _avoidAngle = 0.35f;

	public float _avoidDistance;

	public float _avoidSpeed = 75f;

	public float _stopDistance;

	private float _rotateCounterR;

	private float _rotateCounterL;

	public bool _pushHalfTheTime;

	private bool _pushToggle;

	public float _pushDistance;

	public float _pushForce = 5f;

	private bool _scan;

	public Vector3 _roamingArea;

	public float _walkSpeed = 0.5f;

	public float _runSpeed = 1.5f;

	public float _damping;

	public int _idleProbablity = 20;

	public float _runChance = 0.1f;

	[HideInInspector]
	public Vector3 _waypoint;

	[HideInInspector]
	public float _speed;

	[HideInInspector]
	public float _targetSpeed;

	[HideInInspector]
	public int _mode;

	[HideInInspector]
	public Vector3 _startPosition;

	private bool _reachedWaypoint = true;

	private int _lerpCounter;

	[HideInInspector]
	public bool _scared;

	[HideInInspector]
	public Transform _scaredOf;

	[HideInInspector]
	public bool _eating;

	[HideInInspector]
	public Transform _food;

	public float _groundCheckInterval = 0.1f;

	public string _groundTag = "Ground";

	private Vector3 _ground;

	private Quaternion _groundRot;

	private bool _grounded;

	public float _maxGroundAngle = 45f;

	public float _maxFall = 3f;

	public float _fakeGravity = 5f;

	public LayerMask _herdLayerMask = -1;

	[HideInInspector]
	public HerdSimCore _leader;

	[HideInInspector]
	public Vector3 _leaderArea;

	[HideInInspector]
	public int _leaderSize;

	public float _leaderAreaMultiplier = 0.2f;

	public int _maxHerdSize = 25;

	public int _minHerdSize = 10;

	public float _herdDistance = 2f;

	private int _herdSize;

	[HideInInspector]
	public bool _dead;

	public float _randomDeath = 0.001f;

	public Material _deadMaterial;

	public bool _scaryCorpse;

	public string _animIdle = "idle";

	public float _animIdleSpeed = 1f;

	public string _animSleep = "sleep";

	public float _animSleepSpeed = 1f;

	public string _animWalk = "walk";

	public float _animWalkSpeed = 1f;

	public string _animRun = "run";

	public float _animRunSpeed = 1f;

	public string _animDead = "dead";

	public float _animDeadSpeed = 1f;

	public float _idleToSleepSeconds = 1f;

	private float _sleepCounter;

	private bool _idle;

	private int _updateCounter;

	public int _updateDivisor = 1;

	private static int _updateNextSeed;

	private int _updateSeed = -1;

	private float _newDelta;

	[HideInInspector]
	public bool _enabled;

	public LayerMask _groundLayerMask = -1;

	public LayerMask _pushyLayerMask = -1;

	public string _herdSimLayerName = "HerdSim";

	private int _groundIndex = 25;

	private int _herdSimIndex = 26;

	private Transform _transform;

	private bool _avoidance;

	public void Disable(bool disableModel, bool disableCollider)
	{
		if (_enabled)
		{
			_enabled = false;
			CancelInvoke();
			if (disableModel)
			{
				_model.gameObject.SetActive(false);
			}
			if (disableCollider)
			{
				_collider.gameObject.SetActive(false);
			}
			_transform.GetComponent<HerdSimCore>().enabled = false;
			_model.GetComponent<Animation>().Stop();
		}
	}

	public void Enable()
	{
		if (!_enabled)
		{
			_enabled = true;
			Init();
			if (!_model.gameObject.activeInHierarchy)
			{
				_model.gameObject.SetActive(true);
			}
			if (!_collider.gameObject.activeInHierarchy)
			{
				_collider.gameObject.SetActive(true);
			}
			_transform.GetComponent<HerdSimCore>().enabled = true;
			_model.GetComponent<Animation>().Play();
		}
	}

	private void OnDisable()
	{
		CancelInvoke();
		_model.GetComponent<Animation>().Stop();
	}

	private void OnEnable()
	{
		if (_enabled)
		{
			Init();
			_model.GetComponent<Animation>().Play();
		}
	}

	public void Damage(float d)
	{
		_hitPoints -= d;
		if (_hitPoints <= 0f)
		{
			Death(false);
		}
	}

	public void DamageByPlayer(float d)
	{
		_hitPoints -= d;
		if (_hitPoints <= 0f)
		{
			Death(true);
		}
	}

	public void Effects()
	{
		if (_controller != null && _mode == 2 && _controller._runPS != null && _speed > 1f)
		{
			_controller._runPS.transform.position = base.transform.position;
			_controller._runPS.Emit(1);
		}
		if (_dead && _controller != null && _controller._deadPS != null)
		{
			_controller._deadPS.transform.position = _collider.transform.position;
			_controller._deadPS.Emit(1);
		}
	}

	public void Death(bool byPlayer)
	{
		if (_dead)
		{
			return;
		}
		_dead = true;
		_mode = 0;
		CancelInvoke("Wander");
		CancelInvoke("WalkTimeOut");
		CancelInvoke("FindLeader");
		if (_leader != null)
		{
			if (_leader != this)
			{
				_leader._leaderSize--;
			}
			else
			{
				_leaderSize = 0;
			}
			_leader = null;
		}
		if (_deadMaterial != null && !byPlayer)
		{
			_renderer.sharedMaterial = _deadMaterial;
		}
		_model.GetComponent<Animation>()[_animDead].speed = 1f;
		_model.GetComponent<Animation>().CrossFade(_animDead, 0.1f);
		base.gameObject.SendMessage("DropMeat", byPlayer, SendMessageOptions.DontRequireReceiver);
		if (_scaryCorpse)
		{
			InvokeRepeating("Corpse", 1f, 1f);
		}
	}

	public void Corpse()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 10f, _herdLayerMask);
		HerdSimCore herdSimCore = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.parent != null)
			{
				herdSimCore = array[i].transform.parent.GetComponent<HerdSimCore>();
			}
			if (_scaryCorpse && herdSimCore != null && !herdSimCore._dead && herdSimCore._mode < 1)
			{
				StartCoroutine(herdSimCore.Scare(base.transform));
			}
		}
	}

	public void FindLeader()
	{
		if (_leader == this && _leaderSize <= 1)
		{
			_leader = null;
			_leaderSize = 0;
		}
		else
		{
			if (!(_leader != this))
			{
				return;
			}
			if (_leader != null && _leader._dead)
			{
				_leader = null;
			}
			_leaderSize = 0;
			Collider[] array = Physics.OverlapSphere(base.transform.position, _herdDistance, _herdLayerMask);
			HerdSimCore herdSimCore = null;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].transform.parent != null)
				{
					herdSimCore = array[i].transform.parent.GetComponent<HerdSimCore>();
				}
				if (herdSimCore != null && herdSimCore != this && _type == herdSimCore._type)
				{
					if (_leader == null && herdSimCore._leader == null)
					{
						_leader = this;
						herdSimCore._leader = this;
						_leaderSize += 2;
						break;
					}
					if (_leader == null && herdSimCore._leader != null && herdSimCore._leader._leaderSize < herdSimCore._leader._herdSize)
					{
						_leader = herdSimCore._leader;
						_leader._leaderSize++;
						break;
					}
					if (_leader != null && herdSimCore._leader != _leader && herdSimCore._leader != null && herdSimCore._leader._leaderSize >= _leader._leaderSize && herdSimCore._leader._leaderSize < herdSimCore._leader._herdSize)
					{
						_leader._leaderSize--;
						herdSimCore._leader._leaderSize++;
						_leader = herdSimCore._leader;
						break;
					}
				}
			}
		}
	}

	public void Wander()
	{
		Vector3 waypoint = Vector3.zero;
		if (_leader == this)
		{
			_leaderArea = Vector3.one * ((float)_leaderSize * _leaderAreaMultiplier + 1f);
		}
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (_leader != null && _leader != this)
		{
			zero = _leader._leaderArea;
			zero2 = _leader.transform.position;
		}
		else if (_controller == null)
		{
			zero = _roamingArea;
			zero2 = _startPosition;
		}
		else
		{
			zero = _controller._roamingArea;
			zero2 = _controller.transform.position;
		}
		waypoint.x = UnityEngine.Random.Range(0f - zero.x, zero.x) + zero2.x;
		waypoint.z = UnityEngine.Random.Range(0f - zero.z, zero.z) + zero2.z;
		if (_food != null)
		{
			waypoint = _food.position;
			_mode = 2;
		}
		else if (_transform.position.x < 0f - zero.x + zero2.x || _transform.position.x > zero.x + zero2.x || _transform.position.z < 0f - zero.z + zero2.z || _transform.position.z > zero.z + zero2.z)
		{
			if (UnityEngine.Random.value < 0.1f)
			{
				_mode = 2;
			}
			else
			{
				_mode = 1;
			}
			_waypoint = waypoint;
		}
		else if (_leader != null && _leader != this && UnityEngine.Random.value < 0.75f)
		{
			_mode = 0;
		}
		else if (_reachedWaypoint)
		{
			_mode = UnityEngine.Random.Range(-_idleProbablity, 2);
			if (_mode == 1 && UnityEngine.Random.value < _runChance && (_leader == null || _leader == this))
			{
				_mode = 2;
			}
		}
		if (_reachedWaypoint && _mode > 0)
		{
			_waypoint = waypoint;
			CancelInvoke("WalkTimeOut");
			Invoke("WalkTimeOut", 30f);
			_reachedWaypoint = false;
		}
		_waypoint.y = _collider.transform.position.y;
		_lerpCounter = 0;
		if (UnityEngine.Random.value < _randomDeath)
		{
			Death(false);
		}
	}

	public void Init()
	{
		if (_controller != null)
		{
			InvokeRepeating("Effects", 1f + UnityEngine.Random.value, 0.1f);
		}
		InvokeRepeating("Wander", 1f + UnityEngine.Random.value, 1f);
		InvokeRepeating("GroundCheck", _groundCheckInterval * UnityEngine.Random.value + 1f, _groundCheckInterval);
		InvokeRepeating("FindLeader", UnityEngine.Random.value * 3f, 3f);
	}

	public void Start()
	{
		_transform = base.transform;
		_enabled = true;
		_groundIndex = LayerMask.NameToLayer(_groundTag);
		_herdSimIndex = LayerMask.NameToLayer(_herdSimLayerName);
		if (_updateDivisor > 1)
		{
			int num = _updateDivisor - 1;
			_updateNextSeed++;
			_updateSeed = _updateNextSeed;
			_updateNextSeed %= num;
		}
		if (_groundTag == null)
		{
			_groundTag = "Ground";
		}
		Init();
		_startPosition = _transform.position;
		if (_pushDistance <= 0f)
		{
			_pushDistance = _avoidDistance * 0.25f;
		}
		if (_stopDistance <= 0f)
		{
			_stopDistance = _avoidDistance * 0.25f;
		}
		_ground = (_waypoint = _transform.position);
		float maxFall = _maxFall;
		_maxFall = 1000000f;
		GroundCheck();
		_maxFall = maxFall;
		if (_collider == null)
		{
			_collider = _transform.Find("Collider");
		}
		_herdSize = UnityEngine.Random.Range(_minHerdSize, _maxHerdSize);
		if (_minSize < 1f)
		{
			_transform.localScale = Vector3.one * UnityEngine.Random.Range(_minSize, 1f);
		}
		_model.GetComponent<Animation>()[_animIdle].speed = _animIdleSpeed;
		_model.GetComponent<Animation>()[_animDead].speed = _animDeadSpeed;
		_model.GetComponent<Animation>()[_animSleep].speed = _animSleepSpeed;
	}

	public void AnimationHandler()
	{
		if (_dead)
		{
			return;
		}
		if (_mode == 1)
		{
			if (_speed > 0f)
			{
				_model.GetComponent<Animation>()[_animWalk].speed = _speed * _animWalkSpeed + 0.051f;
			}
			else
			{
				_model.GetComponent<Animation>()[_animWalk].speed = 0.1f;
			}
			_model.GetComponent<Animation>().CrossFade(_animWalk, 0.25f);
			_idle = false;
		}
		else if (_mode == 2)
		{
			if (_speed > 0f)
			{
				_model.GetComponent<Animation>()[_animRun].speed = _speed * _animRunSpeed + 0.051f;
			}
			else
			{
				_model.GetComponent<Animation>()[_animRun].speed = 0.1f;
			}
			_model.GetComponent<Animation>().CrossFade(_animRun, 0.25f);
			_idle = false;
		}
		else if (_speed < 0.25f)
		{
			if (!_idle)
			{
				_sleepCounter = 0f;
				_model.GetComponent<Animation>().CrossFade(_animIdle, 0.5f);
				_idle = true;
			}
			if (_sleepCounter > _idleToSleepSeconds)
			{
				_model.GetComponent<Animation>().CrossFade(_animSleep, 1f);
			}
			else
			{
				_sleepCounter += _newDelta;
			}
		}
	}

	public IEnumerator Scare(Transform t)
	{
		_scaredOf = t;
		_mode = 2;
		if (!_scared)
		{
			_scared = true;
			UnFlock();
			yield return new WaitForSeconds(3f);
			_scared = false;
			Wander();
			_reachedWaypoint = true;
		}
	}

	public void Food(Transform t)
	{
		if (_food == null)
		{
			_food = t;
		}
	}

	public void Pushy()
	{
		RaycastHit hitInfo = default(RaycastHit);
		float num = 0f;
		Vector3 forward = _scanner.forward;
		if (_scan)
		{
			_scanner.Rotate(new Vector3(0f, 1000f * _newDelta, 0f));
		}
		else
		{
			_scanner.Rotate(new Vector3(0f, 250f * _newDelta, 0f));
		}
		if (Physics.Raycast(_collider.transform.position, forward, out hitInfo, _pushDistance, _pushyLayerMask))
		{
			Transform transform = hitInfo.transform;
			if (transform.gameObject.layer != _groundIndex || (transform.gameObject.layer == _groundIndex && Vector3.Angle(Vector3.up, hitInfo.normal) > _maxGroundAngle))
			{
				float distance = hitInfo.distance;
				num = (_pushDistance - distance) / _pushDistance;
				if (base.gameObject.layer != _herdSimIndex)
				{
					_transform.position -= forward * _newDelta * num * _pushForce;
				}
				else if (distance < _pushDistance * 0.5f)
				{
					_transform.position -= forward * _newDelta * (num - 0.5f) * _pushForce;
				}
				_scan = false;
			}
			else
			{
				_scan = true;
			}
		}
		else
		{
			_scan = true;
		}
	}

	public void UnFlock()
	{
		if (_leader != null && _leader != this)
		{
			_reachedWaypoint = true;
			_leader._leaderSize--;
			_leader = null;
			Wander();
		}
	}

	public void WalkTimeOut()
	{
		_reachedWaypoint = true;
		UnFlock();
		Wander();
	}

	private void FixedUpdate()
	{
		if (!_pushHalfTheTime || _pushToggle)
		{
			Pushy();
		}
		_pushToggle = !_pushToggle;
		if (!_dead)
		{
			_avoidance = Avoidance();
		}
	}

	public void Update()
	{
		if (_updateDivisor > 1)
		{
			_updateCounter++;
			if (_updateCounter != _updateSeed)
			{
				_updateCounter %= _updateDivisor;
				return;
			}
			_updateCounter %= _updateDivisor;
			_newDelta = Time.deltaTime * (float)_updateDivisor;
		}
		else
		{
			_newDelta = Time.deltaTime;
		}
		Vector3 position = _transform.position;
		position.y -= (_transform.position.y - _ground.y) * _newDelta * _fakeGravity;
		_transform.position = position;
		if (!_dead)
		{
			AnimationHandler();
			Vector3 zero = Vector3.zero;
			Quaternion to = Quaternion.identity;
			_model.transform.rotation = Quaternion.Slerp(_model.transform.rotation, _groundRot, _newDelta * 5f);
			Quaternion localRotation = _model.transform.localRotation;
			localRotation.eulerAngles = new Vector3(localRotation.eulerAngles.x, 0f, localRotation.eulerAngles.y);
			_model.transform.localRotation = localRotation;
			if (!_scared && _mode > 0)
			{
				zero = _waypoint - _transform.position;
				if (zero != Vector3.zero)
				{
					to = Quaternion.LookRotation(zero);
				}
			}
			else if (_scared)
			{
				zero = _scaredOf.position - _transform.position;
				if (zero != Vector3.zero)
				{
					to = Quaternion.LookRotation(-zero);
				}
			}
			if ((_transform.position - _waypoint).sqrMagnitude < 10f)
			{
				if (_mode > 0)
				{
					_mode = 1;
				}
				_reachedWaypoint = true;
			}
			else
			{
				_eating = false;
			}
			if (_scared || (_leader != null && _leader != this && _leader._mode == 2))
			{
				_mode = 2;
			}
			else if (_eating)
			{
				_mode = 0;
			}
			if (_mode == 1)
			{
				if (_leader != this)
				{
					_targetSpeed = _walkSpeed;
				}
				else
				{
					_targetSpeed = _walkSpeed * 0.75f;
				}
			}
			else if (_mode == 2)
			{
				_targetSpeed = _runSpeed;
			}
			_speed = Mathf.Lerp(_speed, _targetSpeed, (float)_lerpCounter * _newDelta * 0.05f);
			_lerpCounter++;
			if (!_avoidance)
			{
				Quaternion rotation = Quaternion.Slerp(_transform.rotation, to, _newDelta * _damping);
				_transform.rotation = rotation;
			}
			if (_mode == 1)
			{
				_targetSpeed = _walkSpeed;
			}
			else if (_mode == 2)
			{
				_targetSpeed = _runSpeed;
			}
			else if (_mode <= 0)
			{
				_targetSpeed = 0f;
			}
			_transform.rotation = Quaternion.Euler(0f, _transform.rotation.eulerAngles.y, 0f);
		}
		if (!_grounded)
		{
			_scared = false;
			UnFlock();
			Vector3 zero2 = Vector3.zero;
			zero2 = _transform.position;
			zero2.x -= (_transform.position.x - _ground.x) * _newDelta * 15f;
			zero2.z -= (_transform.position.z - _ground.z) * _newDelta * 15f;
			_transform.position = zero2;
		}
		else if (!_dead)
		{
			_transform.position += _transform.TransformDirection(Vector3.forward) * _speed * _newDelta;
		}
	}

	public void GroundCheck()
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(new Vector3(_transform.position.x, _collider.transform.position.y, _transform.position.z), -_transform.up, out hitInfo, _maxFall, _groundLayerMask))
		{
			_grounded = true;
			_groundRot = Quaternion.FromToRotation(_model.transform.up, hitInfo.normal) * _model.transform.rotation;
			_ground = hitInfo.point;
		}
		else
		{
			_grounded = false;
			_waypoint = _transform.position + _transform.right * 5f;
			_speed = 0f;
		}
	}

	public bool Avoidance()
	{
		bool result = false;
		RaycastHit hitInfo = default(RaycastHit);
		float num = 0f;
		Vector3 forward = _model.transform.forward;
		Vector3 right = _model.transform.right;
		Transform transform = null;
		float num2 = Mathf.Clamp(_speed, 0.1f, 1f);
		Quaternion identity = Quaternion.identity;
		if (_mode == 0 && _speed < 0.21f)
		{
			return true;
		}
		if (Physics.Raycast(_collider.transform.position, forward + right * UnityEngine.Random.Range(-0.1f, 0.1f), out hitInfo, _stopDistance, _pushyLayerMask))
		{
			transform = hitInfo.transform;
			if (transform.gameObject.layer != _groundIndex || (transform.gameObject.layer == _groundIndex && Vector3.Angle(Vector3.up, hitInfo.normal) > _maxGroundAngle))
			{
				float distance = hitInfo.distance;
				num = (_stopDistance - distance) / _stopDistance;
				identity = _transform.rotation;
				if (_rotateCounterL > _rotateCounterR)
				{
					identity.eulerAngles = new Vector3(identity.eulerAngles.x, identity.eulerAngles.y - _avoidSpeed * _newDelta * num, identity.eulerAngles.z);
				}
				else
				{
					identity.eulerAngles = new Vector3(identity.eulerAngles.x, identity.eulerAngles.y + _avoidSpeed * _newDelta * num, identity.eulerAngles.z);
				}
				_transform.rotation = identity;
				if (distance < _stopDistance * 0.5f)
				{
					_speed = -0.2f;
					result = true;
				}
				else if (_speed > 0.2f)
				{
					_speed -= _newDelta * (1f - num) * 25f;
				}
				else
				{
					_speed = 0.2f;
				}
				if (_speed < -0.2f)
				{
					_speed = -0.2f;
				}
			}
		}
		if (_mode > 0 && Physics.Raycast(_collider.transform.position, forward + right * (_avoidAngle + _rotateCounterL), out hitInfo, _avoidDistance, _pushyLayerMask))
		{
			transform = hitInfo.transform;
			if (transform.gameObject.layer != _groundIndex || (transform.gameObject.layer == _groundIndex && Vector3.Angle(Vector3.up, hitInfo.normal) > _maxGroundAngle))
			{
				_rotateCounterL += _newDelta;
				num = (_avoidDistance - hitInfo.distance) / _avoidDistance;
				identity = _transform.rotation;
				identity.eulerAngles = new Vector3(identity.eulerAngles.x, identity.eulerAngles.y - _avoidSpeed * _newDelta * num * _rotateCounterL * num2, identity.eulerAngles.z);
				_transform.rotation = identity;
				if (_rotateCounterL > 1.5f)
				{
					_rotateCounterL = 1.5f;
					_rotateCounterR = 0f;
					result = true;
				}
			}
		}
		else if (_mode > 0 && Physics.Raycast(_collider.transform.position, forward + right * (0f - (_avoidAngle + _rotateCounterR)), out hitInfo, _avoidDistance, _pushyLayerMask))
		{
			transform = hitInfo.transform;
			if (transform.gameObject.layer != _groundIndex || (transform.gameObject.layer == _groundIndex && Vector3.Angle(Vector3.up, hitInfo.normal) > _maxGroundAngle))
			{
				_rotateCounterR += _newDelta;
				num = (_avoidDistance - hitInfo.distance) / _avoidDistance;
				identity = _transform.rotation;
				identity.eulerAngles = new Vector3(identity.eulerAngles.x, identity.eulerAngles.y + _avoidSpeed * _newDelta * num * _rotateCounterR * num2, identity.eulerAngles.z);
				_transform.rotation = identity;
				if (_rotateCounterR > 1.5f)
				{
					_rotateCounterR = 1.5f;
					_rotateCounterL = 0f;
					result = true;
				}
			}
		}
		else
		{
			_rotateCounterL = 0f;
			_rotateCounterR = 0f;
		}
		return result;
	}
}
