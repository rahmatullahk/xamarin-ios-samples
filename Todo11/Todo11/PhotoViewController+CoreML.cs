﻿// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreImage;
using System.Linq;
using System.Threading.Tasks;
using CoreFoundation;

// DEMO: 1. Info.plist Photo and Camera usage description
// DEMO: 2. Get the model (see below)
// DEMO: 3. Add model to /Resources/
// DEMO: 4. 

namespace Todo11App
{
    /*
        Needs CoreML model VGG16 downloaded from 
        https://developer.apple.com/machine-learning/
        (https://docs-assets.developer.apple.com/coreml/models/VGG16.mlmodel)

        Then compile and add to Resources folder:
        xcrun coremlcompiler compile {model.mlmodel} {outputFolder}

        See this README for more info
        https://github.com/xamarin/ios-samples/blob/master/ios11/CoreMLImageRecognition/CoreMLImageRecognition/README.md

        eg
        ```
        cd Downloads
        xcrun coremlcompiler compile VGG16.mlmodel vggout
        ```
    */
    public partial class PhotoViewController 
    {
        MachineLearningModel model;
        string observations;

        void ConfigureCoreML()
        {
            model = new MachineLearningModel();

            model.PredictionsUpdated += (s, e) => ShowPrediction(e.Value);
            model.ErrorOccurred += (s, e) => ShowAlert("Processing Error", e.Value);
            model.MessageUpdated += (s, e) => ShowMessage(e.Value);
        }

        void ClassifyImageAsync(UIImage img)
        {
            Task.Run(() => model.Classify(img));
        }

        void ShowPrediction(ImageDescriptionPrediction imageDescriptionPrediction)
        {
            //Grab the first 5 predictions, format them for display, and show 'em
            InvokeOnMainThread(() =>
            {
                var message = $"{imageDescriptionPrediction.ModelName} thinks:\n";
                var topFive = imageDescriptionPrediction.predictions.Take(5);
                foreach (var prediction in topFive)
                {
                    var prob = prediction.Item1;
                    var desc = prediction.Item2;
                    message += $"{desc} : {prob.ToString("P") }\n";
                }

                ShowMessage(message);
            });
        }

        private void ShowAlert(string title, string message)
        {
            var okAlertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (_) => { }));
            InvokeOnMainThread(() => PresentViewController(okAlertController, true, () => { }));
        }

        void ShowMessage(string msg)
        {
            observations = msg;
            InvokeOnMainThread(() => ClassificationLabel.Text = msg);
        }
    }
}
