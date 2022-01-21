FROM openjdk:11.0.13-jre-buster

RUN apt-get update && \
    apt-get install -y curl

ENV KAFKA_VERSION 3.0.0
ENV SCALA_VERSION 2.13

RUN mkdir /tmp/kafka && \
    curl "https://archive.apache.org/dist/kafka/${KAFKA_VERSION}/kafka_${SCALA_VERSION}-${KAFKA_VERSION}.tgz" \
    -o /tmp/kafka/kafka.tgz && \
    mkdir /kafka && cd /kafka && \
    tar -xzf /tmp/kafka/kafka.tgz --strip 1

COPY start-zookeeper.sh /usr/bin
RUN chmod +x /usr/bin/start-zookeeper.sh

CMD ["start-zookeeper.sh"]