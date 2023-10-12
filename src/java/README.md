


mvn archetype:generate -DgroupId=com.redis.app -DartifactId=my-app -DarchetypeArtifactId=maven-archetype-quickstart -DinteractiveMode=false


<dependency>
    <groupId>redis.clients</groupId>
    <artifactId>jedis</artifactId>
    <version>5.0.1</version>
</dependency>

vi my-app/src/main/java/com/redis/app/App.java

mvn package


mvn exec:java -Dexec.mainClass=com.redis.app.App