Imports System.Threading
Imports System.IO
Imports System.Runtime.Serialization.Json
Imports System.Globalization
Imports Microsoft.Phone.Controls.Maps
Imports System.Device.Location
Imports System.Windows.Media.Imaging


Partial Public Class Itinerarios
    Inherits PhoneApplicationPage

    Public Property MapElements As ItinerariosViewModel
    Public Shared Property WebError As Boolean

    Dim GeoLoc As GeoCoordinateWatcher
    Dim vTimerOut As Timer = Nothing

    Public Sub New()
        InitializeComponent()
        MapElements = New ItinerariosViewModel(App.ActiveLinha.Cod)

        PageTitle.Text = App.ActiveLinha.Cod & " - " & App.ActiveLinha.Nome

        If ApplicationBar.Buttons.Count = 1 Then
            Dim Btn As ApplicationBarIconButton
            If MainPage.Favoritos.Contains(App.ActiveLinha) Then
                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/delete.png", UriKind.Relative)) With {.Text = "remover"}
                AddHandler Btn.Click, AddressOf AppBar_Del
                ApplicationBar.Buttons.Add(Btn)
            Else
                Btn = New ApplicationBarIconButton(New Uri("Assets/AppBar/add.png", UriKind.Relative)) With {.Text = "adicionar"}
                AddHandler Btn.Click, AddressOf AppBar_Add
                ApplicationBar.Buttons.Add(Btn)
            End If
            Btn = Nothing
        End If

        If MapElements.Trajeto.Locations.Count Then
            Mapa.Children(1) = MapElements.Trajeto
            Mapa.Children(2) = MapElements.Pontos
            Mapa.SetView(LocationRect.CreateLocationRect(MapElements.Trajeto.Locations))
        End If

        Mapa.Children(3) = MapElements.Veiculos

    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        GeoLoc = New GeoCoordinateWatcher(GeoPositionAccuracy.High)
        AddHandler GeoLoc.PositionChanged, Sub(s, arg)
                                               If Not arg.Position.Location.IsUnknown Then
                                                   Ellipse.SetValue(MapLayer.PositionProperty, arg.Position.Location)
                                                   Ellipse.Visibility = Windows.Visibility.Visible
                                               End If
                                           End Sub
        GeoLoc.Start()

        vTimerOut = New Timer(New TimerCallback( _
                              Sub()
                                  Dispatcher.BeginInvoke(Sub() If WebError Then App.ProgressMsg(Progress, "Ocorreu um erro!", 2))
                                  MapElements.Download()
                              End Sub), Nothing, 0, 5000)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        vTimerOut.Dispose()
        GeoLoc.Stop()
    End Sub

    Private Sub Mapa_Loaded(sender As Object, e As RoutedEventArgs) Handles Mapa.Loaded
        Progress.IsVisible = False
    End Sub

    Private Sub AppBar_Horario(sender As Object, e As EventArgs)
        NavigationService.Navigate(New Uri("/Horarios.xaml", UriKind.Relative))
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

Public Class ItinerariosViewModel

    Public Property Trajeto As MapPolyline
    Public Property Pontos As MapLayer
    Public Property Veiculos As New MapLayer

    Private TypeColor As SolidColorBrush
    Private Provider As New NumberFormatInfo With {.NumberDecimalSeparator = ".", .NumberGroupSeparator = ","}

    Public Sub New(COD As String)

        'App.LinhaVM = New LinhaViewModel(App.ActiveLinha.Cod)
        'If Not App.LinhaVM.LoadedCode.Equals(App.ActiveLinha.Cod) Then App.LinhaVM.LoadFromDatabase()

        TypeColor = (New TypeColor).Convert(App.ActiveLinha.Tipo, GetType(SolidColorBrush), Nothing, CultureInfo.CurrentUICulture)
        If TypeColor.Color = Colors.White Then TypeColor = New SolidColorBrush(Colors.Black)

        Dim GeoColect As New LocationCollection

        'For Each t In App.LinhaVM.Trajeto
        '    GeoColect.Add(New GeoCoordinate(t.Lat, t.Lon))
        'Next
        Trajeto = New MapPolyline With {.Locations = GeoColect, .StrokeThickness = 3, .Stroke = TypeColor}

        Pontos = New MapLayer
        'For Each p In App.LinhaVM.Pontos

        '    Dim Image As New Image With {.Source = New BitmapImage(New Uri("Assets/stop.png", UriKind.Relative)), .Height = 20, .Width = 20}
        '    Dim PtGrid As New Grid With {.Height = 20, .Width = 20, .Background = TypeColor}

        '    PtGrid.Children.Add(Image)

        '    AddHandler PtGrid.Tap, Sub()
        '                               Dim Push As New Pushpin With {.Content = p.Name, .Opacity = 0.8}
        '                               AddHandler Push.Tap, Sub() Pontos.Children.Remove(Push)
        '                               Pontos.AddChild(Push, New GeoCoordinate(p.Lat, p.Lon))
        '                           End Sub

        '    Pontos.AddChild(PtGrid, New GeoCoordinate(p.Lat, p.Lon), New PositionOrigin(0.5, 0.5))

        'Next

    End Sub

    Public Sub GetVeiculos(sender As Object, e As DownloadStringCompletedEventArgs)

        Deployment.Current.Dispatcher.BeginInvoke( _
            Sub()

                If e.Error Is Nothing Then

                    Dim Result As String = e.Result.Replace("""-25,", """-25.").Replace("""-49,", """-49.").Replace("""ADAPT"":""SIM""", """ADAPT"":true").Replace("""ADAPT"":null", """ADAPT"":false")
                    Dim Serializer As New DataContractJsonSerializer(GetType(List(Of Veiculo)))

                    Veiculos.Children.Clear()

                    For Each v In DirectCast(Serializer.ReadObject(New MemoryStream(System.Text.Encoding.UTF8.GetBytes(Result))), List(Of Veiculo))

                        Dim Image As New Image With {.Source = New BitmapImage(New Uri("Assets/bus.png", UriKind.Relative)), .Height = 25, .Width = 25}
                        Dim vGrid As New Grid With {.Height = 25, .Width = 25, .Background = New SolidColorBrush(Colors.Black), .Opacity = 0.8}

                        vGrid.Children.Add(Image)

                        AddHandler vGrid.Tap, Sub()
                                                  Dim Push As New Pushpin With {.Content = v.PREFIXO & " às " & v.HORA, .Opacity = 0.8}
                                                  AddHandler Push.Tap, Sub() Veiculos.Children.Remove(Push)
                                                  Veiculos.AddChild(Push, New GeoCoordinate(Double.Parse(v.LAT, Provider), Double.Parse(v.LON, Provider)))
                                              End Sub

                        Veiculos.AddChild(vGrid, New GeoCoordinate(Double.Parse(v.LAT, Provider), Double.Parse(v.LON, Provider)), New PositionOrigin(0.5, 0.5))

                    Next
                    Itinerarios.WebError = False
                Else
                    Itinerarios.WebError = True
                End If

            End Sub)

    End Sub

    Public Sub Download()
        Dim Web As New WebClient
        AddHandler Web.DownloadStringCompleted, AddressOf GetVeiculos
        Web.DownloadStringAsync(New Uri("http://transporteservico.urbs.curitiba.pr.gov.br/getVeiculosLinha.php?c=1c406&linha=" & App.ActiveLinha.Cod & "&t=" & DateTime.Now.Ticks))
    End Sub

End Class