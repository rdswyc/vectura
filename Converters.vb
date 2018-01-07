Imports System.Windows.Data
Imports System.Collections
Imports System.Globalization

Public Class JumpList
    Implements IValueConverter

    Public Function Convert(ByVal Value As Object, ByVal TargetType As Type, ByVal Parameter As Object, ByVal Culture As CultureInfo) As Object _
        Implements IValueConverter.Convert

        Dim Group As IList = TryCast(Value, IList)

        If Group IsNot Nothing Then

            Select Case Parameter.ToString
                Case "Background"
                    Return DirectCast(Application.Current.Resources(If(Group.Count, "PhoneAccentBrush", "PhoneChromeBrush")), SolidColorBrush)
                Case "Foreground"
                    Return DirectCast(Application.Current.Resources(If(Group.Count, "PhoneForegroundBrush", "PhoneDisabledBrush")), SolidColorBrush)
                Case "TypeListB"
                    Return (New TypeColor).Convert(TryCast(Group(0), Linha).TIPO, GetType(SolidColorBrush), Nothing, CultureInfo.CurrentUICulture)
                Case "TypeListF"
                    Return (New TypeColor).Convert(TryCast(Group(0), Linha).TIPO, GetType(SolidColorBrush), "Foreground", CultureInfo.CurrentUICulture)
                Case Else
                    Return Nothing
            End Select

        Else

            Return Nothing

        End If

    End Function

    Public Function ConvertBack(ByVal Value As Object, ByVal TargetType As Type, ByVal Parameter As Object, ByVal Culture As CultureInfo) As Object _
        Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class TypeColor
    Implements IValueConverter

    Public Function Convert(ByVal Value As Object, ByVal TargetType As Type, ByVal Parameter As Object, ByVal Culture As CultureInfo) As Object _
        Implements IValueConverter.Convert

        Dim TempColor As Color

        If Parameter Is Nothing Then

            Select Case Value.ToString
                Case "A"
                    TempColor = App.Current.Resources("Orange")
                Case "B"
                    TempColor = App.Current.Resources("Emerald")
                Case "C"
                    TempColor = App.Current.Resources("Yellow")
                Case "D"
                    TempColor = App.Current.Resources("Red")
                Case "E"
                    TempColor = App.Current.Resources("Cobalt")
                Case "L"
                    TempColor = App.Current.Resources("Steel")
                Case "M"
                    TempColor = App.Current.Resources("Brown")
                Case "S"
                    TempColor = App.Current.Resources("Cyan")
                Case "H", "I", "T"
                    TempColor = Colors.White
                Case Else
                    TempColor = Color.FromArgb(255, 102, 102, 102)
            End Select

        ElseIf Parameter.ToString = "Foreground" Then

            Select Case Value.ToString
                Case "H", "I", "T", "0"
                    TempColor = Colors.Black
                Case Else
                    TempColor = Colors.White
            End Select

        End If

        If TargetType.Equals(GetType(Color)) Then
            Return TempColor
        Else
            Return New SolidColorBrush(TempColor)
        End If

    End Function

    Public Function ConvertBack(ByVal Value As Object, ByVal TargetType As Type, ByVal Parameter As Object, ByVal Culture As CultureInfo) As Object _
        Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class TimeBrush
    Implements IValueConverter

    Public Function Convert(ByVal Value As Object, ByVal TargetType As Type, ByVal Parameter As Object, ByVal Culture As System.Globalization.CultureInfo) As Object _
        Implements IValueConverter.Convert

        Value = Value.ToString.Replace("24:", "00:")

        If DateTime.Parse(Value) <= DateTime.Now.AddMinutes(-30) Then
            Value = DateTime.Parse(Value).AddDays(1)
        Else
            Value = DateTime.Parse(Value)
        End If

        Select Case CType(Value, DateTime)
            Case DateTime.Now.AddMinutes(-30) To DateTime.Now.AddMinutes(30)
                Return Application.Current.Resources("PhoneAccentBrush")
            Case Else
                Return Application.Current.Resources("PhoneForegroundBrush")
        End Select

    End Function

    Public Function ConvertBack(ByVal Value As Object, ByVal TargetType As Type, ByVal Parameter As Object, ByVal Culture As System.Globalization.CultureInfo) As Object _
        Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class