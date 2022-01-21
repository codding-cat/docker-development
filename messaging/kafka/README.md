# Introduction to Kafka

Official [Docs](https://kafka.apache.org/)

## Building a Docker file

As always, we start with a dockerfile
We can build our dockerfile

```
cd .\messaging\kafka\
docker build . -t aimvector/kafka:3.0.0
```

## Exploring the Kafka Install

We can then run it to explore the contents:

```
docker run --rm --name kafka -it aimvector/kafka:3.0.0 bash

ls -l /kafka/bin/
cat /kafka/config/server.properties
```

We can use the docker cp command to copy the file out of our container:

```
docker cp kafka:/kafka/config/server.properties ./server.properties
docker cp kafka:/kafka/config/zookeeper.properties ./zookeeper.properties
```

Note: We'll need the Kafka configuration to tune our server and Kafka also requires at least one Zookeeper instance in order to function. To achieve high availability, we'll run multiple kafka as well as multiple zookeeper instances in the future

# Zookeeper

Let's build a Zookeeper image. The Apache folks have made it easy to start a Zookeeper instance the same way as the Kafka instance by simply running the `start-zookeeper.sh` script.

```
cd ./zookeeper
docker build . -t aimvector/zookeeper:3.0.0
cd ..
```

Let's create a kafka network and run 1 zookeeper instance

```
docker network create kafka
docker run -d `
--rm `
--name zookeeper-1 `
--net kafka `
-v ${PWD}/config/zookeeper-1/zookeeper.properties:/kafka/config/zookeeper.properties `
aimvector/zookeeper:3.0.0

docker logs zookeeper-1
```

## Kafka - 1

```
docker run -d `
--rm `
--name kafka-1 `
--net kafka `
-v ${PWD}/config/kafka-1/server.properties:/kafka/config/server.properties `
aimvector/kafka:3.0.0

docker logs kafka-1
```

## Kafka - 2

```
docker run -d `
--rm `
--name kafka-2 `
--net kafka `
-v ${PWD}/config/kafka-2/server.properties:/kafka/config/server.properties `
aimvector/kafka:3.0.0

docker logs kafka-2
```

## Kafka - 3

```
docker run -d `
--rm `
--name kafka-3 `
--net kafka `
-v ${PWD}/config/kafka-3/server.properties:/kafka/config/server.properties `
aimvector/kafka:3.0.0

docker logs kafka-3
```

# Topic

Let's create a Topic that allows us to store Order information.
To create a topic, Kafka and Zookeeper have scripts with the installer that allows us to do so.

Access the container:

```
docker exec -it zookeeper-1 bash
```

Create the Topic:

```
/kafka/bin/kafka-topics.sh \
--create \
--bootstrap-server kafka-1:2181 \
--replication-factor 1 \
--partitions 3 \
--topic Orders
```

Describe our Topic:

```
/kafka/bin/kafka-topics.sh \
--describe \
--topic Orders \
--zookeeper zookeeper-1:2181
```

# Simple Producer & Consumer

The Kafka installation also ships with a script that allows us to produce and consume messages to our Kafka network:

We can then run the consumer that will receive that message on that Orders topic:

```
docker exec -it zookeeper-1 bash

/kafka/bin/kafka-console-consumer.sh \
--bootstrap-server kafka-1:9092,kafka-2:9092,kafka-3:9092 \
--topic Orders --from-beginning
```

With a consumer in place, we can start producing messages

```
docker exec -it zookeeper-1 bash

echo "New Order: 1" | \
/kafka/bin/kafka-console-producer.sh \
--broker-list kafka-1:9092,kafka-2:9092,kafka-3:9092 \
--topic Orders > /dev/null
```

Once we have a message in Kafka, we can explore where it got stored in which partition:

```
docker exec -it kafka-1 bash

apt install -y tree
tree /tmp/kafka-logs/

ls -lh /tmp/kafka-logs/Orders-*
```

By seeing 0 bytes in partition 0 and 1, we know the message is sitting in partition 2 as it has 80 bytes.
We can check the message with:

```
cat /tmp/kafka-logs/Orders-2/*.log
```

# Docker Compose with Kafka

So far we've taken a look at staring up Kafka and Zookeeper instances with docker commands.
We've explored the kafka configuration and how to produce and consume messages.
Let's put it all together in a docker compose file.

With compose we'd like to be able to build our containers, pointing to a dockerfile folder with build.context.
We'll also use volumes to mount config files.

Important note that producers and consumers are running kafka images because kafka installation comes prepacked with example consumers and producers as scripts. We override the kafka entrypoint with bash and stdin so it starts in a paused state so that we have run scripts on these instances.
