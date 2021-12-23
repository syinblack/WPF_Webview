using System.Windows;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using MeditationWebBrowser;              // made by meditation
using System.Collections.Generic;
using System.Windows.Navigation;

// 네이티브(WPF) 환경에서 웹뷰(html)를 구현하고, 양방 통신 핸들링을 최종 목적으로 한다.
// {xaml, C#(이하 cs)} 와 {html, JavaScript(이하 js)}간 양방향 통신으로 함수를 구현해보는 실습
// 네이티브 C#은 메소드, JavaScipt는 함수로 구분한다.
namespace WpfApp1
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class MainWindow : Window
    {
        MyWeb wb;

        public MainWindow()
        {
            InitializeComponent();

            // MyWeb 인스턴스를 생성하여 웹뷰 핸들링
            wb = new MyWeb(_WebBrowser);
            // 스크립트 웹뷰에 연결
            wb.InitializeWebBrowser(this);
            // 웹뷰에 이벤트핸들러 등록 Q: wb.메소드로 수정가능한 지 확인
            _WebBrowser.Navigating += _WebBrowser_Navigating;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            wb.GoBack();
        }
        
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            wb.Navigate(addressBar.Text);
        }

        private void JavaScriptButton_Click(object sender, RoutedEventArgs e)
        {
            wb.GoJavaScript();
        }

        private void WhatTime_Click(object sender, RoutedEventArgs e)
        {
            wb.GetCurrentTime();
        }

        // 웹뷰가 전달한 메시지 표시 (해당 메소드는 웹뷰가 호출)
        public void MessageFromJavaScript(string msg)
        {
            wb.ShowMessageBox(msg);
        }
        
        // 웹뷰의 함수를 호출하여 네이티브의 인자를 전달한다.
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            object res = wb.CalculateInputs(Cal_ComboBox.SelectedIndex, NativeTextBox1.Text, NativeTextBox2.Text);
            NativeTextBox3.Text = res.ToString();
        }

        // 웹뷰에서 url 변경하기 전에 이벤트 핸들러 등록
        // _WebBrowser_Navigating 이벤트 핸들러는 웹뷰에서 url 변경하기 전에만 추가되고, 나머지 경우에는 제거된 상태로 있는다.
        public void UrlChangingFromJavaScript(string url)
        {
            wb.Navigate(url);
        }

        // 웹뷰로부터 url 변경 신호를 받으면 (url 변경전에) 파싱한다.
        // url scheme : {protocol}:{host}?{op=}?{param1=}?{param2=}
        public void _WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            wb.ParseURLBeforeNavigation(e, out string cmd, out List<object> list);

            if (cmd == "Calculate")
            {
                Cal_ComboBox.SelectedIndex = (int)list[0];
                NativeTextBox1.Text = (string)list[1];
                NativeTextBox2.Text = (string)list[2];
            }
            else if (cmd == "Navigate")
            {
                return;
            }
            else
            {
                _ = MessageBox.Show("Error : Failed to parse URL");
            }
        }

        private void Window_ContentRendered(object sender, System.EventArgs e)
        {
            wb.Navigate($"https://www.toast.com");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            wb.Closing(e);
        }
    }
}
