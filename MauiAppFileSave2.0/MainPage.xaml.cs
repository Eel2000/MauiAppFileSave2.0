using CommunityToolkit.Maui.Storage;

namespace MauiAppFileSave2._0
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            //Generate a huge dummy file just for the example.
            String sDummyPath = Path.Combine(FileSystem.Current.CacheDirectory, "huge_dummy_file.txt");
            FileStream fs = new FileStream(sDummyPath, FileMode.OpenOrCreate);
            fs.Seek(2048L * 1024 * 1024, SeekOrigin.Begin);
            fs.WriteByte(0);
            fs.Close();


            try
            {

                var progession = new Progress<double>(progression =>
                {
                    this.progess.ProgressTo(progression, 300, Easing.Linear);
                });

                using (FileStream fileStream = new FileStream(sDummyPath, FileMode.Open, FileAccess.Read))
                {
                    var fileSaverResult = await FileSaver.Default.SaveAsync("huge_dummy_copy.txt", fileStream, progession);
                    if (fileSaverResult.IsSuccessful)
                    {
                        await Application.Current.MainPage.DisplayAlert("Wnd fileSave", $"The file was saved successfully to location: {fileSaverResult.FilePath}", "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Wnd fileSave", $"The file was not saved successfully with error: {fileSaverResult.Exception.Message}", "Ok");
                    }
                    fileStream.Close();
                }
            }
            finally
            {

                //Cleanup the original dummy file.
                System.IO.File.Delete(sDummyPath);
            }
        }
    }

}
