using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MkdnViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Open App Button Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".md");

            try
            {
                StorageFile mkdnFile = await openPicker.PickSingleFileAsync();
                if (mkdnFile != null)
                {
                    LoadMkdnFile(mkdnFile);
                }
            }
            catch
            {
                // Nothing to do. It just swallows all exception regarding user experience
            }
        }

        /// <summary>
        /// Load converted html with markdown into webview control
        /// </summary>
        /// <param name="mkdnFile"></param>
        private async void LoadMkdnFile(StorageFile mkdnFile)
        {
            webView.NavigateToString(await GetHtmlContentFromMkdnFile(mkdnFile));
        }

        /// <summary>
        /// Convert makrdown file to html string
        /// </summary>
        /// <param name="mkdnFile">makrdown file</param>
        /// <returns></returns>
        private async Task<string> GetHtmlContentFromMkdnFile(StorageFile mkdnFile)
        {
            return CommonMarkConverter.Convert(await FileIO.ReadTextAsync(mkdnFile));
        }

        /// <remarks>Deprecated</remarks>
        /// <summary>
        /// Convert markdown file to html file in passed folder(as folder param)
        /// </summary>
        /// <param name="folder">Folder location to save html file</param>
        /// <param name="mkdnFile">Markdown file to convert</param>
        /// <returns>unique html file path using GUID and CreationCollisionOption.GenerateUniqueName.
        /// it could be null if fails to create new file</returns>
        private async Task<string> CreateHtmlFileFromMkdnFile(StorageFolder folder, StorageFile mkdnFile)
        {
            StorageFile htmlFile = await folder.CreateFileAsync(Guid.NewGuid().ToString(), CreationCollisionOption.GenerateUniqueName);

            using (var mkdnReadStream = new StreamReader(await mkdnFile.OpenStreamForReadAsync()))
            using (var htmlWriteStream = new StreamWriter(await htmlFile.OpenStreamForWriteAsync()))
            {
                CommonMark.CommonMarkConverter.Convert(mkdnReadStream, htmlWriteStream);
            }

            return htmlFile?.Path;
        }
        
        private const string s_suggestedFileName = "untitled.html";
        private async void ExportAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedFileName = s_suggestedFileName;
            savePicker.FileTypeChoices.Add("HTML", new List<string>() { ".html" });
            savePicker.FileTypeChoices.Add("HTM", new List<string>() { ".htm" });
            StorageFile htmlFile = await savePicker.PickSaveFileAsync();
        }
    }
}
