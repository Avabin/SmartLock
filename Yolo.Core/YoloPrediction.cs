﻿using SkiaSharp;

namespace ObjectDetector.Yolo.Onnx;


    public class YoloPrediction
    {
        public YoloLabel? Label { get; set; }
        public SKRect Rectangle { get; set; }
        public float Score { get; set; }

        public YoloPrediction() { }

        public YoloPrediction(YoloLabel label, float confidence) : this(label)
        {
            Score = confidence;
        }

        public YoloPrediction(YoloLabel label)
        {
            Label = label;
        }
    }