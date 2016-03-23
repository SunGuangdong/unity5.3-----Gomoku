using UnityEngine;
using UnityEngine.UI;

// 每个交叉点逻辑
public class CrossPoint : MonoBehaviour {

    // 位置
    public int GridX;
    public int GridY;

    public MainLoop mainLoop;

	void Start () {
        GetComponent<Button>().onClick.AddListener( ( )=>{
            mainLoop.OnClick(this);
        });
	}
	

}
