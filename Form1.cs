using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ClientModel;
using System.Threading;
using System.Reflection;

#pragma warning disable OPENAI001

namespace topin
{
    public partial class Form1 : Form
    {
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;

        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc _proc;

        private Button overlayButton;
        private Button anotherButton;
        private Label clickLabel;

        public Form1()
        {
            InitializeComponent();
            _proc = HookCallback;
            _hookID = SetHook(_proc);
            this.Load += new EventHandler(Form1_Load);
            this.Shown += new EventHandler(Form1_Shown);
            this.ShowInTaskbar = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MakeFormClickThrough();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Debug.WriteLine("Form1_Shown completed.");
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                UpdateClickLabel(new Point(hookStruct.pt.x, hookStruct.pt.y));
                Debug.WriteLine($"Mouse clicked at hookStruct: X={hookStruct.pt.x}, Y={hookStruct.pt.y}");
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_hookID);
            base.OnFormClosed(e);
        }

        private void UpdateClickLabel(Point location)
        {
            if (clickLabel == null)
            {
                return;
            }

            if (clickLabel.InvokeRequired)
            {
                clickLabel.Invoke(new Action<Point>(UpdateClickLabel), new object[] { location });
            }
            else
            {
                clickLabel.Text = $"X: {location.X}, Y: {location.Y}";
                clickLabel.BringToFront();
                clickLabel.Refresh();
            }
        }

        private void OverlayButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("OverlayButton clicked");
            this.Close();
        }

        private async void AnotherButton_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\tmp";
            try
            {
                Rectangle bounds = new Rectangle(Cursor.Position.X - 256, Cursor.Position.Y - 256, 512, 512);
                using (Bitmap bitmap = new Bitmap(512, 512))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }
                    string screenshotPath = Path.Combine(folderPath, $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    bitmap.Save(screenshotPath, ImageFormat.Png);
                    Debug.WriteLine($"Screenshot saved to {screenshotPath}");

                    // Analyze the captured screenshot
                    // await AnalyzeImage(screenshotPath);
                    // Call a Python script to analyze the image
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "python";
                    startInfo.Arguments = $"{Path.Combine(folderPath, "analyze_image.py")} {screenshotPath}";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true; // Add this line to capture errors
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true; // Add this line to prevent the shell from opening

                    await Task.Run(() =>
                    {
                        using (Process process = Process.Start(startInfo))
                        {
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            process.WaitForExit();
                            string result = string.IsNullOrEmpty(error) ? output : error;
                            this.Invoke((MethodInvoker)delegate {
                                ShowMessageAtCursor(result);
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error capturing screenshot: {ex.Message}");
            }
        }

        private void ShowMessageAtCursor(string message)
        {
            Label messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.AutoSize = true;
            messageLabel.BackColor = Color.Yellow;
            messageLabel.Location = this.PointToClient(Cursor.Position);
            this.Controls.Add(messageLabel);
            messageLabel.BringToFront();
            Task.Delay(3000).ContinueWith(t => this.Invoke((MethodInvoker)delegate { this.Controls.Remove(messageLabel); }));
        }

        // private async Task AnalyzeImage(string imagePath)
        // {
        //     try
        //     {
        //         var apiKey = "YOUR_API_KEY_HERE";
        //         if (string.IsNullOrEmpty(apiKey))
        //         {
        //             throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
        //         }

        //         Debug.WriteLine($"API Key: {apiKey}");

        //         // Initialize OpenAI Client
        //         OpenAIClient openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey));
        //         FileClient fileClient = openAIClient.GetFileClient();
        //         AssistantClient assistantClient = openAIClient.GetAssistantClient();
        //         Debug.WriteLine($"*** This should be the image path {imagePath}");

        //         // Upload the image file
        //         OpenAIFileInfo imgFile = await fileClient.UploadFileAsync(imagePath, FileUploadPurpose.Vision);
        //         // Process the uploaded file

        //         var imageContent = await ReadAllBytesAsync(imagePath);
        //         var imageBase64 = Convert.ToBase64String(imageContent);
        //         var imageMessage = MessageContent.FromImageUrl(new Uri($"data:image/png;base64,{imageBase64}"));

        //         // Create Assistant
        //         var assistant = assistantClient.CreateAssistant("gpt-4o", new AssistantCreationOptions()
        //         {
        //             Instructions = "You are a useful assistant that replies using a funny style and always in Spanish."
        //         });

        //         // Create Thread
        //         var initialMessages = new List<ThreadInitializationMessage>
        //         {
        //             new ThreadInitializationMessage(
        //                 MessageRole.User,
        //                 new List<MessageContent>
        //                 {
        //                     MessageContent.FromText("Hello, assistant! Please describe this image for me:"),
        //                     imageMessage
        //                 }
        //             )
        //         };

        //         var threadCreationOptions = new ThreadCreationOptions();
        //         AssistantThread thread = await assistantClient.CreateThreadAsync(threadCreationOptions);

        //         // Run Streaming
        //         var streamingUpdates = assistantClient.CreateRunStreaming(thread, assistant);

        //         foreach (var streamingUpdate in streamingUpdates)
        //         {
        //             if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
        //             {
        //                 Console.WriteLine("--- Run started! ---");
        //             }
        //             if (streamingUpdate is MessageContentUpdate contentUpdate)
        //             {
        //                 Console.Write(contentUpdate.Text);
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.WriteLine($"An error occurred: {ex.Message}");
        //     }
        // }

        public static async Task<byte[]> ReadAllBytesAsync(string path)
        {
            using (FileStream sourceStream = new FileStream(path,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                var result = new byte[sourceStream.Length];
                await sourceStream.ReadAsync(result, 0, (int)sourceStream.Length);
                return result;
            }
        }

        private void MakeFormClickThrough()
        {
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_LAYERED;
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle);
            this.TransparencyKey = Color.Lime;
            this.BackColor = Color.Lime;

            // Ensure buttons are not click-through
            SetButtonNonClickThrough(overlayButton);
            SetButtonNonClickThrough(anotherButton);

            overlayButton.BringToFront();
            anotherButton.BringToFront();
            overlayButton.Enabled = true;
            anotherButton.Enabled = true;

            Debug.WriteLine("MakeFormClickThrough completed.");
        }

        private void SetButtonNonClickThrough(Control control)
        {
            if (control == null || !control.IsHandleCreated)
                return;

            int exStyle = GetWindowLong(control.Handle, GWL_EXSTYLE);
            exStyle &= ~WS_EX_TRANSPARENT;
            SetWindowLong(control.Handle, GWL_EXSTYLE, exStyle);
            control.BringToFront();
            Debug.WriteLine($"SetButtonNonClickThrough completed for {control.Name}");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
