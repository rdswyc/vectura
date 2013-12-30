Imports System.Xml.Linq


Partial Public Class Horarios
    Inherits PhoneApplicationPage

    Public Sub New()
        InitializeComponent()

        MainPivot.Title = App.ActiveLinha.COD & " - " & App.ActiveLinha.NOME

        If ApplicationBar.Buttons.Count = 1 Then

            Dim Btn As ApplicationBarIconButton

            If MainPage.Favoritos.Contains(App.ActiveLinha) Then
                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/delete.png", UriKind.Relative)) With {.Text = "remover"}
                AddHandler Btn.Click, AddressOf AppBar_Del
                ApplicationBar.Buttons.Insert(1, Btn)
            Else
                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/add.png", UriKind.Relative)) With {.Text = "adicionar"}
                AddHandler Btn.Click, AddressOf AppBar_Add
                ApplicationBar.Buttons.Insert(1, Btn)
            End If

            Btn = Nothing

        End If

        Select Case Date.Today.DayOfWeek
            Case DayOfWeek.Saturday : MainPivot.SelectedIndex = 1
            Case DayOfWeek.Sunday : MainPivot.SelectedIndex = 2
        End Select

    End Sub

    Private Sub MainPivot_LoadingPivotItem(sender As Object, e As PivotItemEventArgs) Handles MainPivot.LoadingPivotItem

        If e.Item.Tag Is Nothing Then

            Dispatcher.BeginInvoke( _
                Sub()
                    Progress.Text = "Carregando..."
                    Progress.IsVisible = True

                    Dim ViewModel As IEnumerable(Of KeyedList(Of String, Horario)) = _
                        From x In XElement.Load("Data/H/" & App.ActiveLinha.COD & ".xml").<d>.<h>
                        Where x.Parent.@i = MainPivot.SelectedIndex + 1
                        Let h = New Horario With {.DIA = MainPivot.SelectedIndex + 1, .HORA = x.<h>.Value, .PONTO = x.<p>.Value}
                        Group h By Ponto = h.PONTO Into Horas = Group
                        Select New KeyedList(Of String, Horario)(Ponto, Horas)

                    If ViewModel.Count = 0 Then
                        e.Item.Content = New TextBlock With {.Style = Resources("EmptyMsg")}
                    Else
                        e.Item.DataContext = ViewModel
                    End If

                End Sub)

            e.Item.Tag = 1

        End If

    End Sub

    Private Sub MainPivot_LoadedPivotItem(sender As Object, e As PivotItemEventArgs) Handles MainPivot.LoadedPivotItem
        Dispatcher.BeginInvoke(Sub() Progress.IsVisible = False)
    End Sub

    Private Sub AppBar_Itinerario(sender As Object, e As EventArgs)
        NavigationService.Navigate(New Uri("/Itinerarios.xaml", UriKind.Relative))
    End Sub

    Private Sub AppBar_Add(sender As Object, e As EventArgs)
        App.ProgressMsg(Progress, MainPage.Favoritos.AddFav(App.ActiveLinha), 2)
        ApplicationBar.Buttons.RemoveAt(1)
        Dim Btn As New ApplicationBarIconButton(New Uri("Assets/AppBar/delete.png", UriKind.Relative)) With {.Text = "remover"}
        AddHandler Btn.Click, AddressOf AppBar_Del
        ApplicationBar.Buttons.Add(Btn)
        Btn = Nothing
    End Sub

    Private Sub AppBar_Del(sender As Object, e As EventArgs)
        App.ProgressMsg(Progress, MainPage.Favoritos.DelFav(App.ActiveLinha), 2)
        ApplicationBar.Buttons.RemoveAt(1)
        Dim Btn As New ApplicationBarIconButton(New Uri("Assets/AppBar/add.png", UriKind.Relative)) With {.Text = "adicionar"}
        AddHandler Btn.Click, AddressOf AppBar_Add
        ApplicationBar.Buttons.Add(Btn)
        Btn = Nothing
    End Sub

End Class