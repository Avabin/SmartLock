FROM ultralytics/ultralytics:latest-cpu
# copy requirements.txt to working directory
LABEL org.opencontainers.image.source=https://github.com/Avabin/SmartLock
WORKDIR /app
COPY requirements.txt .

# copy all files to working directory
COPY detection_service.py .
COPY main.py .
COPY DetectionService_pb2.py .
COPY DetectionService_pb2_grpc.py .
COPY DetectionService_pb2.pyi .

# run the app
CMD ["python3", "main.py"]
