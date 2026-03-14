using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace ExtremeCountdownGenerator
{
    // メインのアプリケーションクラス
    public class CountdownApp : Form
    {
        // コンポーネントの定義
        private Panel headerPanel;
        private Panel mainPanel;
        private Label lblAppName;
        private Label lblInstruction;
        private Label lblResultValue;
        private Label lblResultUnit;
        private Label lblStatus;
        private DateTimePicker targetDatePicker;
        private GroupBox resultGroupBox;
        private Button btnReset;
        private Button btnExportLog;
        private Timer blinkTimer;

        // 定数
        private const string APP_TITLE = "残り〇〇日ジェネレーター Professional Edition";
        private const string DEFAULT_UNIT = "日";

        public CountdownApp()
        {
            InitializeComponent();
            InitializeEvents();
            UpdateCountdown();
        }

        private void InitializeComponent()
        {
            // フォーム本体の設定
            this.Text = APP_TITLE;
            this.Size = new Size(500, 450);
            this.MinimumSize = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // ヘッダーパネル
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;
            headerPanel.BackColor = Color.FromArgb(45, 52, 54);

            lblAppName = new Label();
            lblAppName.Text = APP_TITLE;
            lblAppName.ForeColor = Color.White;
            lblAppName.Font = new Font("メイリオ", 14, FontStyle.Bold);
            lblAppName.TextAlign = ContentAlignment.MiddleCenter;
            lblAppName.Dock = DockStyle.Fill;
            headerPanel.Controls.Add(lblAppName);

            // メインコンテンツパネル
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(20);

            // 入力セクション
            lblInstruction = new Label();
            lblInstruction.Text = "目標とする期日をカレンダーから選択してください：";
            lblInstruction.Location = new Point(25, 80);
            lblInstruction.AutoSize = true;
            lblInstruction.Font = new Font("メイリオ", 9);

            targetDatePicker = new DateTimePicker();
            targetDatePicker.Location = new Point(25, 105);
            targetDatePicker.Width = 430;
            targetDatePicker.Font = new Font("MS UI Gothic", 12);
            targetDatePicker.Format = DateTimePickerFormat.Long;

            // 結果表示グループボックス
            resultGroupBox = new GroupBox();
            resultGroupBox.Text = "計算結果";
            resultGroupBox.Location = new Point(25, 160);
            resultGroupBox.Size = new Size(430, 160);
            resultGroupBox.Font = new Font("メイリオ", 9);

            lblResultValue = new Label();
            lblResultValue.Text = "0";
            lblResultValue.Font = new Font("Arial", 60, FontStyle.Bold);
            lblResultValue.ForeColor = Color.DodgerBlue;
            lblResultValue.TextAlign = ContentAlignment.MiddleRight;
            lblResultValue.Size = new Size(250, 100);
            lblResultValue.Location = new Point(50, 40);

            lblResultUnit = new Label();
            lblResultUnit.Text = DEFAULT_UNIT;
            lblResultUnit.Font = new Font("メイリオ", 20, FontStyle.Bold);
            lblResultUnit.Location = new Point(310, 85);
            lblResultUnit.AutoSize = true;

            resultGroupBox.Controls.Add(lblResultValue);
            resultGroupBox.Controls.Add(lblResultUnit);

            // ボタン類
            btnReset = new Button();
            btnReset.Text = "今日に戻す";
            btnReset.Location = new Point(25, 340);
            btnReset.Size = new Size(100, 30);
            btnReset.UseVisualStyleBackColor = true;

            btnExportLog = new Button();
            btnExportLog.Text = "結果をログ保存";
            btnExportLog.Location = new Point(355, 340);
            btnExportLog.Size = new Size(100, 30);
            btnExportLog.UseVisualStyleBackColor = true;

            // ステータスラベル
            lblStatus = new Label();
            lblStatus.Text = "準備完了";
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Height = 25;
            lblStatus.BackColor = Color.LightGray;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            // コントロールの追加
            this.Controls.Add(btnExportLog);
            this.Controls.Add(btnReset);
            this.Controls.Add(resultGroupBox);
            this.Controls.Add(targetDatePicker);
            this.Controls.Add(lblInstruction);
            this.Controls.Add(headerPanel);
            this.Controls.Add(lblStatus);

            // タイマー（装飾用）
            blinkTimer = new Timer();
            blinkTimer.Interval = 500;
            blinkTimer.Tick += new EventHandler(this.OnBlink);
        }

        private void InitializeEvents()
        {
            targetDatePicker.ValueChanged += new EventHandler(this.OnDateChanged);
            btnReset.Click += new EventHandler(this.OnResetClick);
            btnExportLog.Click += new EventHandler(this.OnExportClick);
        }

        private void OnDateChanged(object sender, EventArgs e)
        {
            UpdateCountdown();
        }

        private void OnResetClick(object sender, EventArgs e)
        {
            targetDatePicker.Value = DateTime.Now;
            UpdateCountdown();
            lblStatus.Text = "日付をリセットしました。";
        }

        private void OnExportClick(object sender, EventArgs e)
        {
            try
            {
                string logText = String.Format("[{0}] 目標日: {1} - 残り: {2}日", 
                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    targetDatePicker.Value.ToShortDateString(),
                    lblResultValue.Text);
                
                File.AppendAllText("countdown_log.txt", logText + Environment.NewLine, Encoding.UTF8);
                MessageBox.Show("ログを保存しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存エラー: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateCountdown()
        {
            DateTime target = targetDatePicker.Value.Date;
            DateTime today = DateTime.Today;

            TimeSpan span = target.Subtract(today);
            int totalDays = (int)span.TotalDays;

            lblResultValue.Text = totalDays.ToString();

            if (totalDays > 0)
            {
                lblResultValue.ForeColor = Color.DodgerBlue;
                lblStatus.Text = "未来の目標をカウントしています...";
                blinkTimer.Stop();
                lblResultValue.Visible = true;
            }
            else if (totalDays == 0)
            {
                lblResultValue.ForeColor = Color.Green;
                lblStatus.Text = "★当日です！おめでとうございます！★";
                blinkTimer.Start();
            }
            else
            {
                lblResultValue.ForeColor = Color.Crimson;
                lblStatus.Text = "指定日はすでに経過しています。";
                blinkTimer.Stop();
                lblResultValue.Visible = true;
            }
        }

        private void OnBlink(object sender, EventArgs e)
        {
            lblResultValue.Visible = !lblResultValue.Visible;
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try 
            {
                Application.Run(new CountdownApp());
            }
            catch (Exception ex)
            {
                MessageBox.Show("致命的なエラーが発生しました:\n" + ex.Message);
            }
        }
    }
}
