using UnityEngine;

public class MainLoop : MonoBehaviour {

    // 棋子的模板
    public GameObject WhitePrefab;
    public GameObject BlackPrefab;

    // 结果窗口
    public ResultWindow ResultWindow;

    enum State
    {
        BlackGo, // 黑方(玩家)走
        WhiteGo, // 白方(电脑)走
        Over,    // 结束
    }

    // 当前状态
    State _state;

    // 棋盘显示
    BoardManager _boardManager;

    // 棋盘数据
    BoardModel _model;

    // 人工智能
    AI _ai;

    bool CanPlace( int gridX, int gridY )
    {
        // 如果这个地方可以下棋子
        return _model.Get(gridX, gridY) == ChessType.None;        
    }

    // base 1
    bool PlaceChess(CrossPoint crossPoint, bool isblack)
    {
        if (crossPoint == null)
            return false;

        // 创建棋子
        var newChess = GameObject.Instantiate<GameObject>(isblack ? BlackPrefab : WhitePrefab);
        newChess.transform.SetParent(crossPoint.gameObject.transform, false);                
        // 设置数据
        _model.Set(crossPoint.GridX, crossPoint.GridY, isblack ? ChessType.Black : ChessType.White);

        var ctype = isblack ? ChessType.Black : ChessType.White;

        var linkCount = _model.CheckLink(crossPoint.GridX, crossPoint.GridY, ctype);

        return linkCount >= BoardModel.WinChessCount;
    }

    public void Restart( )
    {
        _state = State.BlackGo;
        _model = new BoardModel();
        _ai = new AI();
        _boardManager.Reset();
    }

    public void OnClick( CrossPoint crossPoint )
    {
        if (_state != State.BlackGo)
            return;

        // 不能在已经放置过的棋子上放置
        if (CanPlace(crossPoint.GridX, crossPoint.GridY))
        {
            _lastPlayerX = crossPoint.GridX;
            _lastPlayerY = crossPoint.GridY;

            if (PlaceChess(crossPoint , true))
            {
                // 已经胜利
                _state = State.Over;
                ShowResult(ChessType.Black);
            }
            else
            {
                // 换电脑走
                _state = State.WhiteGo;
            }
        }
    }

    void Start( )
    {
        _boardManager = GetComponent<BoardManager>();

        Restart();
    }

    int _lastPlayerX, _lastPlayerY;

    void ShowResult( ChessType winside )
    {
        ResultWindow.gameObject.SetActive(true);
        ResultWindow.Show(winside);
    }

	// Update is called once per frame
	void Update () {

        switch( _state )
        {
            // 白方(电脑)走
            case State.WhiteGo:
                {
                    // 计算电脑下的位置
                    int gridX, gridY;
                    _ai.ComputerDo(_lastPlayerX, _lastPlayerY, out gridX, out gridY);

                    if (PlaceChess(_boardManager.GetCross(gridX, gridY), false))
                    {
                        // 电脑胜利
                        _state = State.Over;
                        ShowResult(ChessType.White);
                    }
                    else
                    {
                        // 换玩家走
                        _state = State.BlackGo;
                    }

                }
                break;
        }

	}
}
