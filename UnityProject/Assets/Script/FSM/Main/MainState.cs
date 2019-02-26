/**
  * 全局状态
 */
public class MainState : State<GameMain> {

    private static MainState m_instance;

    /*构造函数单例化*/
    public static MainState Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MainState();
            return m_instance;
        }
    }

    public override void Enter(GameMain pEntity) {
        //这里添加进入此状态时执行的代码
    }

    public override void Execute(GameMain pEntity) {
        //这里添加持续此状态刷新代码

    }

    public override void Exit(GameMain pEntity) {
        //这里添加离开此状态时执行代码
    }

}