#function to subscribe to a topic and print the messages
from kafka import KafkaConsumer
import os
from dotenv import load_dotenv

def subscribe_to_topic(topic_name):
    print(f"Subscribing to topic: {topic_name}")
    print(f"Kafka Broker: {os.getenv('KAFKA_BROKER')}")
    print(f"Kafka Group ID: {os.getenv('KAFKA_GROUP_ID')}")
    consumer = KafkaConsumer(topic_name, bootstrap_servers=os.getenv("KAFKA_BROKER"), group_id=os.getenv("KAFKA_GROUP_ID"))
    for message in consumer:
        print(message.value.decode('utf-8'))

# load env variables
load_dotenv()
print("Starting Kafka Subscriber...")
if __name__ == "__main__":
    topic_name = os.getenv("TOPIC_NAME")
    print(f"Subscribing to topic: {topic_name}")
    subscribe_to_topic(topic_name)