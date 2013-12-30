Partial Public Class PivotItemControl
    Inherits PivotItem

    Public Sub New()
        InitializeComponent()
        CType(Resources("AccentB"), SolidColorBrush).Color = (New TypeColor).Convert(App.ActiveLinha.Tipo, GetType(Color), Nothing, System.Globalization.CultureInfo.CurrentUICulture)
        CType(Resources("AccentF"), SolidColorBrush).Color = (New TypeColor).Convert(App.ActiveLinha.Tipo, GetType(Color), "Foreground", System.Globalization.CultureInfo.CurrentUICulture)
    End Sub

End Class