from google.protobuf.internal import containers as _containers
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Iterable as _Iterable, Mapping as _Mapping, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class YoloDetectionRequest(_message.Message):
    __slots__ = ["image", "params"]
    IMAGE_FIELD_NUMBER: _ClassVar[int]
    PARAMS_FIELD_NUMBER: _ClassVar[int]
    image: bytes
    params: YoloParams
    def __init__(self, image: _Optional[bytes] = ..., params: _Optional[_Union[YoloParams, _Mapping]] = ...) -> None: ...

class YoloDetectionResponse(_message.Message):
    __slots__ = ["params", "results"]
    PARAMS_FIELD_NUMBER: _ClassVar[int]
    RESULTS_FIELD_NUMBER: _ClassVar[int]
    params: YoloParams
    results: _containers.RepeatedCompositeFieldContainer[YoloDetectionResult]
    def __init__(self, results: _Optional[_Iterable[_Union[YoloDetectionResult, _Mapping]]] = ..., params: _Optional[_Union[YoloParams, _Mapping]] = ...) -> None: ...

class YoloDetectionResult(_message.Message):
    __slots__ = ["confidence", "label", "x1", "x2", "y1", "y2"]
    CONFIDENCE_FIELD_NUMBER: _ClassVar[int]
    LABEL_FIELD_NUMBER: _ClassVar[int]
    X1_FIELD_NUMBER: _ClassVar[int]
    X2_FIELD_NUMBER: _ClassVar[int]
    Y1_FIELD_NUMBER: _ClassVar[int]
    Y2_FIELD_NUMBER: _ClassVar[int]
    confidence: float
    label: str
    x1: int
    x2: int
    y1: int
    y2: int
    def __init__(self, label: _Optional[str] = ..., confidence: _Optional[float] = ..., x1: _Optional[int] = ..., y1: _Optional[int] = ..., x2: _Optional[int] = ..., y2: _Optional[int] = ...) -> None: ...

class YoloDetectionUrlRequest(_message.Message):
    __slots__ = ["params", "url"]
    PARAMS_FIELD_NUMBER: _ClassVar[int]
    URL_FIELD_NUMBER: _ClassVar[int]
    params: YoloParams
    url: str
    def __init__(self, url: _Optional[str] = ..., params: _Optional[_Union[YoloParams, _Mapping]] = ...) -> None: ...

class YoloParams(_message.Message):
    __slots__ = ["conf_threshold", "iou_threshold"]
    CONF_THRESHOLD_FIELD_NUMBER: _ClassVar[int]
    IOU_THRESHOLD_FIELD_NUMBER: _ClassVar[int]
    conf_threshold: float
    iou_threshold: float
    def __init__(self, conf_threshold: _Optional[float] = ..., iou_threshold: _Optional[float] = ...) -> None: ...
