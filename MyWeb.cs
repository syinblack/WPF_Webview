using System;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Navigation;

namespace MeditationWebBrowser
{ 
    public class MyWeb
    {
        private WebBrowser _WebBrowser;

        public MyWeb(WebBrowser _WebBrowser)
        {
            this._WebBrowser = _WebBrowser;
        }

        //----- Properties -----
        public bool CanGoBack
        {
            get { return _WebBrowser.CanGoBack; }
        }

        //----- Methods ----- 
        public void InitializeWebBrowser(WpfApp1.MainWindow _MainWindow)
        {
            _WebBrowser.ObjectForScripting = _MainWindow;
        }

        public void Navigate(Uri uri) { _WebBrowser.Navigate(uri); }
        public void Navigate(string str) { _WebBrowser.Navigate(str); }
        public void Closing(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"종료 하시겠습니까?", "종료", MessageBoxButton.OKCancel);
            if(result == MessageBoxResult.OK)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }
        public void GoBack()
        {
            if (_WebBrowser.CanGoBack)
            {
                _WebBrowser.GoBack();
            }
        }

        public void GoJavaScript()
        {
            _WebBrowser.Navigate($@"file:///C:\Users\NHN\source\repos\WpfApp1\page1.html");
        }

        public void GetCurrentTime()
        {
            if (_WebBrowser.Source.ToString() == "file:///C:/Users/NHN/source/repos/WpfApp1/page1.html")
            {
                // WebBrowser 컨트롤에 연결된 JavaScript 함수를 실행한다.
                _ = _WebBrowser.InvokeScript("WhatTimeIsItNow");
            }
            else
            {
                _ = MessageBox.Show("There's no HTML.");
            }
        }

        public void ShowMessageBox(string msg)
        {
            _ = MessageBox.Show($"Native Message : {msg}");
        }
        
        public object CalculateInputs(int cmd, string input1, string input2)
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

        public void ParseURLBeforeNavigation(System.Windows.Navigation.NavigatingCancelEventArgs e,
             out string cmd, out List<object> list)
        {
            list = new List<object>();

            string[] protocol = e.Uri.ToString().Split(":");

            switch (protocol[0])
            {
                case "calc":
                    cmd = "Calculate";

                    string[] words = e.Uri.ToString().Split("?");
                    string[] word = words[0].Split(":");

                    string host = word[1];
                    string operation = words[1][3..];      // op=add
                    string param2 = words[3][7..];         // param2=2
                    string param1 = words[2][7..];         // param1=1

                    if (operation == "Add") { list.Add(0); }
                    else if (operation == "Sub") { list.Add(1); }
                    else if (operation == "Mul") { list.Add(2); }
                    else if (operation == "Div") { list.Add(3); }

                    list.Add(param1);
                    list.Add(param2);

                    e.Cancel = true;
                    break;
                default:
                    cmd = "Navigate";
                    break;
            }
        }

    }   // class END
}       // namespace END