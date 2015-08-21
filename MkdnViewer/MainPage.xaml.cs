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

        private async void OpenAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".md");
            StorageFile mkdnFile = await openPicker.PickSingleFileAsync();
            if (mkdnFile != null)
            {
                //await Method1(mkdnFile);
                await Method2(mkdnFile);
            }
        }

        private async Task Method2(StorageFile mkdnFile)
        {
            using (StreamReader mkdnReadStream = new StreamReader(await mkdnFile.OpenStreamForReadAsync()))
            using (StreamWriter htmlWriteStream = new StreamWriter(await ApplicationData.Current.TemporaryFolder.OpenStreamForWriteAsync(new Guid().ToString(), CreationCollisionOption.GenerateUniqueName)))
            {
                CommonMark.CommonMarkConverter.Convert(mkdnReadStream, htmlWriteStream);
            }
        }

        private async System.Threading.Tasks.Task Method1(StorageFile mkdnFile)
        {
            string mkdnDoc = await FileIO.ReadTextAsync(mkdnFile);
            string resultHTML = CommonMark.CommonMarkConverter.Convert(mkdnDoc);
            webView.NavigateToString(resultHTML);
        }

        private const string s_suggestedFileName = "untitled.html";
        private async void ExportAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedFileName = s_suggestedFileName;
            savePicker.FileTypeChoices.Add("HTML", new List<string>() { ".html"});
            savePicker.FileTypeChoices.Add("HTM", new List<string>() { ".htm" });
            StorageFile htmlFile = await savePicker.PickSaveFileAsync();
        }
    }
}
