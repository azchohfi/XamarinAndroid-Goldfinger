using System;
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Views;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using AndroidX.AppCompat.Widget;

namespace XamarinAndroid.Goldfinger.SampleApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private IGoldfinger _goldfinger;
        private AppCompatEditText editText;
        private AppCompatButton cancelButton;
        private AppCompatButton encryptButton;
        private AppCompatButton decryptButton;

        private string KEY_NAME = "key";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _goldfinger = new GoldfingerBuilder(this).LogEnabled(BuildConfig.Debug).Build();

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            editText = FindViewById<AppCompatEditText>(Resource.Id.editText);
            cancelButton = FindViewById<AppCompatButton>(Resource.Id.cancelButton);
            cancelButton.Click += CancelButton_Click;
            encryptButton = FindViewById<AppCompatButton>(Resource.Id.encryptButton);
            encryptButton.Click += EncryptButton_Click;
            decryptButton = FindViewById<AppCompatButton>(Resource.Id.decryptButton);
            decryptButton.Click += DecryptButton_Click;
        }

        private void DecryptButton_Click(object sender, EventArgs e)
        {
            cancelButton.Enabled = true;
            _goldfinger.Decrypt(BuildPromptParams(), KEY_NAME, editText.Text, new DecryptGoldfingerCallback(this));
        }

        class DecryptGoldfingerCallback : Java.Lang.Object, IGoldfingerCallback
        {
            private readonly MainActivity _activity;

            public DecryptGoldfingerCallback(MainActivity activity)
            {
                _activity = activity;
            }

            public void OnError(Java.Lang.Exception e)
            {
                /* Critical error happened */
            }

            public void OnResult(GoldfingerResult result)
            {
                _activity.OnResult(result, (value) =>
                {
                    _activity.editText.Text = value;
                });
            }
        }

        private void EncryptButton_Click(object sender, EventArgs e)
        {
            cancelButton.Enabled = true;
            _goldfinger.Encrypt(BuildPromptParams(), KEY_NAME, editText.Text, new EncryptGoldfingerCallback(this));
        }

        class EncryptGoldfingerCallback : Java.Lang.Object, IGoldfingerCallback
        {
            private readonly MainActivity _activity;

            public EncryptGoldfingerCallback(MainActivity activity)
            {
                _activity = activity;
            }

            public void OnError(Java.Lang.Exception e)
            {
                /* Critical error happened */
            }

            public void OnResult(GoldfingerResult result)
            {
                _activity.OnResult(result, (value) =>
                {
                    _activity.editText.Text = value;
                    _activity.decryptButton.Enabled = true;
                });
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.GetItem(0).SetEnabled(_goldfinger.CanAuthenticate());
            return true;
        }

        protected override void OnStop()
        {
            base.OnStop();
            _goldfinger.Cancel();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                if (_goldfinger.CanAuthenticate())
                {
                    _goldfinger.Authenticate(BuildPromptParams(), new GoldfingerCallback(this));
                }
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        GoldfingerPromptParams BuildPromptParams()
        {
            return new GoldfingerPromptParams.Builder(this)
                          .Title("Title")
                          .NegativeButtonText("Cancel")
                          .Description("Description")
                          .Subtitle("Subtitle")
                          .Build();
        }

        class GoldfingerCallback : Java.Lang.Object, IGoldfingerCallback
        {
            private readonly MainActivity _activity;

            public GoldfingerCallback(MainActivity activity)
            {
                _activity = activity;
            }

            public void OnError(Java.Lang.Exception e)
            {
                /* Critical error happened */
            }

            public void OnResult(GoldfingerResult result)
            {
                _activity.OnResult(result, null);
            }
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public void OnResult(GoldfingerResult result, Action<string> success)
        {
            System.Diagnostics.Debug.WriteLine(GetString(Resource.String.status, result.Type(), result.Reason(), result.Value(), result.Message()));

            var type = result.Type();
            if (type == GoldfingerType.Success)
            {
                cancelButton.Enabled = false;
                success?.Invoke(result.Value());
            }
            else if (type == GoldfingerType.Info)
            {

            }
            else if (type == GoldfingerType.Error)
            {
                cancelButton.Enabled = false;
            }
        }
    }
}

