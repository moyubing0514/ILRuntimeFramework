
/**
  * 状态基类 
 */
public class State<T> {
    public T Target;
    //Enter state  
    public virtual void Enter(T entityType) {
        Target = entityType;
    }
    //Execute state
    public virtual void Execute(T entityType) {

    }
    //Exit state
    public virtual void Exit(T entityType) {
        Target = default(T);
    }
}