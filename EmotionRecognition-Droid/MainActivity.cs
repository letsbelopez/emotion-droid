using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Permissions;
using Android.Content.PM;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System.Linq;

namespace EmotionRecognition_Droid
{
    [Activity(Label = "EmotionRecognition_Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private string apiKey = "";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.TakePhotoButton);

            button.Click += async delegate (object sender, EventArgs args)
            {
                await CrossMedia.Current.Initialize();

                // Set camera options and await photo to be taken
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    Name = "emotion.jpg"
                });

                EmotionServiceClient emotionClient = new EmotionServiceClient(apiKey);
                // Do Emotion Recognition
                try
                {
                    using (var photoStream = file.GetStream())
                    {
                        Emotion[] emotionResult = await emotionClient.RecognizeAsync(photoStream);
                        if (emotionResult.Any())
                        {
                            // Emotions detected are happiness, sadness, surprise, anger, fear, contempt, disgust, or neutral.
                            var result = emotionResult.FirstOrDefault().Scores.ToRankedList().FirstOrDefault().Key;
                        }
                        file.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

