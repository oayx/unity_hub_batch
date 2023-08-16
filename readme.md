
## 主要功能


主要用于游戏中的HUD合批，实现了一个批次渲染所有HUD


## 效果演示
<br><img src='image/1.png'><br>


## 优化方向
1.使用GPUInstance批量绘制

2.目前血量修改会提交Mesh，有一定性能消耗。尝试用Shader做进度，但是在Build-in会出现材质复杂。后面尝试URP

## 其他注意

1.需要开启动态合批
