using System.Collections.Generic;
using UnityEngine;

public class EditorStateMachine
{
    private EditorState currentState;
    private LevelEditor editor;
    private Dictionary<State, EditorState> stateDict = new Dictionary<State, EditorState>();
    public EditorStateMachine(LevelEditor editor)
    {
        this.editor = editor;
        
        stateDict[State.Idle] = new EditorState(editor);
        stateDict[State.PlacingTile] = new PlacingTileState(editor);
        currentState = stateDict[State.PlacingTile];
        // Initialize other states as needed
    }
    public void ChangeState(EditorState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
    public void OnTrigger(Vector3Int gridPos)
    {
        currentState.OnTrigger(gridPos);
    }
}