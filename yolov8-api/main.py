from concurrent import futures

import grpc

from detection_service import DetectionService
import DetectionService_pb2_grpc as ds


def serve(service: DetectionService):
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    ds.add_YoloDetectionServiceServicer_to_server(service, server)
    server.add_insecure_port('localhost:50051')
    server.start()
    server.wait_for_termination()


def main():
    service = DetectionService()
    serve(service)


if __name__ == '__main__':
    main()
