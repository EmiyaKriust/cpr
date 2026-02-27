using UnityEngine;
using System.Collections;


//MARK::用户协议
/// <summary>
/// 每个游戏Model都要集成此接口
/// </summary>
public class UserItemDelegate
{
    //用户进入
    public virtual void OnUserEnter(TCP_Buffer userinfo) { }
    //用户状态
    public virtual void OnUserStatus(TCP_Buffer tcpbuffer) { }
    //用户分数
    public virtual void OnUserScore(TCP_Buffer tcpbuffer) { }
    //用户聊天
    public virtual void OnUserChat(TCP_Buffer tcpbuffer) { }

    //进入游戏场景时的数据（空闲状态或者游戏状态数据结构不一样）
    public virtual void OnGameScene(TCP_Buffer tcpbuffer) { }

    public virtual void OnGameMessage(TCP_Buffer tcpbuffer) { }

    public virtual int GetTableID() { return 0; }

    public virtual void OnGameStatus(TCP_Buffer tcpbuffer) { }

    public virtual void OnUpdateUpdateHost(TCP_Buffer tcpbuffer) { }

};