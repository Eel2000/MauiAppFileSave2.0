namespace MauiAppFileSave2._0
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();

            Dispatcher.Dispatch(async () =>
            {
                var storagePerm = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (storagePerm is not PermissionStatus.Granted)
                {
                    do
                    {
                        storagePerm = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    } while (storagePerm is not PermissionStatus.Granted);
                }
            });
        }
    }
}
