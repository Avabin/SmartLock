# Generated by the gRPC Python protocol compiler plugin. DO NOT EDIT!
"""Client and server classes corresponding to protobuf-defined services."""
import grpc

import DetectionService_pb2 as DetectionService__pb2


class YoloDetectionServiceStub(object):
    """Missing associated documentation comment in .proto file."""

    def __init__(self, channel):
        """Constructor.

        Args:
            channel: A grpc.Channel.
        """
        self.DetectStream = channel.stream_stream(
                '/yolo.YoloDetectionService/DetectStream',
                request_serializer=DetectionService__pb2.YoloDetectionRequest.SerializeToString,
                response_deserializer=DetectionService__pb2.YoloDetectionResponse.FromString,
                )
        self.Detect = channel.unary_unary(
                '/yolo.YoloDetectionService/Detect',
                request_serializer=DetectionService__pb2.YoloDetectionRequest.SerializeToString,
                response_deserializer=DetectionService__pb2.YoloDetectionResponse.FromString,
                )
        self.DetectUrl = channel.unary_unary(
                '/yolo.YoloDetectionService/DetectUrl',
                request_serializer=DetectionService__pb2.YoloDetectionUrlRequest.SerializeToString,
                response_deserializer=DetectionService__pb2.YoloDetectionResponse.FromString,
                )


class YoloDetectionServiceServicer(object):
    """Missing associated documentation comment in .proto file."""

    def DetectStream(self, request_iterator, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def Detect(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')

    def DetectUrl(self, request, context):
        """Missing associated documentation comment in .proto file."""
        context.set_code(grpc.StatusCode.UNIMPLEMENTED)
        context.set_details('Method not implemented!')
        raise NotImplementedError('Method not implemented!')


def add_YoloDetectionServiceServicer_to_server(servicer, server):
    rpc_method_handlers = {
            'DetectStream': grpc.stream_stream_rpc_method_handler(
                    servicer.DetectStream,
                    request_deserializer=DetectionService__pb2.YoloDetectionRequest.FromString,
                    response_serializer=DetectionService__pb2.YoloDetectionResponse.SerializeToString,
            ),
            'Detect': grpc.unary_unary_rpc_method_handler(
                    servicer.Detect,
                    request_deserializer=DetectionService__pb2.YoloDetectionRequest.FromString,
                    response_serializer=DetectionService__pb2.YoloDetectionResponse.SerializeToString,
            ),
            'DetectUrl': grpc.unary_unary_rpc_method_handler(
                    servicer.DetectUrl,
                    request_deserializer=DetectionService__pb2.YoloDetectionUrlRequest.FromString,
                    response_serializer=DetectionService__pb2.YoloDetectionResponse.SerializeToString,
            ),
    }
    generic_handler = grpc.method_handlers_generic_handler(
            'yolo.YoloDetectionService', rpc_method_handlers)
    server.add_generic_rpc_handlers((generic_handler,))


 # This class is part of an EXPERIMENTAL API.
class YoloDetectionService(object):
    """Missing associated documentation comment in .proto file."""

    @staticmethod
    def DetectStream(request_iterator,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.stream_stream(request_iterator, target, '/yolo.YoloDetectionService/DetectStream',
            DetectionService__pb2.YoloDetectionRequest.SerializeToString,
            DetectionService__pb2.YoloDetectionResponse.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)

    @staticmethod
    def Detect(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(request, target, '/yolo.YoloDetectionService/Detect',
            DetectionService__pb2.YoloDetectionRequest.SerializeToString,
            DetectionService__pb2.YoloDetectionResponse.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)

    @staticmethod
    def DetectUrl(request,
            target,
            options=(),
            channel_credentials=None,
            call_credentials=None,
            insecure=False,
            compression=None,
            wait_for_ready=None,
            timeout=None,
            metadata=None):
        return grpc.experimental.unary_unary(request, target, '/yolo.YoloDetectionService/DetectUrl',
            DetectionService__pb2.YoloDetectionUrlRequest.SerializeToString,
            DetectionService__pb2.YoloDetectionResponse.FromString,
            options, channel_credentials,
            insecure, call_credentials, compression, wait_for_ready, timeout, metadata)
