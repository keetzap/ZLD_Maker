using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Keetzap.Utils;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CustomEditor(typeof(EnemyController))]
    public class EnemyControllerInspector : BaseEditor
    {
        private EnemyController enemy;

        private SerializedProperty enemyBehaviour;
        private SerializedProperty waypoints, patrolPointsOrder;
        private SerializedProperty enemyStarsInWaitingMode, enemySpawnsOnFirstWaypoint;
        private SerializedProperty waitingModeOrientation, patrolOrientation, alertedOrientation;
        private SerializedProperty useSmoothRotation, rotationSpeed;
        private SerializedProperty addRecoil;
        private SerializedProperty showUnalertedHandles, showAlertedHandles;
        private SerializedProperty showGizmos;
        private SerializedProperty showDistance;
        private SerializedProperty showPathLines, pathLinesColor, arrowLength, arrowAngle, arrowPosition;

        private ReorderableList list;
        private int lineSpace = 6;
        private bool waypointsFollowEnemy;

        void OnEnable()
        {
            enemy = (EnemyController)target;

            enemyBehaviour = serializedObject.FindProperty(EnemyController.FieldNames.EnemyBehaviour);
            waypoints = serializedObject.FindProperty(EnemyController.FieldNames.Waypoints);
            patrolPointsOrder = serializedObject.FindProperty(EnemyController.FieldNames.PatrolPointsOrder);
            enemyStarsInWaitingMode = serializedObject.FindProperty(EnemyController.FieldNames.EnemyStarsInWaitingMode);
            enemySpawnsOnFirstWaypoint = serializedObject.FindProperty(EnemyController.FieldNames.EnemySpawnsOnFirstWaypoint);
            waitingModeOrientation = serializedObject.FindProperty(EnemyController.FieldNames.WaitingModeOrientation);
            patrolOrientation = serializedObject.FindProperty(EnemyController.FieldNames.PatrolOrientation);
            alertedOrientation = serializedObject.FindProperty(EnemyController.FieldNames.AlertedOrientation);
            useSmoothRotation = serializedObject.FindProperty(EnemyController.FieldNames.UseSmoothRotation);
            rotationSpeed = serializedObject.FindProperty(EnemyController.FieldNames.RotationSpeed);
            addRecoil = serializedObject.FindProperty(EnemyController.FieldNames.AddRecoil);
            showUnalertedHandles = serializedObject.FindProperty(EnemyController.FieldNames.ShowAlertedHandles);
            showAlertedHandles = serializedObject.FindProperty(EnemyController.FieldNames.ShowUnalertedHandles);
            showGizmos = serializedObject.FindProperty(EnemyController.FieldNames.ShowGizmos);
            showDistance = serializedObject.FindProperty(EnemyController.FieldNames.ShowDistance);
            showPathLines = serializedObject.FindProperty(EnemyController.FieldNames.ShowPathLines);
            pathLinesColor = serializedObject.FindProperty(EnemyController.FieldNames.PathLinesColor);
            arrowLength = serializedObject.FindProperty(EnemyController.FieldNames.ArrowLength);
            arrowAngle = serializedObject.FindProperty(EnemyController.FieldNames.ArrowAngle);
            arrowPosition = serializedObject.FindProperty(EnemyController.FieldNames.ArrowPosition);

            list = new ReorderableList(serializedObject, waypoints, true, false, true, true);
            list.drawElementCallback = DrawListItems;
            list.elementHeightCallback = (int index) => { return (EditorGUIUtility.singleLineHeight + lineSpace) * 2; };
            list.onAddCallback = AddFirstWaypointOnEnemyPosition;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Section("ENEMY BEHAVIOUR", SectionBehaviour);
            Section("ENEMY ORIENTATION", SectionOrientation);
            Section("ENEMY MOVEMENT and WAYPOINTS", SectionWaypoints);
            Section("OTHER PROPERTIES", SectionOtherProperties);
            Section("DEBUG", SectionDebug);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(enemy);
        }

        private void SectionBehaviour()
        {
            EditorGUIUtility.labelWidth = 180;
            EditorGUILayout.PropertyField(enemyBehaviour, new GUIContent("If player exits alert range:"));
            ResetLabelWidth();
            EditorGUILayout.Space(3);

            switch (enemy.enemyBehaviour)
            {
                case EnemyController.EnemyBehaviour.HuntPlayer:
                {
                    EditorGUILayout.HelpBox("Enemy will chase and attack the player until it is killed.", MessageType.Info, true);
                    enemyStarsInWaitingMode.boolValue = false;
                    break;
                };
                case EnemyController.EnemyBehaviour.BackToPatrol:
                {
                    EditorGUILayout.HelpBox("If player gets out range, the enemy returns to patrol to its waypoints. " +
                        "If there are no waypoints, the enmey will return to its initial position.", MessageType.Info, true);
                    break;
                }
                case EnemyController.EnemyBehaviour.BackToIdle:
                {
                    EditorGUILayout.HelpBox("If player gets out range, the enenmy will stay in idle mode until it gets alerted again.", MessageType.Info, true);
                    enemyStarsInWaitingMode.boolValue = false;
                    break;
                }
            }
        }

        private void SectionWaypoints()
        {
            EditorGUI.BeginDisabledGroup(waypointsFollowEnemy);
            {
                WaypointsList();

                EditorGUI.BeginDisabledGroup(enemy.NoWaypoints);
                {
                    WaypointsUtils();
                    WaypointsOptions();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();

            EditorGUI.BeginDisabledGroup(enemy.NoWaypoints);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                    waypointsFollowEnemy = GUILayout.Toggle(waypointsFollowEnemy, new GUIContent("Freeze Waypoints"), "Button", GUILayout.Height(30));
                    (waypointsFollowEnemy ? new System.Action(() => enemy.ConvertWaypointsToEnemyPosition()) : () => enemy.ConvertWaypointsToWorldPosition())();
                    enemy._waypointsAreInEnemyPosition = waypointsFollowEnemy;
                    EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(10));
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(3);
            }
            EditorGUI.EndDisabledGroup();
        }

        private void WaypointsList()
        {
            Decorators.HeaderBig("List of Waypoints");
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                list.DoLayoutList();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void WaypointsUtils()
        {
            Decorators.SeparatorSimple();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                if (GUILayout.Button("Create Waypoint on Enemy Position", GUILayout.Height(24)))
                {
                    enemy.CreateWaypointOnEnemyPosition();
                }
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(10));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(enemy.NoWaypoints);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                if (GUILayout.Button("Move First Waypoint to Enemy Position", GUILayout.Height(24)))
                {
                    enemy.MoveToEnemyPosition();
                }
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(10));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                if (GUILayout.Button("Move Enemy Position to First Waypoint", GUILayout.Height(24)))
                {
                    enemy.MoveEnemyToWaypoint();
                }
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(10));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
        }

        private void WaypointsOptions()
        {
            Decorators.SeparatorSimple();
            Decorators.HeaderBig("Options");
            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(patrolPointsOrder, new GUIContent("Waypoints sorting"));
            EditorGUILayout.Space(1);
            EditorGUIUtility.labelWidth = 300;

            bool initPosition = enemy.waypoints.Count > 0 && enemy.waypoints[0].position == enemy.transform.position;
            if (initPosition)
            {
                enemySpawnsOnFirstWaypoint.boolValue = true;
            }
            EditorGUI.BeginDisabledGroup(initPosition);
            {
                EditorGUILayout.PropertyField(enemySpawnsOnFirstWaypoint, new GUIContent("Enemy starts on the first waypoint"));
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(enemy.enemyBehaviour != EnemyController.EnemyBehaviour.BackToPatrol);
            {
                EditorGUILayout.PropertyField(enemyStarsInWaitingMode, new GUIContent("Enemy starts on waiting mode until it's alerted"));
            }
            EditorGUI.EndDisabledGroup();

            ResetLabelWidth();
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            float height = EditorGUIUtility.singleLineHeight;

            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, height), new GUIContent("Position"));
            EditorGUI.PropertyField(new Rect(rect.x + 70, rect.y, rect.max.x - 130, height), element.FindPropertyRelative("position"), GUIContent.none);

            EditorGUI.LabelField(new Rect(rect.x, rect.y + height + lineSpace, 85, height), new GUIContent("Wait"));
            EditorGUI.PropertyField(new Rect(rect.x + 85, rect.y + height + lineSpace, 30, height), element.FindPropertyRelative("waitingTime"), GUIContent.none);

            EditorGUI.BeginDisabledGroup(!element.FindPropertyRelative("waitingTime").boolValue);
            {
                EditorGUI.LabelField(new Rect(rect.x + 130, rect.y + height + lineSpace, 100, height), new GUIContent("Time (range)"));

                SerializedProperty time = element.FindPropertyRelative("timeOnWaypoint");
                time.vector2Value = new(ClampValue(time.vector2Value.x), ClampValue(time.vector2Value.y));
                EditorGUI.PropertyField(new Rect(rect.x + 230, rect.y + height + lineSpace, rect.max.x - 290, height), time, GUIContent.none);
            }
            EditorGUI.EndDisabledGroup();
        }

        private float ClampValue(float value) => Mathf.Clamp(value, 0, Mathf.Infinity);

        private void AddFirstWaypointOnEnemyPosition(ReorderableList list)
        {
            int index = GetIndex(list);

            if (enemy.NoWaypoints)
            {
                var newWaypoint = list.serializedProperty.GetArrayElementAtIndex(index);
                newWaypoint.FindPropertyRelative("position").vector3Value = enemy.transform.position;
                newWaypoint.FindPropertyRelative("waitingTime").boolValue = false;
                newWaypoint.FindPropertyRelative("timeOnWaypoint").vector2Value = new Vector2(1, 2);
            }
            else
            {
                var lastWaypoint = list.serializedProperty.GetArrayElementAtIndex(index - 1);
                var newWaypoint = list.serializedProperty.GetArrayElementAtIndex(index);

                newWaypoint.FindPropertyRelative("position").vector3Value = lastWaypoint.FindPropertyRelative("position").vector3Value;
                newWaypoint.FindPropertyRelative("waitingTime").boolValue = lastWaypoint.FindPropertyRelative("waitingTime").boolValue;
                newWaypoint.FindPropertyRelative("timeOnWaypoint").vector2Value = lastWaypoint.FindPropertyRelative("timeOnWaypoint").vector2Value;
            }
        }

        private static int GetIndex(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;

            return index;
        }

        private void SectionOrientation()
        {
            EditorGUILayout.Space(2);
            EditorGUIUtility.labelWidth = 200;

            //EditorGUI.BeginDisabledGroup(enemy.ThereAreWaypoints && !enemyStarsInWaitingMode.boolValue);
            EditorGUILayout.PropertyField(waitingModeOrientation, new GUIContent("On Idle..."));
            //EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(1);

            //EditorGUI.BeginDisabledGroup(enemy.NoWaypoints);
            EditorGUILayout.PropertyField(patrolOrientation, new GUIContent("On Patrolling..."));
            //EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(1);

            EditorGUILayout.PropertyField(alertedOrientation, new GUIContent("On Alerted..."));
            EditorGUILayout.Space(1);

            Decorators.SeparatorSimple();
            EditorGUILayout.Space(3);

            EditorGUILayout.PropertyField(useSmoothRotation, new GUIContent("Use Smooth Rotation"));
            EditorGUI.BeginDisabledGroup(!useSmoothRotation.boolValue);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();

            ResetLabelWidth();
        }

        private void SectionOtherProperties()
        { 
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(addRecoil, new GUIContent("Push-back on Hit"));
        }

        private void SectionDebug()
        {
            EditorGUIUtility.labelWidth = 200;

            EditorGUILayout.PropertyField(showAlertedHandles);
            EditorGUILayout.Space(2);

            EditorGUILayout.PropertyField(showUnalertedHandles);
            EditorGUILayout.Space(2);

            EditorGUILayout.PropertyField(showGizmos);
            if (showGizmos.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(showDistance);
                EditorGUILayout.Space(1);
                EditorGUILayout.PropertyField(showPathLines);
                if (showPathLines.boolValue && enemy.NoWaypoints)
                {
                    EditorGUILayout.HelpBox("There are no waypoints to show path lines", MessageType.Info, true);
                }
                EditorGUI.BeginDisabledGroup(!showPathLines.boolValue);
                {
                    if (enemy.ThereAreWaypoints)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(pathLinesColor);
                        EditorGUILayout.PropertyField(arrowLength);
                        EditorGUILayout.PropertyField(arrowAngle);
                        EditorGUILayout.PropertyField(arrowPosition);
                        EditorGUILayout.Space(1);
                        if (enemy.patrolPointsOrder == EnemyController.PatrolPointsOrder.Random)
                        {
                            EditorGUILayout.HelpBox("Path lines in Random mode only are visibles on runtime.", MessageType.Info, true);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }

            ResetLabelWidth();
        }

        public void OnSceneGUI()
        {
            var enemy = (target as EnemyController);
            var enemyConfig = enemy.Enemy;
            var tr = enemy.transform;
            Vector3 pos = new(tr.position.x, 0.5f, tr.position.z);

            if (enemy.Enemy == null) return;

            if (enemy.showUnalertedHandles)
            {
                DrawHandles(tr, pos, out float newSight, out float newAngle, out float newAttack, 
                            enemyConfig.unalertedRangeOfSight, enemyConfig.unalertedAngleOfAction, enemyConfig.unalertedRangeOfAttack, 
                            enemyConfig.unalertedRangeOfAttackColor, enemyConfig.unalertedRangeOfSightColor, enemyConfig.unalertedAngleOfActionColor);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Change unalerted range");

                    enemyConfig.unalertedRangeOfAttack = Mathf.Clamp(newAttack, 0.1f, newSight);
                    enemyConfig.unalertedRangeOfSight = Mathf.Clamp(newSight, newAttack, Mathf.Infinity);
                    enemyConfig.unalertedAngleOfAction = newAngle;
                }
            }

            if (enemy.showAlertedHandles)
            {
                DrawHandles(tr, pos, out float newSight, out float newAngle, out float newAttack,
                            enemyConfig.alertedRangeOfSight, enemyConfig.alertedAngleOfAction, enemyConfig.alertedRangeOfAttack,
                            enemyConfig.alertedRangeOfAttackColor, enemyConfig.alertedRangeOfSightColor, enemyConfig.alertedAngleOfActionColor);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Change alerted range");

                    enemyConfig.alertedRangeOfAttack = Mathf.Clamp(newAttack, 0.1f, newSight);
                    enemyConfig.alertedRangeOfSight = Mathf.Clamp(newSight, newAttack, Mathf.Infinity);
                    enemyConfig.alertedAngleOfAction = newAngle;
                }
            }

            ShowWaypointHandles();
        }

        private void ShowWaypointHandles()
        {
            EditorGUI.BeginDisabledGroup(waypointsFollowEnemy);
            {
                float scaleFactor = 0.65f;
                Matrix4x4 startMatrix = Handles.matrix;
                Handles.matrix = Matrix4x4.Scale(Vector3.one * scaleFactor);
                var waypointOffset = waypointsFollowEnemy ? enemy.transform.position : Vector3.zero;

                if (enemy.ThereAreWaypoints)
                {
                    for (int i = 0; i < enemy.waypoints.Count; i++)
                    {
                        EditorGUI.BeginChangeCheck();

                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = waypointsFollowEnemy ? Color.black : Color.yellow;
                        style.fontSize = 16;

                        Vector3 waypoint = (enemy.waypoints[i].position + waypointOffset) / scaleFactor;
                        float distLabel = 0.3f;
                        Handles.Label(waypoint + (Vector3.forward * distLabel) + (Vector3.right * distLabel), i.ToString(), style);
                        
                        if (waypointsFollowEnemy)
                        {
                            Handles.color = new Color(0.3f, 0.3f, 0.3f);
                            Handles.ArrowHandleCap(0, waypoint, enemy.transform.rotation * Quaternion.LookRotation(Vector3.right), 1, EventType.Repaint);
                            Handles.ArrowHandleCap(0, waypoint, enemy.transform.rotation * Quaternion.LookRotation(Vector3.up), 1, EventType.Repaint);
                            Handles.ArrowHandleCap(0, waypoint, enemy.transform.rotation * Quaternion.LookRotation(Vector3.forward), 1, EventType.Repaint);
                        }
                        else
                        {
                            Vector3 newPosition = Handles.PositionHandle(waypoint, Quaternion.identity);

                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(target, "Move Waypoint enemy");
                                enemy.waypoints[i].position = (newPosition - waypointOffset) * scaleFactor;
                            }
                        }
                    }

                    Handles.matrix = startMatrix;
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawHandles(Transform tr, Vector3 pos, out float newSight, out float newAngle, out float newAttack, float sight, float angle, float attack, Color sightColor, Color angleColor, Color attackColor)
        {
            EditorGUI.BeginChangeCheck();

            Handles.color = sightColor;
            Handles.DrawSolidArc(pos, tr.up, Quaternion.AngleAxis(angle * -0.5f, Vector3.up) * tr.forward, angle, attack);

            Handles.color = new(sightColor.r, sightColor.g, sightColor.b, 1);
            newAttack = (float)Handles.ScaleValueHandle(attack, pos + tr.forward * attack, tr.rotation, 1.35f, Handles.ConeHandleCap, 1);
            newAttack = Mathf.Clamp(newAttack, 0.1f, Mathf.Infinity);

            Handles.color = angleColor;
            Handles.DrawWireArc(pos, tr.up, Quaternion.AngleAxis(angle * -0.5f, Vector3.up) * tr.forward, angle, sight, 5);

            Handles.color = new(angleColor.r, angleColor.g, angleColor.b, 1);
            newSight = (float)Handles.ScaleValueHandle(sight, pos + tr.forward * sight, tr.rotation, 1.35f, Handles.ConeHandleCap, 1);
            newSight = Mathf.Clamp(newSight, 0.1f, Mathf.Infinity);

            Handles.color = attackColor;
            var remapAngle = Functions.Remap(angle, 1, 360, 0.1f, 2);
            newAngle = (float)Handles.ScaleValueHandle(angle, pos + tr.right * remapAngle, tr.rotation, 1.35f, Handles.SphereHandleCap, 1);
            newAngle = Mathf.Clamp(newAngle, 1, 360);

            Handles.DrawLine(pos, pos + Quaternion.AngleAxis(angle * -0.5f, Vector3.up) * tr.forward * sight, 3);
            Handles.DrawLine(pos, pos + Quaternion.AngleAxis(angle * 0.5f, Vector3.up) * tr.forward * sight, 3);
        }
    }
}