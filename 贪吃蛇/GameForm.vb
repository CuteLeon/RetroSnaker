Public Class GameForm
    ''' <summary>
    ''' 蛇的初始长度
    ''' </summary>
    Private Const DefaultSnakeLength As Integer = 15
    ''' <summary>
    ''' 游戏区域与窗口外边框间距
    ''' </summary>
    Dim PaddingSize As Size = New Size(10, 10)
    ''' <summary>
    ''' 单元格行列数（非像素）
    ''' </summary>
    Dim TabelSize As Size = New Size(24, 18)
    ''' <summary>
    ''' 每个单元格尺寸（像素）
    ''' </summary>
    Dim CellSize As Size = New Size(20, 20)
    ''' <summary>
    ''' 蛇身体范式列表（不包括蛇头）
    ''' </summary>
    Dim SnakeBody As List(Of Point)
    ''' <summary>
    ''' 蛇头（为了方便判断是否咬到自己，单独作为一个变量）
    ''' </summary>
    Dim SnakeHead As Point
    ''' <summary>
    ''' 蛇尾（吃到奖励道具时在上次蛇尾的位置增加新的身体）
    ''' </summary>
    Dim SnakeTail As Point
    ''' <summary>
    ''' 墙的范式列表
    ''' </summary>
    Dim Wall As List(Of Point)
    ''' <summary>
    ''' 奖励道具
    ''' </summary>
    Dim AwardPoint As Point
    ''' <summary>
    ''' 水平移动距离（为-1时向左移动，为0时水平方向不移动，为1时向右移动）
    ''' </summary>
    Dim MoveOnHorizontal As SByte = 1
    ''' <summary>
    ''' 垂直移动距离（类似MoveOnHorizontal，两个变量必须有一个为0）
    ''' </summary>
    Dim MoveOnVertical As SByte = 0

    Private Sub GameForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializaGame()
    End Sub

    ''' <summary>
    ''' 初始化游戏
    ''' </summary>
    Private Sub InitializaGame()
        Dim Index As Integer
        '初始化蛇的身体
        SnakeBody = New List(Of Point)
        For Index = DefaultSnakeLength - 1 To 1 Step -1
            SnakeBody.Add(New Point(Index, 1))
        Next
        SnakeHead = New Point(DefaultSnakeLength, 1)
        '初始化墙
        Wall = New List(Of Point)
        For Index = 0 To 5
            Wall.Add(New Point(Index + 4, 0))
            Wall.Add(New Point(Index + 4, TabelSize.Height - 1))
            Wall.Add(New Point(Index + 14, 0))
            Wall.Add(New Point(Index + 14, TabelSize.Height - 1))
            Wall.Add(New Point(0, Index + 5))
            Wall.Add(New Point(TabelSize.Width - 1, Index + 5))
            Wall.Add(New Point(Index + 9, 9))
        Next
        '初始化奖励道具
        CreateNewAward()
        '初始化游戏
        MoveOnHorizontal = 1
        MoveOnVertical = 0
        UpdateUI()
        '开始游戏
        UnityEngine.Start()
    End Sub

    ''' <summary>
    ''' 刷新界面
    ''' </summary>
    Private Sub UpdateUI()
        Dim UnityBitmap As Bitmap = New Bitmap(My.Resources.UnityResource.GameBGI, Me.Width, Me.Height)
        Dim UnityGrapics As Graphics = Graphics.FromImage(UnityBitmap)
        '绘制蛇
        For Index = 0 To SnakeBody.Count - 1
            UnityGrapics.DrawImage(My.Resources.UnityResource.SnakeBody, New Rectangle(PaddingSize.Width + SnakeBody(Index).X * CellSize.Width, PaddingSize.Height + SnakeBody(Index).Y * CellSize.Height, CellSize.Width, CellSize.Height))
        Next
        '绘制蛇头（根据 MoveOnHorizontal和MoveOnVertical 判断蛇头方向）
        UnityGrapics.DrawImage(My.Resources.UnityResource.ResourceManager.GetObject(("SnakeHead" & MoveOnHorizontal & MoveOnVertical).Replace("-", "_")),
                               New Rectangle(PaddingSize.Width + SnakeHead.X * CellSize.Width, PaddingSize.Height + SnakeHead.Y * CellSize.Height, CellSize.Width, CellSize.Height))
        '绘制墙（如果游戏过程中墙是不变动，可以把墙单独做成一张透明度，直接绘制到UnityBitmap，不需要每次遍历墙的范式列表）
        For Index = 0 To Wall.Count - 1
            UnityGrapics.DrawImage(My.Resources.UnityResource.Wall, New Rectangle(PaddingSize.Width + Wall(Index).X * CellSize.Width, PaddingSize.Height + Wall(Index).Y * CellSize.Height, CellSize.Width, CellSize.Height))
        Next
        '绘制奖励道具
        UnityGrapics.DrawImage(My.Resources.UnityResource.Award, New Rectangle(PaddingSize.Width + AwardPoint.X * CellSize.Width, PaddingSize.Height + AwardPoint.Y * CellSize.Height, CellSize.Width, CellSize.Height))

        Me.BackgroundImage = UnityBitmap
        GC.Collect()
    End Sub

    ''' <summary>
    ''' 执行一次游戏动作 并刷新界面
    ''' </summary>
    Private Sub ExecuteGame()
        Dim Index As Integer
        '记录蛇尾位置，吃到奖励道具时，用于增加新的身体
        SnakeTail = SnakeBody(SnakeBody.Count - 1)
        '身体每个部分向列表前方移动一格（从后向前递进）
        For Index = SnakeBody.Count - 1 To 1 Step -1
            SnakeBody(Index) = SnakeBody(Index - 1)
        Next
        '更新蛇头位置
        SnakeBody(0) = SnakeHead
        SnakeHead = GetNextPoint(SnakeHead)
        '判断是否世道奖励道具
        If SnakeHead.Equals(AwardPoint) Then
            '在蛇尾增加身体（我说过的，就是在这）
            SnakeBody.Add(SnakeTail)
            '生成新的奖励道具
            CreateNewAward()
        End If
        '刷新界面
        UpdateUI()
        '判断是否撞墙
        If Wall.IndexOf(SnakeHead) > -1 Then
            UnityEngine.Stop()
            MsgBox("撞到墙咯！")
            InitializaGame()
        End If
        '判断是否咬到自己
        If SnakeBody.IndexOf(SnakeHead) > -1 Then
            UnityEngine.Stop()
            MsgBox("咬到自己咯！")
            InitializaGame()
        End If
    End Sub

    ''' <summary>
    ''' 生成新的奖励道具
    ''' </summary>
    Private Sub CreateNewAward()
        'Do...Loop 循环防止奖励道具出现在墙和蛇身体上
        Do
            '生成新的奖励道具位置
            AwardPoint = New Point((TabelSize.Width - 1) * VBMath.Rnd, (TabelSize.Height - 1) * VBMath.Rnd)
        Loop Until (Wall.IndexOf(AwardPoint) = -1 And SnakeBody.IndexOf(AwardPoint) = -1)
    End Sub

    ''' <summary>
    ''' 计算蛇头的下一个坐标
    ''' </summary>
    ''' <param name="IniPoint">蛇头当前坐标</param>
    ''' <returns>蛇头下一次坐标</returns>
    Private Function GetNextPoint(IniPoint As Point) As Point
        '(* + TabelSize.Width) Mod TabelSize.Width 可以使蛇穿过游戏区域边缘时在另一边缘出现
        Return New Point((IniPoint.X + MoveOnHorizontal + TabelSize.Width) Mod TabelSize.Width, (IniPoint.Y + MoveOnVertical + TabelSize.Height) Mod TabelSize.Height)
    End Function

    Private Sub UnityEngine_Tick(sender As Object, e As EventArgs) Handles UnityEngine.Tick
        ExecuteGame()
    End Sub

    Private Sub GameForm_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        '窗体失去焦点时暂停游戏
        UnityEngine.Stop()
    End Sub

    Private Sub GameForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        '窗体获得焦点时开始游戏
        If Not UnityEngine.Enabled Then UnityEngine.Start()
    End Sub

    Private Sub GameForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        '按下方向键控制蛇头移动方向
        'If ... Then Exit Sub 语句防止蛇头向反方向运动
        Select Case e.KeyCode
            Case Keys.Up
                If (SnakeBody(0).Y = SnakeHead.Y - 1) Then Exit Sub
                MoveOnVertical = -1
                MoveOnHorizontal = 0
            Case Keys.Down
                If (SnakeBody(0).Y = SnakeHead.Y + 1) Then Exit Sub
                MoveOnVertical = 1
                MoveOnHorizontal = 0
            Case Keys.Left
                If (SnakeBody(0).X = SnakeHead.X - 1) Then Exit Sub
                MoveOnHorizontal = -1
                MoveOnVertical = 0
            Case Keys.Right
                If (SnakeBody(0).X = SnakeHead.X + 1) Then Exit Sub
                MoveOnHorizontal = 1
                MoveOnVertical = 0
        End Select
    End Sub
End Class
