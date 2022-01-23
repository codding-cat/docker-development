# Consuming Data in Kafka

-   Start the Kafka environment
-   Building a consumer in Go? (will try in C#)
-   Starting Kafka Consumer
-   Consume Messages in Random Order
-   Consume Messages in Order

## Start the Kafka environment

Let's start our Kafka components:

```
cd messaging/kafka

#only start the kafka containers, not everything!
docker compose up zookeeper-1 kafka-1 kafka-2 kafka-3

#ensure its running!
docker ps
```

## Create a Topic: Orders

To create a topic, we can exec into any container on our kafka network and create it.
We'll need a Topic for our orders:

```
docker exec -it zookeeper-1 bash

# create
/kafka/bin/kafka-topics.sh \
--create \
--zookeeper zookeeper-1:2181 \
--replication-factor 1 \
--partitions 3 \
--topic Orders

# describe
/kafka/bin/kafka-topics.sh \
--describe \
--zookeeper zookeeper-1:2181 \
--topic Orders

exit
```
