using System.Windows;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using MeditationExtension;              // made by meditation
using System.Collections.Generic;

namespace WpfApp1
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _WebBrowser.ObjectForScripting = this;
            _WebBrowser.Navigated += _WebBrowser_Navigated;
            _WebBrowser.Navigate($"https://www.naver.com");
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
            object res = _WebBrowser.CalculateInputs(Cal_ComboBox.SelectedIndex, NativeTextBox1.Text, NativeTextBox2.Text);

            NativeTextBox3.Text = res.ToString();
        }

        // 웹뷰에서 url 변경하기 전에 이벤트 핸들러 등록
        // _WebBrowser_Navigating 이벤트 핸들러는 웹뷰에서 url 변경하기 전에만 추가되고, 나머지 경우에는 제거된 상태로 있어야 함.
        public void UrlChangingFromJavaScript()
        {
            _WebBrowser.Navigating += _WebBrowser_Navigating; // url 파싱 이벤트 핸들러
        }

        // 웹뷰로부터 url 변경 신호를 받으면 (url 변경전에) 파싱한다.
        // url scheme : {protocol}:{host}?{op=}?{param1=}?{param2=}
        private void _WebBrowser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // ParseURL: url을 파싱하여 {protocol, host, cmd, param1, param2} 순으로 반환
            List<string> URLResources = WebBrowserExtension.ParseURL(e.Uri.ToString());

            _WebBrowser.FillParsedResourceInControls(URLResources, Cal_ComboBox, NativeTextBox1, NativeTextBox2);
        }

        // url에 의해 페이지가 로드된 후 _WebBrowser_Navigating가 등록되어 있다면 제거한다.
        private void _WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            _WebBrowser.Navigating -= _WebBrowser_Navigating;
        }
    }
}
