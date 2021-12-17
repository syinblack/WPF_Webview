using System;
using System.Windows;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 네이티브에 웹뷰를 연결하여 스크립트 사용
            _WebBrowser.ObjectForScripting = this;

            // 초기 웹뷰 주소
            Uri source = new Uri("https://www.naver.com");
            //string source = @"file:///C:\Users\NHN\source\repos\WpfApp1\page1.html";
            _WebBrowser.Navigate(@$"{source}");

        }

     
        // 네이티브(WPF) 환경에서 웹뷰(html)를 구현하고, 양방 통신 핸들링을 최종 목적으로 한다.
        // {xaml, C#(이하 cs)} 와 {html, JavaScript(이하 js)}간 양방향 통신으로 함수를 구현해보는 실습
        // 네이티브 C#은 메소드, JavaScipt는 함수로 구분한다.
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_WebBrowser.CanGoBack)
            {
                _WebBrowser.GoBack();
            }
        }
        
        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            _WebBrowser.Navigate(addressBar.Text);
        }

        private void JavaScriptButton_Click(object sender, RoutedEventArgs e)
        {
            string source = @"file:///C:\Users\NHN\source\repos\WpfApp1\page1.html";

            _WebBrowser.Navigate(@$"{source}");
        }

        private void WhatTime_Click(object sender, RoutedEventArgs e)
        {
            if (_WebBrowser.Source.ToString() == "file:///C:/Users/NHN/source/repos/WpfApp1/page1.html")
            {
                // WebBrowser 컨트롤에 연결된 JavaScript 함수를 실행한다.
                _ = _WebBrowser.InvokeScript("WhatTimeIsItNow");
            }
            else
            {
                MessageBox.Show("There's no HTML.");
            }
        }

        // 웹뷰가 전달한 메시지 표시 (해당 메소드는 웹뷰가 호출)
        public void MessageFromJavaScript(string msg)
        {
            _ = MessageBox.Show($"Native Message : {msg}");
        }
        
        // 웹뷰의 함수를 호출하여 네이티브의 인자를 전달한다.
        private void TransferDataToWebviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int[] inputArray = new int[] {
                Cal_ComboBox.SelectedIndex,
                int.Parse(NativeTextBox1.Text),
                int.Parse(NativeTextBox2.Text) };

                // WebBrowser 컨트롤에 연결된 JavaScript 함수를 호출
                object res = _WebBrowser.InvokeScript("Calculate", inputArray[0], inputArray[1], inputArray[2]);

                NativeTextBox3.Text = res.ToString();
            }
            catch (COMException)
            {
                _ = MessageBox.Show("Html 스크립트를 먼저 호출하세요.");
            }
            catch (FormatException)
            {
                _ = MessageBox.Show("Native Input을 정수로 채워야 합니다.");
            }
            catch (OverflowException)
            {
                _ = MessageBox.Show("정수형 범위가 Int32를 초과합니다.");
            }
        }

        // 웹뷰에서 url 변경하기 전에 이벤트 핸들러 등록
        public void UrlChangingFromJavaScript()
        {
            _WebBrowser.Navigating += Navigating_EventHandler; // url 파싱 이벤트 핸들러
        }

        void Navigating_EventHandler(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            string uri = e.Uri.ToString();

            string[] words = uri.Split("?");
            string[] word = words[0].Split(":");

            string protocol = word[0];
            string host = word[1];

            string cmd = words[1].Substring(3);     // op=add
            string param1 = words[2].Substring(7);  // param1=1
            string param2 = words[3].Substring(7);  // param2=2

            // 새로 웹뷰를 호출하기 전에 이벤트 핸들러 제거. (이벤트 무한 루프 발생하지 )
            _WebBrowser.Navigating -= Navigating_EventHandler;
            switch (protocol)
            {
                case "calc":
                    _WebBrowser.Navigate("file:///C:/Users/NHN/source/repos/WpfApp1/page1.html");

                    if(cmd == "Add") { Cal_ComboBox.SelectedIndex = 0;}
                    else if(cmd == "Sub") { Cal_ComboBox.SelectedIndex = 1;}
                    else if(cmd == "Mul") { Cal_ComboBox.SelectedIndex = 2;}
                    else if(cmd == "Div") { Cal_ComboBox.SelectedIndex = 3;}

                    NativeTextBox1.Text = param1;
                    NativeTextBox2.Text = param2;
                    
                    break;
                case "http":
                case "https":
                    _WebBrowser.Navigate("https://" + host);
                    break;
                default:
                    break;
            }
        }
    }
}
