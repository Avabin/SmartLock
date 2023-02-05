import io
from concurrent import futures

import cv2
import grpc
import numpy as np
import requests
from PIL import Image

import DetectionService_pb2_grpc as ds
from DetectionService_pb2 import YoloDetectionRequest, YoloDetectionResponse
from ultralytics import YOLO

model = YOLO(model="yolov8x.pt")


class DetectionService(ds.YoloDetectionService):
    def __init__(self):
        self.model = model

    def DetectStream(self, request, context, **kwargs):
        for image in request:
            print("Received image")
            # load image into numpy array
            img = Image.open(io.BytesIO(image.image))
            np_image = cv2.cvtColor(np.array(img), cv2.COLOR_RGB2BGR)
            # run inference
            print("Running inference...")
            results = self._infer(np_image, image.params.conf_threshold, image.params.iou_threshold)
            print("Inference complete")
            yield self._parse_results(results)

    def Detect(self, request, context, **kwargs):
        # image bytes from form
        img_bytes = request.image

        # run inference
        results = self._infer(img_bytes, request.params.conf_threshold, request.params.iou_threshold)
        return self._parse_results(results)

    def DetectUrl(self, request, context, **kwargs):
        image_url = request.url
        # load image into numpy array
        response = requests.get(image_url, stream=True)

        # run inference
        results = self._infer(response.content, request.params.conf_threshold, request.params.iou_threshold)
        return self._parse_results(results)

    @staticmethod
    def _infer(image, conf=0.5, iou=0.5):
        # load image into numpy array
        img = Image.open(io.BytesIO(image))
        np_image = cv2.cvtColor(np.array(img), cv2.COLOR_RGB2BGR)
        # run inference
        imgh, imgw, _ = np_image.shape
        imgsz = max(imgh, imgw)
        results = model.predict(np_image, imgsz=imgsz)
        return results

    def _parse_results(self, results):
        responses = []
        for result in results:
            # result is a tensor of shape (1, 6) where 6 is [x1, y1, x2, y2, confidence, class]
            # x1, y1, x2, y2 are the bounding box coordinates

            # get the class name
            for r in result:
                cls = int(r.boxes.cls)
                class_name = self.model.names[cls]
                # get the confidence
                confidence = float(r.boxes.conf)
                # get the bounding box coordinates
                x1, y1, x2, y2 = r.boxes.xyxy[0]
                r = YoloDetectionResponse()
                response = {
                    "label": class_name,
                    "confidence": confidence,
                    "x1": int(x1),
                    "y1": int(y1),
                    "x2": int(x2),
                    "y2": int(y2)
                }
                responses.append(response)
        return YoloDetectionResponse(results=responses)
