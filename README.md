# Setur

docker exec -it 71fa50ecdc53 bash
root@kafka:/# /usr/bin/kafka-topics --create --if-not-exists --zookeeper zookeeper:2181 --partitions 1 --replication-factor 1 --topic raporrequests
