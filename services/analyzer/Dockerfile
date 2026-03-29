# python image with Kafka consumer
FROM python:3.9-slim
WORKDIR /app
COPY kafka-subscriber.py .
RUN pip install kafka-python
RUN pip install dotenv
CMD ["python", "kafka-subscriber.py"]