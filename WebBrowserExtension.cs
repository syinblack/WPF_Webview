using System;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MeditationExtension 
{ 
	public static class WebBrowserExtension
	{
        public static List<string> ParseURL(string url)
        {
            string[] words = url.Split("?");
            string[] word = words[0].Split(":");

            string protocol = word[0];
            string host = word[1];

            string cmd = words[1][3..];     // op=add
            string param1 = words[2][7..];  // param1=1
            string param2 = words[3][7..];  // param2=2

            return new List<string> { protocol, host, cmd, param1, param2 };
        }


        // 아래로는 모두 WebBrowser 클래스에 대한 확장 메소드이다.
        public static object CalculateInputs(this WebBrowser _WebBrowser, int cmd, string input1, string input2)
        {
            try
            {
                // WebBrowser 컨트롤에 연결된 JavaScript 함수를 호출
                return _WebBrowser.InvokeScript("Calculate", cmd, int.Parse(input1), int.Parse(input2));
            }
            catch (COMException e1)
            {
                _ = MessageBox.Show("Html 스크립트를 먼저 호출하세요.");
                return e1;
            }
            catch (FormatException e2)
            {
                _ = MessageBox.Show("Native Input을 정수로 채워야 합니다.");
                return e2;
            }
            catch (OverflowException e3)
            {
                _ = MessageBox.Show("정수형 범위가 Int32를 초과합니다.");
                return e3;
            }
        }

        public static void FillParsedResourceInControls(this WebBrowser _WebBrowser, List<string> URLResources, ComboBox _ComboBox, params TextBox[] _TextBoxes)
        {
            // URLResources : {protocol, host, cmd, param1, param2}
            switch (URLResources[0])
            {
                case "calc":
                    if (URLResources[2] == "Add") { _ComboBox.SelectedIndex = 0; }
                    else if (URLResources[2] == "Sub") { _ComboBox.SelectedIndex = 1; }
                    else if (URLResources[2] == "Mul") { _ComboBox.SelectedIndex = 2; }
                    else if (URLResources[2] == "Div") { _ComboBox.SelectedIndex = 3; }

                    _TextBoxes[0].Text = URLResources[3];
                    _TextBoxes[1].Text = URLResources[4];

                    break;
                //case "http":
                //case "https":
                //    _WebBrowser.Navigate("https://" + URLResources[1]);
                //    break;
                default:
                    break;
            }
        }
	}
}
