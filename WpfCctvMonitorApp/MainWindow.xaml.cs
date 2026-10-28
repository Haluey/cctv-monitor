using LibVLCSharp.Shared;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;
using WpfCctvMonitorApp.Common;
using WpfCctvMonitorApp.Models;
using WpfCctvMonitorApp.Services;

namespace WpfCctvMonitorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        #region "변수 영역"

        // readonly - 생성자에서만 초기화 가능
        private LibVLC libVLC;
        private LibVLCSharp.Shared.MediaPlayer mediaPlayer;
        private ItsCctvService itsCctvService;

        // 지역 선택한 위경도 범위 저장할 변수
        private GeoBound? selectedGeoBound;

        // ProgressRing 창 변수
        private LoadingWindow? loadingWindow;

        private bool isCloseConfirmed;
        private bool isCloseConfirmOpen;

        #endregion

        #region "기본 생성자 및 이벤트핸들러 영역"

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();  // 절대 손대지 말것!

            InitLibVlc();   // VLC 라이브러리 초기화. 메서드를 분리하면 readonly 사용불가
            InitWebApiService();    // 웹서비스 초기화
        }

        /// <summary>
        /// 윈도우 로드완료 후 이벤트핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO : 나중에 지울것. VvwScreen에 ITS페이지 테스트용 스트리밍 띄우기
            //var media = new Media(libVLC, new Uri("https://cctvsec.ktict.co.kr:8082/mstrm0802hls/155C48ACE14C5708C9DF794025D2B2D6/live/04CT000001340/002/SELF/playlist.m3u8?nimblesessionid=23802213&wmsAuthSign=c2VydmVyX3RpbWU9Ny8xLzIwMjYgNDowMToyNiBBTSZoYXNoX3ZhbHVlPXVudUx4aWdwRVBjWk9QZVgrZWJPUVE9PSZ2YWxpZG1pbnV0ZXM9MTIwJmlkPW1sdG0jbnRpY2xpdmUjNzI4OTE="));
            //mediaPlayer.Play(media);

            await InitWebView2Async();  // 웹뷰2 초기화

            //var result = await InitApiKey();  // App.config에서 API키 받아오기
            //if (!result) return;

            InitAppName();
            InitStatusBar();   // xaml 화면 텍스트들 초기화
            InitComboItems();   // 콤보박스 지역리스트업
        }

        private void InitAppName()
        {
            // 비하인드코드로 작업하면 유지보수가 쉬움
            // 윈도우타이틀, WPF UI TitleBar 공통작업
            // HACK : 제목을 변경하려면 AppCommon.appName을 변경하세요
            this.Title = TlbMain.Title = AppCommon.appName;
        }

        /// <summary>
        /// 정상적으로 실행되면 처리하는 이벤트
        /// </summary>
        /// <param name="sender">이벤트발생 객체</param>
        /// <param name="e">컨트롤/객체 소유 속성들</param>
        private void MediaPlayer_Playing(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                TxtConnStat.Text = "● 연결 상태 : 정상";
                TxtConnStat.Foreground = new SolidColorBrush(Colors.Green);
            });
        }

        // 실행 안되면 처리하는 이벤트
        private void MediaPlayer_EncounteredError(object? sender, EventArgs e)
        {
            // UI 스레드와 충돌(응답없음) 방지
            Dispatcher.Invoke(() =>
            {
                //MessageBox.Show("CCTV 스트리밍 재생 중 오류가 발생했습니다.");
                // 상태표시줄의 상태를 빨간색으로 변경
                TxtConnStat.Text = "● 연결 상태 : 불량";
                TxtConnStat.Foreground = new SolidColorBrush(Colors.Red);
            });
        }

        /// <summary>
        /// 초기화버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnInit_Click(object sender, RoutedEventArgs e)
        {
            TglRoadType.IsChecked = true;
            CboRegions.SelectedIndex = 0;
            GrbCctv.Header = $"CCTV 목록  (총 0건)";
            LSbCctv.ItemsSource = null; // ItemsSource로 바인딩했을 때는 Items.Clear()를 못씀

            mediaPlayer.Stop();

            WvwMap.Reload();    // WebBrowser F5(새로고침)

            SetDetailInfo();
            InitStatusBar();
        }

        // 종료여부 확인
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (isCloseConfirmed)
                return;

            // 확인창의 응답을 받기 전에는 종료하지 않음
            e.Cancel = true;

            // 종료 버튼을 여러 번 눌러도 확인창은 하나만 표시
            if (isCloseConfirmOpen)
                return;

            isCloseConfirmOpen = true;

            try {
                bool confirmed = await ShowConfirmAsync(
                    "종료 확인", "종료하시겠습니까?");

                if (confirmed) {
                    isCloseConfirmed = true;
                    Close();
                }
            }
            finally {
                isCloseConfirmOpen = false;
            }
        }

        // 프로그램 종료시 메모리 해제
        private void Window_Closed(object sender, EventArgs e)
        {
            mediaPlayer.Stop();
            mediaPlayer.Dispose();
            libVLC.Dispose();
        }

        //private void BtnExpress_Click(object sender, RoutedEventArgs e)
        //{
        //    AppCommon.RoadType = "ex";
        //}

        //private void BtnNational_Click(object sender, RoutedEventArgs e)
        //{
        //    AppCommon.RoadType = "its";
        //}

        private void CboRegions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            
            if (CboRegions.SelectedIndex <= 0 || CboRegions.SelectedItem is not string regionName) {
                selectedGeoBound = null;
                return;
            }

            selectedGeoBound = GetRegionBounds(regionName);
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Validation Check
            if (selectedGeoBound == null)
            {
                await ShowMessageAsync("오류", "지역을 선택하세요");
                //System.Windows.MessageBox.Show("지역을 선택하세요", "오류");
                return;
            }// 아니면 try ~ catch

            try
            {
                // 로딩창 열기
                ShowLoading();
                BtnSearch.IsEnabled = false;

                // 프로세스 진행
                //AppCommon.MinX = selectedGeoBound.MinLng;
                //AppCommon.MaxX = selectedGeoBound.MaxLng;
                //AppCommon.MinY = selectedGeoBound.MinLat;
                //AppCommon.MaxY = selectedGeoBound.MaxLat;

                //var totalApiUrl = AppCommon.BuildCctvApiUrl();
                //var result = await itsCctvService.GetCctvListAsync(totalApiUrl);
                var request = new CctvRequest
                {
                    CctvType = 1,
                    GetRetType = "json",
                    RoadType = AppCommon.RoadType,
                    MinX = selectedGeoBound.MinLng,
                    MaxX = selectedGeoBound.MaxLng,
                    MinY = selectedGeoBound.MinLat,
                    MaxY = selectedGeoBound.MaxLat
                };

                var result = await itsCctvService.GetBridgeApiAsync(request);

                Debug.WriteLine(result);

                //MessageBox.Show(result.Response.DataCount.ToString());

                LSbCctv.ItemsSource = result;
                var roadName = AppCommon.RoadType == "ex" ? "고속도로" : "국도";
                GrbCctv.Header = $"{roadName} CCTV 목록  (총 {result.Count:N0}건)";
            }
            catch (Exception ex)
            {
                // 대부분 OpenAPI 호출 이후 네트워크문제에서 발생
                await ShowMessageAsync("검색 오류", $"CCTV 검색 중 오류가 발생했습니다. {ex.Message}");
                //System.Windows.MessageBox.Show($"CCTV 검색 중 오류가 발생했습니다. {ex.Message}", "검색 오류",
                //                    System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 로딩창 닫기
                HideLoading();
                BtnSearch.IsEnabled = true;
            }
        }

        // 리스트박스 선택 후 이벤트
        private async void LSbCctv_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LSbCctv.SelectedItem is not CctvResultDto selected)
                return;

            //MessageBox.Show(selected.CctvUrl);
            PlayCctv(selected.CctvUrl);

            await ShowMarkerAsync(selected);

            SetDetailInfo(selected);
            DisplayStatusBarInfo(selected);
        }

        private void TglRoadType_Checked(object sender, RoutedEventArgs e)
        {
            AppCommon.RoadType = "ex";
            TglRoadType.Content = "🛣  고속도로";
        }

        private void TglRoadType_Unchecked(object sender, RoutedEventArgs e)
        {
            AppCommon.RoadType = "its";
            TglRoadType.Content = "🛡  국도";
        }

        #endregion

        #region "커스텀 메서드 영역"

        /// <summary>
        /// Itc OpenApi Service 초기화 메서드
        /// </summary>
        private void InitWebApiService()
        {
            // OpenAPI 서비스 객체 생성
            itsCctvService = new ItsCctvService();
        }

        /// <summary>
        /// LibVLCSharp.WPF 컨트롤 초기화 메서드
        /// </summary>
        private void InitLibVlc()
        {
            // LibVLCSharp 초기화
            libVLC = new LibVLC();
            mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVLC);
            mediaPlayer.Playing += MediaPlayer_Playing;
            mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;

            VvwScreen.MediaPlayer = mediaPlayer;
        }

        //private async Task<bool> InitApiKey()
        //{
        //    AppCommon.ItsApiKey = ConfigurationManager.AppSettings["ItsApiKey"];
        //    //MessageBox.Show(AppCommon.ItsApiKey);

        //    if (string.IsNullOrWhiteSpace(AppCommon.ItsApiKey))
        //    {
        //        //System.Windows.MessageBox.Show("ITS API Key가 설정되지 않았습니다.");
        //        await ShowMessageAsync("오류", "ITS API Key가 설정되지 않았습니다.");
        //        return false;
        //    }

        //    return true;
        //}

        private async Task InitWebView2Async() {
            await WvwMap.EnsureCoreWebView2Async();

            WvwMap.CoreWebView2.Settings.UserAgent += " CctvMonitor/1.0";

            string mapFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "CctvMonitor", "Map");

            System.IO.Directory.CreateDirectory(mapFolder);

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(mapFolder, "map.html"),
                GetLeafletHtml());

            WvwMap.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "cctv-monitor.example",
                mapFolder,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.DenyCors);

            WvwMap.CoreWebView2.Navigate(
                "https://cctv-monitor.example/map.html");
        }

        private void InitStatusBar()
        {
            GrbCctv.Header = "CCTV 목록(총 0건)";

            TxtConnStat.Text = "● 연결 상태 : -";
            TxtConnStat.Foreground = new SolidColorBrush(Colors.Black);

            TxtSelCctvName.Text = "선택 CCTV :";

            TxtSelCctvUrl.Text = "영상 URL :";

            var currTime = DateTime.Now.ToString("HH:mm:ss");
            TxtLastUpdateStat.Text = $"마지막 업데이트 : {currTime}  ⟳";
        }

        private void InitComboItems()
        {
            //CboRegions.Items.Add("전국");
            CboRegions.ItemsSource = AppCommon.Regions;
            CboRegions.SelectedIndex = 0;
        }

        // Leaflet 지도 띄우기 메서드
        private string GetLeafletHtml()
        {
            // """ 여러줄 문자열 사용시 문자열 내부에서 "(쌍따옴표) 사용가능
            return """
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="utf-8" />
                        <style>
                            html, body, #map {
                                width: 100%;
                                height: 100%;
                                margin: 0;
                                padding: 0;
                            }
                        </style>

                        <link rel="stylesheet"
                              href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />

                        <script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
                    </head>
                    <body>
                        <div id="map"></div>

                        <script>
                            let map = L.map("map").setView([36.5, 127.8], 7);

                            L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
                                maxZoom: 19,
                                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                            }).addTo(map);

                            let marker = null;

                            // 새로 생성되는 마커위치로 이동하는 함수
                            function moveMarker(lat, lng, name) {
                                if (marker != null) {
                                    map.removeLayer(marker);
                                }

                                marker = L.marker([lat, lng])
                                    .addTo(map)
                                    .bindPopup(name)
                                    .openPopup();

                                console.log(lat, lng);

                                map.setView([lat, lng], 14);
                            }
                        </script>
                    </body>
                    </html>
                    """;
        }

        // 위경도 범위 리턴 메서드
        private GeoBound GetRegionBounds(string regionName)
        {
            if (string.IsNullOrWhiteSpace(regionName))
                return AppCommon.RegionBounds["전국"];

            /*AppCommon.RegionBounds.TryGetValue(regionName, out GeoBound? bound);

            if (bound == null)
                return AppCommon.RegionBounds["전국"];
            else
                return bound;*/

            return AppCommon.RegionBounds.TryGetValue(regionName, out GeoBound? bound)
                ? bound
                : AppCommon.RegionBounds["전국"];
        }

        private void DisplayStatusBarInfo(CctvResultDto cctv)
        {
            TxtSelCctvName.Text = $"선택 CCTV : {cctv.CctvName}";
            TxtSelCctvUrl.Text = $"영상 URL : {AppCommon.Ellipse(cctv.CctvUrl, 80)}";
        }

        private void SetDetailInfo(CctvResultDto? cctv = null)
        {
            if (cctv != null)   // 실제 데이터 할당
            {
                // 정규식으로 CctvName에 들어온 문자열 분해
                var (cctvName, roadName, direction) = AppCommon.ParseName(cctv.CctvName);

                TxtCctvName.Text = cctvName;    //cctv.CctvName
                TxtRoadName.Text = roadName;
                TxtDirection.Text = direction;
                TxtCoordY.Text = cctv.CoordY.ToString();
                TxtCoordX.Text = cctv.CoordX.ToString();
                TxtCctvFormat.Text = cctv.CctvFormat;
            }
            else    // 초기화
            {
                TxtCctvName.Text = string.Empty;
                TxtRoadName.Text = "";
                TxtDirection.Text = "";
                TxtCoordY.Text = string.Empty;
                TxtCoordX.Text = string.Empty;
                TxtCctvFormat.Text = string.Empty;
            }
        }

        private async Task ShowMarkerAsync(CctvResultDto cctv) {
            string arguments = Newtonsoft.Json.JsonConvert.SerializeObject(
                new object[]
                {
                    cctv.CoordY,
                    cctv.CoordX,
                    cctv.CctvName ?? "CCTV"
                }
            );

            await WvwMap.ExecuteScriptAsync($"moveMarker(...{arguments});");
        }

        // 스트리밍 재생 메서드
        private async Task PlayCctv(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                mediaPlayer.Stop();     // 이전 영상 멈추기
                using var media = new Media(libVLC, new Uri(url));
                mediaPlayer.Play(media);
            }
            catch (Exception ex)
            {
                await ShowMessageAsync("오류", $"스트리밍 재생 오류 발생 : {ex.Message}");
                //System.Windows.MessageBox.Show(ex.Message);
            }

        }

        // 단순 MessageBox.Show 대신 사용할 메서드
        private async Task ShowMessageAsync(string title, string message)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = title,
                Content = message
            };

            _ = await uiMessageBox.ShowDialogAsync();
        }

        // 예/아니오 선택 MessageBox.Show 대신 사용할 메서드
        private async Task<bool> ShowConfirmAsync(string title, string message)
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = title,
                Content = message,

                PrimaryButtonText = "예",
                SecondaryButtonText = "아니오",
                IsCloseButtonEnabled = false
            };

            var result = await uiMessageBox.ShowDialogAsync();
            return result == Wpf.Ui.Controls.MessageBoxResult.Primary;
        }

        // 로딩창 열기
        private void ShowLoading()
        {
            if (loadingWindow != null) return;

            loadingWindow = new LoadingWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            loadingWindow.Show();
        }

        // 로딩창 닫기
        private void HideLoading()
        {
            if (loadingWindow == null) return;

            loadingWindow.Close();
            loadingWindow = null;
        }

        #endregion

    }
}