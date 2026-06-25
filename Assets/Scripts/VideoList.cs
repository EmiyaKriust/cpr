using System.Collections.Generic;

public class VideoList
{
    public int total;//视频数量
    public List<VideoInfo> rows;//视频列表
}
public class VideoInfo
{
    public string videoUrl;//视频地址
    public string videoTitle;//视频名称
    public string videoCover;//视频封面地址
    public string videoTimes;//视频时长
    public string id;
    public string type;//视频分类
}