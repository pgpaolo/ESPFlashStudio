Namespace ESPFlashStudio.Models
    Public Class BoardProfile
        Public Property DisplayName As String = ""
        Public Property Family As String = "ESP32"
        Public Property PlatformIoBoardId As String = ""
        Public Property ArduinoFqbn As String = ""
        Public Property DefaultBaud As Integer = 460800

        Public Overrides Function ToString() As String
            Return DisplayName
        End Function

        Public Shared Function Defaults() As List(Of BoardProfile)
            Return New List(Of BoardProfile) From {
                New BoardProfile With {.DisplayName = "LOLIN D32 (ESP32)", .Family = "ESP32", .PlatformIoBoardId = "lolin_d32", .ArduinoFqbn = "esp32:esp32:lolin_d32", .DefaultBaud = 921600},
                New BoardProfile With {.DisplayName = "LOLIN D32 PRO (ESP32)", .Family = "ESP32", .PlatformIoBoardId = "lolin_d32_pro", .ArduinoFqbn = "esp32:esp32:lolin_d32_pro", .DefaultBaud = 921600},
                New BoardProfile With {.DisplayName = "LOLIN C3 Mini (ESP32-C3)", .Family = "ESP32", .PlatformIoBoardId = "lolin_c3_mini", .ArduinoFqbn = "esp32:esp32:lolin_c3_mini", .DefaultBaud = 460800},
                New BoardProfile With {.DisplayName = "LOLIN S2 Mini (ESP32-S2)", .Family = "ESP32", .PlatformIoBoardId = "lolin_s2_mini", .ArduinoFqbn = "esp32:esp32:lolin_s2_mini", .DefaultBaud = 460800},
                New BoardProfile With {.DisplayName = "LOLIN32 / WEMOS LOLIN32 (ESP32)", .Family = "ESP32", .PlatformIoBoardId = "lolin32", .ArduinoFqbn = "esp32:esp32:lolin32", .DefaultBaud = 921600},
                New BoardProfile With {.DisplayName = "ESP32 Dev Module", .Family = "ESP32", .PlatformIoBoardId = "esp32dev", .ArduinoFqbn = "esp32:esp32:esp32", .DefaultBaud = 921600},
                New BoardProfile With {.DisplayName = "LOLIN D1 mini (ESP8266)", .Family = "ESP8266", .PlatformIoBoardId = "d1_mini", .ArduinoFqbn = "esp8266:esp8266:d1_mini", .DefaultBaud = 460800},
                New BoardProfile With {.DisplayName = "NodeMCU 1.0 (ESP8266)", .Family = "ESP8266", .PlatformIoBoardId = "nodemcuv2", .ArduinoFqbn = "esp8266:esp8266:nodemcuv2", .DefaultBaud = 460800},
                New BoardProfile With {.DisplayName = "Generic ESP8266 Module", .Family = "ESP8266", .PlatformIoBoardId = "esp01_1m", .ArduinoFqbn = "esp8266:esp8266:generic", .DefaultBaud = 460800}
            }
        End Function
    End Class
End Namespace
