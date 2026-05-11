using UnityEngine;

public class EditorState
{

    public State CurrentState { get; private set; } = State.Idle;
    protected LevelEditor editor;
    public EditorState(LevelEditor editor)
    {
        this.editor = editor;
    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void OnTrigger(Vector3Int gridPos) { }
}
public class PlacingTileState : EditorState
{
    public PlacingTileState(LevelEditor editor) : base(editor) { }
    public override void EnterState()
    {
        
    }
    public override void ExitState()
    {
        
    }
    public override void OnTrigger(Vector3Int gridPos)
    {
        var entityData = new LevelEntityData
        {
            Id = -1,
            gridSize = new Vector2Int(1,1),
            gridPosition = new Vector2Int(gridPos.x,gridPos.y)
        };
        //editor.PlaceEntity(entityData,gridPos);
    }
}
public enum State
{
    Idle,
    PlacingTile,
    PlacingEntity,
    Removing,
}
