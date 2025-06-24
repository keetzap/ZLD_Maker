// Backlog para después del MVP

// Pathfinding improvements
// L'enemic busca el punt més proper per fer patrol (Current implementation)
// L'enemic mai torna a fer patrol. Sempre perseguix al character
// El character fa un trail of bread crumbs i l'enemic torna pel mateix lloc

// Waypoints improvements
// Buscar waypoint i si quan va cap allà es queda stucked durant uns segons a la mateixa posició, buscar un nou waypoint
// Buscar waypoint i llençar un raycast. Si hi ha alguna col·lisió pel mig, buscar un nou waypoint.

using Keetzap.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace Keetzap.ZeldaMaker
{
	[RequireComponent(typeof(IHitable))]
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour
	{
		public static class FieldNames
		{
			public static string EnemyBehaviour => nameof(enemyBehaviour);
			public static string Waypoints => nameof(waypoints);
			public static string PatrolPointsOrder => nameof(patrolPointsOrder);
			public static string EnemyStarsInWaitingMode => nameof(enemyStarsInWaitingMode);
			public static string EnemySpawnsOnFirstWaypoint => nameof(enemySpawnsOnFirstWaypoint);
			public static string WaitingModeOrientation => nameof(waitingModeOrientation);
			public static string PatrolOrientation => nameof(patrolOrientation);
			public static string AlertedOrientation => nameof(alertedOrientation);
			public static string UseSmoothRotation => nameof(useSmoothRotation);
			public static string RotationSpeed => nameof(rotationSpeed);
			public static string AddRecoil => nameof(addRecoil);
			public static string ShowUnalertedHandles => nameof(showUnalertedHandles);
			public static string ShowAlertedHandles => nameof(showAlertedHandles);
			public static string ShowGizmos => nameof(showGizmos);
			public static string ShowDistance => nameof(showDistance);
			public static string ShowPathLines => nameof(showPathLines);
			public static string PathLinesColor => nameof(pathLinesColor);
			public static string ArrowLength => nameof(arrowLength);
			public static string ArrowAngle => nameof(arrowAngle);
			public static string ArrowPosition => nameof(arrowPosition);
		}

		private readonly int idle = Animator.StringToHash(StringsData.IDLE);
		private readonly int patrol = Animator.StringToHash(StringsData.PATROL);
		private readonly int chase = Animator.StringToHash(StringsData.CHASE);
		private readonly int attack = Animator.StringToHash(StringsData.ATTACK);

		public Action<EnemyController> EnemyDeathEvent;

		public enum EnemyState { BackToIdle, Idle, Patrol, Chase, Attack, Wounded, Dead }
		public enum EnemyBehaviour { HuntPlayer, BackToPatrol, BackToIdle }
		public enum PatrolPointsOrder { Sequential, Random }
		public enum WaitingModeOrientation { LookAtPlayer, UserDefault, LastKnownOrientation}
		public enum PatrolOrientation { LookAtWaypoint, UserDefault }
		public enum AlertedOrientation { LookAtPlayer, UserDefault }

        [SerializeField] private WaitingModeOrientation waitingModeOrientation = WaitingModeOrientation.UserDefault;
		[SerializeField] private PatrolOrientation patrolOrientation = PatrolOrientation.LookAtWaypoint;
		[SerializeField] private AlertedOrientation alertedOrientation = AlertedOrientation.LookAtPlayer;
		[SerializeField] private bool useSmoothRotation = true;
        [SerializeField] private float rotationSpeed = 0.1f;
		public PatrolPointsOrder patrolPointsOrder = PatrolPointsOrder.Sequential;
		[SerializeField] private bool addRecoil;

		public EnemyBehaviour enemyBehaviour = EnemyBehaviour.HuntPlayer;
        public List<EnemyWaypoint> waypoints = new();
		public bool enemySpawnsOnFirstWaypoint;
		public bool enemyStarsInWaitingMode;
		public bool showUnalertedHandles;
		public bool showAlertedHandles;
		public bool showGizmos;
		public bool showDistance;
		public bool showPathLines;
		public Color pathLinesColor = Color.magenta;
		public float arrowLength = 0.2f;
		public float arrowAngle = 20f;
		public float arrowPosition = 0.5f;

        private IHitable _attackable;
		private Animator _animator;
		private GD_Enemy _configEnemy;
        private PlayerController _player;
		private EnemyState _enemyState;
		private EnemyWaypoint _oldWaypoint, _currentWaypoint;
		private int _waypointIndex = 0;
		private float _counterToWalk;
        private float _timeToWalk;
		private float _stuntTime;
		private Vector3 _targetPosition;
		private Vector3 _initOrientation;
		private Vector3 _targetRotation;

		public bool _waypointsAreInEnemyPosition;

		public GD_Enemy Enemy => GetComponent<IHitable>().GameDataAsset() as GD_Enemy;
		public bool NoWaypoints => waypoints.Count == 0;
		public bool OnlyOneWaypoint => waypoints.Count == 1;
        public bool ThereAreWaypoints => waypoints.Count > 0;

        public void Awake()
		{
			_attackable = GetComponent<IHitable>();
			_attackable.OnAttackedEvent += OnAttacked;
			_attackable.OnDeadEvent += OnDead;
			_configEnemy = _attackable.GameDataAsset() as GD_Enemy;
			_animator = GetComponentInChildren<Animator>();
			_player = FindAnyObjectByType<PlayerController>();
			_initOrientation = transform.forward;

			if (_waypointsAreInEnemyPosition)
			{
				ConvertWaypointsToWorldPosition();
			}

			SetInitialState();
        }

		private void SetInitialState()
        {
			if (NoWaypoints)
			{
				SetDefaultWaypoint();
				_enemyState = EnemyState.BackToIdle;
			}
			else
			{
				if (enemySpawnsOnFirstWaypoint)
				{
					MoveEnemyToFirstWaypoint();
					SetNewTimeToPatrol();

					if (enemyBehaviour == EnemyBehaviour.BackToPatrol)
                    {
						_enemyState = enemyStarsInWaitingMode ? EnemyState.BackToIdle : OnlyOneWaypoint ? EnemyState.Idle : EnemyState.Patrol;
                    }
                    else
                    {
						_enemyState = OnlyOneWaypoint ? EnemyState.Idle : EnemyState.Patrol;
					}
				}
				else
				{
					_ = GetNewControlPoint();

					if (enemyBehaviour == EnemyBehaviour.BackToPatrol)
					{
						_enemyState = enemyStarsInWaitingMode ? EnemyState.BackToIdle : EnemyState.Patrol;
					}
					else
					{
						_enemyState = EnemyState.Patrol;
					}
					
				}
			}

            _targetRotation = GetTargetRotation(_enemyState);
        }

		private void MoveEnemyToFirstWaypoint()
        {
			_currentWaypoint = waypoints[0];
			transform.position = _currentWaypoint.position;
		}

		void Update()
		{
			//Debug.Log(_enemyState);

			switch (_enemyState)
			{
				case EnemyState.BackToIdle:		WaitingMode();	break;
				case EnemyState.Idle:			Idle();			break;
				case EnemyState.Patrol:			Patrol();		break;
				case EnemyState.Chase:			Chase();		break;
				case EnemyState.Attack:			Attack();		break;
				case EnemyState.Wounded:		Wounded();		break;
                case EnemyState.Dead:			Dead();			break;
            }

			HandleRotation();
		}

		private void HandleRotation()
		{
			_targetRotation = GetTargetRotation(_enemyState);
			Quaternion targetRotation = Quaternion.LookRotation(_targetRotation);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, useSmoothRotation ? rotationSpeed : Mathf.Infinity);
		}

		private void WaitingMode()
		{
			EnableAnimatorParameter(idle);
			CheckDistanceValue();
		}

		private void Idle()
		{
			EnableAnimatorParameter(idle);
			CheckDistanceValue();

			if (NoWaypoints || OnlyOneWaypoint) return;

			_counterToWalk += Time.deltaTime;

			if (_counterToWalk >= _timeToWalk)
			{
				_counterToWalk = 0;
                _enemyState = EnemyState.Patrol;
				GetNewControlPoint();
			}
		}

		private void Patrol()
		{
			EnableAnimatorParameter(patrol);

			transform.position = Vector3.MoveTowards(transform.position, _currentWaypoint.position, _configEnemy.unalertedSpeed * Time.deltaTime);
			_animator.transform.localRotation = Quaternion.identity;

			float thresholdDistance = 0.01f;
			if (Vector3.Distance(transform.position, _currentWaypoint.position) < thresholdDistance)
			{
				if (NoWaypoints || OnlyOneWaypoint)
				{
					_enemyState = EnemyState.BackToIdle;
				}
				else if (_currentWaypoint.waitingTime)
                {
					_enemyState = EnemyState.Idle;
					SetNewTimeToPatrol();
				}
				else
                {
					GetNewControlPoint();
				}
			}

			CheckDistanceValue();
		}

		private void Chase()
		{
			EnableAnimatorParameter(chase);

			transform.position = Vector3.MoveTowards(transform.position, _player.Position, _configEnemy.alertedSpeed * Time.deltaTime);

			float dist = GetDistance();

			if (GetAngle() > _configEnemy.alertedAngleOfAction || dist > _configEnemy.alertedRangeOfSight)
            {
				GetOutFromAlertedRange();
			}
			else if (GetAngle() < _configEnemy.alertedAngleOfAction && dist < _configEnemy.alertedRangeOfAttack)
            {
				_enemyState = EnemyState.Attack;
			}
		}

		private void GetOutFromAlertedRange()
        {
			switch (enemyBehaviour)
			{
				case EnemyBehaviour.BackToPatrol:
				{
					_ = GetCloserPoint().position;
					_enemyState = EnemyState.Patrol;
					break;
				}
				case EnemyBehaviour.BackToIdle:
				{
					_enemyState = EnemyState.BackToIdle;
					break;
				}
			}
		}

        private void Attack() => EnableAnimatorParameter(attack);

        public void CheckAttackBehaviour()
        {
			float dist = GetDistance();

			if (GetAngle() < _configEnemy.alertedAngleOfAction)
			{
				if (dist <= _configEnemy.alertedRangeOfSight && dist > _configEnemy.alertedRangeOfAttack)
				{
					_enemyState = EnemyState.Chase;
				}
				else if (dist > _configEnemy.alertedRangeOfSight)
				{
					_ = GetCloserPoint().position;
					_enemyState = EnemyState.Patrol;
				}
			}
		}

		public void OnAttacked(int damage, float power)
        {
			GetAttackedParameters(power);
            
            _animator.SetTrigger(StringsData.WOUNDED);
            _enemyState = EnemyState.Wounded;
        }

		private void Wounded()
        {
            Recoil();

            _stuntTime -= Time.deltaTime;

            if (_stuntTime <= 0)
            {
                _stuntTime = _configEnemy.stuntTime;
                _animator.SetBool(StringsData.ATTACK, true);
            }
        }

        public void OnDead(int damage, float power)
		{
			GetAttackedParameters(power);

			_animator.SetTrigger(StringsData.DEAD);
			_enemyState = EnemyState.Dead;
		}

		private void Dead()
        {
            Recoil();

			_stuntTime -= Time.deltaTime;

            if (_stuntTime <= 0)
            {
                EnemyDeathEvent?.Invoke(this);
               
                Destroy(gameObject);
            }
        }

		private void GetAttackedParameters(float power)
		{
			Vector3 direction = (transform.position - _player.Position).normalized;
			_targetPosition = transform.position + _configEnemy.recoil * power * direction;
			_stuntTime = _configEnemy.stuntTime;

			DisableAnimatorParameters();
		}

		private void Recoil()
        {
            if (addRecoil)
            {
                float step = _configEnemy.recoilSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
            }
        }

		private EnemyWaypoint GetNewControlPoint()
        {
            _oldWaypoint = _currentWaypoint;
			_currentWaypoint = patrolPointsOrder == PatrolPointsOrder.Random ? waypoints[UnityRandom.Range(0, waypoints.Count)] : waypoints[_waypointIndex++];

            if (_waypointIndex == waypoints.Count)
                _waypointIndex = 0;

            return _currentWaypoint;
        }

		EnemyWaypoint GetCloserPoint()
		{
			if (ThereAreWaypoints)
            {
				float distance = Mathf.Infinity;
				EnemyWaypoint closerPoint = new();

				foreach (var waypoint in waypoints)
				{
					float newDistance = Vector3.Distance(_player.Position, waypoint.position);
					if (newDistance < distance)
					{
						distance = newDistance;
						closerPoint = waypoint;
						_waypointIndex = waypoints.FindIndex(w => w == waypoint);
					}
				}

				_currentWaypoint = closerPoint;
            }

			return _currentWaypoint;
		}

		private Vector3 GetTargetRotation(EnemyState enemyState)
        {
			switch (enemyState)
            {
				case EnemyState.BackToIdle:
                {
					switch (waitingModeOrientation)
                    {
						case WaitingModeOrientation.UserDefault: return UserDefaultOrientation();
						case WaitingModeOrientation.LookAtPlayer: return LookAtPlayerOrientation();
						case WaitingModeOrientation.LastKnownOrientation: return LastKnownOrientation();
                    }
					break;
                }
				case EnemyState.Patrol:
                {
					switch (patrolOrientation)
                    {
						case PatrolOrientation.UserDefault: return UserDefaultOrientation();
						case PatrolOrientation.LookAtWaypoint: return LookAtWaypointOrientation();
                    }
					break;
                    }
				default:
                {
					switch(alertedOrientation)
                    {
						case AlertedOrientation.UserDefault: return UserDefaultOrientation();
						case AlertedOrientation.LookAtPlayer: return LookAtPlayerOrientation();
                    }
                }
				break;
            }

			return Vector3.zero;
        }

		private Vector3 UserDefaultOrientation() => _initOrientation;
		private Vector3 LookAtPlayerOrientation() => new Vector3(_player.Position.x, transform.position.y, _player.Position.z) - transform.position;
		private Vector3 LookAtWaypointOrientation() => new Vector3(_currentWaypoint.position.x, transform.position.y, _currentWaypoint.position.z) - transform.position;
		private Vector3 LastKnownOrientation() => transform.forward;

		private void SetNewTimeToPatrol()
		{
			_timeToWalk = _currentWaypoint.TimeOnWaypoint;
        }

		private void CheckDistanceValue()
        {
            if (GetAngle() >= _configEnemy.unalertedAngleOfAction) return;
            
			float dist = GetDistance();

            if (dist <= _configEnemy.unalertedRangeOfSight && dist > _configEnemy.unalertedRangeOfAttack)
            {
                DisableAnimatorParameters();
                _enemyState = EnemyState.Chase;
            }
			else if (dist <= _configEnemy.unalertedRangeOfAttack)
            {
                DisableAnimatorParameters();
                _enemyState = EnemyState.Attack;
            }
        }

        private void DisableAnimatorParameters()
        {
			_animator.SetBool(StringsData.IDLE, false);
			_animator.SetBool(StringsData.PATROL, false);
			_animator.SetBool(StringsData.ATTACK, false);
		}

		public void EnableAnimatorParameter(int parameter)
        {
			if (!_animator.GetBool(parameter))
			{
				_animator.SetBool(parameter, true);
			}
		}

		public void DisableAnimatorParameter(TypeOfParameter animatorParameter) => _animator.SetBool(animatorParameter.ToString(), false);

		private float GetDistance() => Vector3.Distance(transform.position, _player.Position);

		private float GetAngle() => Vector3.Angle(_player.Position - transform.position, transform.forward) * 2;

		public void MoveToEnemyPosition()
        {
			if (waypoints.Count > 0)
            {
				waypoints[0].position = transform.position;
            }
        }

		public void MoveEnemyToWaypoint()
		{
			if (waypoints.Count > 0)
			{
				transform.position = waypoints[0].position;
			}
		}

		private void SetDefaultWaypoint()
        {
			_currentWaypoint = new();
			_currentWaypoint.position = transform.position;
			_currentWaypoint.waitingTime = false;
			_currentWaypoint.timeOnWaypoint = new Vector2(1, 2);
        }

		public void CreateWaypointOnEnemyPosition()
		{
			SetDefaultWaypoint();
			waypoints.Add(_currentWaypoint);
		}

		public void ConvertWaypointsToEnemyPosition()
        {
			if (_waypointsAreInEnemyPosition) return;

			Debug.Log("Converting waypoints to Enemy position.");
			_waypointsAreInEnemyPosition = true;

            foreach (var waypoint in waypoints)
            {
				waypoint.position -= transform.position;
            }
        }

		public void ConvertWaypointsToWorldPosition()
		{
			if (!_waypointsAreInEnemyPosition) return;

			Debug.Log("Converting waypoints to World position.");
			_waypointsAreInEnemyPosition = false;

			foreach (var waypoint in waypoints)
			{
				waypoint.position += transform.position;
			}
		}

		#region DEBUG

		private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            if (showDistance && _player != null)
            {
                switch (_enemyState)
                {
                    case EnemyState.Idle:
                    case EnemyState.Patrol: DrawLine(Color.green); break;
                    case EnemyState.Chase: DrawLine(_configEnemy.alertedRangeOfSightColor); break;
                    case EnemyState.Attack: DrawLine(_configEnemy.alertedRangeOfAttackColor); break;
                }
            }
            
			if (NoWaypoints) return;

            if (showPathLines)
            {
                Gizmos.color = pathLinesColor;

				var waypointOffset = _waypointsAreInEnemyPosition ? transform.position : Vector3.zero;

                if (patrolPointsOrder == PatrolPointsOrder.Sequential)
                {
                    for (int w = 0; w < waypoints.Count; w++)
                    {
                        int nextWaypoint = w == waypoints.Count - 1 ? 0 : w + 1;
                        CustomGizmos.DrawArrow(waypoints[w].position + waypointOffset, waypoints[nextWaypoint].position + waypointOffset, pathLinesColor, arrowLength, arrowAngle, arrowPosition);
                    }
                }
                else
                {
                    if (_oldWaypoint != null)
                    {
                        CustomGizmos.DrawArrow(_oldWaypoint.position + waypointOffset, _currentWaypoint.position + waypointOffset, pathLinesColor, arrowLength, arrowAngle, arrowPosition);
                    }
                }
            }
        }

        private void DrawLine(Color color)
        {
			Debug.DrawLine(transform.position, _player.Position, color, 0.033f);
		}
	}

    #endregion

    [Serializable]
    public struct ControllerEnemy
    {
        public EnemyController enemyBehaviour;
    }
}