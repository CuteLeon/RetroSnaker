Public Class GameForm
    Dim PaddingSize As Size = New Size(10, 10)
    Dim TabelSize As Size = New Size(24, 18)
    Dim CellSize As Size = New Size(20, 20)
    Dim UnitySpeed As Double
    Dim SnakeBody As List(Of Point)
    Dim SnakeHead As Point
    Dim SnakeTail As Point
    Dim Wall As List(Of Point)
    Dim AwardPoint As Point
    Dim MoveOnHorizontal As SByte = 1
    Dim MoveOnVertical As SByte = 0

    Private Sub GameForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializaGame()
    End Sub

    Private Sub InitializaGame()
        Dim Index As Integer
        SnakeBody = New List(Of Point)
        Wall = New List(Of Point)
        AwardPoint = New Point((TabelSize.Width - 1) * VBMath.Rnd, (TabelSize.Height - 1) * VBMath.Rnd)
        MoveOnHorizontal = 1
        MoveOnVertical = 0
        For Index = 4 To 1 Step -1
            SnakeBody.Add(New Point(Index, 1))
        Next
        SnakeHead = New Point(5, 1)
        For Index = 0 To TabelSize.Width - 1
            Wall.Add(New Point(Index, 0))
            Wall.Add(New Point(Index, TabelSize.Height - 1))
        Next
        For Index = 1 To TabelSize.Height - 2
            Wall.Add(New Point(0, Index))
            Wall.Add(New Point(TabelSize.Width - 1, Index))
        Next
        Wall.Add(New Point(5, 5))
        Wall.Add(New Point(5, 6))
        Wall.Add(New Point(5, 7))
        Wall.Add(New Point(15, 12))
        Wall.Add(New Point(16, 12))
        Wall.Add(New Point(17, 12))
        UpdateUI()
        UnityEngine.Start()
    End Sub

    Private Sub UpdateUI()
        Dim UnityBitmap As Bitmap = New Bitmap(My.Resources.UnityResource.GameBGI, Me.Width, Me.Height)
        Dim UnityGrapics As Graphics = Graphics.FromImage(UnityBitmap)
        For Index = 0 To SnakeBody.Count - 1
            UnityGrapics.DrawImage(My.Resources.UnityResource.SnakeBody, New Rectangle(PaddingSize.Width + SnakeBody(Index).X * CellSize.Width, PaddingSize.Height + SnakeBody(Index).Y * CellSize.Height, CellSize.Width, CellSize.Height))
        Next
        UnityGrapics.DrawImage(My.Resources.UnityResource.ResourceManager.GetObject(("SnakeHead" & MoveOnHorizontal & MoveOnVertical).Replace("-", "_")),
                               New Rectangle(PaddingSize.Width + SnakeHead.X * CellSize.Width, PaddingSize.Height + SnakeHead.Y * CellSize.Height, CellSize.Width, CellSize.Height))
        For Index = 0 To Wall.Count - 1
            UnityGrapics.DrawImage(My.Resources.UnityResource.Wall, New Rectangle(PaddingSize.Width + Wall(Index).X * CellSize.Width, PaddingSize.Height + Wall(Index).Y * CellSize.Height, CellSize.Width, CellSize.Height))
        Next
        UnityGrapics.DrawImage(My.Resources.UnityResource.Award, New Rectangle(PaddingSize.Width + AwardPoint.X * CellSize.Width, PaddingSize.Height + AwardPoint.Y * CellSize.Height, CellSize.Width, CellSize.Height))
        Me.BackgroundImage = UnityBitmap
        GC.Collect()
    End Sub

    Private Sub ExecuteGame()
        Dim Index As Integer
        SnakeTail = SnakeBody(SnakeBody.Count - 1)
        For Index = SnakeBody.Count - 1 To 1 Step -1
            SnakeBody(Index) = SnakeBody(Index - 1)
        Next
        SnakeBody(0) = SnakeHead
        SnakeHead = GetNextPoint(SnakeHead)
        If SnakeHead.Equals(AwardPoint) Then
            SnakeBody.Add(SnakeTail)
            Do
                AwardPoint = New Point((TabelSize.Width - 1) * VBMath.Rnd, (TabelSize.Height - 1) * VBMath.Rnd)
            Loop Until (Wall.IndexOf(AwardPoint) = -1 And SnakeBody.IndexOf(AwardPoint) = -1)
        End If
        UpdateUI()
        If Wall.IndexOf(SnakeHead) > -1 Then
            UnityEngine.Stop()
            MsgBox("撞到墙咯！")
            InitializaGame()
        End If
        If SnakeBody.IndexOf(SnakeHead) > -1 Then
            UnityEngine.Stop()
            MsgBox("咬到自己咯！")
            InitializaGame()
        End If
    End Sub

    Private Function GetNextPoint(IniPoint As Point) As Point
        Return New Point(IniPoint.X + MoveOnHorizontal, IniPoint.Y + MoveOnVertical)
    End Function

    Private Sub UnityEngine_Tick(sender As Object, e As EventArgs) Handles UnityEngine.Tick
        ExecuteGame()
    End Sub

    Private Sub GameForm_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        UnityEngine.Stop()
    End Sub

    Private Sub GameForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Not UnityEngine.Enabled Then UnityEngine.Start()
    End Sub

    Private Sub GameForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
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
